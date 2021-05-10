using AzureFunctionClientProjectDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionClientProjectDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        string Baseurl = "http://localhost:7071";

       
        public async Task<ActionResult> GetTodos()
        {
            List<Todo> Todoinfo = new List<Todo>();

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(Baseurl);

                    client.DefaultRequestHeaders.Clear();

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("api/memorytodo/");

                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;

                        Todoinfo = JsonConvert.DeserializeObject<List<Todo>>(Response);


                    }
                    
                    return View(Todoinfo);
                }
                catch (Exception e)
                {
                    ViewBag.message = "No Connection established.Please run API's";
                }
                return View(Todoinfo);
            }
        }


        public IActionResult AddTodo()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {

                ViewBag.message = e.Message;
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddTodo(Todo t)
        {
            Todo obj = new Todo();
            
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(t), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("http://localhost:7071/api/memorytodo/", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    obj = JsonConvert.DeserializeObject<Todo>(apiResponse);
                    
                  

                }
            }
            return RedirectToAction("GetTodos");
        }

        [HttpGet]
        public async Task<ActionResult> UpdateTodoDetails(int id)
        {
            Todo obj = new Todo();


            using (var httpClient = new HttpClient())
            {
                try
                {
                    using (var response = await httpClient.GetAsync("http://localhost:7071/api/memorytodo/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        obj = JsonConvert.DeserializeObject<Todo>(apiResponse);


                    }
                }
                catch(Exception e)
                {
                    ViewBag.message = e.Message;
                }
            }
            return View(obj);

        }

        [HttpPost]
        public async Task<ActionResult> UpdateTodoDetails(Todo t)
        {
            Todo todoobj = new Todo();

            using (var httpClient = new HttpClient())
            {
                string id = t.Id.ToString();

                StringContent content1 = new StringContent(JsonConvert.SerializeObject(t), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PutAsync("http://localhost:7071/api/memorytodo/" + id, content1))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ViewBag.Result = "Success";

                    todoobj = JsonConvert.DeserializeObject<Todo>(apiResponse);


                }
            }
            return RedirectToAction("GetTodos");
        }


        [HttpGet]
        public async Task<ActionResult> DeleteTodoList(int id)
        {
            Todo todoobj = new Todo();
            using (var httpClient = new HttpClient())
            {
                try
                {
                    using (var response = await httpClient.GetAsync("http://localhost:7071/api/memorytodo/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        todoobj = JsonConvert.DeserializeObject<Todo>(apiResponse);
                    }

                }
                catch (Exception e)
                {

                    ViewBag.message=e.Message;
                }
                
            }
            return View(todoobj);
        }


        [HttpPost]
        public async Task<ActionResult> DeleteTodoList(Todo t)
        {
            string Id = t.Id;
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync("http://localhost:7071/api/memorytodo/" + Id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }

            return RedirectToAction("GetTodos");
        }








    }
}
