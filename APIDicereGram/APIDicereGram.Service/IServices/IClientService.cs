using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIDicereGram.Service.IServices
{
    public interface IClientService
    {
        Task<bool> GetUser(string phone);
        Task<bool> CheckLimit();
        Task<bool> SetLimit(int limit);
    }
}
