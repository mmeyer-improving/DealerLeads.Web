using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DealerLead.Web.Controllers
{
    public class DealershipsController : Controller
    {
        private readonly DealerLeadDBContext _context;

        public DealershipsController(DealerLeadDBContext context) 
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            
            return View(await _context.Dealership.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dealership = await _context.Dealership.FirstOrDefaultAsync(d => d.Id == id);
            if (dealership == null)
            {
                return NotFound();
            }

            return View(dealership);
        }

        public IActionResult Create()
        {
            var user = this.User;

            var oid = user.Identities.ElementAt(0).Claims.ElementAt(2).Value;
            ViewBag.Oid = oid;

            ViewBag.SupportedStates = new SelectList(_context.SupportedState, "Abbreviation", "Abbreviation");
            return View();
        }

        //The bind attribute is there to prevent from overposting, but in this instance it may not be necessary.
        //put [BindNever] on the attributes you are not binding in the model instead, and can maybe skip it?
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Dealership dealership)
        {
            if (ModelState.IsValid)
            {
                var user = this.User;
                var oid = Guid.Parse(user.Identities.ElementAt(0).Claims.ElementAt(2).Value);
                var userId = _context.DealerLeadUser.FirstOrDefault(u => u.AzureADId == oid).Id;
                dealership.CreatingUserId = userId;
                _context.Add(dealership);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["SupportedStates"] = new SelectList(_context.SupportedState, "Abbreviation", "Abbreviation", dealership.State);
            return View(dealership);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dealership = await _context.Dealership.FindAsync(id);
            if (dealership == null)
            {
                return NotFound();
            }

            ViewData["SupportedStates"] = new SelectList(_context.SupportedState, "Abbreviation", "Abbreviation", dealership.State);
            return View(dealership);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Dealership dealership)
        {
            if (id != dealership.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dealership.ModifyDate = DateTime.Now;
                    _context.Update(dealership);
                    await _context.SaveChangesAsync();
                } 
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Dealership.Any(d => d.Id == id))
                    {
                        return NotFound();
                    } else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupportedStates"] = new SelectList(_context.SupportedState, "Abbreviation", "Abbreviation", dealership.State);
            return View(dealership);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dealership = await _context.Dealership.FirstOrDefaultAsync(d => d.Id == id);
            if (dealership == null)
            {
                return NotFound();
            }

            return View(dealership);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dealership = await _context.Dealership.FindAsync(id);
            _context.Dealership.Remove(dealership);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
