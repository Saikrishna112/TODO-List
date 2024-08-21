using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TODO_List.Models;

namespace TODO_List.Controllers;

public class TodoController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    public TodoController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var todoItems = await _dbContext.TodoItems.ToListAsync();
        return View(todoItems);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TodoItem todoItem)
    {
        if (ModelState.IsValid)
            {
                _dbContext.Add(todoItem);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        return View(todoItem);
    }

    public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _dbContext.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }
            return View(todoItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IsCompleted")] TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(todoItem);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoItemExists(todoItem.Id))
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
            return View(todoItem);
        }

        private bool TodoItemExists(int id)
        {
            return _dbContext.TodoItems.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _dbContext.TodoItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return View(todoItem);
        }

         public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var todoItem = await _dbContext.TodoItems.FindAsync(id);
                _dbContext.TodoItems.Remove(todoItem);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
}