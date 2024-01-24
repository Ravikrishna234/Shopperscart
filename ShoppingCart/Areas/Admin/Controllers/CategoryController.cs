using Microsoft.AspNetCore.Mvc;
using Shoppingcart.DatabAccess.Data;
using Shoppingcart.Models;
using Shoppingcart.DatabAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Shoppingcart.Utility;

namespace ShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    // To restrict access to the user with url. If url is known to any other user we should restrict it 
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork db)
        {
            _unitOfWork = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }
        public IActionResult Create()
        {
            return View(new Category());
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            /*Console.WriteLine("hello", category.Id);*/
            //Custom Validations
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Display Order cannot exactly match the name");
            }
            if (category.Name.Length < 4)
            {
                ModelState.AddModelError("", "Name should be atleast 4 characters");
            }
            if (category != null && ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();

                /*return View();*/
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index", "Category"); // Action, Controller
            }
            return View();
        }
        public IActionResult Edit(int? id) {
            if (id == null || id == 0) {
                TempData["error"] = "Error Occured";
                return NotFound();
            }
            Category? categoryfromdb = _unitOfWork.Category.Get(u=>u.Id==id);// Only works on primary key 
            if (categoryfromdb == null)
            {
                return NotFound();
            }
            return View(categoryfromdb);

        }
        //public IActionResult Delete() { }
        [HttpPost]
        public IActionResult Edit(Category category) {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Display Order cannot exactly match the name");
            }
            if (category.Name.Length < 4)
            {
                ModelState.AddModelError("", "Name should be atleast 4 characters");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();
                TempData["success"] = "Category Updated Successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }
        public IActionResult Delete(int? id) {
            if (id == null || id == 0)
            {
                TempData["error"] = "Error Occured";
                return NotFound();
            }
            Category? categoryfromdb = _unitOfWork.Category.Get(u => u.Id == id);// Only works on primary key 
            if (categoryfromdb == null)
            {
                return NotFound();
            }
            return View(categoryfromdb);

        }
        [HttpPost, ActionName("Delete")]
        public IActionResult Delete_From_Db(int? id)
        {
            Category? _obj = _unitOfWork.Category.Get(u => u.Id == id);
            if (_obj == null) { return NotFound(); }
            _unitOfWork.Category.Remove(_obj);
            _unitOfWork.Save();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index", "Category");
            //return View(); 
        }
    }
}

