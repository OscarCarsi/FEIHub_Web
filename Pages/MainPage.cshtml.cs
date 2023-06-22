using System.Reflection.Metadata;
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
    PostsAPIServices postsAPIServices = new PostsAPIServices();
    public SingletonUser user = SingletonUser.Instance;
    public List<Posts>? posts {get; set;}
    public User thisUser {get; set;}
    public List<User> following {get; set;}
    [BindProperty]
    public string idPost {get; set;}
    [BindProperty]
    public string titlePost {get; set;}
    [TempData]
    public string ErrorMessage { get; set; }

    [TempData]
    public string WarningMessage { get; set; }
    [TempData]
    public string SuccessMessage { get; set; }
    [TempData]
    public string SuccessDelete { get; set; }


    public async void AddPostWithoutFollowings()
    {
        var task = Task.Run(async () =>
        {
            List<Posts> postsObtained = await postsAPIServices.GetPostsWithoutFollowings(SingletonUser.Instance.Rol);
            if(postsObtained.Count > 0)
            {
                if(postsObtained[0].StatusCode == System.Net.HttpStatusCode.OK)
                {
                    foreach(Posts post in postsObtained)
                    {
                        User userData = await usersAPIServices.GetUser(post.author);
                        if (userData.profilePhoto == null)
                        {
                            userData.profilePhoto = "Resources/usuario.png";
                        }
                        post.AuthorUser = userData;
                        if(post.target == "EVERYBODY")
                        {
                            post.target= "Todos";
                        }
                        if (post.target == "ACADEMIC")
                        {
                            post.target = "Académicos";
                        }
                        if (post.target == "STUDENT")
                        {
                            post.target = "Estudiantes";
                        }
                        posts.Add(post);
                    }
                }
                if (postsObtained[0].StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
                    SingletonUser.Instance.BorrarSinglenton();
                }
                if (postsObtained[0].StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    ErrorMessage = "Tuvimos un error al obtener las publicaciones, inténtalo más tarde";
                }
            }
            else
            {
                WarningMessage = "No existen publicaciones recientes";
            }
        });
        task.GetAwaiter().GetResult();
        task.Dispose();
    }
    public async void AddFollowing()
    {

        Task.Run(async () =>
        {
                following = await usersAPIServices.GetListUsersFollowing(SingletonUser.Instance.Username).ConfigureAwait(false);
        }).GetAwaiter().GetResult();

        if (following.Count > 0)
        {
            if (following[0].StatusCode == System.Net.HttpStatusCode.OK)
            {
                for (int i = 0; i < following.Count; i++){
                    if (following[i].profilePhoto == null){
                        following[i].profilePhoto = "Resources/usuario.png";
                    }
                }
            }
            if (following[0].StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
                SingletonUser.Instance.BorrarSinglenton();
            }
            if (following[0].StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                ErrorMessage = "Tuvimos un error al obtener a quiénes sigues, inténtalo más tarde";
            }
        }
        else
        {
            WarningMessage = "Sigue a tus amigos para verlos aquí";
            AddPostWithoutFollowings();
        }
    }
    public async void AddPostWithTarget()
    {
        var task = Task.Run(async () =>
        {
            posts = await postsAPIServices.GetPostsByTarget(following,SingletonUser.Instance.Rol);

            if (posts.Count > 0)
            {
                if (posts[0].StatusCode == System.Net.HttpStatusCode.OK)
                {
                    for (int i = 0; i < posts.Count; i++)
                    {
                        User userData = await usersAPIServices.GetUser(posts[i].author);
                        if (userData.profilePhoto == null)
                        {
                            userData.profilePhoto = "Resources/usuario.png";
                        }
                        posts[i].AuthorUser = userData;
                        if(posts[i].target == "EVERYBODY")
                        {
                            posts[i].target= "Todos";
                        }
                        if (posts[i].target == "ACADEMIC")
                        {
                            posts[i].target = "Académicos";
                        }
                        if (posts[i].target == "STUDENT")
                        {
                            posts[i].target = "Estudiantes";
                        }
                    }
                }
                if (posts[0].StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
                    SingletonUser.Instance.BorrarSinglenton();
                }
                if (posts[0].StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    ErrorMessage = "Tuvimos un error al obtener las publicaciones, inténtalo más tarde";
                }
            }
            else
            {
                WarningMessage = "No existen publicaciones recientes";
            }
        });
        task.GetAwaiter().GetResult();
        task.Dispose();
    }
    public async void AddPrincipalPosts()
    {
        Task.Run(async () =>
        {
            posts = await postsAPIServices.GetPrincipalPosts(following, SingletonUser.Instance.Rol).ConfigureAwait(false);

            if (posts.Count > 0)
            {
                if (posts[0].StatusCode == System.Net.HttpStatusCode.OK)
                {
                    for (int i = 0; i < posts.Count; i++)
                    {
                        User userData = await usersAPIServices.GetUser(posts[i].author);
                        if (userData.profilePhoto == null)
                        {
                            userData.profilePhoto = "Resources/usuario.png";
                        }
                        posts[i].AuthorUser = userData;
                        if(posts[i].target == "EVERYBODY")
                        {
                            posts[i].target= "Todos";
                        }
                        if (posts[i].target == "ACADEMIC")
                        {
                            posts[i].target = "Académicos";
                        }
                        if (posts[i].target == "STUDENT")
                        {
                            posts[i].target = "Estudiantes";
                        }
                    }
                }
                if (posts[0].StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
                    SingletonUser.Instance.BorrarSinglenton();
                }
                if (posts[0].StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    ErrorMessage = "Tuvimos un error al obtener las publicaciones, inténtalo más tarde";
                }
            }
            else
            {
                WarningMessage = "No existen publicaciones recientes";
            }
        }).GetAwaiter().GetResult();
    }

    public MainPageModel(){
        user = SingletonUser.Instance;
        posts = new List<Posts>();
        following = new List<User>();
    }

    public async Task<IActionResult> OnGet(string rol = "Everybody")
    {
        AddFollowing();

        if(rol != "Everybody"){
            AddPostWithTarget();
        }
        else {
            AddPrincipalPosts();
        }
        return Page();
    }
    public IActionResult OnPostEverybody(){
        return RedirectToPage("/MainPage");
    }

    public IActionResult OnPostRol(){
        return RedirectToPage("/MainPage", new {rol = SingletonUser.Instance.Rol});
    }

    public IActionResult OnPostCompletePost()
    {
        return RedirectToPage("/CompletePost", new {idPost = idPost, titlePost = titlePost});
    }
    public async Task<IActionResult> OnPostLike(){
        await LikePost();
        return RedirectToPage("/MainPage");
    }
    public async Task<IActionResult> OnPostDislike(){
        await DislikePost();
        return RedirectToPage("/MainPage");
    }
    public async Task<IActionResult> OnPostReport(){
        await ReportThisPost();
        return RedirectToPage("/MainPage");
    }
    private async Task LikePost()
    {
        HttpResponseMessage response = await postsAPIServices.AddLike(idPost);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
            SingletonUser.Instance.BorrarSinglenton();
        }
        if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            ErrorMessage = "No se pudo agregar el me gusta publicación inténtalo más tarde";
        }
    }
    private async Task DislikePost()
    {
        HttpResponseMessage response = await postsAPIServices.AddDislike(idPost);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
            SingletonUser.Instance.BorrarSinglenton();
        }
        if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            ErrorMessage = "No se pudo agregar el me gusta publicación inténtalo más tarde";
        }
    }
    private async Task ReportThisPost()
    {
        HttpResponseMessage response =  await postsAPIServices.AddReport(idPost, 1);
        if (response.IsSuccessStatusCode)
        {
            SuccessMessage = "Reporte enviado";
        }
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
            SingletonUser.Instance.BorrarSinglenton();
        }
        if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            ErrorMessage = "Tuvimos un error al enviar el reporte, inténtalo más tarde";
        }
    }
}
