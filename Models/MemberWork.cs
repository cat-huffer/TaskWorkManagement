using System.ComponentModel.DataAnnotations;

namespace TaskWorkManagement.Models
{
    public class MemberWork
    {
        public int MemberWorkId { get; set; }


        public int WorkId { get; set; }
        public int MemberId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "完了日")]
        public DateTime? CompletedDate { get; set; }

        [Required]
        [Display(Name = "负责人")]
        public String Director { get; set; }

        /// <summary>
        /// 导航属性
        /// </summary>
        public Work Work { get; set; }
        public Member Member { get; set; }
    }
}
