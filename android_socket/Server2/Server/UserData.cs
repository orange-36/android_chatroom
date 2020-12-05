using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class UserData
    {
        private string uid;
        private string username;
        private string msg;
        private string connect;


        public UserData(string uid, string username, string msg, string connect)
        {
            this.uid = uid;
            this.username = username;
            this.msg = msg;
            this.connect = connect;
        }

        public string Uid { get => uid; set => uid = value; }
        public string Username { get => username; set => username = value; }
        public string Msg { get => msg; set => msg = value; }
        public string Connect { get => connect; set => connect = value; }
    }
}
