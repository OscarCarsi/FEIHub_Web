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
    [TempData]
    public string SuccessDelete { get; set; }
    [BindProperty]
    public string idComment {get; set;}
    [BindProperty]
    public string commentEdited {get; set;}
    public string visibilityComment {get; set;}



    public CompletePostModel(){
        PostInformation = new Posts();
        postOwner = "none";
        visibilityComment = "block";
        thisComment = "";
        if (SingletonUser.Instance.Rol == "ADMIN")
        {
            visibilityComment = "none";
        }
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
        return RedirectToPage("/EditPost", new {idPost = idThisPost, title = titleThisPost});
    }
    public async Task<IActionResult> OnPostDeletePost(){
        int page = await DeletePost();
        switch (page)
        {
            case 1:
                return RedirectToPage("/ManagePosts");
            case 2:
                return RedirectToPage("/MainPage");
            case 3:
                return RedirectToPage("/LogIn");
            case 4:
                return RedirectToPage("/CompletePost", new {idPost = idThisPost, titlePost = titleThisPost});
            default:
                return RedirectToPage("/CompletePost", new {idPost = idThisPost, titlePost = titleThisPost});
        }
    }
    public async Task<IActionResult> OnPostEditComment(string idComment, string idPost){
        //ErrorMessage= "Edit comment" + " idComment: " + idComment + " idPost: " + idPost + " comment: " + commentEdited;
        await EditComment(idComment, commentEdited, idPost);
        return RedirectToPage("/CompletePost", new {idPost = idThisPost, titlePost = titleThisPost});
    }
    public async Task<IActionResult> OnPostDeleteComment(string idComment, string idPost){
        await DeleteComment(idComment, idPost);
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
    public async Task<IActionResult> OnPostReport(){
        await ReportThisPost();
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
    private async Task ReportThisPost()
    {
        HttpResponseMessage response =  await postsAPIServices.AddReport(idThisPost, 1);
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
    private async Task DeleteComment(string idComment, string idPost)
    {
        Posts postCommented = await postsAPIServices.DeleteComment(idComment, idPost);
        if (postCommented.StatusCode == System.Net.HttpStatusCode.OK)
        {
            SuccessMessage = "Comentario eliminado con éxito";
        }
        if (postCommented.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
            SingletonUser.Instance.BorrarSinglenton();
        }
        if (postCommented.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            ErrorMessage = "Tuvimos un error al borrar el comentario, inténtalo más tarde";
        }
    }

          /**
          * Returns:
          * 1 if you have to return to manage posts
          * 2 if you have to return to main page
          * 3 if you have to return to logIn
          * 4 if you have to return to this page
          */
    private async Task<int> DeletePost()
    {
        int page = 4;
        PostInformation.id = idThisPost;
        HttpResponseMessage response = await postsAPIServices.DeletePost(PostInformation);
        if (response.IsSuccessStatusCode)
        {
            SuccessDelete = "Publicación eliminada";
            if(SingletonUser.Instance.Rol == "ADMIN")
            {
                page = 1;
                //return RedirectToPage("/ManagePosts");
            }
            else
            {
                page = 2;
                //return RedirectToPage("/MainPage");
            }
        }
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
            SingletonUser.Instance.BorrarSinglenton();
            page = 3;
            //return RedirectToPage("/LogIn");
        }
        if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            ErrorMessage = "No se pudo eliminar tu publicación inténtalo más tarde";
            //return RedirectToPage("/CompletePost", new {idPost = idThisPost, titlePost = titleThisPost});
        }
        return page;
    }
    private async Task EditComment(string idComment, string commentToEdit, string idPost)
    {
        if (!String.IsNullOrEmpty(commentToEdit))
        {
            Comment comment = new Comment();
            comment.commentId = idComment;
            comment.body = commentToEdit;
            Posts postCommented = await postsAPIServices.EditComment(comment, idPost);
            if (postCommented.StatusCode == System.Net.HttpStatusCode.OK)
            {
                SuccessMessage = "Comentario editado con éxito";
            }
            if (postCommented.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
                SingletonUser.Instance.BorrarSinglenton();
            }
            if (postCommented.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                ErrorMessage = "Tuvimos un error al editar el comentario, inténtalo más tarde";
            }
        }
        else
        {
            ErrorMessage = "El comentario no puede ir vacío";
        }
    }

}
