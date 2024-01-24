using Shoppingcart.DatabAccess.Data;
using Shoppingcart.DatabAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppingcart.DatabAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _contextDb;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }

        public UnitOfWork(ApplicationDbContext contextDb) { 
            _contextDb = contextDb;
            Category = new CategoryRepository(_contextDb);
            Product = new ProductRepository(_contextDb);

        }
        public void Save()
        {
            _contextDb.SaveChanges();
        }
    }
}
