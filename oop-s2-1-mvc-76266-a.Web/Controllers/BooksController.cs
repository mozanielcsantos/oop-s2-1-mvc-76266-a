using Library.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OopS21Mvc76266A.Web.Data;

namespace OopS21Mvc76266A.Web.Controllers;

public class BooksController : Controller
{
    private readonly ApplicationDbContext _db;

    public BooksController(ApplicationDbContext db)
    {
        _db = db;
    }

    // ---------------------------
    // BOOKS LIST
    // ---------------------------
    public async Task<IActionResult> Index(
        string searchTitle,
        string searchAuthor,
        string availability,
        string sortOrder)
    {
        IQueryable<Book> query = _db.Books
            .Include(b => b.Loans);

        // ---------------------------
        // SEARCH
        // ---------------------------
        if (!string.IsNullOrWhiteSpace(searchTitle))
        {
            query = query.Where(b =>
                b.Title.Contains(searchTitle));
        }

        if (!string.IsNullOrWhiteSpace(searchAuthor))
        {
            query = query.Where(b =>
                b.Author.Contains(searchAuthor));
        }

        // ---------------------------
        // AVAILABILITY FILTER
        // ---------------------------
        if (availability == "available")
        {
            query = query.Where(b =>
                !b.Loans.Any(l => l.ReturnDate == null));
        }

        if (availability == "onloan")
        {
            query = query.Where(b =>
                b.Loans.Any(l => l.ReturnDate == null));
        }

        // ---------------------------
        // SORTING
        // ---------------------------
        query = sortOrder switch
        {
            "title_desc" => query.OrderByDescending(b => b.Title),
            "author" => query.OrderBy(b => b.Author),
            "author_desc" => query.OrderByDescending(b => b.Author),
            _ => query.OrderBy(b => b.Title)
        };

        var books = await query.ToListAsync();

        return View(books);
    }
}