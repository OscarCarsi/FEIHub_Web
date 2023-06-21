using System.Reflection.Metadata;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FeiHub.Models;
using FeiHub.Services;
using FEIHub_Web.wwwroot.Resources;


namespace FEIHub_Web.Pages;

public class EditPostModel : PageModel
{
    UsersAPIServices usersAPIServices = new UsersAPIServices();
    SingletonUser user = SingletonUser.Instance;
    public Posts PostToEdit {get; set;}
    public User thisUser {get; set;}


    public EditPostModel(){
        PostToEdit = new Posts();
        thisUser = new User();
    }

    public void OnPostInitializate(string idPost){
        //buscar post por id
        PostToEdit = new Posts(){
            id = idPost,
                title = "Otra publicación de prueba",
                author = "Nombre de usuario2",
                body = "Aquí va el texto de la publicación",
                dateOfPublish = DateTime.Now,
                target = "Student",
                likes = 3,
                dislikes = 5
            };
    }
    public void OnGet(string idPost){
        //buscar post por id
        PostToEdit = new Posts(){
            id = idPost,
                title = "Otra publicación de prueba",
                author = "Nombre de usuario2",
                body = "Aquí va el texto de la publicación",
                dateOfPublish = DateTime.Now,
                target = "Student",
                likes = 3,
                dislikes = 5
            };
    }
}
