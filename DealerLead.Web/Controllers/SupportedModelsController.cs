using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DealerLead;

namespace DealerLead.Web.Controllers
{
    public class SupportedModelsController : Controller
    {
        private readonly DealerLeadDBContext _context;

        public SupportedModelsController(DealerLeadDBContext context)
        {
            _context = context;
        }

        // GET: SupportedModels
        public async Task<IActionResult> Index()
        {
            var dealerLeadDBContext = _context.SupportedModel.Include(s => s.SupportedMake);
            return View(await dealerLeadDBContext.ToListAsync());
        }

        // GET: SupportedModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supportedModel = await _context.SupportedModel
                .Include(s => s.SupportedMake)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supportedModel == null)
            {
                return NotFound();
            }

            return View(supportedModel);
        }

        // GET: SupportedModels/Create
        public IActionResult Create()
        {
            ViewData["SupportedMakeId"] = new SelectList(_context.SupportedMake, "Id", "Name");
            return View();
        }

        // POST: SupportedModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,SupportedMakeId")] SupportedModel supportedModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supportedModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupportedMakeId"] = new SelectList(_context.SupportedMake, "Id", "Name", supportedModel.SupportedMakeId);
            return View(supportedModel);
        }

        // GET: SupportedModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supportedModel = await _context.SupportedModel.FindAsync(id);
            if (supportedModel == null)
            {
                return NotFound();
            }
            ViewData["SupportedMakeId"] = new SelectList(_context.SupportedMake, "Id", "Name", supportedModel.SupportedMakeId);
            return View(supportedModel);
        }

        // POST: SupportedModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,SupportedMakeId")] SupportedModel supportedModel)
        {
            if (id != supportedModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supportedModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupportedModelExists(supportedModel.Id))
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
            ViewData["SupportedMakeId"] = new SelectList(_context.SupportedMake, "Id", "Name", supportedModel.SupportedMakeId);
            return View(supportedModel);
        }

        // GET: SupportedModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supportedModel = await _context.SupportedModel
                .Include(s => s.SupportedMake)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supportedModel == null)
            {
                return NotFound();
            }

            return View(supportedModel);
        }

        // POST: SupportedModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supportedModel = await _context.SupportedModel.FindAsync(id);
            _context.SupportedModel.Remove(supportedModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupportedModelExists(int id)
        {
            return _context.SupportedModel.Any(e => e.Id == id);
        }
    }
}
