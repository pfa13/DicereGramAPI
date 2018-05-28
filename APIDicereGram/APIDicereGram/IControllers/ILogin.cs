using APIDicereGram.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIDicereGram.IControllers
{
    public interface ILogin
    {
        Task<string> GetHash(User user);
        Task<string> Auth(User user);
    }
}
