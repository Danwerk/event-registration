using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Contracts.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;

namespace WebApp.Controllers
{
    public class EventsController : Controller
    {
        private readonly IAppUOW _uow;

        public EventsController(IAppUOW uow)
        {
            _uow = uow;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            return View(await _uow.EventRepository.AllAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _uow.EventRepository.FindAsync(id.Value);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,DateTime,Location,AdditionalInfo,Id")] Event @event)
        {
            if (@event.DateTime <= DateTime.Now)
            {
                ModelState.AddModelError("DateTime", "Toimumisaeg peab olema tulevikus.");
            }

            if (!ModelState.IsValid)
            {
                return View(@event);
            }

            @event.Id = Guid.NewGuid();
            _uow.EventRepository.Add(@event);
            await _uow.SaveChangesAsync();

            if (Request.Headers["Accept"] == "application/json")
            {
                // Kui JSON request, siis tagasta loodud objekt
                return CreatedAtAction(nameof(Edit), new { id = @event.Id }, @event);
            }

            // Kui tavaline vormipäring, siis redirect
            return RedirectToAction(nameof(Index), "Home");
        }


        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _uow.EventRepository.FindAsync(id.Value);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,DateTime,Location,AdditionalInfo,Id")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                _uow.EventRepository.Update(@event);
                await _uow.SaveChangesAsync();

                return RedirectToAction(nameof(Index), "Home");

            }

            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _uow.EventRepository.FindAsync(id.Value);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var @event = await _uow.EventRepository.FindAsync(id);


            if (@event == null)
            {
                return NotFound();
            }

            var eventParticipants = (await _uow.EventParticipantRepository.AllAsync(id)).ToList();
            eventParticipants.ForEach(p => _uow.EventParticipantRepository.Remove(p));

            _uow.EventRepository.Remove(@event);
            await _uow.SaveChangesAsync();
            return RedirectToAction(nameof(Index), "Home");
        }

        private bool EventExists(Guid id)
        {
            return (_uow.EventRepository.AllAsync().Result?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}