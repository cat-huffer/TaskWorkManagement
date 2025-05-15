using System.ComponentModel.DataAnnotations;

namespace TaskWorkManagement.Models
{
    public class Work
    {
        public int WorkId { get; set; }

        [Required(ErrorMessage = "タスク名を入力してください")]
        [Display(Name = "タスク名")]
        public string Title { get; set; }

        [Display(Name = "詳細")]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "登録日")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "締切日を入力してください")]
        [Display(Name = "締切日")]
        public DateTime DueDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "完了日")]
        public DateTime? CompletedDate { get; set; }

        [Display(Name = "完了済み")]
        public bool IsCompleted { get; set; } = false;

        [Display(Name = "優先度")]
        [Range(1, 5, ErrorMessage = "優先度は1～5の範囲で設定してください")]
        public int Priority { get; set; } = 3;

        [Display(Name = "所要時間")]
        [Range(1, 365, ErrorMessage = "所要時間は1～365日の範囲で設定してください")]
        public int Duration { get; set; } = 1;

        [Display(Name = "カラー")]
        public string Color { get; set; } = "#3498db";

        public ICollection<MemberWork> MemberWorks { get; set; } = new List<MemberWork>();
    }
}
