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
                            DueDate = DateTime.Now.AddDays(7)
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
                if (!context.Member.Any())
                {
                    context.Member.AddRange(
                        new Member
                        {
                            MemberName = "吴徐俊彦",
                            Position = "实习生"
                        },
                        new Member
                        {
                            MemberName = "顾守强",
                            Position = "领导"
                        },
                        new Member
                        {
                            MemberName = "李正之",
                            Position = "成员"
                        }
                    );
                }
                context.SaveChanges();
            }

        }
    }
}
