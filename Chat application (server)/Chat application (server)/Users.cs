using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_application__server_
{
    public class Users
    {
        public static List<Users> listUser = new List<Users>();

        public Users(string ipPort, string username)
        {
            IpPort = ipPort;
            Username = username;
        }

        public string IpPort { get; set; }
        public string Username { get; set; }
    }
}
