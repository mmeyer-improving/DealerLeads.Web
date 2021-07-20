using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DealerLead.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DealerLead.Web.Controllers
{
    
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DealerLeadDBContext _context;

        public HomeController(ILogger<HomeController> logger, DealerLeadDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var user = this.User;

            ViewBag.isAuthenticatedUser = user.Identity.IsAuthenticated;
            if (user.Identity.IsAuthenticated)
            {
                var oid = GetOid();
                ViewBag.Oid = oid;
                ViewBag.DealerLeadUser = await _context.DealerLeadUser.FirstOrDefaultAsync(x => x.AzureADId.Equals(oid));
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(string oid)
        {
            var newUser = new DealerLeadUser();
            newUser.AzureADId = Guid.Parse(oid);
            await _context.AddAsync<DealerLeadUser>(newUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private Guid GetOid()
        {
            var user = this.User;

            var oid = user.Identities.ElementAt(0).Claims.ElementAt(2).Value;

            return Guid.Parse(oid);
        }

        [AllowAnonymous]
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
