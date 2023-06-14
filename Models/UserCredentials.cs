using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FeiHub.Models
{

    public class UserCredentials
    {
        public string username { get; set; }
        public string token { get; set; }
        public string rol { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }

}
