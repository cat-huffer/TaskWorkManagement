using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
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
        public async Task<IActionResult> Index()
        {
            var members = await _context.Member
                .OrderBy(m => m.MemberName)
                .ToListAsync();

            var works = await _context.Work
                .Include(w => w.MemberWorks)// 类似于SQL中的左连接.但只用于加载导航属性关联的数据，不会将关联实体的属性"合并"到主实体中
                .ThenInclude(mw => mw.Member)
                .OrderBy(w => w.DueDate)
                .ThenByDescending(w => w.Priority)
                .ToListAsync();

            var workViewModel = works.Select(w => new WorkItemViewModel // 转换集合中的每个元素
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
                SelectedMemberIds = w.MemberWorks
                    .Select(mw => mw.MemberId)
                    .ToList()
            }).ToList();

            var model = new WorkIndexViewModel
            {
                Members = members,
                Works = workViewModel
            };

            return View(model);
        }

        // GET: Works/Gantt
        public async Task<IActionResult> Gantt()//TODO 拖动左右任务条
                                                //TODO 模态框标题居中
                                                //TODO 切换时间：上中下旬，月份，日期
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

            var workViewModels = works.Select(w => new WorkItemViewModel
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
                SelectedMemberIds = w.MemberWorks
                    .Select(mw => mw.MemberId)
                    .ToList()
            }).ToList();

            var viewModel = new WorkIndexViewModel
            {
                Members = members,
                Works = workViewModels
            };

            return View(viewModel);
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

            string status = "進行中";
            if (work.MemberWorks.FirstOrDefault()?.CompletedDate != null && work.MemberWorks.FirstOrDefault()?.CompletedDate <= work.DueDate)
            {
                status = "完了";
            }
            else if (work.DueDate < DateTime.Today)
            {
                status = "期限切れ";
            }

            return Json( new
            {
                title = work.Title,
                description = work.Description,
                startDate = work.StartDate.ToString("yyyy-MM-dd"),
                dueDate = work.DueDate.ToString("yyyy-MM-dd"),
                priority = work.Priority,
                priorityText = work.Priority.ToString(),
                members = work.MemberWorks.Select(mw => mw.Member.MemberName).ToList(),
                completedDate = work.MemberWorks.FirstOrDefault()?.CompletedDate?.ToString("yyyy-MM-dd"),
                status = status
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
                .Include(w => w.MemberWorks) 
                .FirstOrDefaultAsync(m => m.WorkId == id);

            if (work == null)
            {
                return NotFound();
            }

            var memberWorks = await _context.MemberWork
                .Where(mw => mw.WorkId == id)
                .Include(mw => mw.Member)
                .ToListAsync();

            var ViewModel = new WorkItemViewModel 
            {
                WorkId = work.WorkId,
                Title = work.Title,
                Description = work.Description,
                StartDate = work.StartDate,
                DueDate = work.DueDate,
                Priority = work.Priority,
                CompletedDate = memberWorks
                   .Select(mw => mw.CompletedDate)
                   .FirstOrDefault(),
                SelectedMemberIds = work.MemberWorks
                    .Select(mw => mw.MemberId)
                    .ToList()
            };

            ViewBag.Member = await _context.Member
                .OrderBy(m => m.MemberName)
                .ToListAsync();

            return View(ViewModel);
        }

        // GET: Works/Create. 用于显示空表单（用户填写前）
        public IActionResult Create() 
        {
            var allMembers = _context.Member
        .OrderBy(m => m.MemberName)
        .Select(m => new SelectListItem
        {
            Value = m.MemberId.ToString(),
            Text = m.MemberName
        })
        .ToList();

            var viewModel = new WorkItemViewModel
            {
                AvailableMembers = allMembers
            };

            return View(viewModel);
        }

        // POST: Works/Create
        [HttpPost]
        [ValidateAntiForgeryToken] // 用于防止跨站请求伪造攻击。它的作用是确保表单提交的请求来自您的网站，而不是恶意伪造的请求。
        public async Task<IActionResult> Create([Bind("WorkId,Title,CompletedDate,Description,StartDate,DueDate,Priority,SelectedMemberIds")] WorkItemViewModel ViewModel)// 用于控制模型绑定过程中哪些属性可以被绑定，防止过度提交攻击
        {
            if (ModelState.IsValid) //用于验证模型数据的关键检查，如[Required]、[Range]
            {
                var work = new Work
                {                    
                    Title = ViewModel.Title,
                    Description = ViewModel.Description,
                    StartDate = ViewModel.StartDate,
                    DueDate = ViewModel.DueDate,
                    Priority = ViewModel.Priority
                };

                _context.Add(work);
                await _context.SaveChangesAsync(); // 必须先保存，才能获取WorkId


                // 添加关联的MemberWork
                foreach (var memberId in ViewModel.SelectedMemberIds ?? new List<int>())
                {
                    var member = await _context.Member.FindAsync(memberId);
                    if (member != null)
                    {
                        var memberWork = new MemberWork
                        {
                            WorkId = work.WorkId,
                            MemberId = memberId,
                            CompletedDate = ViewModel.CompletedDate
                        };
                        _context.MemberWork.Add(memberWork);
                    }
                }

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

            var work = await _context.Work
                .Include(w => w.MemberWorks)
                .ThenInclude(mw => mw.Member) // 确保加载MemberWorks和对应的Member
                .FirstOrDefaultAsync(w => w.WorkId == id);
            if (work == null)
            {
                return NotFound();
            }

            var allMembers = await _context.Member.ToListAsync();

            var viewModel = new WorkItemViewModel
            {
                WorkId = work.WorkId,
                Title = work.Title,
                Description = work.Description,
                StartDate = work.StartDate,
                DueDate = work.DueDate,
                Priority = work.Priority,
                CompletedDate = work.MemberWorks.FirstOrDefault()?.CompletedDate,//TODO 我们暂且假设所有人任务时间是相同的
                
                AvailableMembers = allMembers.Select(m => new SelectListItem
                {
                    Value = m.MemberId.ToString(),
                    Text = m.MemberName
                }).ToList(),
                SelectedMemberIds = work.MemberWorks.Select(mw => mw.MemberId).ToList()
            };

            return View(viewModel);
        }

        // POST: Works/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkId,Title,CompletedDate,Description,StartDate,DueDate,Priority,SelectedMemberIds")] WorkItemViewModel viewModel) // TODO 并发问题
        {
            if (id != viewModel.WorkId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var work = await _context.Work
                              .Include(w => w.MemberWorks)
                              .FirstOrDefaultAsync(w => w.WorkId == id);

                    if (work == null)
                    {
                        return NotFound();
                    }
                    work.Title = viewModel.Title;
                    work.Description = viewModel.Description;
                    work.StartDate = viewModel.StartDate;
                    work.DueDate = viewModel.DueDate;
                    work.Priority = viewModel.Priority;

                    _context.Work.Update(work); 

                    _context.MemberWork.RemoveRange(work.MemberWorks);

                    foreach (var memberId in viewModel.SelectedMemberIds ?? new List<int>())
                    {
                        var member = await _context.Member.FindAsync(memberId);
                        if (member != null)
                        {
                            var memberWork = new MemberWork
                            {
                                WorkId = work.WorkId,
                                MemberId = memberId,
                                CompletedDate = viewModel.CompletedDate
                            };
                            _context.MemberWork.Add(memberWork);
                        }
                    }

                    await _context.SaveChangesAsync();                    
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkExists(viewModel.WorkId))
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

            // 如果模型验证失败，重新加载可用成员
            viewModel.AvailableMembers = (await _context.Member.ToListAsync())
                .Select(m => new SelectListItem
                {
                    Value = m.MemberId.ToString(),
                    Text = m.MemberName
                }).ToList();
            return View(viewModel);
        }

        // GET: Works/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var work = await _context.Work
                .Include(w => w.MemberWorks)
                .ThenInclude(mw => mw.Member)
                .FirstOrDefaultAsync(m => m.WorkId == id);
            if (work == null)
            {
                return NotFound();
            }

            var memberWorks = await _context.MemberWork
                .Where(mw => mw.WorkId == id)
                .Include(mw => mw.Member)
                .ToListAsync();

            var ViewModel = new WorkItemViewModel
            {
                WorkId = work.WorkId,
                Title = work.Title,
                Description = work.Description,
                StartDate = work.StartDate,
                DueDate = work.DueDate,
                Priority = work.Priority,
                CompletedDate = memberWorks
                   .Select(mw => mw.CompletedDate)
                   .FirstOrDefault(),
                SelectedMemberIds = work.MemberWorks
                    .Select(mw => mw.MemberId)
                    .ToList()
            };

            ViewBag.Member = await _context.Member
                .OrderBy(m => m.MemberName)
                .ToListAsync();

            return View(ViewModel);
        }

        // POST: Works/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var work = await _context.Work
                .Include(w => w.MemberWorks) // 确保删除工作项时也删除关联的MemberWork
                .FirstOrDefaultAsync(w => w.WorkId == id);
            if (work == null)
            {
                return NotFound();
            }

            _context.MemberWork.RemoveRange(work.MemberWorks);
            _context.Work.Remove(work);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool WorkExists(int id)
        {
            return _context.Work.Any(e => e.WorkId == id);
        }
    }
}
