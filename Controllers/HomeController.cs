using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wall.Models;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;

namespace Wall.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbConnector _dbConnector;
        public HomeController(DbConnector connect)
        {
            _dbConnector = connect;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPostAttribute]
        [RouteAttribute("register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {
                string insertString = $@"INSERT INTO users (first_name, last_name, email, password, created_at, updated_at)
                                        VALUES ('{user.FirstName}', '{user.LastName}', '{user.Email}', '{user.Password}', NOW(), NOW());
                                        SELECT LAST_INSERT_ID()";
                _dbConnector.Execute(insertString);
                List<Dictionary<string, object>> newGuy = _dbConnector.Query($"SELECT id FROM users WHERE email = '{user.Email}'");
                System.Console.WriteLine(newGuy[0]["id"]);
                HttpContext.Session.SetInt32("id", (int)newGuy[0]["id"]);
                return RedirectToAction("Index", "Wall");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPostAttribute]
        [RouteAttribute("sup")]
        public IActionResult Login(LogUser user)
        {
            System.Console.WriteLine("login method");
            System.Console.WriteLine(user);
            // if password matches user with email, redirect to success
            List<Dictionary<string, object>> query = _dbConnector.Query($"SELECT password, id FROM users WHERE email = '{user.LogEmail}'");
            if(query.Count < 1 || user.LogPassword != (string)query[0]["password"])
            {
                return View("Index");
            }
            // else return with view
            HttpContext.Session.SetInt32("id", (int)query[0]["id"]);
            return RedirectToAction("Index", "Wall");
            
        }
        

        // [HttpGetAttribute]
        // [RouteAttribute("success")]
        // public IActionResult Success()
        // {
        //     int id = (int)HttpContext.Session.GetInt32("id");
        //     List<Dictionary<string, object>> query = _dbConnector.Query($"SELECT first_name FROM users WHERE id = {id}");
        //     User theUser = new User
        //     {
        //         FirstName = (string)query[0]["first_name"]
        //     };
        //     return View(theUser);
        // }
    }
}
