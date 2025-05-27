using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskWorkManagement.Data;
using TaskWorkManagement.Models;

namespace TaskManagement.Controllers
{
    public class WorksController : Controller
    {
        private readonly TaskWorkManagementContext _context;

        public WorksController(TaskWorkManagementContext context)
        {
            _context = context;
        }

        // GET: Works
        public async Task<IActionResult> Index()//TODO 修改表的排序;修改日期部分
                                                //TODO 任务列表标签居中
                                                //TODO 修改完了和完成时间的设计
        {
            var members = await _context.Member
                .OrderBy(m => m.MemberName)
                .ToListAsync();

            var works = await _context.Work
                .Include(w => w.MemberWorks)// 类似于SQL中的左连接.但只用于加载导航属性关联的数据，不会将关联实体的属性"合并"到主实体中
                .ThenInclude(mw => mw.Member)
                .OrderBy(w => w.Priority)
                .ThenByDescending(w => w.DueDate)
                .ToListAsync();

            var workViewModel = works.Select(w => new WorkItemViewModel // 用于转换集合中的每个元素
            {
                WorkId = w.WorkId,
                Title = w.Title,
                Description = w.Description,
                StartDate = w.StartDate,
                DueDate = w.DueDate,
                Priority = w.Priority,
                CompletedDate = w.MemberWorks
                    .Select(mw => mw.CompletedDate)
                    .FirstOrDefault(), 
                Members = string.Join(", ", w.MemberWorks.Select(mw => mw.Member.MemberName))
            }).ToList();

            var model = new WorkIndexViewModel
            {
                Members = members,
                Works = workViewModel
            };

            return View(model);
        }

        // GET: Works/Gantt
        public async Task<IActionResult> Gantt()//TODO 拖动左右任务条，保留左侧的名称
        {
            var members = await _context.Member
                .OrderBy(m => m.MemberName)
                .ToListAsync();

            var works = await _context.Work
                .Include(w => w.MemberWorks)
                .ThenInclude(mw => mw.Member)
                .OrderBy(w => w.Priority)
                .ThenBy(w => w.DueDate)
                .ToListAsync();

            var latestTime = _context.Work
                .OrderByDescending(w => w.StartDate)//将较晚的日期排在前面
                .Select(w => w.StartDate)
                .FirstOrDefault();//没有数据时会返回 default(DateTime)

            if (latestTime == default(DateTime))
            {
                latestTime = DateTime.Today;
            }

            var model = new WorkGanttViewModel
            {
                Members = members,
                Works = works,
                StartDate = latestTime,
                EndDate = DateTime.Today.AddDays(30)
            };

            return View(model);
        }
                
        [HttpGet("Works/GetWorkDetails/{id}")]
        public async Task<IActionResult> GetWorkDetails(int id)
        {
            var work = await _context.Work
                .Include(w => w.MemberWorks)
                .ThenInclude(mw => mw.Member)
                .FirstOrDefaultAsync(w => w.WorkId == id);

            if (work == null)
            {
                return NotFound();
            }

            return Json(new
            {
                title = work.Title,
                description = work.Description,
                startDate = work.StartDate.ToString("yyyy-MM-dd"),
                dueDate = work.DueDate.ToString("yyyy-MM-dd"),
                priority = work.Priority,
                priorityText = work.Priority.ToString(),
                members = work.MemberWorks.Select(mw => mw.Member.MemberName).ToList(),
                completedDate = work.MemberWorks.FirstOrDefault()?.CompletedDate?.ToString("yyyy-MM-dd")
            });
        }

        // GET: Works/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var work = await _context.Work
                .FirstOrDefaultAsync(m => m.WorkId == id);
            if (work == null)
            {
                return NotFound();
            }
            return View(work);
        }

        // GET: Works/Create. 用于显示空表单（用户填写前）
        public IActionResult Create() 
        {
            return View();
        }

        // POST: Works/Create
        [HttpPost]
        [ValidateAntiForgeryToken] // 用于防止跨站请求伪造攻击。它的作用是确保表单提交的请求来自您的网站，而不是恶意伪造的请求。
        public async Task<IActionResult> Create([Bind("WorkId,Title,Description,StartDate,DueDate,Priority")] WorkIndexViewModel ViewModel)// 用于控制模型绑定过程中哪些属性可以被绑定，防止过度提交攻击
        {
            if (ModelState.IsValid) //用于验证模型数据的关键检查，如[Required]、[Range]
            {
                var work = new Work
                {
                    Title = ViewModel.Works.First().Title,
                    Description = ViewModel.Works.First().Description,
                    StartDate = ViewModel.Works.First().StartDate,
                    DueDate = ViewModel.Works.First().DueDate,
                    Priority = ViewModel.Works.First().Priority
                };

                _context.Add(work);
                await _context.SaveChangesAsync(); // 必须先保存，才能获取WorkId

                var memberWork = new MemberWork
                {
                    CompletedDate = ViewModel.Works.First().CompletedDate,
                    Director = ViewModel.Works.First().Members
                };

                _context.Add(memberWork);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            // 如果验证失败
            return View(ViewModel);
        }

        // GET: Works/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var work = await _context.Work.FindAsync(id);
            if (work == null)
            {
                return NotFound();
            }
            return View(work);
        }

        // POST: Works/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkId,Title,Description,StartDate,IsCompleted,Priority")] Work work)// bind 限制绑定的属性,只绑定指定的属性
        {
            if (id != work.WorkId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(work);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkExists(work.WorkId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(work);
        }

        // GET: Works/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var work = await _context.Work
                .FirstOrDefaultAsync(m => m.WorkId == id);
            if (work == null)
            {
                return NotFound();
            }

            return View(work);
        }

        // POST: Works/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var work = await _context.Work.FindAsync(id);
            if (work != null)
            {
                _context.Work.Remove(work);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkExists(int id)
        {
            return _context.Work.Any(e => e.WorkId == id);
        }
    }
}
