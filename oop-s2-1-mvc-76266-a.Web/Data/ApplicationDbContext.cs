using Library.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OopS21Mvc76266A.Web.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Domain tables
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Book rules
        builder.Entity<Book>(entity =>
        {
            entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
            entity.Property(b => b.Author).IsRequired().HasMaxLength(150);
            entity.Property(b => b.Isbn).IsRequired().HasMaxLength(20);

            entity.HasIndex(b => b.Isbn).IsUnique();
        });

        // Member rules
        builder.Entity<Member>(entity =>
        {
            entity.Property(m => m.FullName).IsRequired().HasMaxLength(150);
            entity.Property(m => m.Email).IsRequired().HasMaxLength(200);

            entity.HasIndex(m => m.Email).IsUnique();
        });

        // Loan rules + relationships
        builder.Entity<Loan>(entity =>
        {
            entity.Property(l => l.LoanDate).IsRequired();
            entity.Property(l => l.DueDate).IsRequired();

            entity.HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(l => l.Member)
                .WithMany(m => m.Loans)
                .HasForeignKey(l => l.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}