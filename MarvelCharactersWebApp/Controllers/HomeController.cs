using MarvelCharactersWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

namespace MarvelCharactersWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _baseUrl = "http://gateway.marvel.com/v1/public/characters";
        private readonly string _timestamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
        private readonly string _publicApiKey = "c73680becadfae11f251241311bf89a8";
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, IConfiguration config, HttpClient httpClient)
        {
            _logger = logger;
            _config = config;
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            var url = $"{_baseUrl}?ts={_timestamp}&apikey={_publicApiKey}&hash={GetHash()}";


            // 1. Send request to endpoint (url)
           var marvelResponse =  _httpClient.GetStringAsync(url).Result;

            // 2. Parse JSON, refer to Kanye exercise
            var marvelObject = JObject.Parse(marvelResponse);

            var instance = new MarvelProperties()
            {
                Name = marvelObject["data"]["results"][0]["name"].ToString()
            };

            return View(instance);
        }

        private string GetHash() 
        {

            // https://developer.marvel.com/documentation/authorization




            var privateApiKey = _config["Marvel:PrivateKey"];
            var md5 = MD5.Create();
            var input = $"{_timestamp}{privateApiKey}{_publicApiKey}";

            var hash = string.Empty;
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                hash = BitConverter.ToString(data).Replace("-", string.Empty).ToLower();
            }
            return hash;

            

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
    }
}