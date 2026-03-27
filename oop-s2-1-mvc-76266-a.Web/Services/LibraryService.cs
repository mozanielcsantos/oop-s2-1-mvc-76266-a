using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_76266_a.Web.Data;

namespace oop_s2_1_mvc_76266_a.Web.Services
{
    public class LibraryService
    {
        private readonly ApplicationDbContext _context;

        public LibraryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Book> BuildBooksQuery(string? searchTerm, string? category, string? availability)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();

                query = query.Where(b =>
                    b.Title.ToLower().Contains(term) ||
                    b.Author.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(b => b.Category == category);
            }

            if (!string.IsNullOrWhiteSpace(availability))
            {
                if (availability == "Available")
                {
                    query = query.Where(b => b.IsAvailable);
                }
                else if (availability == "OnLoan")
                {
                    query = query.Where(b => !b.IsAvailable);
                }
            }

            return query;
        }

        public async Task<bool> CanCreateLoanAsync(int bookId)
        {
            return !await _context.Loans.AnyAsync(l => l.BookId == bookId && l.ReturnedDate == null);
        }

        public async Task<Loan?> CreateLoanAsync(int bookId, int memberId, DateTime loanDate, DateTime dueDate)
        {
            var canCreate = await CanCreateLoanAsync(bookId);

            if (!canCreate)
            {
                return null;
            }

            var loan = new Loan
            {
                BookId = bookId,
                MemberId = memberId,
                LoanDate = loanDate,
                DueDate = dueDate,
                ReturnedDate = null
            };

            var book = await _context.Books.FindAsync(bookId);
            if (book != null)
            {
                book.IsAvailable = false;
            }

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return loan;
        }

        public async Task<bool> MarkReturnedAsync(int loanId, DateTime returnedDate)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == loanId);

            if (loan == null)
            {
                return false;
            }

            loan.ReturnedDate = returnedDate;

            if (loan.Book != null)
            {
                loan.Book.IsAvailable = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public IQueryable<Loan> GetOverdueLoansQuery(DateTime today)
        {
            return _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Where(l => l.DueDate < today && l.ReturnedDate == null);
        }
    }
}