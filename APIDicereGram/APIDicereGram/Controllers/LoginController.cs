using APIDicereGram.Service;
using APIDicereGram.Service.IServices.APIDicereGram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using TLSharp.Core.Network;
using System.Threading;
using APIDicereGram.Service.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using APIDicereGram.Data.Repositories;

namespace APIDicereGram.Controllers
{
    public class LoginController : ApiController, ILogin
    {
        LoginService _loginService = new LoginService();

        [HttpPost]
        [Route("login/auth")]
        public string Auth(string phone, string code)
        {
            ClientService _clientService = new ClientService();
            var client = _clientService.client;
            string token = "";
            
            _loginService.Auth(phone, code, client);
            if(client.IsUserAuthorized())
            {
                _loginService.SaveCode(phone, code);
                _loginService.SaveContacts(phone, client);
                token = _loginService.SaveToken(phone).Result;
            }
            
            return token;
        }

        [HttpPost]
        [Route("login")]
        public string GetHash([FromBody]string phone)
        {
            ClientService _clientService = new ClientService();
            var client = _clientService.client;
            var hash = _loginService.GetHash(phone, client);
            return hash;
        }
    }
}
