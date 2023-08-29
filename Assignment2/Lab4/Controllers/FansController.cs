using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab4.Data;
using Lab4.Models;
using Lab4.Models.ViewModels;

namespace Lab4.Controllers
{
    public class FansController : Controller
    {
        private readonly SportsDbContext _context;

        public FansController(SportsDbContext context)
        {
            _context = context;
        }

        // Add Subscription
        public async Task<IActionResult> AddSubscription(string sportClubId, int fanId)
        {
            var addSubscriber = new Subscription // subscription object is created
            {
                SportClubId = sportClubId,      // and initialized with sportsClubId, fanID
                FanId = fanId
            };

            _context.Subscriptions.Add(addSubscriber);  // the new subscription is added to the Dbset
            await _context.SaveChangesAsync();      // changes are save to databse

            return RedirectToAction("EditSubscription", new { id = fanId });
        }

        // Edit Subscription
        public async Task<IActionResult> EditSubscription(int id)
        {
            var viewModel = new SportClubViewModel();   // created the object of SportClubViewModel to hold the data for the view

            var fanSubscriptions = await _context.Subscriptions         // existing subscription of fan is retrived from the database and their associated sportclubId is extracted
                .Where(s => s.FanId == id)
                .Select(s => s.SportClubId)
                .ToListAsync();

            var allSportClubs = await _context.SportClubs.ToListAsync();    // sportclubs from database are fetched

            var unregisteredSportClubs = allSportClubs      // sportclubs that fan is not subscribed to
                .Where(sc => !fanSubscriptions.Contains(sc.Id))
                .OrderBy(sc => sc.Title)
                .ToList();

            var registeredSportClubs = allSportClubs        // sportclubs that fan is already subscribed to
                .Where(sc => fanSubscriptions.Contains(sc.Id))
                .OrderBy(sc => sc.Title)
                .ToList();

            var duplicates = new HashSet<string>();// to keep tract of already added sportsclub
            var uniqueUnregisteredSportClubs = new List<SportClub>();// list of uniqueUnregisterd sportsclub

            foreach (var club in unregisteredSportClubs.Concat(registeredSportClubs))
            {
                if (!duplicates.Contains(club.Id))
                {
                    duplicates.Add(club.Id);
                    uniqueUnregisteredSportClubs.Add(club);
                }
            }

            viewModel.Fans = await _context.Fans
                .Where(f => f.Id == id)
                .ToListAsync();

            viewModel.Subscriptions = await _context.Subscriptions
                .Where(s => s.FanId == id)
                .ToListAsync();

            viewModel.SportClubs = uniqueUnregisteredSportClubs;

            return View(viewModel);
        }

        // Remove Subscription
        public async Task<IActionResult> RemoveSubscription(int FanId, string SportClubId)
        {

            var removeSubscriber = _context.Subscriptions           // corresponding subscription is retriever from the dbset based on the fanid and sportsclubid
                .Where(x => (x.SportClubId.Equals(SportClubId))
                && (x.FanId == FanId)).FirstOrDefault();

            if (removeSubscriber != null)
            {
                _context.Subscriptions.Remove(removeSubscriber);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("EditSubscription", new { id = FanId });
        }

        // GET: Fans
        public async Task<IActionResult> Index(int? id)
        {
            var viewModel = new SportClubViewModel
            {
                Fans = await _context.Fans.ToListAsync()
            };
            
            if (id != null)
            {
                ViewData["id"] = id;
                viewModel.SportClubs = await _context.Subscriptions
                    .Where(sub => sub.FanId == id)
                    .Join(_context.SportClubs, sub => sub.SportClubId
                    , sprtClub => sprtClub.Id, (sub, sprtClub) => sprtClub)
                    .Distinct().ToListAsync();
            }
            return View(viewModel);
        }

        // GET: Fans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Fans == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fan == null)
            {
                return NotFound();
            }

            return View(fan);
        }

        // GET: Fans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,BirthDate")] Fan fan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fan);
        }

        // GET: Fans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Fans == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans.FindAsync(id);
            if (fan == null)
            {
                return NotFound();
            }
            return View(fan);
        }

        // POST: Fans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,BirthDate")] Fan fan)
        {
            if (id != fan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FanExists(fan.Id))
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
            return View(fan);
        }

        // GET: Fans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Fans == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fan == null)
            {
                return NotFound();
            }

            return View(fan);
        }

        // POST: Fans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Fans == null)
            {
                return Problem("Entity set 'SportsDbContext.Fans'  is null.");
            }
            var fan = await _context.Fans.FindAsync(id);
            if (fan != null)
            {
                _context.Fans.Remove(fan);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FanExists(int id)
        {
          return _context.Fans.Any(e => e.Id == id);
        }
    }
}
