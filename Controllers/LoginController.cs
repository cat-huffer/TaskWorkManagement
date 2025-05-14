using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskWorkManagement.Models;
using TaskWorkManagement.Data;

namespace Login.Controllers
{
    public class LoginController : Controller
    {
        private TaskWorkManagementContext TaskWorkManagementContext;

        public LoginController(TaskWorkManagementContext context)
        {
            TaskWorkManagementContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
            {
                ViewBag.Msg = "用户名或密码为空";
                return View("Index", user);
            }
            else
            {
                var item = TaskWorkManagementContext.User.FirstOrDefault(i => i.UserName == user.UserName && i.Password == user.Password);
                if (item != null)
                {
                    HttpContext.Session.SetInt32("UserId", item.Id);
                    return Redirect("/Home");
                }
                else
                {
                    ViewBag.Msg = "用户名或密码验证错误";
                    return View("Index", user);
                }

            }
        }
    }
}