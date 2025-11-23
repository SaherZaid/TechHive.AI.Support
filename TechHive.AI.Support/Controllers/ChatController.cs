using Microsoft.AspNetCore.Mvc;
using TechHive.AI.Support.Models.Chat;
using TechHive.AI.Support.Services;

namespace TechHive.AI.Support.Controllers;


public class ChatController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
