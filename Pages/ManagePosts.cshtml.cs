using System.Reflection.Metadata;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FeiHub.Models;
using FeiHub.Services;
using FEIHub_Web.wwwroot.Resources;


namespace FEIHub_Web.Pages;

public class ManagePostsModel : PageModel
{
    UsersAPIServices usersAPIServices = new UsersAPIServices();
    PostsAPIServices postsAPIServices = new PostsAPIServices();
    public List<Posts> posts = new List<Posts>();
    [TempData]
    public string ErrorMessage { get; set; }
    [TempData]
    public string SuccessMessage { get; set; }
    [TempData]
    public string SuccessDelete { get; set; }
    [BindProperty]
    public string idPost {get; set;}
    [BindProperty]
    public string titlePost {get; set;}


    public ManagePostsModel()
    {

    }

    public void OnGet()
    {
        AddPosts();
    }

    public async void AddPosts()
    {
        var task = Task.Run(async () =>
        {
            posts = await postsAPIServices.GetReporteredPosts();
            if(posts.Count > 0)
            {
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
    private async Task<int> DeletePost()
    {
        Posts postToDelete = new Posts();
        postToDelete.id = idPost;
        HttpResponseMessage response = await postsAPIServices.DeletePost(postToDelete);
        if (response.IsSuccessStatusCode)
        {
            SuccessDelete = "Se eliminó correctamente la publicación";
            return 1;

        }
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
            SingletonUser.Instance.BorrarSinglenton();
            return 2;
        }
        if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            ErrorMessage = "No se pudo eliminar la publicación inténtalo más tarde";
        }
        return 1;
    }
     public async Task<IActionResult> OnPostDeletePost(){
        int page = await DeletePost();
        switch (page)
        {
            case 1:
                return RedirectToPage("/ManagePosts");
            case 2:
                return RedirectToPage("/LogIn");
            default:
                return RedirectToPage("/ManagePosts");
        }
    }

}
