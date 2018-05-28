using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIDicereGram.Models
{
    public class User
    {
        public string Phone { get; set; }
        public string Hash { get; set; }
        public string Code { get; set; }
        public string Token { get; set; }
    }
}