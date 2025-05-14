using System.ComponentModel.DataAnnotations;

namespace TaskWorkManagement.Models
{
    public class Work
    {
        public int WorkId { get; set; }

        [Required(ErrorMessage = "任务标题不能为空")]
        [Display(Name = "任务")]
        public string Title { get; set; }

        [Display(Name = "任务描述")]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "开始日期")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "截止日期不能为空")]
        [Display(Name = "截止日期")]
        public DateTime DueDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "完成日期")]
        public DateTime? CompletedDate { get; set; }

        [Display(Name = "是否完成")]
        public bool IsCompleted { get; set; } = false;

        [Display(Name = "优先级")]
        [Range(1, 5, ErrorMessage = "优先级必须在1-5之间")]
        public int Priority { get; set; } = 3;

        [Display(Name = "持续时间(天)")]
        [Range(1, 365, ErrorMessage = "持续时间必须在1-365天之间")]
        public int Duration { get; set; } = 1;

        [Display(Name = "颜色")]
        public string Color { get; set; } = "#3498db";

        public ICollection<MemberWork> MemberWorks { get; set; } = new List<MemberWork>();
    }
}
