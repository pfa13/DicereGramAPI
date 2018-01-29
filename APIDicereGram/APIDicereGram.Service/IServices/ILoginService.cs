using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLSharp.Core;

namespace APIDicereGram.Service.IServices
{
    public interface ILoginService
    {
        string GetHash(string phone, TelegramClient client);
        void Auth(string phone, string code, TelegramClient client);
        Task<bool> SaveCode(string phone, string code);
        Task<string> SaveToken(string phone);
        Task<bool> SaveContacts(string phone, TelegramClient client);
        Task<bool> SetLimit(int time);
        Task<bool> CheckLimit();
    }
}
