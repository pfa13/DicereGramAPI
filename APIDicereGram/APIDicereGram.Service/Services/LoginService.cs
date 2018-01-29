using APIDicereGram.Data.Repositories;
using APIDicereGram.Service.IServices;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Users;
using TLSharp.Core;
using TLSharp.Core.Network;

namespace APIDicereGram.Service.Services
{
    public class LoginService : ILoginService
    {
        LoginRepository _loginRepository;

        public LoginService()
        {
            _loginRepository = new LoginRepository();
        }

        public void Auth(string phone, string code, TelegramClient client)
        {
            TLUser user;
            string hash = _loginRepository.GetHash(phone).Result;
            try
            {
                var u = Task.Run(() => client.MakeAuthAsync(phone, hash, code));
                u.Wait();
                user = u.Result;
            }            
            catch(FloodException flood)
            {
                _loginRepository.SetLimit(Convert.ToInt32(flood.TimeToWait)).Wait();
                Thread.Sleep(flood.TimeToWait);
                Auth(phone, code, client);
            }
        }
        
        public string GetHash(string phone, TelegramClient client)
        {
            string h;
            try
            {
                var hash = Task.Run(() => client.SendCodeRequestAsync(phone));
                hash.Wait();
                h = hash.Result;
                _loginRepository.SaveHash(phone, hash.Result).Wait();
            }
            catch (FloodException flood)
            {
                _loginRepository.SetLimit(Convert.ToInt32(flood.TimeToWait)).Wait();
                Thread.Sleep(flood.TimeToWait);
                var hash = client.SendCodeRequestAsync(phone);
                hash.Wait();
                h = hash.Result;
                _loginRepository.SaveHash(phone, hash.Result).Wait();
            }
            catch(Exception ex)
            {
                return "Error acceso BD API";
            }
            return h;
        }

        public async Task<bool> SaveCode(string phone, string code)
        {
            bool output = false;
            try
            {
                output = _loginRepository.SaveCode(phone, code).Result;
            }
            catch(SqlException ex)
            {
                return false;
            }
            return output;
        }

        public async Task<bool> SaveContacts(string phone, TelegramClient client)
        {
            try
            {
                var c = client.GetContactsAsync();

                var contacts = c.Result.Users.ToList();
                foreach (var co in contacts)
                {
                    var cc = co as TLUser;
                    TLRequestGetFullUser req = new TLRequestGetFullUser
                    {
                        Id = new TLInputUser
                        {
                            UserId = cc.Id,
                            AccessHash = (long)cc.AccessHash
                        }
                    };
                    Thread.Sleep(1000);
                    var userFull = client.SendRequestAsync<TLUserFull>(req);
                    _loginRepository.SaveContacts(phone, cc.Id, cc.Phone, cc.FirstName, cc.LastName, cc.Status.ToString(), userFull.Result.Blocked, cc.AccessHash.Value);
                }
            }
            catch(FloodException flood)
            {
                _loginRepository.SetLimit(Convert.ToInt32(flood.TimeToWait)).Wait();
                Thread.Sleep(flood.TimeToWait);
            }
            catch (SqlException ex)
            {
                return false;
            }
            return true;
        }

        public async Task<string> SaveToken(string phone)
        {
            string output = "";
            try
            {
                output = _loginRepository.SaveToken(phone).Result;
            }
            catch (SqlException ex)
            {
                return "Error acceso BD API";
            }
            return output;
        }

        public async Task<bool> SetLimit(int time)
        {
            bool output = false;
            try
            {
                output = _loginRepository.SetLimit(time).Result;
            }
            catch (SqlException ex)
            {
                return false;
            }
            return output;
        }
        
        public async Task<bool> CheckLimit()
        {
            bool output = false;
            try
            {
                output = _loginRepository.CheckLimit().Result;
            }
            catch (SqlException ex)
            {
                return false;
            }
            return output;
        }
    }
}
