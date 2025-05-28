using Microsoft.EntityFrameworkCore;
using TaskWorkManagement.Models;
using TaskWorkManagement.Data;

namespace TaskWorkManagement.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new TaskWorkManagementContext(
                serviceProvider.GetRequiredService<DbContextOptions<TaskWorkManagementContext>>()))
            {
                // Look for any works.
                if (!context.Work.Any())
                {
                    context.Work.AddRange(
                        new Work
                        {
                            Title = "默认任务",
                            Description = "默认描述",
                            StartDate = DateTime.Now,
                            DueDate = DateTime.Now.AddDays(7),
                            Priority = PriorityLevel.中
                        }
                    );
                }
                if (!context.Member.Any())
                {
                    context.Member.AddRange(
                        new Member
                        {
                            MemberName = "吴徐俊彦",
                            Position = "实习生"
                        }
                    );
                }

                if (!context.User.Any())
                {
                    context.User.AddRange(
                        new User
                        {
                            UserName = "admin",
                            Password = "123456",
                            NickName = "kiriri"
                        }
                    );
                }
                //为了确保MemberWork表有数据，先添加Work和Member数据
                context.SaveChanges();

                if (!context.MemberWork.Any())
                {                    
                    var work = context.Work.FirstOrDefault();
                    var member = context.Member.FirstOrDefault();

                    if (work != null && member != null)
                    {
                        context.MemberWork.AddRange(
                            new MemberWork
                            {
                                WorkId = work.WorkId, 
                                MemberId = member.MemberId, 
                                CompletedDate = DateTime.Now
                            }
                        );
                    }
                }

                context.SaveChanges();
            }

        }
    }
}
