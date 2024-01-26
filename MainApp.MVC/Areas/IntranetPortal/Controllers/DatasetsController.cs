using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.ApplicationStorage;
using Entities.DatasetEntities;
using DAL.Repositories.DatasetRepositories;
using DAL.Interfaces.Repositories.DatasetRepositories;
using Services.Interfaces.Services;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class DatasetsController : Controller
    {
        private readonly IDatasetsRepository _datasetRepository;
        private readonly IUserManagementService _userManagementService;

        public DatasetsController(IDatasetsRepository datasetRepository, IUserManagementService userManagementService)
        {
            _datasetRepository = datasetRepository;
            _userManagementService = userManagementService;
        }

        // GET: IntranetPortal/Datasets
        public async Task<IActionResult> Index()
        {
            var all = await _datasetRepository.GetAll(includeProperties: "CreatedBy,ParentDataset, UpdatedBy");
            return View(all.Data);
        }

        // GET: IntranetPortal/Datasets/Details/5
        //public async Task<IActionResult> Details(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var dataset = await _context.Datasets
        //        .Include(d => d.CreatedBy)
        //        .Include(d => d.ParentDataset)
        //        .Include(d => d.UpdatedBy)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (dataset == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(dataset);
        //}

        // GET: IntranetPortal/Datasets/Create
        public async Task<IActionResult> Create()
        {
            var datasets = await _datasetRepository.GetAll();
            var users = await _userManagementService.GetAllIntanetPortalUsers();
            ViewData["CreatedById"] = new SelectList(users, "Id", "Id");
            ViewData["UpdatedById"] = new SelectList(users, "Id", "Id");
            ViewData["ParentDatasetId"] = new SelectList(datasets.Data, "Id", "CreatedById");
            return View();
        }

        // POST: IntranetPortal/Datasets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,IsPublished,ParentDatasetId,CreatedById,CreatedOn,UpdatedById,UpdatedOn,Id")] Dataset dataset)
        {
            if (ModelState.IsValid)
            {
                dataset.Id = Guid.NewGuid();
                await _datasetRepository.Create(dataset);
                return RedirectToAction(nameof(Index));
            }
            var datasets = await _datasetRepository.GetAll();
            var users = await _userManagementService.GetAllIntanetPortalUsers();
            ViewData["CreatedById"] = new SelectList(users, "Id", "Id");
            ViewData["UpdatedById"] = new SelectList(users, "Id", "Id");
            ViewData["ParentDatasetId"] = new SelectList(datasets.Data, "Id", "CreatedById");
            return View(dataset);
        }

        //// GET: IntranetPortal/Datasets/Edit/5
        //public async Task<IActionResult> Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var dataset = await _context.Datasets.FindAsync(id);
        //    if (dataset == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", dataset.CreatedById);
        //    ViewData["ParentDatasetId"] = new SelectList(_context.Datasets, "Id", "CreatedById", dataset.ParentDatasetId);
        //    ViewData["UpdatedById"] = new SelectList(_context.Users, "Id", "Id", dataset.UpdatedById);
        //    return View(dataset);
        //}

        //// POST: IntranetPortal/Datasets/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, [Bind("Name,Description,IsPublished,ParentDatasetId,CreatedById,CreatedOn,UpdatedById,UpdatedOn,Id")] Dataset dataset)
        //{
        //    if (id != dataset.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(dataset);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!DatasetExists(dataset.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", dataset.CreatedById);
        //    ViewData["ParentDatasetId"] = new SelectList(_context.Datasets, "Id", "CreatedById", dataset.ParentDatasetId);
        //    ViewData["UpdatedById"] = new SelectList(_context.Users, "Id", "Id", dataset.UpdatedById);
        //    return View(dataset);
        //}

        //// GET: IntranetPortal/Datasets/Delete/5
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var dataset = await _context.Datasets
        //        .Include(d => d.CreatedBy)
        //        .Include(d => d.ParentDataset)
        //        .Include(d => d.UpdatedBy)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (dataset == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(dataset);
        //}

        //// POST: IntranetPortal/Datasets/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var dataset = await _context.Datasets.FindAsync(id);
        //    if (dataset != null)
        //    {
        //        _context.Datasets.Remove(dataset);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool DatasetExists(Guid id)
        //{
        //    return _context.Datasets.Any(e => e.Id == id);
        //}
    }
}
