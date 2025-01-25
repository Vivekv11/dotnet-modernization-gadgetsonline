using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadgetsOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetsOnline.Services
{
    public class Inventory
    {
        private readonly GadgetsOnlineEntities _context;

        public Inventory(GadgetsOnlineEntities context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetBestSellers(int count)
        {
            return await _context.Products
                    .Take(count)
                    .ToListAsync();
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<List<Product>> GetAllProductsInCategory(string category)
        {
            return await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.Category.Name == category)
                    .ToListAsync();
        }

#nullable enable
        public async Task<Product?> GetProductById(int id)
        {
            return await _context.Products
                    .Include(p => p.Category)
                   .Where(p => p.ProductId == id)
                   .FirstOrDefaultAsync();
        }

        public async Task<string?> GetProductNameById(int id)
        {
            var product = await _context.Products
                   .Where(p => p.ProductId == id)
                   .Select(p => p.Name)
                   .FirstOrDefaultAsync();

            return product;
        }
    }
}