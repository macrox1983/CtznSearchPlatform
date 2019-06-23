using Haka.CtznSearch.Front.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

using BreakingStorm.Common.Extensions;

namespace Haka.CtznSearch.Front.Controllers
{
    public class AjaxAPIController : Controller
    {
        private readonly DBContext _db;
        private readonly IHostingEnvironment _env;
        public AjaxAPIController(DBContext db, IHostingEnvironment env)
        {
            _db = db;
            _env = env;
        }
        [HttpPost]
        public IActionResult SearchUpdatePosition(string lat, string lon, int ticketId)
        {
            if (!HttpContext.Request.Cookies.TryGetValue("uid", out string uid)) return new JsonResult(new QueryResult(QueryResultEnum.Error_SessionError));

            using (var trans = _db.Database.BeginTransaction())
            {
                int lastId = _db.SearchTracks.Select(s => s.Id).OrderBy(s => s).LastOrDefault();
                lastId++;
                SearchTrack st = new SearchTrack() { Id = lastId, TicketId = ticketId };
                st.UserId = int.Parse(uid);
                st.Longitude = lon;
                st.Latitude = lat;
                _db.SearchTracks.Add(st);
                _db.SaveChanges();
                trans.Commit();
            }
            return new JsonResult(new QueryResult(QueryResultEnum.Executed));
        }
        [HttpPost]
        public IActionResult GetSearchPositions(int ticketId, int lastId)
        {
            if (!HttpContext.Request.Cookies.TryGetValue("uid", out string uid)) return new JsonResult(new QueryResult(QueryResultEnum.Error_SessionError));
            var list = _db.SearchTracks.Where(s => s.TicketId == ticketId && s.Id > lastId).ToList();            
            return new JsonResult(new QueryResult(QueryResultEnum.Executed, list.ToJSON()));
        }
        [HttpPost]
        public IActionResult Registration(string name, string phone)
        {
            string clearPhone = "";
            for (int i = 0; i < phone.Length; i++)
                if (char.IsNumber(phone.ElementAt(i))) clearPhone += phone.ElementAt(i);
            using (var trans = _db.Database.BeginTransaction())
            {
                var user = _db.Users.Where(u => u.Phone == clearPhone).FirstOrDefault();
                if (user == null)
                {
                    var lastId = _db.Users.OrderBy(s => s.Id).Select(s => s.Id).LastOrDefault();
                    user = new User() { Id = lastId + 1, Name = name, Phone = clearPhone };
                    _db.Users.Add(user);
                    _db.SaveChanges();
                }               
                HttpContext.Response.Cookies.Append("uid", user.Id.ToString());
                trans.Commit();
            }
            return new JsonResult(new QueryResult(QueryResultEnum.Executed));
        }

        [HttpPost]
        public async Task<ActionResult> AddTicket(IFormFile file, string name, string lastTime, int ages, string descr, string lon, string lat)
        {
            if (!HttpContext.Request.Cookies.TryGetValue("uid", out string uid)) return new JsonResult(new QueryResult(QueryResultEnum.Error_SessionError));
            Task<MemoryStream> uploadTask = Task.Run<MemoryStream>(() =>
            {
                MemoryStream memoryStream = new MemoryStream();
                try { file.CopyTo(memoryStream); }
                catch { return null; }
                return memoryStream;
            });
            MemoryStream imageStream = await uploadTask;
            using (var trans = _db.Database.BeginTransaction())
            {
                int lastId = _db.Tickets.Select(t => t.Id).OrderBy(t => t).LastOrDefault();
                lastId++;
                using (FileStream filestream = new FileStream(_env.ContentRootPath + "/wwwroot/images/missings/" + lastId + ".jpg", FileMode.Create))
                {
                    imageStream.Position = 0;
                    imageStream.WriteTo(filestream);
                }
                imageStream.Dispose();
                Ticket ticket = new Ticket();
                ticket.Id = lastId;
                ticket.Description = descr;
                ticket.Ages = ages;
                ticket.Name = name;
                if (DateTime.TryParse(lastTime, out DateTime lastTimeDT)) ticket.Last = lastTimeDT; else ticket.Last = DateTime.Now - TimeSpan.FromHours(2);
                ticket.Longitude = lon;
                ticket.Latitude = lat;
                ticket.Created = DateTime.Now;
                ticket.Status = TicketStatusEnum.Open;
                ticket.Zone = String.Join("|", LoadZones(ticket).ToArray());
                ticket.CreatorId = int.Parse(uid);


                _db.Tickets.Add(ticket);
                _db.SaveChanges();
                trans.Commit();
                return new JsonResult(new QueryResult(QueryResultEnum.Executed, ticket.Id));
            }            
        }

        
        private List<string> LoadZones(Ticket ticket)
        {
            string html = string.Empty;
            string lonStr = ticket.Longitude.ToString().Replace(',', '.');
            string latStr = ticket.Latitude.ToString().Replace(',', '.');
            List<string> result = new List<string>();
            int range = 1800;
            for (int i = 0; i < 3; i++)
            {
                string url = @"https://isoline.route.api.here.com/routing/7.2/calculateisoline.json?mode=fastest%3Bpedestrian%3B&start=" + latStr + "%2C" + lonStr + "&rangetype=time&range=" + range + "&app_id=LAd4jQiGovsTycoXaEpi&app_code=yVa8wakZVG2TI4gB1WfEEQ";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                    result.Add(html);
                }
                range += 1800;
            }
            return result;

        }
    }
}
