using Job_Recruitment.Contexts;
using Job_Recruitment.Helpers;
using Job_Recruitment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Job_Recruitment.Controllers;
public class InterviewsController : Controller
{
    private readonly JobDbContext _context;

    public InterviewsController(JobDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> MyScheduledInterviews()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var interviews = await _context.Interview.Include(i => i.Application).Include(i => i.Application.Job) 
                                       .Where(i => i.Application.CandidateId == currentUserId)
                                       .ToListAsync();
        return View(interviews); 
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Result")] Interview interview)
    {
        if (id != interview.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(interview);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InterviewExists(interview.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(MyScheduledInterviews));
        }
        return View(interview);
    }

    private bool InterviewExists(int id)
    {
        return _context.Interview.Any(e => e.Id == id);
    }


    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var interview = await _context.Interview.FindAsync(id);
        if (interview == null)
        {
            return NotFound();
        }
        return View(interview);
    }

}
