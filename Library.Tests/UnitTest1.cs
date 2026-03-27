using Library.Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_76266_a.Web.Data;
using oop_s2_1_mvc_76266_a.Web.Services;

namespace Library.Tests
{
    public class UnitTest1 : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ApplicationDbContext _context;
        private readonly LibraryService _service;

        public UnitTest1()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            SeedData();
            _service = new LibraryService(_context);
        }

        private void SeedData()
        {
            var books = new List<Book>
            {
                new Book { Title = "Clean Code", Author = "Robert C. Martin", Isbn = "1111111111111", Category = "Programming", IsAvailable = false },
                new Book { Title = "The Hobbit", Author = "J.R.R. Tolkien", Isbn = "2222222222222", Category = "Fantasy", IsAvailable = false },
                new Book { Title = "Refactoring", Author = "Martin Fowler", Isbn = "3333333333333", Category = "Programming", IsAvailable = true }
            };

            var members = new List<Member>
            {
                new Member { FullName = "Alice Johnson", Email = "alice@example.com", Phone = "111111111" },
                new Member { FullName = "Brian Smith", Email = "brian@example.com", Phone = "222222222" }
            };

            _context.Books.AddRange(books);
            _context.Members.AddRange(members);
            _context.SaveChanges();

            var loans = new List<Loan>
            {
                new Loan
                {
                    BookId = books[0].Id,
                    MemberId = members[0].Id,
                    LoanDate = DateTime.Today.AddDays(-10),
                    DueDate = DateTime.Today.AddDays(5),
                    ReturnedDate = null
                },
                new Loan
                {
                    BookId = books[1].Id,
                    MemberId = members[1].Id,
                    LoanDate = DateTime.Today.AddDays(-20),
                    DueDate = DateTime.Today.AddDays(-3),
                    ReturnedDate = null
                },
                new Loan
                {
                    BookId = books[2].Id,
                    MemberId = members[0].Id,
                    LoanDate = DateTime.Today.AddDays(-30),
                    DueDate = DateTime.Today.AddDays(-20),
                    ReturnedDate = DateTime.Today.AddDays(-15)
                }
            };

            _context.Loans.AddRange(loans);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Cannot_create_loan_for_book_already_on_active_loan()
        {
            var activeBookId = _context.Books.First(b => b.Title == "Clean Code").Id;
            var memberId = _context.Members.First(m => m.FullName == "Brian Smith").Id;

            var createdLoan = await _service.CreateLoanAsync(
                activeBookId,
                memberId,
                DateTime.Today,
                DateTime.Today.AddDays(7));

            Assert.Null(createdLoan);
        }

        [Fact]
        public async Task Returned_loan_makes_book_available_again()
        {
            var book = _context.Books.First(b => b.Title == "Clean Code");
            var loan = _context.Loans.First(l => l.BookId == book.Id && l.ReturnedDate == null);

            var result = await _service.MarkReturnedAsync(loan.Id, DateTime.Today);

            Assert.True(result);

            book = _context.Books.First(b => b.Id == book.Id);
            Assert.True(book.IsAvailable);
        }

        [Fact]
        public void Book_search_returns_expected_matches()
        {
            var results = _service.BuildBooksQuery("martin", null, null).ToList();

            Assert.Equal(2, results.Count);
            Assert.Contains(results, b => b.Title == "Clean Code");
            Assert.Contains(results, b => b.Title == "Refactoring");
        }

        [Fact]
        public void Overdue_logic_returns_only_active_overdue_loans()
        {
            var overdueLoans = _service.GetOverdueLoansQuery(DateTime.Today).ToList();

            Assert.Single(overdueLoans);
            Assert.Equal("The Hobbit", overdueLoans[0].Book!.Title);
            Assert.Null(overdueLoans[0].ReturnedDate);
        }

        [Fact]
        public async Task Can_create_loan_when_book_is_available()
        {
            var availableBook = _context.Books.First(b => b.Title == "Refactoring");
            var memberId = _context.Members.First(m => m.FullName == "Brian Smith").Id;

            var createdLoan = await _service.CreateLoanAsync(
                availableBook.Id,
                memberId,
                DateTime.Today,
                DateTime.Today.AddDays(14));

            Assert.NotNull(createdLoan);

            availableBook = _context.Books.First(b => b.Id == availableBook.Id);
            Assert.False(availableBook.IsAvailable);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}