using Microsoft.AspNetCore.Mvc;
using Shoppingcart.DatabAccess.Data;
using Shoppingcart.Models;
using Shoppingcart.DatabAccess.Repository.IRepository;
using Shoppingcart.DatabAccess.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shoppingcart.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Shoppingcart.Utility;
namespace ShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    // To restrict access to the user with url. If url is known to any other user we should restrict it 
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class ProductController: Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork _db, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = _db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objproductslist = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            
            return View(objproductslist);
        }
        public IActionResult Upsert(int? id) // Combining Create + Update == Upsert => Update + Insert
        {
            // Without Using View Model - To combine multiple models into 1 object we can use below implementation of ViewModel
            //IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            //{
            //    Text = u.Name,
            //    Value = u.Id.ToString(),
            //});
            //// ViewBag.CategoryList = CategoryList;  // Key:Value
            // ViewData["CategoryList"] = CategoryList;  // ViewData is a dictionarytype so we need to follow dictionary syntax
            ProductVM productVM = new ProductVM()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                }),
                Product = new Product()
            };
            if(id == null || id == 0)
            {
                //create
                return View(productVM);
            } else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
            
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file) {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath; // To access the root folder
                if(file != null)
                {
                    string file_name = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string product_path = Path.Combine(wwwRootPath, @"images\product");

                    // Suppose if we need to update the image then we need to delete the old image and replace it with new one.
                    if(!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete the old image
                        // need to remove '\' in image path to identify the file
                        var oldImage_path = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImage_path))
                        {
                            System.IO.File.Delete(oldImage_path);
                        }
                    }

                    using(var fileStream = new FileStream(Path.Combine(product_path, file_name), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + file_name;
                }
                if(productVM.Product.Id != 0)
                {
                    _unitOfWork.Product.update(productVM.Product);
                    TempData["success"] = "Product Updated Successfully";
                }
                else
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product Created Successfully";

                }
                _unitOfWork.Save();
                return RedirectToAction("Index", "Product");

            } else { // If ModelState.IsInvalid that means over view of product gets rendered but the Categorylist will not be rendered to populate the CategoryList
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                });
            return View(productVM);

            }
        }
        //public IActionResult Edit(int? id) {
        //    if (id == null || id == 0)
        //    {
        //        TempData["error"] = "Error Occured";
        //        return NotFound();
        //    }
        //    Product? productfromdb = _unitOfWork.Product.Get(u => u.Id == id);
        //    if(productfromdb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productfromdb);
        //}
        //[HttpPost]
        //public IActionResult Edit(Product product) {
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Add(product);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product Edited Successfully";
        //        return RedirectToAction("Index", "Product");
        //    }
        //    return View();
        //}
        //public IActionResult Delete(int? id) {
        //    if (id == null || id == 0)
        //    {
        //        TempData["error"] = "Error Occured";
        //        return NotFound();
        //    }
        //    Product? productfromdb = _unitOfWork.Product.Get(u => u.Id == id);
        //    if (productfromdb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productfromdb);
        //}
            //[HttpPost, ActionName("Delete")]
            //public IActionResult Delete_From_Db(int? id)
            //{
            //    Product? _obj = _unitOfWork.Product.Get(u => u.Id == id);
            //    if (_obj == null) { return NotFound(); }
            //    _unitOfWork.Product.Remove(_obj);
            //    _unitOfWork.Save();
            //    TempData["success"] = "Product Deleted Successfully";
            //    return RedirectToAction("Index", "Product");
            //}

        #region API CALLS
        // An API Call for Datatable implementation
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objproductslist = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objproductslist });
        }
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            var oldImage_path = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImage_path))
            {
                System.IO.File.Delete(oldImage_path);
            }
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Product Deleted Successfully" });
        }
        #endregion

    }
}
