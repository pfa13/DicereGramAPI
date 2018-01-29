using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleSharp.TL;

namespace APIDicereGram.Service.IServices.APIDicereGram
{
    public interface ILogin
    {
        string GetHash(string phone);
        string Auth(string phone, string code);
    }
}
