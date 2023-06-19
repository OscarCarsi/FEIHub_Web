using System.Reflection.Metadata;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FeiHub.Models;
using FeiHub.Services;
using FEIHub_Web.wwwroot.Resources;


namespace FEIHub_Web.Pages;

public class ProfileModel : PageModel
{
    UsersAPIServices usersAPIServices = new UsersAPIServices();
    SingletonUser user = SingletonUser.Instance;
    public Posts newPost {get; set;}
    public User thisUser {get; set;}


    public ProfileModel(){
        newPost = new Posts();
        thisUser = new User();
    }

    public void OnPostInitializate(string username){
        thisUser.username = username;
    }
}
