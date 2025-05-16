using System.ComponentModel.DataAnnotations;

namespace TaskWorkManagement.Models
{
    public class Member
    {
        public int MemberId { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "メンバー名")]
        public string MemberName { get; set; }

        [Display(Name = "ポジション")]
        public string? Position { get; set; }

        public ICollection<MemberWork> MemberWorks { get; set; } = new List<MemberWork>();
    }

}
