using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskWorkManagement.Models;

namespace TaskWorkManagement.Data
{
    public class TaskWorkManagementContext : DbContext
    {
        public TaskWorkManagementContext (DbContextOptions<TaskWorkManagementContext> options)
            : base(options)
        {
        }

        public DbSet<TaskWorkManagement.Models.Work> Work { get; set; } = default!;
        public DbSet<User> User { get; set; }
        public DbSet<Member> Member { get; set; }
        public DbSet<MemberWork> MemberWork { get; set; }
    }
}
