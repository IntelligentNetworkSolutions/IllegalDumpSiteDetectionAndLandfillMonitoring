using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.ApplicationStorage;
using Entities.DatasetEntities;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class DatasetClassesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DatasetClassesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: IntranetPortal/DatasetClasses
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DatasetClasses.Include(d => d.CreatedBy).Include(d => d.Dataset);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: IntranetPortal/DatasetClasses/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datasetClass = await _context.DatasetClasses
                .Include(d => d.CreatedBy)
                .Include(d => d.Dataset)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (datasetClass == null)
            {
                return NotFound();
            }

            return View(datasetClass);
        }

        // GET: IntranetPortal/DatasetClasses/Create
        public IActionResult Create()
        {
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["DatasetId"] = new SelectList(_context.Datasets, "Id", "CreatedById");
            return View();
        }

        // POST: IntranetPortal/DatasetClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClassId,ClassName,DatasetId,CreatedById,CreatedOn,Id")] DatasetClass datasetClass)
        {
            if (ModelState.IsValid)
            {
                datasetClass.Id = Guid.NewGuid();
                _context.Add(datasetClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", datasetClass.CreatedById);
            ViewData["DatasetId"] = new SelectList(_context.Datasets, "Id", "CreatedById", datasetClass.DatasetId);
            return View(datasetClass);
        }

        // GET: IntranetPortal/DatasetClasses/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datasetClass = await _context.DatasetClasses.FindAsync(id);
            if (datasetClass == null)
            {
                return NotFound();
            }
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", datasetClass.CreatedById);
            ViewData["DatasetId"] = new SelectList(_context.Datasets, "Id", "CreatedById", datasetClass.DatasetId);
            return View(datasetClass);
        }

        // POST: IntranetPortal/DatasetClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ClassId,ClassName,DatasetId,CreatedById,CreatedOn,Id")] DatasetClass datasetClass)
        {
            if (id != datasetClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(datasetClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DatasetClassExists(datasetClass.Id))
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
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", datasetClass.CreatedById);
            ViewData["DatasetId"] = new SelectList(_context.Datasets, "Id", "CreatedById", datasetClass.DatasetId);
            return View(datasetClass);
        }

        // GET: IntranetPortal/DatasetClasses/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datasetClass = await _context.DatasetClasses
                .Include(d => d.CreatedBy)
                .Include(d => d.Dataset)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (datasetClass == null)
            {
                return NotFound();
            }

            return View(datasetClass);
        }

        // POST: IntranetPortal/DatasetClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var datasetClass = await _context.DatasetClasses.FindAsync(id);
            if (datasetClass != null)
            {
                _context.DatasetClasses.Remove(datasetClass);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DatasetClassExists(Guid id)
        {
            return _context.DatasetClasses.Any(e => e.Id == id);
        }
    }
}
