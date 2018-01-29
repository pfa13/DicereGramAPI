using APIDicereGram.Data.Repositories;
using System;
using System.Collections.Generic;
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
    public class ClientService
    {
        private int ApiId = 190786;
        private string ApiHash = "b7e8ca8d987b1fd6f7e068c558a7cd8f";
        public TelegramClient client;
        public TLUser user = null;

        public ClientService()
        {
            LoginRepository _loginRepository = new LoginRepository();
            if (_loginRepository.CheckLimit().Result)
            {                
                try
                {
                    var store = new FileSessionStore();
                    client = new TelegramClient(ApiId, ApiHash, store);
                    var t = Task.Run(() => client.ConnectAsync());
                    t.Wait();
                }
                catch (FloodException flood)
                {
                    _loginRepository.SetLimit(Convert.ToInt32(flood.TimeToWait)).Wait();
                    Thread.Sleep(flood.TimeToWait);
                    new ClientService();
                }
                catch(Exception ex)
                {
                    Trace.TraceError(ex.InnerException.Message);
                }
            }            
        }
    }
}
