using System.ComponentModel.DataAnnotations;
using System;

namespace TaskWorkManagement.Models
{
    public class Work
    {
        public int WorkId { get; set; }

        [Required(ErrorMessage = "作業名を入力してください")]
        [Display(Name = "作業名")]
        public string Title { get; set; }

        [Display(Name = "詳細")]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "登録日を入力してください")]
        [Display(Name = "登録日")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "期限日を入力してください")]
        [Display(Name = "期限日")]
        public DateTime DueDate { get; set; }

        [Display(Name = "優先度")]
        [Required(ErrorMessage = "優先度を入力してください")]
        [Range(0, 2, ErrorMessage = "優先度は0（低い）～2（高い）の範囲で設定してください")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.中;

        /// <summary>
        /// 导航属性
        /// </summary>
        public ICollection<MemberWork> MemberWorks { get; set; } = new List<MemberWork>();
    }
}

public enum PriorityLevel
{
    低い = 0,
    中 = 1,
    高い = 2
};
