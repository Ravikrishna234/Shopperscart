using Shoppingcart.DatabAccess.Data;
using Shoppingcart.DatabAccess.Repository.IRepository;
using Shoppingcart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppingcart.DatabAccess.Repository
{
    public class ProductRepository: Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(Product product)
        {
            var productFromPostAction = _db.Products.FirstOrDefault(p => p.Id == product.Id);
            if (productFromPostAction != null)
            {
                productFromPostAction.Title = product.Title;
                productFromPostAction.Description = product.Description;
                productFromPostAction.ISBN = product.ISBN;
                productFromPostAction.Author = product.Author;
                productFromPostAction.ListPrice = product.ListPrice;
                productFromPostAction.Price = product.Price;
                productFromPostAction.Price50 = product.Price50;
                productFromPostAction.Price100 = product.Price100;
                if (productFromPostAction.ImageUrl != null) { 
                    productFromPostAction.ImageUrl = product.ImageUrl;
                }
            }
            
            _db.Products.Update(productFromPostAction);
        }
    }
}
