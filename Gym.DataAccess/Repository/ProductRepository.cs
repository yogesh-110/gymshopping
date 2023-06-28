using Gym.DataAccess.Data;
using Gym.DataAccess.Repository.IRepository;
using Gym.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.DataAccess.Repository
{
    public class ProductRepositoy : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepositoy(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product obj)
        {
            var objfromDb = _context.Products.FirstOrDefault(u => u.Id == obj.Id);
            if (objfromDb != null)
            {
                objfromDb.Name = obj.Name;
                objfromDb.Description = obj.Description;
                objfromDb.Price = obj.Price;
                objfromDb.DiscountPrice = obj.DiscountPrice;               
                objfromDb.CategoryId = obj.CategoryId;
                if (obj.ImageUrl != null)
                {
                    objfromDb.ImageUrl = obj.ImageUrl;
                }

            }
        }
    }
}
