using APIDicereGram.Service;
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
using APIDicereGram.Models;
using APIDicereGram.IControllers;
using TeleSharp.TL;

namespace APIDicereGram.Controllers
{
    public class LoginController : ApiController, ILogin
    {
        LoginService _loginService = new LoginService();
        LoginRepository _loginRepository = new LoginRepository();

        [HttpPost]
        [Route("login/auth")]
        public async Task<string> Auth([FromBody] User user)
        {
            ClientService _clientService = new ClientService(user.Phone);
            var client = _clientService.client;
            string token = "";            
            TLUser usu;
                        
            if (!client.IsUserAuthorized())
            {
                await _loginService.Auth(user.Phone, user.Code, client);
                await _loginService.SaveCode(user.Phone, user.Code);
                //token = _loginService.SaveToken(user.Phone).Result;
                //_loginService.SaveContacts(user.Phone, client);
                return "Auth realizado correctamente";
            }
            else
            {
                usu = client.Session.TLUser;
            }
            return "Su usuario ya está autorizado";
        }

        [HttpPost]
        [Route("login")]
        public async Task<string> GetHash([FromBody] User user)
        {
            ClientService _clientService = new ClientService(user.Phone);
            var client = _clientService.client;
            if(!client.IsUserAuthorized())
            {
                var hash = await _loginService.GetHash(user.Phone, client);
                return hash;
            }
            return "Su usuario ya está autorizado";
        }

        [HttpPost]
        [Route("contact")]
        public async Task<string> GetContact([FromBody] User user)
        {
            ClientService _clientService = new ClientService(user.Phone);
            var client = _clientService.client;
            if(client.IsUserAuthorized())
            {
                 await _loginService.SaveContacts(user.Phone, client);
            }
            return "ok";
        }
    }
}
