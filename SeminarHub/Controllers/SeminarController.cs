using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeminarHub.Data;
using SeminarHub.Models;
using System.Globalization;
using System.Security.Claims;

namespace SeminarHub.Controllers
{
    [Authorize]
    public class SeminarController : Controller
    {
        private readonly SeminarHubDbContext context;

        public SeminarController(SeminarHubDbContext _context)
        {
            context = _context;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            SeminarFormViewModel seminar = new()
            {
                Categories = await GetCategories()
            };

            return View(seminar);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SeminarFormViewModel model) 
        {
            if (!DateTime.TryParseExact(model.DateAndTime, ValidationConstants.SeminarDateFormat, 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                ModelState.AddModelError(nameof(model.DateAndTime), 
                    $"Incorrect Date/Time pattern! Use correct format: {ValidationConstants.SeminarDateFormat}");

                if (!ModelState.IsValid)
                {
                    model.Categories = await GetCategories();
                    return View(model);
                }
            }

            Seminar newSeminar = new()
            {
                Topic = model.Topic,
                Lecturer = model.Lecturer,
                Details = model.Details,
                DateAndTime = date,
                Duration = model.Duration,
                OrganizerId = FindUserId(),
                CategoryId = model.CategoryId
            };

            await context.Seminars.AddAsync(newSeminar);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var seminars = await context.Seminars
                 .AsNoTracking()
                 .Select(s => new SeminarsViewModel(
                     s.Id,
                     s.Topic,
                     s.Lecturer,
                     s.Category.Name,
                     s.DateAndTime,
                     s.Organizer.UserName
                     ))
                 .ToListAsync();

            return View(seminars);
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            string userId = FindUserId();

            var semParticipants = await context.Seminars
                .Where(e => e.Id == id)
                .Include(e => e.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (semParticipants == null)
            {
                return BadRequest();
            }

            if (!semParticipants.SeminarsParticipants.Any(p => p.ParticipantId == userId))
            {
                semParticipants.SeminarsParticipants.Add(new SeminarParticipant()
                {
                    SeminarId = semParticipants.Id,
                    ParticipantId = userId
                });

                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Joined));
            }

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            string userId = FindUserId();

            var model = await context.SeminarsPatricipants
                .Where(p => p.ParticipantId == userId)
                .AsNoTracking()
                .Select(p => new SeminarsViewModel(
                     p.SeminarId,
                     p.Seminar.Topic,
                     p.Seminar.Lecturer,
                     p.Seminar.Category.Name,
                     p.Seminar.DateAndTime,
                     p.Seminar.Organizer.UserName
                    ))
                .ToListAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Leave(int id)
        {
            var seminar = await context.Seminars
                .Where(s => s.Id == id)
                .Include(sp => sp.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (seminar == null)
            {
                return BadRequest();
            }

            string userId = FindUserId();

            var sp = seminar.SeminarsParticipants
                .FirstOrDefault(ep => ep.ParticipantId == userId);

            if (sp == null)
            {
                return BadRequest();
            }

            seminar.SeminarsParticipants.Remove(sp);

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var seminar = await context.Seminars.FindAsync(id);

            if (seminar == null)
            {
                return BadRequest();
            }

            if (seminar.OrganizerId != FindUserId())
            {
                return Unauthorized();
            }

            SeminarFormViewModel model = new()
            {
                Topic = seminar.Topic,
                Lecturer = seminar.Lecturer,
                Details = seminar.Details,
                DateAndTime = seminar.DateAndTime.ToString(ValidationConstants.SeminarDateFormat),
                Duration = seminar.Duration,
                CategoryId = seminar.CategoryId,
                Categories = await GetCategories()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SeminarFormViewModel model, int id)
        {
            var seminar = await context.Seminars.FindAsync(id);

            if (seminar == null)
            {
                return BadRequest();
            }

            if (seminar.OrganizerId != FindUserId())
            {
                return Unauthorized();
            }

            if (!DateTime.TryParseExact(
                model.DateAndTime,
                ValidationConstants.SeminarDateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime date))
            {
                ModelState.AddModelError(nameof(model.DateAndTime), $"Incorrect Date/Time format! Please use this pattern: {ValidationConstants.SeminarDateFormat}");

                if (!ModelState.IsValid)
                {
                    model.Categories = await GetCategories();
                    return View(model);
                }
            }

            seminar.Topic = model.Topic;
            seminar.Lecturer = model.Lecturer;
            seminar.Details = model.Details;
            seminar.DateAndTime = date;
            seminar.Duration = model.Duration;
            seminar.CategoryId = model.CategoryId;

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var seminar = await context.Seminars
                .Where(s => s.Id == id)
                .AsNoTracking()
                .Select(s => new SeminarDetailsViewModel()
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    DateAndTime = s.DateAndTime.ToString(ValidationConstants.SeminarDateFormat),
                    Duration = s.Duration,
                    Lecturer = s.Lecturer,
                    Category = s.Category.Name,
                    Details = s.Details,
                    Organizer = s.Organizer.UserName
                })
                .FirstOrDefaultAsync();

            if (seminar == null)
            {
                return BadRequest();
            }

            return View(seminar);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var seminar = await context.Seminars.FindAsync(id);

            if (seminar == null)
            {
                return BadRequest();
            }

            SeminarDeleteViewModel model = new()
            {
                Id = id,
                Topic = seminar.Topic,
                DateAndTime = seminar.DateAndTime,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seminar = await context.Seminars.FindAsync(id);

            if (seminar == null)
            {
                return NotFound();
            }

            context.Seminars.Remove(seminar);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        private string FindUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        private async Task<IEnumerable<CategoryViewModel>> GetCategories()
        {
            return await context.Categories
                .AsNoTracking()
                .Select(t => new CategoryViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();
        }
    }
}
