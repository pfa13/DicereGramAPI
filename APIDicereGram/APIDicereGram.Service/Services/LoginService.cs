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

        public async Task Auth(string phone, string code, TelegramClient client)
        {
            string hash = await _loginRepository.GetHash(phone);
            try
            {
                if(!await client.IsPhoneRegisteredAsync(phone))
                {
                    var usu = Task.Run(() => client.SignUpAsync(phone, hash, code, "", ""));
                    usu.Wait();
                }
                var u = Task.Run(() => client.MakeAuthAsync(phone, hash, code));
                u.Wait();
            }            
            catch(FloodException flood)
            {
                await _loginRepository.SetLimit(Convert.ToInt32(flood.TimeToWait));
                Thread.Sleep(flood.TimeToWait);
                await Auth(phone, code, client);
            }
        }
        
        public async Task<string> GetHash(string phone, TelegramClient client)
        {
            string h;
            try
            {
                var hash = Task.Run(() => client.SendCodeRequestAsync(phone));
                hash.Wait();
                h = hash.Result;
                await _loginRepository.SaveHash(phone, hash.Result);
            }
            catch (FloodException flood)
            {
                await _loginRepository.SetLimit(Convert.ToInt32(flood.TimeToWait));
                Thread.Sleep(flood.TimeToWait);
                var hash = client.SendCodeRequestAsync(phone);
                hash.Wait();
                h = hash.Result;
                await _loginRepository.SaveHash(phone, hash.Result);
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
                output = await _loginRepository.SaveCode(phone, code);
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
                    await _loginRepository.SaveContacts(phone, cc.Id, cc.Phone, cc.FirstName, cc.LastName, cc.Status.ToString(), userFull.Result.Blocked, cc.AccessHash.Value);
                }
            }
            catch(FloodException flood)
            {
                await _loginRepository.SetLimit(Convert.ToInt32(flood.TimeToWait));
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
                output = await _loginRepository.SaveToken(phone);
            }
            catch (Exception ex)
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
                output = await _loginRepository.SetLimit(time);
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
                output = await _loginRepository.CheckLimit();
            }
            catch (SqlException ex)
            {
                return false;
            }
            return output;
        }
    }
}
