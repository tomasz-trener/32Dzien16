using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using P09ShopWebApp.Client.Data;
using P09ShopWebApp.Client.Models;

namespace P09ShopWebApp.Client.Pages.ProductsAutoGen
{
    public class IndexModel : PageModel
    {
        private readonly P09ShopWebApp.Client.Data.ShopContext _context;

        public IndexModel(P09ShopWebApp.Client.Data.ShopContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Product = await _context.Products.ToListAsync();
        }
    }
}
