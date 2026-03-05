using System;

namespace oop_s2_1_mvc_76266_a.Web.Models
{
    public class LoanListItemVm
    {
        public int LoanId { get; set; }

        public string BookTitle { get; set; } = "";
        public string BookIsbn { get; set; } = "";

        public string MemberFullName { get; set; } = "";
        public string MemberEmail { get; set; } = "";

        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public bool IsReturned => ReturnDate.HasValue;
        public bool IsOverdue => !IsReturned && DueDate.Date < DateTime.UtcNow.Date;
    }
}