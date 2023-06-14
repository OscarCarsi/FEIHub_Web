using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static FeiHub.Models.Chats;

namespace FeiHub.Models
{
    public class Chats
    {
        public class Chat
        {
            public string Username { get; set; }
            public string Message { get; set; }
            public DateTime DateOfMessage { get; set; }
            public string DateOfMessageString { get; set; }
            public string DateAPI { get; set; }
            override
            public string ToString()
            {
                return Username + ": " + Message;
            }
        }

        public HttpStatusCode StatusCode { get; set; }
        public Participant[] participants { get; set; }
        public Message[]? messages { get; set; }
        public Chat[] chats { get; set; }
        


        public class Participant
        {
            public string username { get; set; }
            public string _id { get; set; }
        }


        public class Message
        {
            public string username { get; set; }
            public string message { get; set; }
            public DateTime dateOfMessage { get; set; }
            override
            public string ToString()
            {
                return dateOfMessage.ToString() + " " + username + ": " + message;
            }
        }

        public class Dateofmessage
        {
            public DateTime date { get; set; }

        }


    }
}
