using System.ComponentModel.DataAnnotations;

namespace TaskWorkManagement.Models
{
    public class WorkItemViewModel
    {
        public int WorkId { get; set; }

        [Display(Name = "作業名")]
        public string Title { get; set; }

        [Display(Name = "詳細")]
        public string? Description { get; set; }

        [Display(Name = "登録日")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "期限日")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Display(Name = "優先度")]
        public PriorityLevel Priority { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "完了日")]
        public DateTime? CompletedDate { get; set; }

        [Display(Name = "担当者")]
        [Required(ErrorMessage = "负责人不能为空")]
        public string Members { get; set; } // 将多个负责人合并为一个字符串
    }
}
