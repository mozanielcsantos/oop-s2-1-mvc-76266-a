using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OopS21Mvc76266A.Web.Data;
using oop_s2_1_mvc_76266_a.Web.Models;

namespace oop_s2_1_mvc_76266_a.Web.Controllers
{
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LoansController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Loans
        public async Task<IActionResult> Index(string? status)
        {
            // status options (optional): "all", "active", "returned", "overdue"
            var normalized = (status ?? "all").Trim().ToLower();

            // Join tables explicitly so this works even if your Loan entity
            // doesn't contain navigation properties like Loan.Book / Loan.Member.
            var query =
                from l in _db.Loans.AsNoTracking()
                join b in _db.Books.AsNoTracking() on l.BookId equals b.Id
                join m in _db.Members.AsNoTracking() on l.MemberId equals m.Id
                select new LoanListItemVm
                {
                    LoanId = l.Id,
                    BookTitle = b.Title,
                    BookIsbn = b.Isbn,
                    MemberFullName = m.FullName,
                    MemberEmail = m.Email,
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate
                };

            if (normalized == "active")
            {
                query = query.Where(x => x.ReturnDate == null);
            }
            else if (normalized == "returned")
            {
                query = query.Where(x => x.ReturnDate != null);
            }
            else if (normalized == "overdue")
            {
                // overdue = not returned AND due date in the past (UTC-based)
                query = query.Where(x => x.ReturnDate == null && x.DueDate < System.DateTime.UtcNow);
            }

            var loans = await query
                .OrderByDescending(x => x.LoanDate)
                .ThenBy(x => x.BookTitle)
                .ToListAsync();

            ViewData["Title"] = "Loans";
            ViewData["Status"] = normalized;

            return View(loans);
        }
    }
}