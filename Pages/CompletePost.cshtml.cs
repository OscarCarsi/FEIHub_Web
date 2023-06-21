using System.Reflection.Metadata;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FeiHub.Models;
using FeiHub.Services;
using FEIHub_Web.wwwroot.Resources;


namespace FEIHub_Web.Pages;

public class CompletePostModel : PageModel
{
    UsersAPIServices usersAPIServices = new UsersAPIServices();
    PostsAPIServices postsAPIServices = new PostsAPIServices();
    public Posts PostInformation {get; set;}
    public string postOwner {get; set;}
    public string thisComment {get; set;}
    [BindProperty]
    public string idThisPost {get; set;}
    [BindProperty]
    public string titleThisPost {get; set;}
    [TempData]
    public string ErrorMessage { get; set; }
    [TempData]
    public string CommentMessage { get; set; }
    [TempData]
    public string SuccessMessage { get; set; }



    public CompletePostModel(){
        PostInformation = new Posts();
        postOwner = "none";
        thisComment = "";
    }

    public async void SearchPostByIdAndTitle(string idPost, string titlePost)
    {
        var task = Task.Run(async () =>
        {
        PostInformation = await postsAPIServices.GetPostByIdAndTitle(idPost, titlePost);
        if (PostInformation.StatusCode == System.Net.HttpStatusCode.OK)
        {
            User userData = await usersAPIServices.GetUser(PostInformation.author);
            if (userData.profilePhoto == null)
            {
                userData.profilePhoto = "Resources/usuario.png";
            }
            PostInformation.AuthorUser = userData;
            if (PostInformation.comments != null )
            {
                for (int i = 0; i < PostInformation.comments.Count(); i++)
                {
                    User userComment = await usersAPIServices.GetUser(PostInformation.comments[i].author);
                    if (userComment.profilePhoto == null)
                    {
                        userComment.profilePhoto = "Resources/usuario.png";
                    }
                    PostInformation.comments[i].AuthorUser = new User();
                    PostInformation.comments[i].AuthorUser = userComment;
                    if (PostInformation.comments[i].author == SingletonUser.Instance.Username)
                    {
                        PostInformation.comments[i].commentOwner = "block";
                    }
                    else
                    {
                        PostInformation.comments[i].commentOwner = "none";
                    }
                }
            }
            if (PostInformation.author == SingletonUser.Instance.Username){
                postOwner = "block";
            }
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

    private async Task<bool> CommentPost()
    {
        bool ComentarioExitoso = false;
        var idPost = PostInformation.id;
        var body = thisComment;
        var date = DateTime.Now.Date;
        var author = SingletonUser.Instance.Username;
        Comment comment = new Comment();
        comment.author = author;
        comment.dateOfComment = date;
        comment.body = body;
        Posts postCommented = await postsAPIServices.AddComment(comment, idPost);
        if (postCommented.StatusCode == System.Net.HttpStatusCode.OK)
        {
            ComentarioExitoso = true;
        }
        if (postCommented.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
            SingletonUser.Instance.BorrarSinglenton();
        }
        if (postCommented.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            ErrorMessage = "Tuvimos un error al crear el comentario, inténtalo más tarde";
        }
        return ComentarioExitoso;
    }

    public async Task  OnGet(string idPost, string titlePost)
    {
        SearchPostByIdAndTitle(idPost, titlePost);
    }

    public async Task<IActionResult>  OnPostComment(string idPost, string titlePost, string comment)
    {

        SearchPostByIdAndTitle(idPost, titlePost);
        thisComment = comment;
        if (!String.IsNullOrWhiteSpace(thisComment))
        {
            var sucessfullComment = await CommentPost();
            if (sucessfullComment)
            {
                return RedirectToPage("/CompletePost", new {idPost = PostInformation.id, titlePost = PostInformation.title});
            }
        }
        else
        {
            CommentMessage = "El comentario no puede ir vacío";
        }
        return Page();
    }
    public IActionResult OnPostEditPost()
    {
        return RedirectToPage("/EditPost", new {idPost = idThisPost});
    }

    public IActionResult OnPostDeletePost(){
        ErrorMessage= "Delete";
        return RedirectToPage("/CompletePost", new {idPost = idThisPost, titlePost = titleThisPost});
    }
    public IActionResult OnPostEditComment(string idComment, string idPost){
        ErrorMessage= "Edit comment";
        return RedirectToPage("/CompletePost", new {idPost = idThisPost, titlePost = titleThisPost});
    }
    public IActionResult OnPostDeleteComment(string idComment, string idPost){
        ErrorMessage= "Delete comment";
        return RedirectToPage("/CompletePost", new {idPost = idThisPost, titlePost = titleThisPost});
    }

    public async Task<IActionResult> OnPostLike(){
        await LikePost();
        return RedirectToPage("/CompletePost", new {idPost = idThisPost, titlePost = titleThisPost});
    }
    public async Task<IActionResult> OnPostDislike(){
        await DislikePost();
        return RedirectToPage("/CompletePost", new {idPost = idThisPost, titlePost = titleThisPost});
    }
    public IActionResult OnPostReport(){
        ErrorMessage= "Report";
        return RedirectToPage("/CompletePost", new {idPost = idThisPost, titlePost = titleThisPost});
    }
    private async Task LikePost()
    {
        PostInformation.likes++;
        HttpResponseMessage response = await postsAPIServices.AddLike(idThisPost);
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
        PostInformation.dislikes++;
        HttpResponseMessage response = await postsAPIServices.AddDislike(idThisPost);
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

}
