using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using H5Encryption.Models;
using H5Encryption.Models.DB;
using H5Encryption.Data;
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace H5Encryption.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDbContext _context;
        private readonly IDataProtector _provider;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, MyDbContext context, IDataProtectionProvider provider, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _config = config;
            _provider = provider.CreateProtector(_config["SecretKey"]);
        }

        public IActionResult Index()
        {
            return Redirect("/Home/Login");
        }

        public IActionResult Privacy()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Redirect("/Home/Login");
            }

            return View();

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            user.Password = BC.HashPassword(user.Password);
            _context.User.Add(user);
            _context.SaveChanges();
            return Redirect("/home/login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {


            if (username == null || password == null)
            {
                ViewBag.Message = "Brugernavn og kodeord skal udfyldes";
                return View();
            }

            var user = _context.User.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                ViewBag.Message = "Ukendt brugernavn";
                return View();
            }
            else
            {
                if (BC.Verify(password, user.Password))
                {
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    return Redirect("/home/Todos");
                }
                else
                {
                    ViewBag.Message = "Forkert brugernavn eller kodeord";
                    return View();
                }
            }         
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/home/login");
        }

        /*
        [HttpGet]
        public IActionResult Todos()
        {
            return View();
        }*/

        
        [HttpGet]
        async public Task<IActionResult> Todos()
        {
            var UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return Redirect("/home/login");
            }

            ViewBag.User = await _context.User.Include(u => u.TodoItems).FirstOrDefaultAsync(u => u.Id == UserId);

            var todoItems = _context.TodoItem.Where(t => t.User.Id == UserId).ToList();

            foreach (TodoItem todo in ViewBag.User.TodoItems)
            {
                try
                {
                    todo.Title = _provider.Unprotect(todo.Title);
                    todo.Description = _provider.Unprotect(todo.Description);
                }
                catch (Exception ex)
                {
                
                }
            }

            return View();
        }

        [HttpPost]
        public IActionResult Todos(TodoItem todoItem)
        {
            var UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return Redirect("/home/login");
            }
            else
            {
                todoItem.User = _context.User.FirstOrDefault(u => u.Id == UserId);
                todoItem.Created = DateTime.Now;

                todoItem.Title = _provider.Protect(todoItem.Title);
                todoItem.Description = _provider.Protect(todoItem.Description);

                _context.TodoItem.Add(todoItem);
                _context.SaveChanges();
            }

            return Redirect("/home/todos");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
