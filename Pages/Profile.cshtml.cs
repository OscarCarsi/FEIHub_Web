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
    PostsAPIServices postsAPIServices = new PostsAPIServices();
    public List<Posts>? posts {get; set;}
    public User thisUser {get; set;}
    public string visibilityFollowing = "none";
    public string visibilityFollowers = "none";
    public string visibilityPosts = "block";
    public string visibilityEditProfile= "none";
    public string visibilityUser = "block";
    public string visibilityAcademic = "block";
    public string userRol = "";
    public string visibilityFollow = "none";
    public string visibilityUnfollow = "none";
    public string disable = "disable";
    public List<User> following {get; set;}
    public List<User> followers {get; set;}
    [TempData]
    public string ErrorMessage { get; set; }
    [TempData]
    public string WarningMessage { get; set; }
    [TempData]
    public string SuccessMessage { get; set; }
    [BindProperty]
    public string idPost {get; set;}
    [BindProperty]
    public string titlePost {get; set;}
    [BindProperty]
    public string username {get; set;}

    public ProfileModel(){
        thisUser = new User();
    }
    public async void GetUser(string username)
    {
        var task = Task.Run(async () =>
        {
            thisUser = await usersAPIServices.GetUser(username);
            if (thisUser.profilePhoto == null)
            {
                thisUser.profilePhoto = "Resources/usuario.png";
            }
            if (String.IsNullOrEmpty(thisUser.educationalProgram))
            {
                userRol = "Académico";
                visibilityAcademic = "none";
            }
            else
            {
                userRol = "Estudiante";
            }
        });
        task.GetAwaiter().GetResult();
        task.Dispose();
    }

    public async void AddFollowing()
    {
        Task.Run(async () =>
        {
            following = await usersAPIServices.GetListUsersFollowing(thisUser.username).ConfigureAwait(false);
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
    }

    public async void AddFollowers()
    {
        Task.Run(async () =>
        {
            followers = await usersAPIServices.GetListUsersFollowers(thisUser.username).ConfigureAwait(false);
        }).GetAwaiter().GetResult();

        if (followers.Count > 0)
        {
            if (followers[0].StatusCode == System.Net.HttpStatusCode.OK)
            {
                for (int i = 0; i < followers.Count; i++){
                    if (followers[i].profilePhoto == null){
                        followers[i].profilePhoto = "Resources/usuario.png";
                    }
                }
            }
            if (followers[0].StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
                SingletonUser.Instance.BorrarSinglenton();
            }
            if (followers[0].StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                ErrorMessage = "Tuvimos un error al obtener a quiénes sigues, inténtalo más tarde";
            }
        }
    }

    public async void AddPosts()
    {
        var task = Task.Run(async () =>
        {
            posts = await postsAPIServices.GetPostsByUsername(thisUser.username);
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
        });
        task.GetAwaiter().GetResult();
        task.Dispose();
    }

    private async void IsFollowingUser()
    {
        if(thisUser.username != SingletonUser.Instance.Username)
        {
            var task = Task.Run(async () =>
            {
                List<string> followings = await usersAPIServices.GetListFollowing(SingletonUser.Instance.Username);
                if (followings.Contains(thisUser.username))
                {
                    visibilityUnfollow = "block";
                    visibilityFollow = "none";
                }
                else
                {
                    visibilityUnfollow = "none";
                    visibilityFollow = "block";
                }
            });
            task.GetAwaiter().GetResult();
            task.Dispose();
        }
    }

    public void OnGet(string username){
        if (username == SingletonUser.Instance.Username)
        {
            visibilityEditProfile = "block";
            visibilityUser = "none";
        }
        GetUser(username);
        AddPosts();
        AddFollowing();
        AddFollowers();
        IsFollowingUser();
        thisUser.username = username;
    }
    public async Task<IActionResult> OnPostFollow(string username)
    {
        if (!String.IsNullOrEmpty(username))
        {
            var sucessfullFollow = await Follow(username);
            if (sucessfullFollow)
            {
                return RedirectToPage("/Profile", new {username = username});
            }
            else
            {
                ErrorMessage = $"Tuvimos un error al seguir a {username}";
            }
        }
        return Page();
    }
    public async Task<IActionResult> OnPostUnfollow(string username)
    {
        if (!String.IsNullOrEmpty(username))
        {
            var sucessfullFollow = await Unfollow(username);
            if (sucessfullFollow)
            {
                return RedirectToPage("/Profile", new {username = username});
            }
            else
            {
                ErrorMessage = $"Tuvimos un error al dejar de seguir a {username}";
            }
        }
        return Page();
    }

    private async Task<bool> Follow(string username)
    {
        bool followSuccessfull = false;
        HttpResponseMessage response = await usersAPIServices.Follow(username, SingletonUser.Instance.Username);
        if (response.IsSuccessStatusCode)
        {
            SuccessMessage = $"Haz empezado a seguir a {username}";
            followSuccessfull = true;
            //visibilityUnfollow = "block";
            //visibilityFollow = "none";
        }
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
            SingletonUser.Instance.BorrarSinglenton();

        }
        if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            ErrorMessage = $"Tuvimos un error al seguir a {username}";
        }
        return followSuccessfull;
    }

    private async Task<bool> Unfollow(string username)
    {
        bool followSuccessfull = false;
        HttpResponseMessage response = await usersAPIServices.Unfollow(username, SingletonUser.Instance.Username);
        if (response.IsSuccessStatusCode)
        {
            SuccessMessage = $"Haz dejado de seguir a {username}";
            followSuccessfull = true;
            //visibilityUnfollow = "block";
            //visibilityFollow = "none";
        }
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
            SingletonUser.Instance.BorrarSinglenton();

        }
        if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            ErrorMessage = $"Tuvimos un error al dejar de seguir a {username}";
        }
        return followSuccessfull;
    }

    public IActionResult OnPostCompletePost()
    {
        return RedirectToPage("/CompletePost", new {idPost = idPost, titlePost = titlePost});
    }

    public async Task<IActionResult> OnPostLike(){
        await LikePost();
        return RedirectToPage("/Profile", new {username = username});
    }
    public async Task<IActionResult> OnPostDislike(){
        await DislikePost();
        return RedirectToPage("/Profile", new {username = username});
    }
    public async Task<IActionResult> OnPostReport(){
        await ReportThisPost();
        return RedirectToPage("/Profile", new {username = username});
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
