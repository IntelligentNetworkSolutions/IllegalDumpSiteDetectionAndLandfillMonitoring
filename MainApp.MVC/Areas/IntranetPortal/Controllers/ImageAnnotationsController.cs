using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.ApplicationStorage;
using Entities.DatasetEntities;
using DAL.Interfaces.Repositories.DatasetRepositories;
using SD;
using Services.Interfaces.Services;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class ImageAnnotationsController : Controller
    {
        private readonly IImageAnnotationsRepository _imageAnnotationsRepository;
        private readonly IUserManagementService _userManagementService;

        public ImageAnnotationsController(IImageAnnotationsRepository imageAnnotationsRepository, IUserManagementService userManagementService)
        {
            _imageAnnotationsRepository = imageAnnotationsRepository;
            _userManagementService = userManagementService;
        }

        // GET: IntranetPortal/ImageAnnotations
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.ImageAnnotations.Include(i => i.CreatedBy).Include(i => i.DatasetImage).Include(i => i.UpdatedBy);
            var all = await _imageAnnotationsRepository.GetAll(includeProperties: "CreatedBy,DatasetImage, UpdatedBy");
            
            return View(all.Data);
        }

        //// GET: IntranetPortal/ImageAnnotations/Details/5
        //public async Task<IActionResult> Details(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var imageAnnotation = await _context.ImageAnnotations
        //        .Include(i => i.CreatedBy)
        //        .Include(i => i.DatasetImage)
        //        .Include(i => i.UpdatedBy)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (imageAnnotation == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(imageAnnotation);
        //}

        // GET: IntranetPortal/ImageAnnotations/Create
        public async Task<IActionResult> CreateAsync()
        {
            var users = await _userManagementService.GetAllIntanetPortalUsers();
            ViewData["CreatedById"] = new SelectList(users, "Id", "Id");
            ViewData["DatasetImageId"] = new SelectList(new List<DatasetImage>(), "Id", "CreatedById");
            ViewData["UpdatedById"] = new SelectList(users, "Id", "Id");
            return View();
        }

        // POST: IntranetPortal/ImageAnnotations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AnnotationsGeoJson,Geom,IsEnabled,DatasetImageId,CreatedById,CreatedOn,UpdatedById,UpdatedOn,Id")] ImageAnnotation imageAnnotation)
        {
            if (ModelState.IsValid)
            {
                imageAnnotation.Id = Guid.NewGuid();
                ResultDTO resCreate = await _imageAnnotationsRepository.Create(imageAnnotation);
                return RedirectToAction(nameof(Index));
            }
            var users = await _userManagementService.GetAllIntanetPortalUsers();
            ViewData["CreatedById"] = new SelectList(users, "Id", "Id");
            ViewData["DatasetImageId"] = new SelectList(new List<DatasetImage>(), "Id", "CreatedById");
            ViewData["UpdatedById"] = new SelectList(users, "Id", "Id");
            return View(imageAnnotation);
        }

        //// GET: IntranetPortal/ImageAnnotations/Edit/5
        //public async Task<IActionResult> Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var imageAnnotation = await _context.ImageAnnotations.FindAsync(id);
        //    if (imageAnnotation == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", imageAnnotation.CreatedById);
        //    ViewData["DatasetImageId"] = new SelectList(_context.DatasetImages, "Id", "CreatedById", imageAnnotation.DatasetImageId);
        //    ViewData["UpdatedById"] = new SelectList(_context.Users, "Id", "Id", imageAnnotation.UpdatedById);
        //    return View(imageAnnotation);
        //}

        //// POST: IntranetPortal/ImageAnnotations/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, [Bind("AnnotationsGeoJson,Geom,IsEnabled,DatasetImageId,CreatedById,CreatedOn,UpdatedById,UpdatedOn,Id")] ImageAnnotation imageAnnotation)
        //{
        //    if (id != imageAnnotation.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(imageAnnotation);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ImageAnnotationExists(imageAnnotation.Id))
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
        //    ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", imageAnnotation.CreatedById);
        //    ViewData["DatasetImageId"] = new SelectList(_context.DatasetImages, "Id", "CreatedById", imageAnnotation.DatasetImageId);
        //    ViewData["UpdatedById"] = new SelectList(_context.Users, "Id", "Id", imageAnnotation.UpdatedById);
        //    return View(imageAnnotation);
        //}

        //// GET: IntranetPortal/ImageAnnotations/Delete/5
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var imageAnnotation = await _context.ImageAnnotations
        //        .Include(i => i.CreatedBy)
        //        .Include(i => i.DatasetImage)
        //        .Include(i => i.UpdatedBy)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (imageAnnotation == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(imageAnnotation);
        //}

        //// POST: IntranetPortal/ImageAnnotations/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var imageAnnotation = await _context.ImageAnnotations.FindAsync(id);
        //    if (imageAnnotation != null)
        //    {
        //        _context.ImageAnnotations.Remove(imageAnnotation);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool ImageAnnotationExists(Guid id)
        //{
        //    return _context.ImageAnnotations.Any(e => e.Id == id);
        //}
    }
}
