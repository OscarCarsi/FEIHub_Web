using System.Reflection.Metadata;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FeiHub.Models;
using FeiHub.Services;
using FEIHub_Web.wwwroot.Resources;


namespace FEIHub_Web.Pages;

public class EditProfileModel : PageModel
{
    UsersAPIServices usersAPIServices = new UsersAPIServices();
    S3Service s3Service = new S3Service();
    public User thisUser {get; set;}
    [BindProperty]
    public String name {get; set;}
    [BindProperty]
    public String paternalSurname {get; set;}
    [BindProperty]
    public String maternalSurname {get; set;}
    [TempData]
    public string ErrorMessage { get; set; }
    [TempData]
    public string SuccessMessage { get; set; }
    [BindProperty]
    public IFormFile file { get; set; }

    public async Task GetUser(string username)
    {
        var task = Task.Run(async () =>
        {
            thisUser = await usersAPIServices.GetUser(username);
            if (thisUser.profilePhoto == null)
            {
                thisUser.profilePhoto = "Resources/usuario.png";
            }
        });
        task.GetAwaiter().GetResult();
        task.Dispose();
    }

    public EditProfileModel()
    {
        thisUser = new User();
    }

    public void OnGet()
    {
        GetUser(SingletonUser.Instance.Username);
    }
    public IActionResult OnPostCancel()
    {
        return RedirectToPage("/Profile", new {username = SingletonUser.Instance.Username});
    }

    public async Task<IActionResult> OnPostEdit()
    {
        if (!String.IsNullOrWhiteSpace(name) && !String.IsNullOrWhiteSpace(paternalSurname))
        {
            User userUpdated = new User();
            userUpdated.username = SingletonUser.Instance.Username;
            userUpdated.name = name;
            userUpdated.paternalSurname = paternalSurname;
            userUpdated.maternalSurname = maternalSurname;
            if (file != null && file.Length > 0)
            {
                if (file.ContentType.StartsWith("image/"))
                {
                    bool uploadSuccess = await s3Service.UploadImage(file.Name, SingletonUser.Instance.Username, file);
                    if (!uploadSuccess)
                    {
                        ErrorMessage = "Ocurrió un error al actualizar tu foto de perfil";
                    }
                }
                else
                {
                    ErrorMessage = "La foto de perfil debe ser de formato de imagen.";
                    return RedirectToPage("/EditProfile");
                }
            }
            string imageUrl = s3Service.GetImageURL(SingletonUser.Instance.Username);
            userUpdated.profilePhoto = imageUrl;
            HttpResponseMessage response = await usersAPIServices.EditUser(userUpdated);
            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Perfil editado correctamente";
                return RedirectToPage("/Profile", new {username = SingletonUser.Instance.Username});
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
                SingletonUser.Instance.BorrarSinglenton();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                ErrorMessage = "Tuvimos un error al editar tu perfil, inténtalo más tarde";
            }
        }
        else
        {
            ErrorMessage = "El nombre y el apellido paterno son obligatorios";
        }
        return RedirectToPage("/EditProfile");
    }
}
