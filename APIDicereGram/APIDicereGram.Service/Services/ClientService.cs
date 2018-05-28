using APIDicereGram.Data.Repositories;
using APIDicereGram.Service.IServices;
using APIDicereGram.Service.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeleSharp.TL;
using TLSharp.Core;
using TLSharp.Core.Network;

namespace APIDicereGram.Service
{
    public class ClientService : IClientService
    {
        private int ApiId = 00;
        private string ApiHash = "xxxxxx";
        public TLClient client;
        public TLUser user = null;
        LoginRepository _loginRepository = new LoginRepository();
        ClientRepository _clientRepository = new ClientRepository();

        public ClientService(string phone)
        {
            if (CheckLimit().Result)
            {                
                try
                {
                    var store = new FileSessionStore();
                    client = new TLClient(ApiId, ApiHash, store, phone);
                    var t = Task.Run(() => client.ConnectAsync());                    
                    t.Wait();
                }
                catch (FloodException flood)
                {
                    SetLimit(Convert.ToInt32(flood.TimeToWait)).Wait();
                    Thread.Sleep(flood.TimeToWait);
                    new ClientService(phone);
                }
                catch(Exception ex)
                {
                    Trace.TraceError(ex.InnerException.Message);
                }
            }            
        }

        public async Task<bool> GetUser(string phone)
        {
            bool output = false;
            try
            {
                output = await _clientRepository.GetUser(phone);
            }
            catch (SqlException ex)
            {
                return false;
            }
            return output;
        }

        public async Task<bool> CheckLimit()
        {
            var output = await _loginRepository.CheckLimit();
            return output;
        }

        public async Task<bool> SetLimit(int limit)
        {
            var output = await _loginRepository.SetLimit(limit);
            return output;
        }
    }
}
