using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>,IProductRepository
    {
        private ApplicationDbContext _Db;
        public ProductRepository(ApplicationDbContext dbContext):base(dbContext) 
        {
            _Db = dbContext;
        }
        public void Save()
        {
            _Db.SaveChanges();
        }
        public void Update(Product obj)
        {
            var ObjFromDb = _Db.Product.FirstOrDefault(x => x.Id == obj.Id);
            if (ObjFromDb != null) 
            {
                ObjFromDb.Title = obj.Title;
                ObjFromDb.Description = obj.Description;
                ObjFromDb.CategoryId = obj.CategoryId;
                ObjFromDb.Description =obj.Description;
                ObjFromDb.ListPrice = obj.ListPrice;
                ObjFromDb.Price = obj.Price;
                ObjFromDb.Price50 = obj.Price50;
                ObjFromDb.Price100 = obj.Price100;
                ObjFromDb.Author = obj.Author;
                ObjFromDb.ISBN = obj.ISBN;
                ObjFromDb.ListPrice = obj.ListPrice;
                if(obj.ImageUrl !=null)
                {
                    ObjFromDb.ImageUrl = obj.ImageUrl;
                }

            }
           // _Db.Product.Update(obj);
        }
    }
}
