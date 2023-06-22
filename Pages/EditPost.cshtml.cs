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
    PostsAPIServices postsAPIServices = new PostsAPIServices();
    SingletonUser user = SingletonUser.Instance;
    public Posts PostInformation {get; set;}
    public User thisUser {get; set;}
    [TempData]
    public string ErrorMessage { get; set; }
    [TempData]
    public string SuccessMessage {get; set;}
    [BindProperty]
    public string idThisPost {get; set;}
    [BindProperty]
    public string titleThisPost {get; set;}
    [BindProperty]
    public String title {get; set;}
    [BindProperty]
    public String body {get; set;}
    [BindProperty]
    public String target {get; set;}

    public void OnGet(string idPost, string title, string newTitle = "", string newBody = ""){
        SearchPostByIdAndTitle(idPost, title);
        if (!String.IsNullOrWhiteSpace(newTitle))
        {
            PostInformation.title = newTitle;
        }
        if (!String.IsNullOrWhiteSpace(newBody))
        {
            PostInformation.body = newBody;
        }
    }
    public async void SearchPostByIdAndTitle(string idPost, string titlePost)
    {
        var task = Task.Run(async () =>
        {
        PostInformation = await postsAPIServices.GetPostByIdAndTitle(idPost, titlePost);
        if (PostInformation.StatusCode == System.Net.HttpStatusCode.OK)
        {
            if(PostInformation.target == "EVERYBODY")
            {
                PostInformation.target= "Todos";
            }
            if (PostInformation.target == "ACADEMIC")
            {
                PostInformation.target = "Académicos";
            }
            if (PostInformation.target == "STUDENT")
            {
                PostInformation.target = "Estudiantes";
            }
        }
        if (PostInformation.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
            SingletonUser.Instance.BorrarSinglenton();
        }
        if (PostInformation.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            ErrorMessage = "Tuvimos un error al mostrar la publicación, inténtalo más tarde";
        }
         });
        task.GetAwaiter().GetResult();
        task.Dispose();
    }

    [HttpPost]
    public async Task<IActionResult> OnPostEditPost()
    {
        bool withoutFieldsNull = ValidateNullFields();
        if (withoutFieldsNull)
        {
            if (target == "none")
            {
                ErrorMessage = "Debe seleccionar la audiencia a la que será dirigida la publicación";
                return RedirectToPage("/EditPost", new {
                    idPost = idThisPost,
                    title = titleThisPost,
                    newTitle = title,
                    newBody = body,
                });
            }
            else
            {
                Posts postToEdit = new Posts()
                {
                    id = idThisPost,
                    title = this.title,
                    body = this.body,
                    target = this.target,
                    dateOfPublish = DateTime.Now
                };
                HttpResponseMessage response = await postsAPIServices.EditPost(postToEdit);
                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Publicación editada exitosamente";
                    return RedirectToPage("/CompletePost", new {
                        idPost = idThisPost,
                        titlePost = postToEdit.title
                    });
                }
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
                    SingletonUser.Instance.BorrarSinglenton();
                }
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    ErrorMessage = "Tuvimos un error al editar tu publicación, inténtalo más tarde";
                }
            }
        }
        else
        {
            ErrorMessage = "No puede dejar campos vacíos";
        }
        return RedirectToPage("/EditPost", new {
            idPost = idThisPost,
            title = titleThisPost
        });
    }
    private bool ValidateNullFields()
    {
        bool fullFields = false;
            if (!String.IsNullOrWhiteSpace(title) && !String.IsNullOrWhiteSpace(body))
            {
                fullFields = true;
            }
            return fullFields;
    }

}
