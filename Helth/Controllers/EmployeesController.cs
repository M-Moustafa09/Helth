using Helth.Data;
using Helth.Models;
using Helth.Services;
using Helth.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helth.Controllers;

[Authorize]
public class EmployeesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IPhotoService _photoService;
    private readonly IPdfService _pdfService;
    private readonly IWebHostEnvironment _env;

    public EmployeesController(ApplicationDbContext context, IPhotoService photoService, IPdfService pdfService, IWebHostEnvironment env)
    {
        _context = context;
        _photoService = photoService;
        _pdfService = pdfService;
        _env = env;
    }

    // GET: /Employees
    public async Task<IActionResult> Index(string? searchTerm)
    {
        var query = _context.Employees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                e.FullName.Contains(searchTerm) ||
                e.NationalId.Contains(searchTerm));
        }

        var employees = await query.OrderByDescending(e => e.CreatedAt).ToListAsync();

        var model = new EmployeeSearchViewModel
        {
            SearchTerm = searchTerm,
            Employees = employees
        };

        return View(model);
    }

    // GET: /Employees/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        return View(employee.ToFormViewModel());
    }

    // GET: /Employees/Create
    public IActionResult Create()
    {
        return View(new EmployeeFormViewModel());
    }

    // POST: /Employees/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var employee = new Employee
        {
            CreatedAt = DateTime.UtcNow
        };
        model.ApplyTo(employee);

        if (model.PhotoFile != null)
        {
            employee.PhotoPath = await _photoService.SavePhotoAsync(model.PhotoFile);
        }

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = employee.Id });
    }

    // GET: /Employees/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        return View(employee.ToFormViewModel());
    }

    // POST: /Employees/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.PhotoPath = employee.PhotoPath;
            return View(model);
        }

        model.ApplyTo(employee);
        employee.UpdatedAt = DateTime.UtcNow;

        if (model.PhotoFile != null)
        {
            _photoService.DeletePhoto(employee.PhotoPath);
            employee.PhotoPath = await _photoService.SavePhotoAsync(model.PhotoFile);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = employee.Id });
    }

    // POST: /Employees/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        _photoService.DeletePhoto(employee.PhotoPath);
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Employees/Share/5
    public async Task<IActionResult> Share(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        var publicUrl = Url.Action("Employee", "Public", new { id = employee.Id }, Request.Scheme)!;
        var pdfBytes = _pdfService.GenerateEmployeeCertificate(employee, publicUrl, _env.WebRootPath);

        return File(pdfBytes, "application/pdf", $"certificate-{employee.Id}.pdf");
    }
}
