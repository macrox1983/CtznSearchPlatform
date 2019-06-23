using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prometheus.CtznSearch.ApiService.ApiModel;
using Prometheus.CtznSearch.ApiService.DataContext;
using Prometheus.CtznSearch.ApiService.HostedServices;
using Prometheus.CtznSearch.ApiService.Options;
using Prometheus.Infrastructure.Component;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Prometheus.CtznSearch.ApiService.Controllers
{
    public class Version
    {
        public string Major { get; set; }

        public string Minor { get; set; }
    }

    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("CtznSearchApi")]
    public class CtznSearchApiController : ControllerBase
    {
        private readonly CtznSearchRecognitionHostedService _hostedService;
        private readonly IComponentDbContextFactory<CtznSearchDbContext> _dbContextFactory;
        private readonly CtznSearchApiServiceOptions _options;

        public CtznSearchApiController(CtznSearchRecognitionHostedService hostedService, IComponentDbContextFactory<CtznSearchDbContext> dbContextFactory, CtznSearchApiServiceOptions options)
        {
            _hostedService = hostedService ?? throw new ArgumentNullException(nameof(hostedService));
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _options = options;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet()]
        [Microsoft.AspNetCore.Mvc.Route("Version")]
        public Version Version()
        {
            return new Version { Major = "1", Minor = "0" };
        }

        [Microsoft.AspNetCore.Mvc.HttpGet()]
        [Microsoft.AspNetCore.Mvc.Route("GetState")]
        public async Task<ResponseData> GetState()
        {
            return new ResponseData<string>("available");
        }

        [Microsoft.AspNetCore.Mvc.HttpPost()]
        [Microsoft.AspNetCore.Mvc.Route("QuickLogin")]
        public async Task<ResponseData> QuickLogin([FromForm(Name = "Name")]string name, [FromForm(Name = "Phone")]string phone)
        {
            return await Executor.Try(async ()=> 
            {
                using (var context = _dbContextFactory.Create())
                {
                    var user = new Model.ApplicationUser
                    {
                        UserId = Guid.NewGuid(),
                        Login = phone,
                        Phone = phone,
                        UserName = name,
                        Email = "df@",
                        RoleId = (int)Model.Roles.Applicant,
                        PasswordHash = new byte[] { },
                        PasswordSalt = new byte[] { },
                        IsDeleted = false
                    };
                    context.ApplicationUser.Add(user);

                    try
                    {
                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    HttpContext.Response.Cookies.Append("uid", Convert.ToString(user.UserId));
                    return new ResponseData();
                }
            });            
        }

        [Microsoft.AspNetCore.Mvc.HttpPost()]
        [Microsoft.AspNetCore.Mvc.Route("CreateSearchTicket")]
        public async Task<ResponseData> CreateSearchTicket(SearchTicketData ticketData)
        {
            return await Executor.Try(async()=> 
            {
                using (var context = _dbContextFactory.Create())
                {

                    var newTicket = new Model.SearchTicket
                    {
                        SearchTicketId = Guid.NewGuid(),
                        SearchTicketTime = DateTime.Now,
                        LostPersonName = ticketData.LostPersonName,
                        LostPersonBirthDate = ticketData.LostPersonBirthDate,
                        LostPersonDescription = ticketData.LostPersonDescription,
                        LostDateTime = ticketData.LostDateTime,
                        LastLongitude = ticketData.LastLongitude,
                        LastLatitude = ticketData.LastLatitude,
                        IsMale = ticketData.IsMale,
                        LostPersonEyeColor = ticketData.LostPersonEyeColor,
                        LostPersonHairColor = ticketData.LostPersonHairColor,
                        LostPersonGrowth = ticketData.LostPersonGrowth,
                        LostPersonWeight = ticketData.LostPersonWeight,
                        LostPersonPhoto = ticketData.LostPersonPhoto,
                        UserId = ticketData.UserId,
                        SearchTicketStatus = (int)Model.SearchTicketStatuses.Active
                    };
                    context.SearchTicket.Add(newTicket);
                    await context.SaveChanges();
                    return new ResponseData<Guid>(newTicket.SearchTicketId);
                }
            });            
        }

        [Microsoft.AspNetCore.Mvc.HttpPost()]
        [Microsoft.AspNetCore.Mvc.Route("FinishSearchTicket")]
        public async Task<ResponseData> FinishSearchTicket(Guid ticketId)
        {
            return await Executor.Try(async () =>
            {
                using (var context = _dbContextFactory.Create())
                {

                    var ticket = context.SearchTicket.FirstOrDefault(st=>st.SearchTicketId.Equals(ticketId));
                    if (ticket != null)
                    {
                        ticket.SearchTicketStatus = (int)Model.SearchTicketStatuses.Finished;
                        context.SearchTicket.Update(ticket);
                        await context.SaveChanges();
                    }
                    return new ResponseData<Guid>(ticketId);
                }
            });
        }

        [Microsoft.AspNetCore.Mvc.HttpPost()]
        [Microsoft.AspNetCore.Mvc.Route("Detect")]
        [RequestSizeLimit(2147483648)]
        public async Task<ResponseData> Detect([FromForm]IFormFile file,[FromForm] decimal lon, [FromForm]decimal lat, [FromForm]int id)
        {
            return await Executor.Try(async () =>
            {
                var stream = new MemoryStream();
                file.CopyTo(stream);
                stream.Position = 0;

                if(await _hostedService.Detect(stream.ToArray(), Path.GetExtension(file.FileName),id))
                {
                    return new ResponseData();
                    //if (!string.IsNullOrEmpty(_options.RequestUriAfterDetect))
                    //{
                    //    var httpClient = new HttpClient();
                    //    var builder = new UriBuilder(_options.RequestUriAfterDetect);
                    //    var query = HttpUtility.ParseQueryString(builder.Query);
                    //    query["id"] = Convert.ToString(id);
                    //    query["lon"] =  Convert.ToString(lon);
                    //    query["lat"] =  Convert.ToString(lat);
                    //    builder.Query = query.ToString();
                    //    string url = builder.ToString();
                    //    await httpClient.GetAsync(url);
                    //}
                }                
                return new ResponseData(1, null);
            });
        }

        //[Microsoft.AspNetCore.Mvc.HttpPost()]
        //[Microsoft.AspNetCore.Mvc.Route("FaceForSearch")]
        //[RequestSizeLimit(2147483648)]
        //public async Task<ResponseData> FaceForSearch([FromForm]IFormFile image)
        //{
        //    return await Executor.Try(async () =>
        //    {
        //        var stream = new MemoryStream();
        //        image.CopyTo(stream);
        //        stream.Position = 0;

        //        _hostedService.AddImageForSearch(stream.ToArray(), Guid.NewGuid(), image.FileName);
        //        //using (var context = _dbContextFactory.Create())
        //        //{

        //        //    var ticket = context.SearchTicket.FirstOrDefault(st => st.SearchTicketId.Equals(ticketId));
        //        //    if (ticket != null)
        //        //    {
        //        //        ticket.SearchTicketStatus = (int)Model.SearchTicketStatuses.Finished;
        //        //        context.SearchTicket.Update(ticket);
        //        //        await context.SaveChanges();
        //        //    }
        //        //    return new ResponseData<Guid>(ticketId);
        //        //}
        //        return new ResponseData(0, null);
        //    });
        //}
    }

    public class Executor
    {
        public static async Task<ResponseData> Try<TResult>(Func<Task<TResult>> action) where TResult:ResponseData
        {
            try
            {                
                return await action();
            }
            catch (Exception ex)
            {
                return await HandleException(ex);
            }
        }

        private static async Task<ResponseData> HandleException(Exception exception)
        {
            //return new ResponseData($"{exception.Message}{Environment.NewLine}{exception.InnerException?.InnerException.Message}");
            return new ResponseData(1, $"{exception.Message}{Environment.NewLine}{exception.InnerException?.InnerException.Message}{Environment.NewLine}{exception.StackTrace}");
        }
    }
}
