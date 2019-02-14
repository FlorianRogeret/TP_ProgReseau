using IutInfo.ProgReseau.SignalR.WebUi.Hubs;
using IutInfo.ProgReseau.SignalR.WebUi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IutInfo.ProgReseau.SignalR.WebUi.Controllers
{
    public class HomeController : Controller
    {
        private IHubContext<MessengerHub> m_context;
        public HomeController(IHubContext<MessengerHub> context) // On demande au framework de charger notre contexte de Hub
        {
            m_context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Messenger()
        {
            // Une fois charger, on peut consommer les événements proposés par le Hub pour agir sur les clients
            await m_context.Clients.All.SendAsync("ReceiveMessage", "system", "Nouveau user");
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
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
}