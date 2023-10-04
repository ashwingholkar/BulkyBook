using BulkyBookLab.Data;
using BulkyBookLab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookLab.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _context.Categories.ToListAsync(); 
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
                _context.Categories.Add(objCategory);
                _context.SaveChanges();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View(objCategory);

        }

        public IActionResult Edit(int? id)
        {
            if(id == null || id==0)
            {
                return NotFound();
            }
            var categoryFromDb =  _context.Categories.Find(id);   
            if(categoryFromDb == null) { return NotFound(); }
            return View(categoryFromDb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Edit(Category objCategory)
        {
            if (objCategory.Name == objCategory.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "display order cannot be same as Name, Please try to use different");
            }
            if (ModelState.IsValid)
            {
                _context.Categories.Update(objCategory);
                _context.SaveChanges();
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
            var categoryFromDb = _context.Categories.Find(Id);
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
            var category = _context.Categories.Find(id);
            _context.Categories.Remove(category);
            _context.SaveChanges();
            TempData["delete"] = "Category deleted successfully";
            return RedirectToAction("Index");



        }
    }
}
