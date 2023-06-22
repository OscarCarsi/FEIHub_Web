using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;



namespace FeiHub.Models
{
    public class Posts
    {
        public Posts(){
            AuthorUser = new User();
        }
        public string _id { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string body { get; set; }
        public DateTime dateOfPublish { get; set; }
        public Photo[]? photos { get; set; }

        public User AuthorUser{get;set;}
        public string target{get; set;}

        public int likes { get; set; }
        public int dislikes { get; set; }
        public int reports { get; set; }
        public Comment[]? comments { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public int __v { get; set; }
    }

    public class Photo
    {
        public string url { get; set; }
        public string _id { get; set; }
    }

    public class Comment
    {
        public string commentId { get; set; }
        public string author { get; set; }
        public string body { get; set; }
        public DateTime dateOfComment { get; set; }
        public string _id { get; set; }
        public User AuthorUser{get;set;}
        public string commentOwner{get; set;}
    }

}
