using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FeiHub.Models;
using FeiHub.Services;
using FEIHub_Web.wwwroot.Resources;
using EmailValidation;

namespace FEIHub_Web.Pages;

public class SingInModel : PageModel
{
    UsersAPIServices usersAPIServices = new UsersAPIServices();
    SingletonUser user = SingletonUser.Instance;

    [BindProperty]
    public String activeButton {get; set;}
    [TempData]
    public string Message { get; set; }
    [BindProperty]
    public String studentName {get; set;}
    [BindProperty]
    public String studentPaternalSurname {get; set;}
    [BindProperty]
    public String studentMaternalSurname {get; set;}
    [BindProperty]
    public String studentSchoolId {get; set;}
    [BindProperty]
    public String studentEducationalProgram {get; set;}
    [BindProperty]
    public String studentUsername {get; set;}
    [BindProperty]
    public String studentPassword {get; set;}
    [BindProperty]
    public String academicName {get; set;}
    [BindProperty]
    public String academicPaternalSurname {get; set;}
    [BindProperty]
    public String academicMaternalSurname {get; set;}
    [BindProperty]
    public String academicEmail {get; set;}
    [BindProperty]
    public String academicUsername {get; set;}
    [BindProperty]
    public String academicPassword {get; set;}
    public string emailStudent;
    public string rolAcademic = "ACADEMIC";
    public string rolStudent = "STUDENT";
    public void OnGet()
    {
        
    }    
    

