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
    public class VehiclesController : Controller
    {
        private readonly DealerLeadDBContext _context;

        public VehiclesController(DealerLeadDBContext context)
        {
            _context = context;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            var dealerships = await GetDealerships();

            var dealershipIds = dealerships.Select(d => d.Id).ToList();



            var dealerLeadDBContext = _context.Vehicle.Include(v => v.Dealership).Include(v => v.SupportedModel);
            return View(await dealerLeadDBContext.Where(v => dealershipIds.Contains(v.DealershipId)).ToListAsync());
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle
                .Include(v => v.Dealership)
                .Include(v => v.SupportedModel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public async Task<IActionResult> Create()
        {
            var dealerships = await GetDealerships();
            var dList = dealerships.ToList();
            
            ViewData["DealershipId"] = new SelectList(dList, "Id", "Name");
            ViewData["SupportedModelId"] = new SelectList(_context.SupportedModel, "Id", "Name");
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SupportedModelId,MSRP,StockNumber,Color,DealershipId,SellDate")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var dealerships = await GetDealerships();
            var dList = dealerships.ToList();

            ViewData["DealershipId"] = new SelectList(dList, "Id", "Name", vehicle.DealershipId);
            ViewData["SupportedModelId"] = new SelectList(_context.SupportedModel, "Id", "Name", vehicle.SupportedModelId);
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            var dealerships = await GetDealerships();
            var dList = dealerships.ToList();

            ViewData["DealershipId"] = new SelectList(dList, "Id", "Name", vehicle.DealershipId);
            ViewData["SupportedModelId"] = new SelectList(_context.SupportedModel, "Id", "Name", vehicle.SupportedModelId);
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SupportedModelId,MSRP,StockNumber,Color,DealershipId,SellDate")] Vehicle vehicle)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
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

            var dealerships = await GetDealerships();
            var dList = dealerships.ToList();

            ViewData["DealershipId"] = new SelectList(dList, "Id", "Name", vehicle.DealershipId);
            ViewData["SupportedModelId"] = new SelectList(_context.SupportedModel, "Id", "Name", vehicle.SupportedModelId);
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle
                .Include(v => v.Dealership)
                .Include(v => v.SupportedModel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicle.FindAsync(id);
            _context.Vehicle.Remove(vehicle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<IQueryable<Dealership>> GetDealerships()
        {
            var oid = AuthenticationHelper.GetOid(this.User);
            var user = await _context.DealerLeadUser.FirstOrDefaultAsync(u => u.AzureADId == oid);
            var dealerships = await _context.Dealership.Where(d => d.CreatingUserId == user.Id).ToListAsync();
            return dealerships.AsQueryable();
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicle.Any(e => e.Id == id);
        }
    }
}
