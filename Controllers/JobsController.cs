using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Job_Recruitment.Contexts;
using Job_Recruitment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Transactions;
using Job_Recruitment.ViewModels;
using Job_Recruitment.Helpers;
using System.Security.Claims;

namespace Job_Recruitment.Controllers
{
    public class JobsController : Controller
    {
        private readonly JobDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public JobsController(JobDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> SearchForJob(string searchTerm)
        {
            var jobs = await _context.Jobs
                .Where(j => j.Title.Contains(searchTerm))
                .Select(j => new
                {
                    id = j.Id,
                    text = j.Title
                })
                .ToListAsync();

            return Json(jobs);
        }


        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> ViewApplications(int? jobId)
        {
            if (jobId == null)
            {
                return NotFound();
            }

            var applications = await _context.Applications
                                             .Include(a => a.Job)
                                             .Where(a => a.Job.EmployerId == User.FindFirstValue(ClaimTypes.NameIdentifier) && a.Job.Id == jobId)
                                             .ToListAsync();

            return View(applications);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> BecomeEmployer()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            if (await _userManager.IsInRoleAsync(user, "Employer"))
            {
                return BadRequest("You are already an Employer.");
            }
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (await _userManager.IsInRoleAsync(user, "Employee"))
                {
                    var removeResult = await _userManager.RemoveFromRoleAsync(user, "Employee");
                    if (!removeResult.Succeeded)
                    {
                        return BadRequest("Failed to remove from Employee role.");
                    }
                }
                var addResult = await _userManager.AddToRoleAsync(user, "Employer");
                if (!addResult.Succeeded)
                {
                    return BadRequest("Failed to become an Employer.");
                }

                transaction.Complete();
            }

            return RedirectToAction("Index", "Jobs");
        }

        // GET: Jobs
        public async Task<IActionResult> Index()
        {
            var jobs = await _context.Jobs.Include(j => j.Employer).ToListAsync();
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var roles = await _userManager.GetRolesAsync(user);
                var modelNotNull = new UserVm()
                {
                    User = user,
                    Jobs = jobs,
                    Role = roles.FirstOrDefault()
                };
                return View(modelNotNull);
            }
            var model = new UserVm()
            {
                Jobs = jobs,
            };

            return View(model);
        }

        // GET: Jobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs.Include(j => j.Employer).FirstOrDefaultAsync(j => j.Id == id);
            var employer = await _context.Users.FirstOrDefaultAsync(e => e.Id == job.EmployerId);
            if (job == null)
            {
                return NotFound();
            }
            var applications = await _context.Applications
                                    .Where(a => a.JobId == id)
                                    .ToListAsync();

            var candidateIds = applications.Select(a => a.CandidateId).ToList();
            var candidateNames = await _context.Users
                                               .Where(u => candidateIds.Contains(u.Id))
                                               .ToDictionaryAsync(u => u.Id, u => u.FirstName);

            var model = new DetailVm()
            {
                User = employer,
                Job = job,
                Applications = applications,
                CandidateNames = candidateNames
            };

            return View(model);
        }

        // GET: Jobs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Location,Salary,JobType,IsActive")] Job job)
        {
            var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            job.EmployerId = employerId;
            _context.Add(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Jobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Location,Salary,JobType,IsActive")] Job job)
        {
            if (id != job.Id)
            {
                return NotFound();
            }
            var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            job.EmployerId = employerId;
            try
            {
                _context.Update(job);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(job.Id))
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

        // GET: Jobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Jobs == null)
            {
                return Problem("Entity set 'JobDbContext.Jobs'  is null.");
            }
            var job = await _context.Jobs.FindAsync(id);
            if (job != null)
            {
                _context.Jobs.Remove(job);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobExists(int id)
        {
            return (_context.Jobs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
