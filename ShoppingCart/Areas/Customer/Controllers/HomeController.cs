using Microsoft.AspNetCore.Mvc;
using Shoppingcart.DatabAccess.Repository.IRepository;
using Shoppingcart.Models;
using System.Diagnostics;

namespace ShoppingCart.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties:"Category");
            return View(productList);
        }

        public IActionResult Details(int productId) // Parameter should be same as asp-route-paramId in Index.cshtml
        {
            Product product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties:"Category");
            return View(product);
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
