using APIDicereGram.Data.IRepositories;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIDicereGram.Data.Repositories
{
    public class ClientRepository : Connection, IClientRepository
    {
        private MySqlConnection connection;

        public ClientRepository()
        {
            connection = new MySqlConnection(conn);
        }

        public async Task<bool> GetUser(string phone)
        {
            bool output = false;
            string query = "SELECT * FROM user WHERE phone='" + phone + "'";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                output = true;
            }
            connection.Close();
            return output;
        }
    }
}
