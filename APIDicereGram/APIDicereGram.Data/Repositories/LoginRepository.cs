using APIDicereGram.Data.IRepositories;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace APIDicereGram.Data.Repositories
{
    public class LoginRepository : Connection, ILoginRepository
    {
        private MySqlConnection connection;
        internal const string Inputkey = "560A18CD-6346-4CF0-A2E8-671F9B6B9EA9";
        string salt = "pauladiceregram";

        public LoginRepository()
        {
            connection = new MySqlConnection(conn);
        }
        
        public async Task<string> GetHash(string phone)
        {
            string output = "";
            string query = "SELECT hash FROM user WHERE phone='" + phone + "'";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dr = cmd.ExecuteReader();
            if(dr.Read())
            {
                output = dr[0].ToString();
            }
            connection.Close();
            return output;
        }
        
        public async Task<bool> SaveCode(string phone, string code)
        {
            string query = "UPDATE user SET code='" + code + "' WHERE phone='" + phone + "')";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            connection.Close();
            return true;
        }

        public async Task<bool> SaveHash(string phone, string hash)
        {
            bool exist = false;
            string query1 = "SELECT * FROM user WHERE phone='" + phone + "'";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query1, connection);
            MySqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                exist = true;
            }
            if(!exist)
            {
                string query = "INSERT INTO user (phone, hash) VALUES('" + phone + "', '" + hash + "')";
                cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
            }
            else
            {
                string query = "UPDATE user SET hash='" + hash + "' WHERE phone='" + phone + "')";
                cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
            return true;
        }

        public async Task<string> SaveToken(string phone)
        {
            string token = EncryptRijndael(phone, salt);
            string query = "UPDATE user SET token='" + token + "' WHERE phone='" + phone + "')";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            connection.Close();
            return token;
        }

        public static string EncryptRijndael(string text, string salt)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            var aesAlg = NewRijndaelManaged(salt);

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(text);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        private static RijndaelManaged NewRijndaelManaged(string salt)
        {
            if (salt == null) throw new ArgumentNullException("salt");
            var saltBytes = Encoding.ASCII.GetBytes(salt);
            var key = new Rfc2898DeriveBytes(Inputkey, saltBytes);

            var aesAlg = new RijndaelManaged();
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

            return aesAlg;
        }

        public async Task<bool> SaveContacts(string phone, long id, string telephone, string name, string lastname, string status, bool block, long accesshash)
        {
            string query = "INSERT INTO contact VALUES(" + id + ", '" + telephone + "', '" + name + "', '" + lastname + "', '" + status + "', " + block + ", '" + accesshash + "')";
            string query2 = "INSERT INTO user_contact VALUES('" + phone + "', '" + id + "', '" + telephone + "')";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            cmd = new MySqlCommand(query2, connection);
            cmd.ExecuteNonQuery();
            connection.Close();
            return true;
        }

        public async Task<bool> SetLimit(int time)
        {
            string qdel = "DELETE FROM flood";
            string query = "INSERT INTO flood VALUES(" + DateTime.Now + ", " + time + ")";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(qdel, connection);
            cmd.ExecuteNonQuery();
            cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            connection.Close();
            return true;
        }

        public async Task<bool> CheckLimit()
        {
            bool output = false;
            string query = "SELECT * FROM flood";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                var d = Convert.ToDateTime(dr["start"].ToString());
                if(d.AddSeconds(Convert.ToInt32(dr["duration"].ToString())) < DateTime.Now)
                {
                    output = true;
                }
            }
            else
            {
                output = true;
            }
            connection.Close();
            return output;
        }
    }
}
