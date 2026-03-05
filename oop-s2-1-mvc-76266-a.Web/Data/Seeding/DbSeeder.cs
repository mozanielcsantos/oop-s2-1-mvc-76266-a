using Bogus;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OopS21Mvc76266A.Web.Data;

namespace OopS21Mvc76266A.Web.Data.Seeding;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        // Make sure database exists and is migrated
        await db.Database.MigrateAsync();

        // If we already have books, assume seeded
        if (await db.Books.AnyAsync())
            return;

        // -------------------------
        // 1) Seed Books (20)
        // -------------------------
        var bookFaker = new Faker<Book>()
            .RuleFor(b => b.Title, f => f.Commerce.ProductName())
            .RuleFor(b => b.Author, f => f.Name.FullName())
            .RuleFor(b => b.Isbn, f => f.Random.ReplaceNumbers("978-##########"))
            .RuleFor(b => b.PublishedYear, f => f.Random.Int(1950, DateTime.UtcNow.Year));

        var books = bookFaker.Generate(20);

        // Ensure unique ISBNs (because we created a unique index)
        var usedIsbns = new HashSet<string>();
        foreach (var book in books)
        {
            while (!usedIsbns.Add(book.Isbn))
            {
                book.Isbn = new Faker().Random.ReplaceNumbers("978-##########");
            }
        }

        db.Books.AddRange(books);
        await db.SaveChangesAsync();

        // -------------------------
        // 2) Seed Members (10)
        // -------------------------
        var memberFaker = new Faker<Member>()
            .RuleFor(m => m.FullName, f => f.Name.FullName())
            .RuleFor(m => m.Email, f => f.Internet.Email())
            .RuleFor(m => m.JoinedOn, f => f.Date.Past(3, DateTime.UtcNow));

        var members = memberFaker.Generate(10);

        // Ensure unique Emails (because we created a unique index)
        var usedEmails = new HashSet<string>();
        foreach (var member in members)
        {
            while (!usedEmails.Add(member.Email))
            {
                member.Email = new Faker().Internet.Email();
            }
        }

        db.Members.AddRange(members);
        await db.SaveChangesAsync();

        // -------------------------
        // 3) Seed Loans (15)
        // -------------------------
        var rng = new Random();

        var loans = new List<Loan>();
        for (int i = 0; i < 15; i++)
        {
            var book = books[rng.Next(books.Count)];
            var member = members[rng.Next(members.Count)];

            var loanDate = DateTime.UtcNow.AddDays(-rng.Next(1, 30));
            var dueDate = loanDate.AddDays(14);

            // ~50% returned
            DateTime? returnDate = null;
            if (rng.NextDouble() < 0.5)
            {
                returnDate = loanDate.AddDays(rng.Next(1, 20));
                if (returnDate > DateTime.UtcNow) returnDate = null;
            }

            loans.Add(new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                LoanDate = loanDate,
                DueDate = dueDate,
                ReturnDate = returnDate
            });
        }

        db.Loans.AddRange(loans);
        await db.SaveChangesAsync();
    }
}