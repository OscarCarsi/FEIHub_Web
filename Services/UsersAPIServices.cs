using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using FeiHub.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Amazon.Runtime.Internal;

namespace FeiHub.Services
{
    public class UsersAPIServices
    {
        private readonly HttpClient httpClient;

        public UsersAPIServices()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:8083/apiusersfeihub");
        }
        public async Task<UserCredentials> GetUserCredentials(string username, string password)
        {
            try
            {
                string apiUrl = "/credentials/login";
                var requestData = new { username, password };
                string jsonRequest = JsonConvert.SerializeObject(requestData);
                StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(httpClient.BaseAddress + apiUrl, content);

                UserCredentials userCredentials = new UserCredentials();
                userCredentials.StatusCode = response.StatusCode;
                string jsonResponse = await response.Content.ReadAsStringAsync();
                userCredentials = JsonConvert.DeserializeObject<UserCredentials>(jsonResponse);
                userCredentials.StatusCode = response.StatusCode;
                return userCredentials;
            }
            catch (Exception ex)
            {
                UserCredentials userCredentials = new UserCredentials();
                userCredentials.username = null;
                userCredentials.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return userCredentials;
            }
        }
        public async Task<HttpResponseMessage> CreateCredencials(Credentials newCredentials)
        {
            try
            {
                string apiUrl = "/credentials";
                string jsonRequest = JsonConvert.SerializeObject(newCredentials);
                StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(httpClient.BaseAddress + apiUrl, content);
                return response;
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return response;
            }
        }
        public async Task<HttpResponseMessage> CreateUser(User newUser, string rol)
        {
            try
            {
                string apiUrl = "/users";
                var requestData = new { username = newUser.username, name = newUser.name, paternalSurname = newUser.paternalSurname,
                maternalSurname = newUser.maternalSurname, schoolId = newUser.schoolId, educationalProgram = newUser.educationalProgram,
                rol = rol};
                string jsonRequest = JsonConvert.SerializeObject(requestData);
                StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(httpClient.BaseAddress + apiUrl, content);
                return response;
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return response;
            }
        }
        public async Task<User> GetUser(string username)
        {
                string apiUrl = $"/users/{username}";
                HttpResponseMessage response = await httpClient.GetAsync(httpClient.BaseAddress + apiUrl);
                User userObtained = new User();
                string jsonResponse = await response.Content.ReadAsStringAsync();
                userObtained = JsonConvert.DeserializeObject<User>(jsonResponse);
                return userObtained;
            
        }
        public async Task<string> GetExistingUser(string email)
        {
            try
            {
                string apiUrl = $"/credentials/{email}";
                HttpResponseMessage response = await httpClient.GetAsync(httpClient.BaseAddress + apiUrl);
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JsonDocument document = JsonDocument.Parse(jsonResponse);
                string emailObtained = document.RootElement.GetProperty("email").GetString();

                return emailObtained;

            }
            catch
            {
                return null;
            }
            
        }
        public async Task<List<User>> GetListUsersFollowing(string username)
        {
            try
            {
                string apiUrl = $"/follows/followingUsers/{username}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, httpClient.BaseAddress + apiUrl);
                request.Headers.Add("token", SingletonUser.Instance.Token);
                HttpResponseMessage response = await httpClient.SendAsync(request);
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JsonDocument document = JsonDocument.Parse(jsonResponse);
                List<User> userList = new List<User>();
                JsonElement root = document.RootElement;
                if(response.IsSuccessStatusCode)
                {
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement element in root.EnumerateArray())
                        {
                            User user = new User();
                            user.username = element.GetProperty("username").GetString();
                            user.name = element.GetProperty("name").GetString();
                            user.paternalSurname = element.GetProperty("paternalSurname").GetString();
                            user.maternalSurname = element.GetProperty("maternalSurname").GetString();
                            user.schoolId = element.GetProperty("schoolId").GetString();
                            user.educationalProgram = element.GetProperty("educationalProgram").GetString();
                            user.profilePhoto = element.GetProperty("profilePhoto").GetString();
                            user.StatusCode = System.Net.HttpStatusCode.OK;
                            userList.Add(user);
                        }
                    }

                    
                }
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                    User user = new User();
                    user.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    userList.Add(user) ;
                }
                return userList;
            }
            catch
            {
                List<User> userList = new List<User>();
                User user = new User();
                user.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                userList.Add(user);
                return userList;
            }

        }
        public async Task<List<User>> FindUsers(string username)
        {
            try
            {
                string apiUrl = $"/users/findUsers/{username}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, httpClient.BaseAddress + apiUrl);
                request.Headers.Add("token", SingletonUser.Instance.Token);
                HttpResponseMessage response = await httpClient.SendAsync(request);
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JsonDocument document = JsonDocument.Parse(jsonResponse);
                List<User> userList = new List<User>();
                JsonElement root = document.RootElement;
                if (response.IsSuccessStatusCode)
                {
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement element in root.EnumerateArray())
                        {
                            User user = new User();
                            user.username = element.GetProperty("username").GetString();
                            user.name = element.GetProperty("name").GetString();
                            user.paternalSurname = element.GetProperty("paternalSurname").GetString();
                            user.maternalSurname = element.GetProperty("maternalSurname").GetString();
                            user.schoolId = element.GetProperty("schoolId").GetString();
                            user.educationalProgram = element.GetProperty("educationalProgram").GetString();
                            user.profilePhoto = element.GetProperty("profilePhoto").GetString();
                            user.StatusCode = System.Net.HttpStatusCode.OK;
                            userList.Add(user);
                        }
                    }


                }
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    User user = new User();
                    user.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    userList.Add(user);
                }
                return userList;
            }
            catch
            {
                List<User> userList = new List<User>();
                User user = new User();
                user.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                userList.Add(user);
                return userList;
            }

        }
        public async Task<List<User>> GetListUsersFollowers(string username)
        {
            try
            {
                string apiUrl = $"/follows/followersUsers/{username}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, httpClient.BaseAddress + apiUrl);
                request.Headers.Add("token", SingletonUser.Instance.Token);
                HttpResponseMessage response = await httpClient.SendAsync(request);
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JsonDocument document = JsonDocument.Parse(jsonResponse);
                List<User> userList = new List<User>();
                JsonElement root = document.RootElement;
                if (response.IsSuccessStatusCode)
                {
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement element in root.EnumerateArray())
                        {
                            User user = new User();
                            user.username = element.GetProperty("username").GetString();
                            user.name = element.GetProperty("name").GetString();
                            user.paternalSurname = element.GetProperty("paternalSurname").GetString();
                            user.maternalSurname = element.GetProperty("maternalSurname").GetString();
                            user.schoolId = element.GetProperty("schoolId").GetString();
                            user.educationalProgram = element.GetProperty("educationalProgram").GetString();
                            user.profilePhoto = element.GetProperty("profilePhoto").GetString();
                            user.StatusCode = System.Net.HttpStatusCode.OK;
                            userList.Add(user);
                        }
                    }


                }
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    User user = new User();
                    user.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    userList.Add(user);
                }
                return userList;
            }
            catch
            {
                List<User> userList = new List<User>();
                User user = new User();
                user.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                userList.Add(user);
                return userList;
            }

        }
        public async Task<HttpResponseMessage> EditUser(User newUser)
        {
            try
            {
                string apiUrl = $"/users/{newUser.username}";
                string jsonRequest = JsonConvert.SerializeObject(newUser);
                StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                content.Headers.Add("token", SingletonUser.Instance.Token);
                HttpResponseMessage response = await httpClient.PutAsync(httpClient.BaseAddress + apiUrl, content);
                return response;
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return response;
            }
        }

        public async Task<List<string>> GetListFollowing(string username)
        {
            try
            {
                string apiUrl = $"/follows/following/{username}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, httpClient.BaseAddress + apiUrl);
                request.Headers.Add("token", SingletonUser.Instance.Token);
                HttpResponseMessage response = await httpClient.SendAsync(request);
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JsonDocument document = JsonDocument.Parse(jsonResponse);
                List<string> userList = new List<string>();
                JsonElement root = document.RootElement;
                if (response.IsSuccessStatusCode)
                {
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement element in root.EnumerateArray())
                        {
                            if (element.ValueKind == JsonValueKind.String)
                            {
                                string usernameObtained = element.GetString();
                                userList.Add(usernameObtained);
                            }
                        }
                    }
                }
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    userList.Add(System.Net.HttpStatusCode.Unauthorized.ToString());
                }
                return userList;
            }
            catch
            {
                List<string> userList = new List<string>();
                userList.Add(System.Net.HttpStatusCode.InternalServerError.ToString());
                return userList;
            }
        }
        public async Task<HttpResponseMessage> Follow(string userFollow, string myUsername)
        {
            try
            {
                string apiUrl = "/follows";
                var requestData = new
                {
                    follower = myUsername,
                    following = userFollow
                };
                string jsonRequest = JsonConvert.SerializeObject(requestData);
                StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                content.Headers.Add("token", SingletonUser.Instance.Token);
                HttpResponseMessage response = await httpClient.PostAsync(httpClient.BaseAddress + apiUrl, content);
                return response;
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return response;
            }
        }

        public async Task<HttpResponseMessage> Unfollow(string userFollow, string myUsername)
        {
            try
            {
                string apiUrl = $"/follows/{myUsername}/{userFollow}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, httpClient.BaseAddress + apiUrl);
                request.Headers.Add("token", SingletonUser.Instance.Token);
                HttpResponseMessage response = await httpClient.SendAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return response;
            }
        }

    }
}