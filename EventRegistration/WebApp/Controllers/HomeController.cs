using System.Diagnostics;
using App.Contracts.DAL;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAppUOW _uow;


    public HomeController(ILogger<HomeController> logger, IAppUOW uow)
    {
        _logger = logger;
        _uow = uow;
    }

    public async Task<IActionResult> Index()
    {
        var events = await _uow.EventRepository.AllAsync();
        var now = DateTime.UtcNow;

        var vm = new EventViewModel
        {
            FutureEvents = events.Where(e => e.DateTime > now).OrderBy(e => e.DateTime).ToList(),
            PastEvents = events.Where(e => e.DateTime <= now).OrderByDescending(e => e.DateTime).ToList(),
        };
        return View(vm);
    }

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