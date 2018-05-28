using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIDicereGram.Data.IRepositories
{
    public interface IClientRepository
    {
        Task<bool> GetUser(string phone);
    }
}
