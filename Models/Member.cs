using System.ComponentModel.DataAnnotations;

namespace TaskWorkManagement.Models
{
    public class Member
    {
        public int MemberId { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "成员姓名")]
        public string MemberName { get; set; }

        [Display(Name = "职位")]
        public string? Position { get; set; }

        [Display(Name = "颜色")]
        public string Color { get; set; } = "#e74c3c";

        public ICollection<MemberWork> MemberWorks { get; set; } = new List<MemberWork>();
    }

}
