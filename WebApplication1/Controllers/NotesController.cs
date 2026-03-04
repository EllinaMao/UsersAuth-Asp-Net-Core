using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication1.Models;
using WebApplication1.Repositories;
using WebApplication1.ViewModels;

[Authorize]
public class NotesController : Controller
{
    private readonly INoteRepository _noteRepository;
    private readonly UserManager<User> _userManager;
    private readonly IAuthorizationService _authService;

    public NotesController(INoteRepository noteRepository, UserManager<User> userManager, IAuthorizationService authService)
    {
        _noteRepository = noteRepository;
        _userManager = userManager;
        _authService = authService;
    }

    // GET: Notes
    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Challenge();
        }

        var notes = await _noteRepository.GetNotesByUserIdAsync(currentUser.Id);
        return View(notes);
    }

    // GET: Notes/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var note = await _noteRepository.GetNoteByIdAsync(id.Value);
        if (note == null)
        {
            return NotFound();
        }

        var authResult = await _authService.AuthorizeAsync(User, note, "CanManageNote");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        return View(note);
    }

    // GET: Notes/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Notes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateNoteViewModel model)
    {
        if (ModelState.IsValid)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var note = new Note
            {
                Name = model.Name,
                Description = model.Description,
                UserId = currentUser.Id
            };

            await _noteRepository.AddNoteAsync(note);
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: Notes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var note = await _noteRepository.GetNoteByIdAsync(id.Value);
        if (note == null)
        {
            return NotFound();
        }

        var authResult = await _authService.AuthorizeAsync(User, note, "CanManageNote");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        var model = new EditNoteViewModel
        {
            Id = note.Id,
            Name = note.Name,
            Description = note.Description
        };

        return View(model);
    }

    // POST: Notes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditNoteViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var note = await _noteRepository.GetNoteByIdAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            var authResult = await _authService.AuthorizeAsync(User, note, "CanManageNote");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            note.Name = model.Name;
            note.Description = model.Description;

            await _noteRepository.UpdateNoteAsync(note);
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: Notes/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var note = await _noteRepository.GetNoteByIdAsync(id.Value);
        if (note == null)
        {
            return NotFound();
        }

        var authResult = await _authService.AuthorizeAsync(User, note, "CanManageNote");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        return View(note);
    }

    // POST: Notes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var note = await _noteRepository.GetNoteByIdAsync(id);
        if (note == null)
        {
            return NotFound();
        }

        var authResult = await _authService.AuthorizeAsync(User, note, "CanManageNote");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        await _noteRepository.DeleteNoteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}