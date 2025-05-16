using System.ComponentModel.DataAnnotations;

namespace TaskWorkManagement.Models
{
    public class MemberWork
    {
        public int MemberWorkId { get; set; }

        [Display(Name = "役割")]
        public string? Role { get; set; } // 如"负责人","参与者"

        public int WorkId { get; set; }
        public Work Work { get; set; }

        public int MemberId { get; set; }
        public Member Member { get; set; }
    }
}
