using Library.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_76266_a.Web.Data;
using oop_s2_1_mvc_76266_a.Web.Models;

namespace oop_s2_1_mvc_76266_a.Web.Controllers
{
    [Authorize]
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .OrderByDescending(l => l.LoanDate)
                .Select(l => new LoanListItemVm
                {
                    Id = l.Id,
                    BookTitle = l.Book != null ? l.Book.Title : string.Empty,
                    MemberName = l.Member != null ? l.Member.FullName : string.Empty,
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    ReturnedDate = l.ReturnedDate
                })
                .ToListAsync();

            return View(loans);
        }

        public async Task<IActionResult> Create()
        {
            await LoadCreateDataAsync();
            return View(new Loan
            {
                LoanDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(14)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Loan loan)
        {
            var activeLoanExists = await _context.Loans
                .AnyAsync(l => l.BookId == loan.BookId && l.ReturnedDate == null);

            if (activeLoanExists)
            {
                ModelState.AddModelError(string.Empty, "This book is already on an active loan.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCreateDataAsync();
                return View(loan);
            }

            loan.ReturnedDate = null;

            var book = await _context.Books.FindAsync(loan.BookId);
            if (book != null)
            {
                book.IsAvailable = false;
            }

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Return(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        [HttpPost, ActionName("Return")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnConfirmed(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
            {
                return NotFound();
            }

            loan.ReturnedDate = DateTime.Today;

            if (loan.Book != null)
            {
                loan.Book.IsAvailable = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCreateDataAsync()
        {
            var availableBooks = await _context.Books
                .Where(b => b.IsAvailable)
                .OrderBy(b => b.Title)
                .ToListAsync();

            var members = await _context.Members
                .OrderBy(m => m.FullName)
                .ToListAsync();

            ViewBag.BookId = new SelectList(availableBooks, "Id", "Title");
            ViewBag.MemberId = new SelectList(members, "Id", "FullName");
        }
    }
}