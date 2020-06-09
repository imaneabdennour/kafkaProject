using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KafkaNet;
using KafkaNet.Model;
using Microsoft.AspNetCore.Mvc;
using kafka.Models;

namespace kafka.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.message = "";
            return View();
        }
        [HttpPost]
        public IActionResult Index(string k)
        {
            Uri uri = new Uri("http://localhost:9092");
            string topic = "messageTopic";

            //  Si message est vide :
            if (Request.Form["message"] == string.Empty)    
            {
                ViewBag.message = "Please Enter Message";
                return View("index");
            }

            // Sinon :
            string txtMessage = Request.Form["message"];
            string payload = txtMessage.Trim();
            var sendMessage = new Thread(() => {
                //Method Message() encodes message to byte streams. Most of the time a message will be string based.
                KafkaNet.Protocol.Message msg = new KafkaNet.Protocol.Message(payload);
                var options = new KafkaOptions(uri);
                var router = new BrokerRouter(options);
                var client = new Producer(router);
                client.SendMessageAsync(topic, new List<KafkaNet.Protocol.Message> { msg }).Wait();
            });

            // Starting the Thread :
            sendMessage.Start();
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
    }
}
