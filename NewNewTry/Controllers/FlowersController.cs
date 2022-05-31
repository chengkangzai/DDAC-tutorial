using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewNewTry.Data;
using NewNewTry.Models;

namespace NewNewTry.Controllers
{
    public class FlowersController : Controller
    {
        private readonly NewNewTryNewContext _context;

        public FlowersController(NewNewTryNewContext context)
        {
            _context = context;
        }

        // GET: Flowers
        public async Task<IActionResult> Index(string searchString, string FlowerType)
        {
            //retrieve distinct flower types and attach to viewbag
            IQueryable<string> sql = from f in _context.Flower
                orderby f.FlowerType
                select f.FlowerType;

            // IEnumerable<SelectListItem> items = sql.Distinct().Select(f => new SelectListItem { Value = f, Text = f });
            IEnumerable<SelectListItem> items = new SelectList(await sql.Distinct().ToListAsync());
            ViewBag.FlowerType = items;

            var flowers = from f in _context.Flower
                select f;

            if (!string.IsNullOrEmpty(searchString))
            {
                flowers = flowers.Where(s => s.FlowerName.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(FlowerType))
            {
                flowers = flowers.Where(x => x.FlowerType == FlowerType);
            }


            return View(await flowers.ToListAsync());
        }


        // GET: Flowers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flower = await _context.Flower.FirstOrDefaultAsync(m => m.ID == id);
            if (flower == null)
            {
                return NotFound();
            }

            return View(flower);
        }

        // GET: Flowers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Flowers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FlowerName,FlowerPrice,FlowerType,FlowerSentToShop")] Flower flower)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flower);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(flower);
        }

        // GET: Flowers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flower = await _context.Flower.FindAsync(id);
            if (flower == null)
            {
                return NotFound();
            }
            return View(flower);
        }

        // POST: Flowers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FlowerName,FlowerPrice,FlowerType,FlowerSentToShop")] Flower flower)
        {
            if (id != flower.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flower);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlowerExists(flower.ID))
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
            return View(flower);
        }

        // GET: Flowers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flower = await _context.Flower
                .FirstOrDefaultAsync(m => m.ID == id);
            if (flower == null)
            {
                return NotFound();
            }

            return View(flower);
        }

        // POST: Flowers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flower = await _context.Flower.FindAsync(id);
            _context.Flower.Remove(flower);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlowerExists(int id)
        {
            return _context.Flower.Any(e => e.ID == id);
        }
    }
}