    public async Task<IActionResult> OnPostSingin()
    {   
        if(activeButton == "ACADEMIC"){
            bool withoutNullFields = ValidateNullFieldsAcademic();
            if (withoutNullFields)
            {
                bool correctFields = ValidateFieldsAcademic();
                if (correctFields)
                {
                    Credentials credentialsAcademic = new Credentials();
                    credentialsAcademic.username = academicUsername;
                    credentialsAcademic.password = Encryptor.Encrypt(academicPassword);;
                    credentialsAcademic.email = academicEmail;
                    credentialsAcademic.rol = rolAcademic;
                    User userAcademic = new User();
                    userAcademic.username = academicUsername;
                    userAcademic.name = academicName;
                    userAcademic.paternalSurname = academicPaternalSurname;
                    userAcademic.maternalSurname = academicMaternalSurname;
                    string validateExistingUser = await usersAPIServices.GetExistingUser(academicEmail);
                    if (validateExistingUser != academicEmail)
                    {

                        User validateUsername = await usersAPIServices.GetUser(academicUsername);
                        if (validateUsername == null) 
                        {
                        
                            HttpResponseMessage responseCredentials = await usersAPIServices.CreateCredencials(credentialsAcademic);
                            if (responseCredentials.IsSuccessStatusCode)
                            {
                                HttpResponseMessage responseUser = await usersAPIServices.CreateUser(userAcademic, rolAcademic);
                                if (responseUser.IsSuccessStatusCode)
                                {
                                    Message ="Usuario creado exitosamente, inicia sesión para ingresar";
                                    clearFields();
                                }
                                else
                                {
                                    Message ="No se pudo crear tu usuario, inténtalo más tarde";
                                    clearFields();
                                }
                            }
                            else
                            {
                                Message = "No se pudieron crear tus credenciales, inténtalo más tarde";
                                clearFields();
                            }
                        }
                        else
                        {
                            Message = "Nombre de usuario en uso, ingresa otro nombre de usuario";
                            clearFields();
                        }
                    }
                    else
                    {
                        Message = "Ya tienes una cuenta en este sistema, ve al inicio de sesión e ingresa tus credenciales de acceso";
                        clearFields();
                    }
                }
                else
                {
                    Message = "Verifica los datos proporcionados";
                    clearFields();
                }
            }
            else
            {
                Message = "No puedes dejar campos vacíos";
                clearFields();
            }
        }
        else{
            bool withoutNullFields = ValidateNullFieldsStudents();
            if (withoutNullFields)
            {
            bool correctFields = ValidateFieldsStudent();
                if (correctFields)
                {
                    emailStudent = "z" + studentSchoolId.Substring(0, 1).ToLower() + studentSchoolId.Substring(1, 8) + "@estudiantes.uv.mx";
                    Credentials credentialsStudent = new Credentials();
                    credentialsStudent.username = studentUsername;
                    credentialsStudent.password = Encryptor.Encrypt(studentPassword);;
                    credentialsStudent.email = emailStudent;
                    credentialsStudent.rol = rolStudent;
                    User userStudent = new User();
                    userStudent.username = studentUsername; 
                    userStudent.name = studentName;
                    userStudent.paternalSurname = studentPaternalSurname;
                    userStudent.maternalSurname = studentMaternalSurname;
                    userStudent.educationalProgram = studentEducationalProgram;
                    userStudent.schoolId = studentSchoolId;
                    string validateExistingUser = await usersAPIServices.GetExistingUser(emailStudent);
                    if(validateExistingUser != emailStudent)
                    {

                        User validateUsername = await usersAPIServices.GetUser(studentUsername);
                        if (validateUsername == null)
                        {

                            HttpResponseMessage responseCredentials = await usersAPIServices.CreateCredencials(credentialsStudent);
                            if (responseCredentials.IsSuccessStatusCode)
                            {
                                HttpResponseMessage responseUser = await usersAPIServices.CreateUser(userStudent, rolStudent);
                                if (responseUser.IsSuccessStatusCode)
                                {
                                    Message = "Usuario creado exitosamente, inicia sesión para ingresar";
                                    clearFields();
                                }
                                else
                                {
                                    Message = "No se pudo crear tu usuario, inténtalo más tarde";
                                    clearFields();
                                }
                            }
                            else
                            {
                                Message = "No se pudieron crear tus credenciales, inténtalo más tarde";
                                clearFields();
                            }
                        }
                        else
                        {
                            Message = "Nombre de usuario en uso, ingresa otro nombre de usuario";
                            clearFields();
                        }
                    }
                    else
                    {
                        Message ="Ya tienes una cuenta en este sistema, ve al inicio de sesión e ingresa tus credenciales de acceso";
                        clearFields();
                    }
                }
                else
                {
                    Message ="Verifica los datos proporcionados";
                    clearFields();
                }
            }
            else
            {
                Message = "No puedes dejar campos vacíos";
                clearFields();
            }
        }
        return Page();

    }
    public bool ValidateSpecialCharacter(string verify)
    {
        bool withoutSpecialCharacter = true;
        string specialCharacteres = "*#+-_;.@%&/()=!?¿¡{}[]^<>";
        foreach (char character in verify)
        {
            if (character >= '0' && character <= '9')
            {
                withoutSpecialCharacter = false;
            }
        }
        foreach (char character in verify)
        {
            foreach (char specialCharacter in specialCharacteres)
            {
                if (character == specialCharacter)
                {
                    withoutSpecialCharacter = true;
                }

            }

            if (withoutSpecialCharacter)
            {
                break;
            }
        }
        return withoutSpecialCharacter;
    }
    public bool ValidateFieldsStudent()
    {
        bool correctField = false;
        correctField = studentSchoolId.Length == 9;
        correctField = correctField && studentSchoolId.Substring(0, 1) == "S";
        correctField = correctField && ValidateSpecialCharacter(studentName);
        correctField = correctField && ValidateSpecialCharacter(studentPaternalSurname);
        correctField = correctField && ValidateSpecialCharacter(studentMaternalSurname);
        return correctField;
    }
    public bool ValidateFieldsAcademic()
    {
        bool correctField = false;
        correctField = EmailValidator.Validate(academicEmail);
        correctField = correctField && ValidateSpecialCharacter(academicName);
        correctField = correctField && ValidateSpecialCharacter(academicPaternalSurname);
        correctField = correctField && ValidateSpecialCharacter(academicMaternalSurname);
        correctField = correctField && academicEmail.EndsWith("@uv.mx", StringComparison.OrdinalIgnoreCase);
        return correctField;
    }
    public bool ValidateNullFieldsAcademic()
    {
        bool fullFields = false;
        if (!String.IsNullOrWhiteSpace(academicName) && !String.IsNullOrWhiteSpace(academicPaternalSurname) && !String.IsNullOrWhiteSpace(academicMaternalSurname) && !String.IsNullOrWhiteSpace(academicEmail) && !String.IsNullOrWhiteSpace(academicPassword) && !String.IsNullOrWhiteSpace(academicUsername))
        {
            fullFields = true;
        }
        return  fullFields;
    }
    public bool ValidateNullFieldsStudents()
    {
        bool fullFields = false;
        if (!String.IsNullOrWhiteSpace(studentName) && !String.IsNullOrWhiteSpace(studentPaternalSurname) && !String.IsNullOrWhiteSpace(studentMaternalSurname) && !String.IsNullOrWhiteSpace(studentSchoolId) && !String.IsNullOrWhiteSpace(studentPassword))
        {
            fullFields = true;
        }
        return fullFields;
    }

    public void clearFields()
    {
        studentName = "" ;
        studentPaternalSurname = "";
        studentMaternalSurname = "";
        studentSchoolId = "";
        studentEducationalProgram = "";
        studentUsername = "";
        studentPassword = "";
        academicName = "";
        academicPaternalSurname = "";
        academicMaternalSurname = "";
        academicEmail = "";
        academicUsername = "";
        academicPassword = "";
        emailStudent = "";
    }
    
}
