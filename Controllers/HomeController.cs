using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ninjaGold.Controllers
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }

    public class HomeController : Controller
    {
        DateTime date = DateTime.Now;
        Random randNum = new Random();
        // GET: /Home/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            
            if(HttpContext.Session.GetInt32("gold") != null )
            {
                ViewBag.gold = HttpContext.Session.GetInt32("gold");
                ViewBag.activities = HttpContext.Session.GetObjectFromJson<List<string>>("Activities");
   
            }else{
                HttpContext.Session.SetInt32("gold", 0);
                ViewBag.gold = HttpContext.Session.GetInt32("gold");
                HttpContext.Session.SetObjectAsJson("Activities", new List<string>());
                ViewBag.activities = HttpContext.Session.GetObjectFromJson<List<string>>("Activities");
            }
            return View("index");
        }


        [HttpPost]
        [Route("process_money")]
        public IActionResult Method(string building)
        {
            int currentGold = (int)HttpContext.Session.GetInt32("gold");
            int goldEarned = 0;

            if(building == "Farm")
            {
                goldEarned = randNum.Next(10,20);

            }else if(building == "Cave")
            {
                goldEarned = randNum.Next(5,10);
                
            }else if(building == "House")
            {
                goldEarned = randNum.Next(2,5);
                
            }else if(building == "Casino")
            {
                goldEarned = randNum.Next(-50, 50);
                
            }
            currentGold += goldEarned;
            HttpContext.Session.SetInt32("gold", currentGold);

            if(goldEarned >= 0)
            {
                var activity = HttpContext.Session.GetObjectFromJson<List<string>>("Activities");
                activity.Add($"<p class='positive'>Earned {goldEarned} at the {building} at {date} </p>");
                HttpContext.Session.SetObjectAsJson("Activities", activity);
            }
            if(goldEarned < 0)
            {
                var activity = HttpContext.Session.GetObjectFromJson<List<string>>("Activities");
                activity.Add($"<p class='negative'>Lost {goldEarned} at the Casino at {date} </p>");
                HttpContext.Session.SetObjectAsJson("Activities", activity);
            }

            return RedirectToAction("index");
        }

        [HttpPost]
        [Route("reset")]
        public IActionResult Reset()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("index");
        }
    }
}
