using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {

        private readonly IUnitOfWork _uniOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment ;

        public ProductsController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _uniOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            IEnumerable<Product> product = _uniOfWork.Product.GetAll(includeProperties:"Category").ToList();

            return View(product);
        }
        public IActionResult Upsert(int? id) //updateInsert = upsert # update and insert in one functionality
        {
            //Projections in core 
            //IEnumerable<SelectListItem> CategoryLst = _uniOfWork.Category
            //    .GetAll()
            //    .Select(u => new SelectListItem
            //    {
            //        Text = u.Name,
            //        Value = u.Id.ToString(),
            //    });

            //ViewBag.CategoryLst = CategoryLst;
            //ViewData["CategoryLst"] = CategoryLst;


            ProductVM productVM = new() {
                CategoryList = _uniOfWork.Category
                .GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),
                Product = new Product()
            };
            if(id == null || id == 0) 
            {
                //create
                return View(productVM);
            }
            else
            {
                //update fucntionality
                productVM.Product = _uniOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM,IFormFile? file)
        {

            if (ModelState.IsValid) 
            {
                string wwwwRootPath = _webHostEnvironment.WebRootPath;
                if(file!=null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwwRootPath,@"Images\Product");
                 
                    if(!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var OldPath = Path.Combine(wwwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(OldPath))
                        {
                            System.IO.File.Delete(OldPath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create ))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\Images\Product\" + fileName;
                }
                if(productVM.Product.Id == 0)
                {
                    //Adding
                    _uniOfWork.Product.Add(productVM.Product);
                }
                else
                {//Updating
                    _uniOfWork.Product.Update(productVM.Product);   
                }
                _uniOfWork.Save();
                
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _uniOfWork.Category
                .GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });
            }

            return View(productVM);
        }

         
        //public async Task<IActionResult> Delete(int? Id)
        //{
        //    if (Id == null || Id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product productFromDb = _uniOfWork.Product.Get(u => u.Id == Id);
        //    if (productFromDb == null) { return NotFound(); }
        //    return View(productFromDb); ;
        //}
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeletePOST(int? id)
        //{
        //    if (id == null | id == 0)
        //    {
        //        return NotFound();

        //    }
        //    var Product =  _uniOfWork.Product.Get(u => u.Id == id);
        //    _uniOfWork.Product.Remove(Product);
        //    _uniOfWork.Save();
        //    TempData["delete"] = "Product deleted successfully";
        //    return RedirectToAction("Index");

        //}
        #region API Call
        [HttpGet]
        public ActionResult GetAll()
        {
            IEnumerable<Product> product = _uniOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = product });
        }
       [HttpDelete] 
        public ActionResult Delete(int? id)
        {

            var productToBeDeleted = _uniOfWork.Product.Get(u => u.Id == id);
            if(productToBeDeleted == null)
            {
                return Json(new { success= false,message ="Error while deleting"});
            }

            var oldImage = Path.Combine(_webHostEnvironment.WebRootPath,
                productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImage))
            {
                System.IO.File.Delete(oldImage);
            }

            _uniOfWork.Product.Remove(productToBeDeleted);
            _uniOfWork.Save();
            return Json(new { success = true,message = "Product is deleted"  });
        }


       
        }
        #endregion
    }

