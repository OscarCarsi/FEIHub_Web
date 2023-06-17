using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FeiHub.Models;
using FeiHub.Services;
using FEIHub_Web.wwwroot.Resources;


namespace FEIHub_Web.Pages;

public class MainPageModel : PageModel
{
    UsersAPIServices usersAPIServices = new UsersAPIServices();
    SingletonUser user = SingletonUser.Instance;
    public List<Posts>? posts {get; set;}
    public User thisUser {get; set;}
    public List<User> following {get; set;}

    public MainPageModel(){
        posts = new List<Posts>()
        {
            new Posts(){
                id = "1123456",
                title = "Publicación de prueba",
                author = "Nombre de usuario",
                body = "Aquí va el texto de la publicación",
                dateOfPublish = DateTime.Now,
                target = "Students",
                likes = 3,
                dislikes = 5
            },
            new Posts(){
                id = "1123458",
                title = "Otra publicación de prueba",
                author = "Nombre de usuario2",
                body = "Aquí va el texto de la publicación",
                dateOfPublish = DateTime.Now,
                target = "Students",
                likes = 3,
                dislikes = 5
            },
        };

        Photo photo = new Photo();
        photo.url = "Resources/uv.png";
        posts.ElementAt(0).photos = new Photo[2];
        posts.ElementAt(0).photos[0] = photo;
        photo = new Photo();
        photo.url = "https://www.grandespymes.com.ar/wp-content/uploads/2020/08/gente-feliz-830x553.jpg";
        posts.ElementAt(0).photos[1] = photo;

        thisUser = new User(){
            username = "Saraiche",
            profilePhoto = "/Resources/pic.jpg"

        };
        following = new List<User>(){
            new User(){
                username="Carsiano",
                profilePhoto="/Resources/uv.png"
            },
            new User(){
                username="Ferchito",
                profilePhoto="/Resources/pic.jpg"
            }
        };

    }



}
