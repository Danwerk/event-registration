using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;

namespace WebApp.Controllers
{
    public class EventParticipantsController : Controller
    {
        private readonly AppUOW _uow;

        public EventParticipantsController(AppUOW uow)
        {
            _uow = uow;
        }

        // GET: EventParticipants
        public async Task<IActionResult> Index()
        {
            var vm = await _uow.EventParticipantRepository.AllAsync();
            return View(vm);
        }

        // GET: EventParticipants/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventParticipant = await _uow.EventParticipantRepository.FindAsync(id.Value);
            if (eventParticipant == null)
            {
                return NotFound();
            }

            return View(eventParticipant);
        }

        // GET: EventParticipants/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_uow.EventRepository.AllAsync().Result, "Id", "AdditionalInfo");
            ViewData["ParticipantId"] = new SelectList(_uow.ParticipantRepository.AllAsync().Result, "Id", "AdditionalInfo");
            return View();
        }

        // POST: EventParticipants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,ParticipantId,Id")] EventParticipant eventParticipant)
        {
            if (ModelState.IsValid)
            {
                eventParticipant.Id = Guid.NewGuid();
                _uow.EventParticipantRepository.Add(eventParticipant);
                await _uow.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_uow.EventRepository.AllAsync().Result, "Id", "AdditionalInfo", eventParticipant.EventId);
            ViewData["ParticipantId"] = new SelectList(_uow.ParticipantRepository.AllAsync().Result, "Id", "AdditionalInfo", eventParticipant.ParticipantId);
            return View(eventParticipant);
        }

        // GET: EventParticipants/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventParticipant = await _uow.EventParticipantRepository.FindAsync(id.Value);
            if (eventParticipant == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_uow.EventRepository.AllAsync().Result, "Id", "AdditionalInfo", eventParticipant.EventId);
            ViewData["ParticipantId"] = new SelectList(_uow.ParticipantRepository.AllAsync().Result, "Id", "AdditionalInfo", eventParticipant.ParticipantId);
            return View(eventParticipant);
        }

        // POST: EventParticipants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("EventId,ParticipantId,Id")] EventParticipant eventParticipant)
        {
            if (id != eventParticipant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _uow.EventParticipantRepository.Update(eventParticipant);
                    await _uow.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventParticipantExists(eventParticipant.Id))
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
            ViewData["EventId"] = new SelectList(_uow.EventRepository.AllAsync().Result, "Id", "AdditionalInfo", eventParticipant.EventId);
            ViewData["ParticipantId"] = new SelectList(_uow.ParticipantRepository.AllAsync().Result, "Id", "AdditionalInfo", eventParticipant.ParticipantId);
            return View(eventParticipant);
        }

        // GET: EventParticipants/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventParticipant = await _uow.EventParticipantRepository.FindAsync(id.Value);
                
            if (eventParticipant == null)
            {
                return NotFound();
            }

            return View(eventParticipant);
        }

        // POST: EventParticipants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var eventParticipant = await _uow.EventParticipantRepository.FindAsync(id);
            if (eventParticipant != null)
            {
                _uow.EventParticipantRepository.Remove(eventParticipant);
            }

            await _uow.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventParticipantExists(Guid id)
        {
            return (_uow.EventParticipantRepository.AllAsync().Result?.Any(e=>e.Id == id)).GetValueOrDefault();
        }
    }
}
