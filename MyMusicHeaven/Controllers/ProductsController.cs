using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyMusicHeaven.Data;
using MyMusicHeaven.Models;

namespace MyMusicHeaven.Views
{
    public class ProductsController : Controller
    {
        private readonly MyMusicHeavenNewContext _context;

        public ProductsController(MyMusicHeavenNewContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string SearchString, string Category)
        {
            var products = from m in _context.Product //full list
                           select m;

            if (!String.IsNullOrEmpty(SearchString)) //if got any search word
            {
                products = products.Where(s => s.ProductName.Contains(SearchString)); //filter list
            }

            //add category in dropdown list
            IQueryable<String> TypeQuery = from m in _context.Product
                                           orderby m.Category
                                           select m.Category;
            IEnumerable<SelectListItem> items = new SelectList(await TypeQuery.Distinct().ToListAsync());
            ViewBag.Category = items;

            if (!String.IsNullOrEmpty(Category)) //if got any search word
            {
                products = products.Where(s => s.Category.Equals(Category)); //filter list
            }

            return View(await products.ToListAsync());
        }

        // GET: CustomerProducts
        public async Task<IActionResult> CustomerProduct(string SearchString, string Category)
        {
            var products = from m in _context.Product //full list
                           select m;

            if (!String.IsNullOrEmpty(SearchString)) //if got any search word
            {
                products = products.Where(s => s.ProductName.Contains(SearchString)); //filter list
            }

            //add category in dropdown list
            IQueryable<String> TypeQuery = from m in _context.Product
                                           orderby m.Category
                                           select m.Category;
            IEnumerable<SelectListItem> items = new SelectList(await TypeQuery.Distinct().ToListAsync());
            ViewBag.Category = items;

            if (!String.IsNullOrEmpty(Category)) //if got any search word
            {
                products = products.Where(s => s.Category.Equals(Category)); //filter list
            }

            return View(await products.ToListAsync());
        }

        // GET: CustomerProducts2
        public async Task<IActionResult> CustomerProduct2(string SearchString, string Category)
        {
            var products = from m in _context.Product //full list
                           select m;

            if (!String.IsNullOrEmpty(SearchString)) //if got any search word
            {
                products = products.Where(s => s.ProductName.Contains(SearchString)); //filter list
            }

            //add category in dropdown list
            IQueryable<String> TypeQuery = from m in _context.Product
                                           orderby m.Category
                                           select m.Category;
            IEnumerable<SelectListItem> items = new SelectList(await TypeQuery.Distinct().ToListAsync());
            ViewBag.Category = items;

            if (!String.IsNullOrEmpty(Category)) //if got any search word
            {
                products = products.Where(s => s.Category.Equals(Category)); //filter list
            }

            return View(await products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            IFormFile file = Request.Form.Files.FirstOrDefault();
            try
            {
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    product.ProductPicture = dataStream.ToArray();
                }
            }catch(Exception ex) { }
            Product pd = new Product
            {
                ProductName = product.ProductName,
                StockInDate = product.StockInDate,
                Category = product.Category,
                ProductPrice = product.ProductPrice,
                Rating = product.Rating,
                ProductPicture = product.ProductPicture,
            };

            if (ModelState.IsValid)
            {              
                _context.Add(pd);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));               
            }
            return View(product);
            
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ProductName,StockInDate,Category,ProductPrice,Rating,ProductPicture")] Product product)
        {
            if (id != product.ID)
            {
                return NotFound();
            }
           
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ID == id);
        }
    }
}
