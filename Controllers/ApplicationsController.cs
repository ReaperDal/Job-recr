using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Job_Recruitment.Contexts;
using Job_Recruitment.Models;
using System.Security.Claims;
using Job_Recruitment.ViewModels;
using Job_Recruitment.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Job_Recruitment.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly JobDbContext _context;

        public ApplicationsController(JobDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetApplicationStatsForCurrentUser()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var acceptedCount = await _context.Applications.CountAsync(a => a.CandidateId == currentUserId && a.Status == "Accepted");
            var rejectedCount = await _context.Applications.CountAsync(a => a.CandidateId == currentUserId && a.Status == "Rejected");
            var pendingCount = await _context.Applications.CountAsync(a => a.CandidateId == currentUserId && a.Status == "Pending");

            return Json(new
            {
                Accepted = acceptedCount,
                Rejected = rejectedCount,
                Pending = pendingCount
            });
        }



        [HttpPost]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> UpdateApplication(int applicationId, string newStatus, string action)
        {
            var application = await _context.Applications.Include(a => a.Job)
                                                          .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null || application.Job.EmployerId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Unauthorized();
            }

            if (action == "deactivate")
            {
                application.IsActive = false;
            }
            else
            {
                application.Status = newStatus;
            }

            _context.Update(application);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Jobs", new { id = application.Job.Id });
        }


        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(await _context.Applications.Where(a => a.CandidateId == userId).ToListAsync());
        }
        public IActionResult ScheduleInterview(int id)
        {
            var viewModel = new ScheduleInterviewVm
            {
                ApplicationId = id
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> SaveInterview(ScheduleInterviewVm viewModel)
        {
            var interview = new Interview
            {
                ApplicationId = viewModel.ApplicationId,
                DateScheduled = viewModel.DateScheduled,
                InterviewerName = viewModel.InterviewerName,
                Result = viewModel.Result ?? "Pending"
            };


            _context.Interview.Add(interview);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Jobs");
        }

        // GET: Applications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Applications == null)
            {
                return NotFound();
            }

            var application = await _context.Applications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // GET: Applications/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Applications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound("Job not found.");
            }

            var employeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var application = new Application
            {
                CandidateId = employeeId,
                JobId = id,
                Status = "Pending"
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Jobs");
        }


        // POST: Applications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CandidateId,DateApplied,Status")] Application application)
        {
            if (id != application.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(application);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationExists(application.Id))
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
            return View(application);
        }


        // POST: Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Applications == null)
            {
                return Problem("Entity set 'JobDbContext.Applications'  is null.");
            }
            var application = await _context.Applications.FindAsync(id);
            if (application != null)
            {
                _context.Applications.Remove(application);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationExists(int id)
        {
            return (_context.Applications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
