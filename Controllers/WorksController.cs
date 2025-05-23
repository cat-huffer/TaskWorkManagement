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
                .Include(w => w.MemberWorks)// 类似于SQL中的左连接
                .ThenInclude(mw => mw.Member)
                .OrderBy(w => w.Priority)
                .ThenByDescending(w => w.DueDate)
                .ToListAsync();

            var model = new WorkIndexViewModel
            {
                Members = members,
                Works = works,
                StartDate = DateTime.Today.AddDays(-7),
                EndDate = DateTime.Today.AddDays(30)
            };

            return View(model);
        }

        // GET: Works/Gantt
        public async Task<IActionResult> Gantt()//TODO 拖动左右任务条，保留左侧的名称，人
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
                .Where(w=>!w.IsCompleted)
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
                isCompleted = work.IsCompleted,
                priority = work.Priority,
                priorityText = work.Priority.ToString(),
                members = work.MemberWorks.Select(mw => mw.Member.MemberName).ToList(),
                completedDate = work.CompletedDate?.ToString("yyyy-MM-dd")
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

        // GET: Works/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Works/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkId,Title,Description,StartDate,DueDate,CompletedDate,IsCompleted,Priority")] Work work)
        {
            if (ModelState.IsValid)
            {
                _context.Add(work);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(work);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkId,Title,Description,StartDate,DueDate,CompletedDate,IsCompleted,Priority")] Work work)// bind 限制绑定的属性,只绑定指定的属性
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
