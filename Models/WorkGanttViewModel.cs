namespace TaskWorkManagement.Models
{
    public class WorkGanttViewModel
    {
        public List<Member> Members { get; set; }
        public List<Work> Works { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
