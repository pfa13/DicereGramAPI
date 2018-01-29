using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIDicereGram.Data.IRepositories
{
    public interface ILoginRepository
    {
        Task<bool> SaveHash(string phone, string hash);
        Task<bool> SaveCode(string phone, string code);
        Task<string> SaveToken(string phone);
        Task<bool> SaveContacts(string phone, long id, string telephone, string name, string lastname, string status, bool block, long accesshash);
        Task<bool> SetLimit(int time);
        Task<bool> CheckLimit();
        Task<string> GetHash(string phone);
    }
}
