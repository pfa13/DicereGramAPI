using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLSharp.Core;
using TLSharp.Core.Network;

namespace APIDicereGram.Service.Services
{
    public class TLClient : TelegramClient
    {
        public Session Session { get { return Session; } }
        public TLClient(int apiId, string apiHash, ISessionStore store = null, string sessionUserId = "session", TcpClientConnectionHandler handler = null) : base(apiId, apiHash, store, sessionUserId, handler)
        {
        }
    }
}
