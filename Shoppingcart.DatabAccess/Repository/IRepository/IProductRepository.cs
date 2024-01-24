using Shoppingcart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppingcart.DatabAccess.Repository.IRepository
{
    public interface IProductRepository: IRepository<Product>
    {
        void update(Product product);
    }
}
