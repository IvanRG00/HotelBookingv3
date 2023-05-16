using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelBookingv3.Data;
using HotelBookingv3.Models;
using Microsoft.AspNetCore.Authorization;
using NuGet.Packaging.Signing;
using HotelBookingv3;

namespace HotelBookingv3.Controllers
{
    public class HotelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HotelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Hotels
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentfilter,
            string searchString,
            int? pagenumber)
        { 
        
                ViewData["CurrentSort"] = sortOrder;
                ViewData["HotelnameSortParam"] = sortOrder == "Hotel" ? "HotelNameDesc" : "";
                
                if (searchString != null)
                {
                    pagenumber = 1;
                }
                else
                {
                    searchString = currentfilter;
                }
                ViewData["CurrentFilter"] = searchString;

                var hotels = from s in _context.Hotels select s;

                if (!String.IsNullOrEmpty(searchString))
                {
                    hotels = hotels.Where(s => s.HotelName.Contains(searchString));
                }


                switch (sortOrder)
                {
                    case "HotelName":
                        hotels = hotels.OrderByDescending(s => s.HotelName);
                        break;
                    case "HotelPrice":
                        hotels = hotels.OrderBy(s => s.HotelPrice);
                        break;
                    default:
                        hotels = hotels.OrderBy(s => s.HotelName);
                        break;
                }

                int pageSize = 3;
                return View(await PaginatedList<Hotels>.CreateAsync(hotels.AsNoTracking(), pagenumber ?? 1, pageSize));
            }

        // GET: Hotels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Hotels == null)
            {
                return NotFound();
            }

            var hotels = await _context.Hotels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hotels == null)
            {
                return NotFound();
            }

            return View(hotels);
        }

        // GET: Hotels/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Hotels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HotelName,HotelLocation,HotelPrice")] Hotels hotels)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hotels);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hotels);
        }

        // GET: Hotels/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Hotels == null)
            {
                return NotFound();
            }

            var hotels = await _context.Hotels.FindAsync(id);
            if (hotels == null)
            {
                return NotFound();
            }
            return View(hotels);
        }

        // POST: Hotels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HotelName,HotelLocation,HotelPrice")] Hotels hotels)
        {
            if (id != hotels.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hotels);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelsExists(hotels.Id))
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
            return View(hotels);
        }

        // GET: Hotels/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Hotels == null)
            {
                return NotFound();
            }

            var hotels = await _context.Hotels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hotels == null)
            {
                return NotFound();
            }

            return View(hotels);
        }

        // POST: Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Hotels == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Hotels'  is null.");
            }
            var hotels = await _context.Hotels.FindAsync(id);
            if (hotels != null)
            {
                _context.Hotels.Remove(hotels);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HotelsExists(int id)
        {
          return (_context.Hotels?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
