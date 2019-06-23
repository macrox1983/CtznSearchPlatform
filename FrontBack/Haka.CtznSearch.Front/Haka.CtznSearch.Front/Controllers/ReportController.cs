using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Haka.CtznSearch.Front;
using BreakingStorm.Common.Loggers;

namespace Haka.CtznSearch.Front.Controllers
{
    public class ReportController : Controller
    {
        private readonly Logger _logger;
        public ReportController(Logger logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Bug(string message, string stack)
        {
            this._logger.Error(String.Format("BugReport: message:{0} stack:{1}", message, stack));
            return new JsonResult(new QueryResult(QueryResultEnum.Executed));
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        
    }
}
