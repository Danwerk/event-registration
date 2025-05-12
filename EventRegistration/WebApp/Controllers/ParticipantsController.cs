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
        private readonly AppDbContext _context;
        private readonly IAppUOW _uow;

        public ParticipantsController(AppDbContext context, IAppUOW uow )
        {
            _context = context;
            _uow = uow;
        }

        // GET: Participants
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Participants.Include(p => p.PaymentMethod);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Participants/Details/5
        public async Task<IActionResult> Details(Guid? id, Guid? eventId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participant = await _context.Participants
                .Include(p => p.PaymentMethod)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (participant == null)
            {
                return NotFound();
            }
            
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
        public IActionResult Create()
        {
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "Id", "Name");
            return View();
        }

        // POST: Participants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentMethodId,AdditionalInfo,Id")] Participant participant)
        {
            if (ModelState.IsValid)
            {
                participant.Id = Guid.NewGuid();
                _context.Add(participant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "Id", "Name", participant.PaymentMethodId);
            return View(participant);
        }

        // // GET: Participants/Edit/5
        // public async Task<IActionResult> Edit(Guid? id)
        // {
        //     if (id == null) return NotFound();
        //
        //     var participant = await _uow.ParticipantRepository.FindAsync(id.Value);
        //     if (participant == null) return NotFound();
        //
        //     var paymentMethods = await _uow.PaymentMethodRepository.AllAsync();
        //     ViewBag.PaymentMethods = paymentMethods
        //         .Select(pm => new SelectListItem
        //         {
        //             Value = pm.Id.ToString(),
        //             Text = pm.Name
        //         }).ToList();
        //
        //     if (participant is PrivatePerson privatePerson)
        //     {
        //         var vm = new PrivatePersonEditViewModel
        //         {
        //             Id = privatePerson.Id,
        //             FirstName = privatePerson.FirstName,
        //             LastName = privatePerson.LastName,
        //             PersonalCode = privatePerson.PersonalCode,
        //             PaymentMethodId = privatePerson.PaymentMethodId,
        //             AdditionalInfo = privatePerson.AdditionalInfo
        //         };
        //         return View("EditPrivatePerson", vm);
        //     }
        //     else if (participant is LegalPerson legalPerson)
        //     {
        //         var vm = new LegalPersonEditViewModel
        //         {
        //             Id = legalPerson.Id,
        //             CompanyName = legalPerson.CompanyName,
        //             RegistryCode = legalPerson.RegistryCode,
        //             NumberOfAttendees = legalPerson.NumberOfAttendees,
        //             PaymentMethodId = legalPerson.PaymentMethodId,
        //             AdditionalInfo = legalPerson.AdditionalInfo
        //         };
        //         return View("EditLegalPerson", vm);
        //     }
        //
        //     return NotFound();
        // }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
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
            if (id == null)
            {
                return NotFound();
            }

            var participant = await _context.Participants
                .Include(p => p.PaymentMethod)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (participant == null)
            {
                return NotFound();
            }

            return View(participant);
        }

        // POST: Participants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant != null)
            {
                _context.Participants.Remove(participant);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParticipantExists(Guid id)
        {
            return _context.Participants.Any(e => e.Id == id);
        }
    }
}
