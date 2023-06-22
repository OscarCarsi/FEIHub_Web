using System.Reflection.Metadata;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FeiHub.Models;
using FeiHub.Services;
using FEIHub_Web.wwwroot.Resources;


namespace FEIHub_Web.Pages;

public class SearchResultsModel : PageModel
{
    UsersAPIServices usersAPIServices = new UsersAPIServices();
    public List<User> usersObtained = new List<User>();
    public string StringToSearch {get; set;}
    [TempData]
    public string ErrorMessage {get; set;}


    public SearchResultsModel(){

    }

    public void OnGet(string search){
        StringToSearch = search;
        AddUsersFind(StringToSearch);
    }

    public async void AddUsersFind(string usernameFind)
    {
        var task = Task.Run(async () =>
        {
            usersObtained = await usersAPIServices.FindUsers(usernameFind);
            if (usersObtained.Count > 0)
            {
                if (usersObtained[0].StatusCode == System.Net.HttpStatusCode.OK)
                {
                    for (int i = 0; i < usersObtained.Count; i++)
                    {
                        if (usersObtained[i].profilePhoto == null)
                        {
                            usersObtained[i].profilePhoto = "Resources/usuario.png";
                        }
                    }
                }
                if (usersObtained[0].StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ErrorMessage = "Su sesión expiró, vuelve a iniciar sesión";
                    SingletonUser.Instance.BorrarSinglenton();
                }
                if (usersObtained[0].StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    ErrorMessage = "Tuvimos un error al obtener a quiénes sigues, inténtalo más tarde";
                }
            }
        });
        task.GetAwaiter().GetResult();
        task.Dispose();
    }


}
