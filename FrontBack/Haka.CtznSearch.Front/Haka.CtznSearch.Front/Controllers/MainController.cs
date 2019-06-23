using Haka.CtznSearch.Front.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;


namespace Haka.CtznSearch.Front.Controllers
{
    public class MainController : Controller
    {
        private readonly DBContext _db;
        private readonly IHostingEnvironment _env;
        public MainController(DBContext db, IHostingEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Registration()
        {
            if (HttpContext.Request.Cookies.TryGetValue("uid", out string uid)) Response.Cookies.Delete("uid");
            return View();
        }

        public IActionResult AddTicket()
        {
            if (!HttpContext.Request.Cookies.TryGetValue("uid", out string uid)) return Redirect("/registration");
            return View();
        }

        public IActionResult TicketsList()
        {
            var list = _db.Tickets.Where(s => s.Status == TicketStatusEnum.Open).ToList();
            return View(list);
        }       

        public IActionResult TicketInfo(int ticketId)
        {
            var ticket = _db.Tickets.Where(t => t.Id == ticketId).FirstOrDefault();
            if (ticket == null) return Redirect("/error404");
            return View(ticket);
        }

        public IActionResult Search(int ticketId)
        {
            if (!HttpContext.Request.Cookies.TryGetValue("uid", out string uid)) return Redirect("/registration");
            ViewBag.uid = uid;
            var ticket = _db.Tickets.Where(t => t.Id == ticketId).FirstOrDefault();
            if (ticket == null) return Redirect("/error404");
            return View(ticket);
        }
    }
}
