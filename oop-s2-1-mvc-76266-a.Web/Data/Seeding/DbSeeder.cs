using Library.Domain.Entities;

namespace oop_s2_1_mvc_76266_a.Web.Data.Seeding
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Books.Any() || context.Members.Any() || context.Loans.Any())
                return;

            var books = new List<Book>
            {
                new Book { Title = "Clean Code", Author = "Robert C. Martin", Isbn = "9780132350884", Category = "Programming", IsAvailable = false },
                new Book { Title = "The Pragmatic Programmer", Author = "Andrew Hunt", Isbn = "9780201616224", Category = "Programming", IsAvailable = false },
                new Book { Title = "Design Patterns", Author = "Erich Gamma", Isbn = "9780201633610", Category = "Programming", IsAvailable = false },
                new Book { Title = "Refactoring", Author = "Martin Fowler", Isbn = "9780201485677", Category = "Programming", IsAvailable = true },
                new Book { Title = "Introduction to Algorithms", Author = "Thomas H. Cormen", Isbn = "9780262033848", Category = "Programming", IsAvailable = true },
                new Book { Title = "The Hobbit", Author = "J.R.R. Tolkien", Isbn = "9780261103344", Category = "Fantasy", IsAvailable = false },
                new Book { Title = "The Lord of the Rings", Author = "J.R.R. Tolkien", Isbn = "9780261102385", Category = "Fantasy", IsAvailable = true },
                new Book { Title = "Harry Potter and the Philosopher's Stone", Author = "J.K. Rowling", Isbn = "9780747532743", Category = "Fantasy", IsAvailable = true },
                new Book { Title = "Sapiens", Author = "Yuval Noah Harari", Isbn = "9780062316097", Category = "History", IsAvailable = true },
                new Book { Title = "Educated", Author = "Tara Westover", Isbn = "9780399590504", Category = "Biography", IsAvailable = true }
            };

            var members = new List<Member>
            {
                new Member { FullName = "Alice Johnson", Email = "alice@example.com", Phone = "111111111" },
                new Member { FullName = "Brian Smith", Email = "brian@example.com", Phone = "222222222" },
                new Member { FullName = "Catherine Brown", Email = "catherine@example.com", Phone = "333333333" },
                new Member { FullName = "David Wilson", Email = "david@example.com", Phone = "444444444" },
                new Member { FullName = "Emma Davis", Email = "emma@example.com", Phone = "555555555" }
            };

            context.Books.AddRange(books);
            context.Members.AddRange(members);
            context.SaveChanges();

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
                    BookId = books[5].Id,
                    MemberId = members[2].Id,
                    LoanDate = DateTime.Today.AddDays(-15),
                    DueDate = DateTime.Today.AddDays(-1),
                    ReturnedDate = null
                }
            };

            context.Loans.AddRange(loans);
            context.SaveChanges();
        }
    }
}