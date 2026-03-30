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
                new Book { Title = "Introduction to Algorithms", Author = "Cormen", Isbn = "9780262033848", Category = "Programming", IsAvailable = true },
                new Book { Title = "The Hobbit", Author = "Tolkien", Isbn = "9780261103344", Category = "Fantasy", IsAvailable = false },
                new Book { Title = "LOTR", Author = "Tolkien", Isbn = "9780261102385", Category = "Fantasy", IsAvailable = true },
                new Book { Title = "Harry Potter", Author = "Rowling", Isbn = "9780747532743", Category = "Fantasy", IsAvailable = true },
                new Book { Title = "Sapiens", Author = "Harari", Isbn = "9780062316097", Category = "History", IsAvailable = true },
                new Book { Title = "Educated", Author = "Westover", Isbn = "9780399590504", Category = "Biography", IsAvailable = true },
                new Book { Title = "Dune", Author = "Frank Herbert", Isbn = "9780441172719", Category = "Sci-Fi", IsAvailable = true },
                new Book { Title = "1984", Author = "George Orwell", Isbn = "9780451524935", Category = "Fiction", IsAvailable = true },
                new Book { Title = "Animal Farm", Author = "George Orwell", Isbn = "9780451526342", Category = "Fiction", IsAvailable = true },
                new Book { Title = "The Alchemist", Author = "Paulo Coelho", Isbn = "9780061122415", Category = "Fiction", IsAvailable = true },
                new Book { Title = "The Road", Author = "Cormac McCarthy", Isbn = "9780307387899", Category = "Fiction", IsAvailable = true },
                new Book { Title = "Thinking Fast and Slow", Author = "Kahneman", Isbn = "9780374533557", Category = "Psychology", IsAvailable = true },
                new Book { Title = "Atomic Habits", Author = "James Clear", Isbn = "9780735211292", Category = "Self-help", IsAvailable = true },
                new Book { Title = "Deep Work", Author = "Cal Newport", Isbn = "9781455586691", Category = "Productivity", IsAvailable = true },
                new Book { Title = "Zero to One", Author = "Peter Thiel", Isbn = "9780804139298", Category = "Business", IsAvailable = true },
                new Book { Title = "Lean Startup", Author = "Eric Ries", Isbn = "9780307887894", Category = "Business", IsAvailable = true }
            };

            var members = new List<Member>
            {
                new Member { FullName = "Alice", Email = "alice@test.com", Phone = "111" },
                new Member { FullName = "Bob", Email = "bob@test.com", Phone = "222" },
                new Member { FullName = "Charlie", Email = "charlie@test.com", Phone = "333" },
                new Member { FullName = "David", Email = "david@test.com", Phone = "444" },
                new Member { FullName = "Emma", Email = "emma@test.com", Phone = "555" },
                new Member { FullName = "Frank", Email = "frank@test.com", Phone = "666" },
                new Member { FullName = "Grace", Email = "grace@test.com", Phone = "777" },
                new Member { FullName = "Hannah", Email = "hannah@test.com", Phone = "888" },
                new Member { FullName = "Ivan", Email = "ivan@test.com", Phone = "999" },
                new Member { FullName = "Julia", Email = "julia@test.com", Phone = "000" }
            };

            context.Books.AddRange(books);
            context.Members.AddRange(members);
            context.SaveChanges();

            var loans = new List<Loan>();

            for (int i = 0; i < 15; i++)
            {
                loans.Add(new Loan
                {
                    BookId = books[i].Id,
                    MemberId = members[i % 10].Id,
                    LoanDate = DateTime.Today.AddDays(-i * 2),
                    DueDate = DateTime.Today.AddDays(7 - i),
                    ReturnedDate = i % 3 == 0 ? DateTime.Today.AddDays(-1) : null
                });
            }

            context.Loans.AddRange(loans);
            context.SaveChanges();
        }
    }
}