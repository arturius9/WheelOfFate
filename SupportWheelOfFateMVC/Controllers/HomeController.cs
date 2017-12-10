using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SupportWheelOfFateMVC.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using SupportWheelOfFateWebApi.Data;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace SupportWheelOfFateMVC.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;
        string _webApiBaseUrl;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
            _webApiBaseUrl = _configuration["WebApi:url"];
        }

        public async Task<ActionResult> Index()
        {
            return View();
        }

        public async Task<ActionResult> BAU([FromRoute] DateTime? date)
        {
            if (date.HasValue)
            {
                ViewData["date"] = date.Value.ToString("yyyy-MM-dd");
                return await GetResponseFromApi($"api/Baus/{ViewData["date"]}");
            }
            else
            {
                ViewData["date"] = DateTime.Today.ToString("yyyy-MM-dd");
                return View(new List<BAU>());
            }
        }

        private async Task<ActionResult> GetResponseFromApi(string requestUrl)
        {
            var bauInfo = new List<BAU>();

            try
            {
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(_webApiBaseUrl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetBau using HttpClient  
                    HttpResponseMessage res = await client.GetAsync(requestUrl);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var bauResponse = res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Person list  
                        bauInfo = JsonConvert.DeserializeObject<List<BAU>>(bauResponse);
                        bauInfo = bauInfo.OrderBy(x => x.Date).ToList();
                    }
                    //returning the Person list to view  
                    return View(bauInfo);
                }
            }
            catch
            {
                return Redirect(Url.Action("Error"));
            }
        }

        public async Task<ActionResult> AllBAUs()
        {
            return await GetResponseFromApi($"api/Baus/");
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("PostDate")]
        public ActionResult PostDate(DateTime date)
        {
            if (!ModelState.IsValid)
            {
                return Redirect(Url.Action("BAU")); //BadRequest(ModelState);
            }
            return Redirect(Url.Action("BAU", "Home") + "/" + date.ToString("yyyy-MM-dd"));
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("DeleteAllBaus")]
        public async Task<ActionResult> DeleteAllBaus()
        {
            if (!ModelState.IsValid)
            {
                return Redirect(Url.Action("Error")); //BadRequest(ModelState);
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_webApiBaseUrl);

                    HttpResponseMessage res = await client.DeleteAsync("api/BAUs");

                    if (!res.IsSuccessStatusCode)
                    {
                        return Redirect(Url.Action("Error")); //BadRequest(ModelState);
                    }

                    var result = res.Content.ReadAsStringAsync().Result;
                    ViewData["info"] = "Number of BAUs deleted: " + result.ToString();
                }
                return View("AllBAUs");
            }
            catch
            {
                return Redirect(Url.Action("Error"));
            }
        }

        public async Task<ActionResult> Employees()
        {
            List<Person> peopleInfo = new List<Person>();
            try
            {
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(_webApiBaseUrl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetPeople using HttpClient  
                    HttpResponseMessage Res = await client.GetAsync("api/People");

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var peopleResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Person list  
                        peopleInfo = JsonConvert.DeserializeObject<List<Person>>(peopleResponse);

                    }
                    //returning the Person list to view  
                    return View(peopleInfo);
                }
            }
            catch
            {
                return Redirect(Url.Action("Error"));
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
