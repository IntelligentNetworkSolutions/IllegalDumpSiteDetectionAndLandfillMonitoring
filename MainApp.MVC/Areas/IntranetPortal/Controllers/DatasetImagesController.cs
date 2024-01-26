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
    public class DatasetImagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DatasetImagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: IntranetPortal/DatasetImages
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DatasetImages.Include(d => d.CreatedBy).Include(d => d.Dataset).Include(d => d.UpdatedBy);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: IntranetPortal/DatasetImages/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datasetImage = await _context.DatasetImages
                .Include(d => d.CreatedBy)
                .Include(d => d.Dataset)
                .Include(d => d.UpdatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (datasetImage == null)
            {
                return NotFound();
            }

            return View(datasetImage);
        }

        // GET: IntranetPortal/DatasetImages/Create
        public IActionResult Create()
        {
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["DatasetId"] = new SelectList(_context.Datasets, "Id", "CreatedById");
            ViewData["UpdatedById"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: IntranetPortal/DatasetImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FileName,ImagePath,IsEnabled,DatasetId,CreatedById,CreatedOn,UpdatedById,UpdatedOn,Id")] DatasetImage datasetImage)
        {
            if (ModelState.IsValid)
            {
                datasetImage.Id = Guid.NewGuid();
                _context.Add(datasetImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", datasetImage.CreatedById);
            ViewData["DatasetId"] = new SelectList(_context.Datasets, "Id", "CreatedById", datasetImage.DatasetId);
            ViewData["UpdatedById"] = new SelectList(_context.Users, "Id", "Id", datasetImage.UpdatedById);
            return View(datasetImage);
        }

        // GET: IntranetPortal/DatasetImages/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datasetImage = await _context.DatasetImages.FindAsync(id);
            if (datasetImage == null)
            {
                return NotFound();
            }
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", datasetImage.CreatedById);
            ViewData["DatasetId"] = new SelectList(_context.Datasets, "Id", "CreatedById", datasetImage.DatasetId);
            ViewData["UpdatedById"] = new SelectList(_context.Users, "Id", "Id", datasetImage.UpdatedById);
            return View(datasetImage);
        }

        // POST: IntranetPortal/DatasetImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("FileName,ImagePath,IsEnabled,DatasetId,CreatedById,CreatedOn,UpdatedById,UpdatedOn,Id")] DatasetImage datasetImage)
        {
            if (id != datasetImage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(datasetImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DatasetImageExists(datasetImage.Id))
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
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", datasetImage.CreatedById);
            ViewData["DatasetId"] = new SelectList(_context.Datasets, "Id", "CreatedById", datasetImage.DatasetId);
            ViewData["UpdatedById"] = new SelectList(_context.Users, "Id", "Id", datasetImage.UpdatedById);
            return View(datasetImage);
        }

        // GET: IntranetPortal/DatasetImages/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datasetImage = await _context.DatasetImages
                .Include(d => d.CreatedBy)
                .Include(d => d.Dataset)
                .Include(d => d.UpdatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (datasetImage == null)
            {
                return NotFound();
            }

            return View(datasetImage);
        }

        // POST: IntranetPortal/DatasetImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var datasetImage = await _context.DatasetImages.FindAsync(id);
            if (datasetImage != null)
            {
                _context.DatasetImages.Remove(datasetImage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DatasetImageExists(Guid id)
        {
            return _context.DatasetImages.Any(e => e.Id == id);
        }
    }
}
