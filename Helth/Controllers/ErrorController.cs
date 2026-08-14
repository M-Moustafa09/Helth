using System.Diagnostics;
using Helth.Models;
using Microsoft.AspNetCore.Mvc;

namespace Helth.Controllers;

public class ErrorController : Controller
{
    [Route("/Error")]
    public IActionResult Index()
    {
        return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
