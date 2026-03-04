namespace Library.Domain.Entities;

public class Member
{
    public int Id { get; set; }

    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime JoinedOn { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}