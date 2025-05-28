using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TaskWorkManagement.Models
{
    // 视图模型仅用于数据传输和表单绑定，不直接映射到数据库
    public class WorkItemViewModel // 用于显示工作项的视图模型。在每一个工作中中加入了该工作的负责人。理解为work类的替代品
    {
        public int WorkId { get; set; }

        [Display(Name = "作業名")]
        public string Title { get; set; }

        [Display(Name = "詳細")]
        public string? Description { get; set; }

        [Display(Name = "登録日")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Display(Name = "期限日")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.Today;


        [Display(Name = "優先度")]
        public PriorityLevel Priority { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "完了日")]
        public DateTime? CompletedDate { get; set; }
        

        //渲染视图用
        public List<SelectListItem> AvailableMembers { get; set; } = new List<SelectListItem>();

        // 已选中的成员ID列表
        [Display(Name = "担当者")]
        public List<int> SelectedMemberIds { get; set; } = new List<int>();
    }
}
