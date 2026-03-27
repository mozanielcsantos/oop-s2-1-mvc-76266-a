namespace oop_s2_1_mvc_76266_a.Web.Models
{
    public class LoanListItemVm
    {
        public int Id { get; set; }

        public string BookTitle { get; set; } = string.Empty;

        public string MemberName { get; set; } = string.Empty;

        public DateTime LoanDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? ReturnedDate { get; set; }
    }
}