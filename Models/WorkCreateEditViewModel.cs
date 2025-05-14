using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TaskWorkManagement.Models
{
    public class WorkCreateEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "任务标题不能为空")]
        [Display(Name = "任务")]
        public string Title { get; set; }

        [Display(Name = "任务描述")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "开始日期")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [Display(Name = "截止日期")]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);

        [Display(Name = "持续时间(天)")]
        [Range(1, 365, ErrorMessage = "持续时间必须在1-365天之间")]
        public int Duration { get; set; } = 1;

        [Display(Name = "优先级")]
        [Range(1, 5, ErrorMessage = "优先级必须在1-5之间")]
        public int Priority { get; set; } = 3;

        [Display(Name = "负责成员")]
        public List<int> SelectedMemberIds { get; set; } = new List<int>();

        public List<SelectListItem> AvailableMembers { get; set; }

        [Display(Name = "颜色")]
        public string Color { get; set; } = "#3498db";
    }
}
