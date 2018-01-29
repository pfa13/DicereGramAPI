using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIDicereGram.Data
{
    public class Connection
    {
        public string conn;
        public Connection()
        {
            conn = "Server=localhost;Port=3306;Database=diceregram;Uid=root;";
        }
    }
}
