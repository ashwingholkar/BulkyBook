using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
            return View(categories);
        }

        //Get 
        public async Task<IActionResult> Create()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category objCategory)
        {
            if (objCategory.Name == objCategory.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "display order cannot be same as Name, Please try to use different");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(objCategory);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View(objCategory);

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);

            if (categoryFromDb == null) { return NotFound(); }
            return View(categoryFromDb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category objCategory)
        {
            if (objCategory.Name == objCategory.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "display order cannot be same as Name, Please try to use different");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(objCategory);
                _unitOfWork.Save();
                TempData["edited"] = "Category Edited successfully";
                return RedirectToAction("Index");
            }
            return View(objCategory);

        }
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            Category categoryFromDb = _unitOfWork.Category.Get(u => u.Id == Id);
            if (categoryFromDb == null) { return NotFound(); }
            return View(categoryFromDb); ;
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            if (id == null | id == 0)
            {
                return NotFound();

            }
            var category = _unitOfWork.Category.Get(u => u.Id == id);
            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();
            TempData["delete"] = "Category deleted successfully";
            return RedirectToAction("Index");

        }
    }
}
