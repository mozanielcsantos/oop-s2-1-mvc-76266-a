namespace Library.Domain.Entities;

public class Loan
{
    public int Id { get; set; }

    // Foreign keys
    public int BookId { get; set; }
    public int MemberId { get; set; }

    // Dates
    public DateTime LoanDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    // Navigation
    public Book? Book { get; set; }
    public Member? Member { get; set; }

    // Helper
    public bool IsReturned => ReturnDate != null;
}