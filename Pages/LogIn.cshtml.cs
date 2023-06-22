using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FeiHub.Models;
using FeiHub.Services;
using FEIHub_Web.wwwroot.Resources;


namespace FEIHub_Web.Pages;

public class LoginModel : PageModel
{
    UsersAPIServices usersAPIServices = new UsersAPIServices();
    SingletonUser user = SingletonUser.Instance;

    [BindProperty]
    public String username {get; set;}

    [BindProperty]
    public String password {get; set;}
    [TempData]
    public string Message { get; set; }


    public void OnGet()
    {

    }


    public async Task<IActionResult> OnPostLogin()
    {
        bool withoutFieldsNull = ValidateNullFieldsLogin();
        if (withoutFieldsNull)
        {

            string usernameLogin = username;
            string passwordLogin = Encryptor.Encrypt(password);
            UserCredentials userCredentials = await usersAPIServices.GetUserCredentials(usernameLogin, passwordLogin);
            if (userCredentials.StatusCode == System.Net.HttpStatusCode.OK)
            {
                user.Username = userCredentials.username;
                user.Rol = userCredentials.rol;
                user.Token = userCredentials.token;
                if(userCredentials.rol == "ADMIN")
                {
                    Message = $"Bienvenido (a), {userCredentials.username}!";
                    return RedirectToPage("/ManagePosts");
                }
                else
                {
                    Message = $"Bienvenido (a), {userCredentials.username}!";
                    return RedirectToPage("/MainPage");
                }
            }
            else
            {
                Message = "Verifica tus credenciales";
            }
        }
        else
        {
            Message = "No puedes dejar campos vacíos";
        }
        return Page();

    }
    public bool ValidateNullFieldsLogin()
        {
            bool fullFields = false;
            if (!String.IsNullOrWhiteSpace(password) && !String.IsNullOrWhiteSpace(username))
            {
                fullFields = true;
            }
            return fullFields;
        }

    public void OnPostLogOut(){
        SingletonUser.Instance.BorrarSinglenton();
    }
}
