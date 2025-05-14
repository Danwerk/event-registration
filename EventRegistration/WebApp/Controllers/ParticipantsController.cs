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
using WebApp.Models;

namespace WebApp.Controllers
{
    public class ParticipantsController : Controller
    {
        private readonly IAppUOW _uow;

        public ParticipantsController(IAppUOW uow )
        {
            _uow = uow;
        }

        // GET: Participants
        public async Task<IActionResult> Index()
        {
            var participants = await _uow.ParticipantRepository.AllAsync();
            return View(participants);
        }

        // GET: Participants/Details/5
        public async Task<IActionResult> Details(Guid? id, Guid? eventId)
        {
            if (id == null) return NotFound();

            var participant = await _uow.ParticipantRepository.FindAsync(id.Value);
            if (participant == null) return NotFound();

            var paymentMethods = await _uow.PaymentMethodRepository.AllAsync();
            ViewBag.PaymentMethods = paymentMethods
                .Select(pm => new SelectListItem
                {
                    Value = pm.Id.ToString(),
                    Text = pm.Name
                }).ToList();
            ViewBag.EventId = eventId;

            return View(participant);
        }

        // GET: Participants/Create
        public async Task<IActionResult> Create()
        {
            var paymentMethods = await _uow.PaymentMethodRepository.AllAsync();
            ViewData["PaymentMethodId"] = new SelectList(paymentMethods, "Id", "Name");
            return View();
        }

        // POST: Participants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("PaymentMethodId,AdditionalInfo")] Participant participant)
        {
            if (ModelState.IsValid)
            {
                participant.Id = Guid.NewGuid();
                _uow.ParticipantRepository.Add(participant);
                await _uow.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var paymentMethods = await _uow.PaymentMethodRepository.AllAsync();
            ViewData["PaymentMethodId"] = new SelectList(paymentMethods, "Id", "Name", participant.PaymentMethodId);
            return View(participant);
        }

        [HttpPost]
        public async Task<IActionResult> EditPrivatePerson(PrivatePersonEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Details), new { id = vm.Id, eventId = vm.EventId });
            }

            var participant = await _uow.ParticipantRepository.FindAsync(vm.Id);
            if (participant is not PrivatePerson privatePersonDb)
            {
                return NotFound();
            }

            privatePersonDb.FirstName = vm.FirstName;
            privatePersonDb.LastName = vm.LastName;
            privatePersonDb.PersonalCode = vm.PersonalCode;
            privatePersonDb.PaymentMethodId = vm.PaymentMethodId;
            privatePersonDb.AdditionalInfo = vm.AdditionalInfo;

            _uow.ParticipantRepository.Update(privatePersonDb);
            await _uow.SaveChangesAsync();

            return RedirectToAction("Index", "EventParticipants", new { eventId = vm.EventId });
        }

        
        [HttpPost]
        public async Task<IActionResult> EditLegalPerson(LegalPersonEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Details), new { id = vm.Id, eventId = vm.EventId });
            }

            var participant = await _uow.ParticipantRepository.FindAsync(vm.Id);
            if (participant is not LegalPerson legalPerson)
            {
                return NotFound();
            }

            legalPerson.CompanyName = vm.CompanyName;
            legalPerson.RegistryCode = vm.RegistryCode;
            legalPerson.NumberOfAttendees = vm.NumberOfAttendees;
            legalPerson.PaymentMethodId = vm.PaymentMethodId;
            legalPerson.AdditionalInfo = vm.AdditionalInfo;

            _uow.ParticipantRepository.Update(legalPerson);
            await _uow.SaveChangesAsync();

            return RedirectToAction("Index", "EventParticipants", new { eventId = vm.EventId });
        }


        // GET: Participants/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var participant = await _uow.ParticipantRepository.FindAsync(id.Value);
            if (participant == null) return NotFound();

            return View(participant);
        }


        // POST: Participants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var participant = await _uow.ParticipantRepository.FindAsync(id);
            if (participant != null)
            {
                _uow.ParticipantRepository.Remove(participant);
                await _uow.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ParticipantExists(Guid id)
        {
            var participant = await _uow.ParticipantRepository.FindAsync(id);
            return participant != null;
        }
    }
}
