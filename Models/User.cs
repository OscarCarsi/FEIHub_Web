using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FeiHub.Models
{

    public class User
    {
        public string username { get; set; }
        public string name { get; set; }
        public string paternalSurname { get; set; }
        public string maternalSurname { get; set; }
        public string? schoolId { get; set; }
        public string? educationalProgram { get; set; }
        public string? profilePhoto { get; set;  }
        public HttpStatusCode StatusCode { get; set; }
        public string CompleteName(){
            return name + " " + paternalSurname + " " + maternalSurname;
        }
    }

}
