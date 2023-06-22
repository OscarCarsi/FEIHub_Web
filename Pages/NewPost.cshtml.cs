using System.Reflection.Metadata;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FeiHub.Models;
using FeiHub.Services;
using FEIHub_Web.wwwroot.Resources;


namespace FEIHub_Web.Pages;

public class NewPostModel : PageModel
{
    UsersAPIServices usersAPIServices = new UsersAPIServices();
    PostsAPIServices postsAPIServices = new PostsAPIServices();
    S3Service s3Service = new S3Service();
    SingletonUser user = SingletonUser.Instance;
    public Posts newPost {get; set;}
    [BindProperty]
    public String title {get; set;}
    [BindProperty]
    public String body {get; set;}
    [BindProperty]
    public String target {get; set;}
    [BindProperty]
    public List<IFormFile> files { get; set; }
    [TempData]
    public string SuccessMessage {get; set;}
    [TempData]
    public string ErrorMessage {get; set;}

    public NewPostModel(){
        newPost = new Posts();
    }
    public void OnGet(string title = "", string body = "")
    {
        newPost.title = title;
        newPost.body = body;
    }


    public async Task<IActionResult> OnPostNewPost(){
        bool withoutFieldsNull = ValidateNullFields();
        if (withoutFieldsNull)
        {
            if(target == "none"){
                ErrorMessage = "Debe seleccionar la audiencia a la que será dirigida la publicación";
                return RedirectToPage("/NewPost", new {
                    title = this.title,
                    body = this.body
                });
            }
            else{
                Posts postToCreate = new Posts()
                {
                    title = this.title,
                    body = this.body,
                    target = this.target,
                    author = SingletonUser.Instance.Username,
                    photos = new Photo[0],
                    dateOfPublish = DateTime.Now
                };
                Posts postCreated = await postsAPIServices.CreatePost(postToCreate);
                if (postCreated.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    if(files != null && files.Count > 0){
                        int counter = 1;
                        List<Photo> tempPhotos = new List<Photo>(postCreated.photos);
                        foreach(var file in files){
                            if (file != null && file.Length > 0){
                                if (file.ContentType.StartsWith("image/")){
                                    string customName = $"{postCreated.id}{counter++}";
                                    bool uploadSuccess = await s3Service.UploadImage(file.Name, customName, file);
                                    if (uploadSuccess)
                                    {
                                        string imageUrl = s3Service.GetImageURL(customName);
                                        tempPhotos.Add(new Photo { url = imageUrl });
                                    }
                                    else
                                    {
                                        ErrorMessage = "Ocurrió un error al guardar las imágenes de la publicación";
                                    }
                                }
                            }
                        }
                        postCreated.photos = tempPhotos.ToArray();
                        HttpResponseMessage response = await postsAPIServices.EditPost(postCreated);
                        if (response.IsSuccessStatusCode)
                        {
                            SuccessMessage = "Publicación creada exitosamente, puedes regresar a la pantalla principal";
                        }
                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
                            SingletonUser.Instance.BorrarSinglenton();
                            return RedirectToPage("/LogIn");
                        }
                        if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        {
                            ErrorMessage = "Tuvimos un error al crear tu publicación, inténtalo más tarde";
                        }
                    }
                    else
                    {
                        postCreated.photos = new Photo[0];
                        SuccessMessage = "Publicación creada exitosamente, puedes regresar a la pantalla principal";
                    }
                }
                if (postCreated.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
                    SingletonUser.Instance.BorrarSinglenton();
                    return RedirectToPage("/LogIn");
                }
                if (postCreated.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    ErrorMessage = "Tuvimos un error al crear tu publicación, inténtalo más tarde";
                }
            }
        }
        else
        {
            ErrorMessage = "No puede dejar campos vacíos";
            return RedirectToPage("/NewPost", new {
                title = this.title,
                body = this.body
            });
        }
        return Page();
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
