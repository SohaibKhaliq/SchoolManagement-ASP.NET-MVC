using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers;

public class StudentsController : Controller
{
    private readonly SchoolDbContext _context;

    public StudentsController(SchoolDbContext context)
    {
        _context = context;
    }

    // READ - List all students
    public async Task<IActionResult> Index()
    {
        return View(await _context.Students.ToListAsync());
    }

    // READ - One student
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == id);
        if (student == null) return NotFound();

        return View(student);
    }

    // CREATE - Show empty form
    public IActionResult Create()
    {
        return View();
    }

    // CREATE - Save new student
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Id,FullName,Email,Age,DateOfBirth")] Student student)
    {
        if (ModelState.IsValid)
        {
            _context.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    // UPDATE - Show filled form
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var student = await _context.Students.FindAsync(id);
        if (student == null) return NotFound();

        return View(student);
    }

    // UPDATE - Save changes
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,FullName,Email,Age,DateOfBirth")] Student student)
    {
        if (id != student.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(student.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    // DELETE - Show confirmation page
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == id);
        if (student == null) return NotFound();

        return View(student);
    }

    // DELETE - Delete after confirmation
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool StudentExists(int id)
    {
        return _context.Students.Any(s => s.Id == id);
    }
}
