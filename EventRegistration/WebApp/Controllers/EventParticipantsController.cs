using App.Contracts.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Microsoft.Build.Framework;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class EventParticipantsController : Controller
    {
        private readonly IAppUOW _uow;

        public EventParticipantsController(IAppUOW uow)
        {
            _uow = uow;
        }

        // GET: EventParticipants
        public async Task<IActionResult> Index(Guid eventId)
        {
            var eventEntity = await _uow.EventRepository.FindAsync(eventId);

            if (eventEntity == null)
            {
                return NotFound();
            }

            var eventParticipants = await _uow.EventParticipantRepository.AllAsync(eventId);
            
            var participants = new List<ParticipantDisplayViewModel>();

            foreach (var ep in eventParticipants)
            {
                if (ep.Participant is App.Domain.PrivatePerson privatePerson)
                {
                    participants.Add(new ParticipantDisplayViewModel
                    {
                        EventParticipantId = ep.Id,
                        Id = privatePerson.Id,
                        Name = $"{privatePerson.FirstName} {privatePerson.LastName}",
                        Code = privatePerson.PersonalCode
                    });
                }
                else if (ep.Participant is App.Domain.LegalPerson legalPerson)
                {
                    participants.Add(new ParticipantDisplayViewModel
                    {
                        EventParticipantId = ep.Id,
                        Id = legalPerson.Id,
                        Name = legalPerson.CompanyName,
                        Code = legalPerson.RegistryCode
                    });
                }
            }
            
            var paymentMethods = await _uow.PaymentMethodRepository.AllAsync();
            ViewBag.PaymentMethods = paymentMethods
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
                .ToList();
            var allParticipants = await _uow.ParticipantRepository.AllAsync();
            ViewBag.AllParticipants = allParticipants.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p is PrivatePerson privateP
                    ? $"{privateP.FirstName} {privateP.LastName} ({privateP.PersonalCode})"
                    : p is LegalPerson legalP
                        ? $"{legalP.CompanyName} ({legalP.RegistryCode})"
                        : "Tundmatu"
            }).ToList();

            var vm = new EventParticipantViewModel()
            {
                Event = eventEntity,
                Participants = participants
            };
            
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
        public async Task<IActionResult> Create(EventParticipantCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var vmIndex = await LoadEventParticipantViewModel(vm.EventId);
                return View("Index", vmIndex);
            }

            if (vm.PaymentMethodId == null || vm.PaymentMethodId == Guid.Empty)
            {
                ModelState.AddModelError("", "Palun vali korrektne makseviis.");
                var vmIndex = await LoadEventParticipantViewModel(vm.EventId);
                return View("Index", vmIndex);
            }

            

            if (vm.ParticipantType == "private")
            {
                if (string.IsNullOrWhiteSpace(vm.PersonalCode) || !System.Text.RegularExpressions.Regex.IsMatch(vm.PersonalCode, @"^\d{11}$"))
                {
                    ModelState.AddModelError("PersonalCode", "Isikukood peab olema 11 numbrit.");
                }
                
                var privatePerson = new PrivatePerson
                {
                    Id = Guid.NewGuid(),
                    FirstName = vm.FirstName!,
                    LastName = vm.LastName!,
                    PersonalCode = vm.PersonalCode!,
                    PaymentMethodId = vm.PaymentMethodId.Value,
                    AdditionalInfo = vm.AdditionalInfo ?? ""
                };
                _uow.ParticipantRepository.Add(privatePerson);
                await _uow.SaveChangesAsync();

                var eventParticipant = new EventParticipant
                {
                    Id = Guid.NewGuid(),
                    EventId = vm.EventId,
                    ParticipantId = privatePerson.Id,
                };
                _uow.EventParticipantRepository.Add(eventParticipant);
            }
            else if (vm.ParticipantType == "legal")
            {
                var legalPerson = new LegalPerson
                {
                    Id = Guid.NewGuid(),
                    CompanyName = vm.CompanyName!,
                    RegistryCode = vm.RegistryCode!,
                    NumberOfAttendees = vm.NumberOfAttendees ?? 1, // vaikimisi 1 kui mitte määratud
                    PaymentMethodId = vm.PaymentMethodId.Value,
                    AdditionalInfo = vm.AdditionalInfo ?? ""
                };

                _uow.ParticipantRepository.Add(legalPerson);
                await _uow.SaveChangesAsync();

                var eventParticipant = new EventParticipant
                {
                    Id = Guid.NewGuid(),
                    EventId = vm.EventId,
                    ParticipantId = legalPerson.Id
                };
                _uow.EventParticipantRepository.Add(eventParticipant);
            }

            await _uow.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { eventId = vm.EventId });
        }
        
        [HttpPost]
        public async Task<IActionResult> CreatePrivate(EventParticipantCreatePrivateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var vmIndex = await LoadEventParticipantViewModel(vm.EventId);
                return View("Index", vmIndex);
            }

            var privatePerson = new PrivatePerson
            {
                Id = Guid.NewGuid(),
                FirstName = vm.FirstName!,
                LastName = vm.LastName!,
                PersonalCode = vm.PersonalCode!,
                PaymentMethodId = vm.PaymentMethodId!.Value,
                AdditionalInfo = vm.AdditionalInfo ?? ""
            };
            _uow.ParticipantRepository.Add(privatePerson);
            await _uow.SaveChangesAsync();

            var eventParticipant = new EventParticipant
            {
                Id = Guid.NewGuid(),
                EventId = vm.EventId,
                ParticipantId = privatePerson.Id
            };
            _uow.EventParticipantRepository.Add(eventParticipant);

            await _uow.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { eventId = vm.EventId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateLegal(EventParticipantCreateLegalViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var vmIndex = await LoadEventParticipantViewModel(vm.EventId);
                return View("Index", vmIndex);
            }

            var legalPerson = new LegalPerson
            {
                Id = Guid.NewGuid(),
                CompanyName = vm.CompanyName!,
                RegistryCode = vm.RegistryCode!,
                NumberOfAttendees = vm.NumberOfAttendees ?? 1,
                PaymentMethodId = vm.PaymentMethodId!.Value,
                AdditionalInfo = vm.AdditionalInfo ?? ""
            };
            _uow.ParticipantRepository.Add(legalPerson);
            await _uow.SaveChangesAsync();

            var eventParticipant = new EventParticipant
            {
                Id = Guid.NewGuid(),
                EventId = vm.EventId,
                ParticipantId = legalPerson.Id
            };
            _uow.EventParticipantRepository.Add(eventParticipant);

            await _uow.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { eventId = vm.EventId });
        }
        
        [HttpPost]
        public async Task<IActionResult> AddExistingParticipant(Guid eventId, Guid participantId)
        {
            if (eventId == Guid.Empty || participantId == Guid.Empty)
            {
                return BadRequest("EventId ja ParticipantId peavad olema määratud.");
            }

            var eventEntity = await _uow.EventRepository.FindAsync(eventId);
            if (eventEntity == null)
            {
                return NotFound("Üritust ei leitud.");
            }

            var participantEntity = await _uow.ParticipantRepository.FindAsync(participantId);
            if (participantEntity == null)
            {
                return NotFound("Osavõtjat ei leitud.");
            }

            // Kontrollime, kas see osaleja on juba sellel üritusel
            var existing = await _uow.EventParticipantRepository.AllAsync(eventId);
            if (existing.Any(ep => ep.ParticipantId == participantId))
            {
                ModelState.AddModelError("", "See osaleja on juba sellel üritusel olemas.");
                var vmIndex = await LoadEventParticipantViewModel(eventId);
                return View("Index", vmIndex);
            }

            var eventParticipant = new EventParticipant
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                ParticipantId = participantId
            };
            _uow.EventParticipantRepository.Add(eventParticipant);
            await _uow.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { eventId = eventId });
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
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var eventParticipant = await _uow.EventParticipantRepository.FindAsync(id);
            if (eventParticipant == null)
            {
                return NotFound();
            }

            var eventId = eventParticipant.EventId;

            _uow.EventParticipantRepository.Remove(eventParticipant);
            await _uow.SaveChangesAsync();

            
            return RedirectToAction(nameof(Index), new { eventId = eventId });
        }

        private bool EventParticipantExists(Guid id)
        {
            return (_uow.EventParticipantRepository.AllAsync().Result?.Any(e=>e.Id == id)).GetValueOrDefault();
        }
        
        private async Task<EventParticipantViewModel> LoadEventParticipantViewModel(Guid eventId)
        {
            var eventEntity = await _uow.EventRepository.FindAsync(eventId);
            if (eventEntity == null)
            {
                throw new Exception("Event not found");
            }

            var eventParticipants = await _uow.EventParticipantRepository.AllAsync(eventId);
            var participants = new List<ParticipantDisplayViewModel>();

            foreach (var ep in eventParticipants)
            {
                if (ep.Participant is PrivatePerson privatePerson)
                {
                    participants.Add(new ParticipantDisplayViewModel
                    {
                        Id = privatePerson.Id,
                        Name = $"{privatePerson.FirstName} {privatePerson.LastName}",
                        Code = privatePerson.PersonalCode
                    });
                }
                else if (ep.Participant is LegalPerson legalPerson)
                {
                    participants.Add(new ParticipantDisplayViewModel
                    {
                        Id = legalPerson.Id,
                        Name = legalPerson.CompanyName,
                        Code = legalPerson.RegistryCode
                    });
                }
            }
            var paymentMethods = await _uow.PaymentMethodRepository.AllAsync();
            ViewBag.PaymentMethods = paymentMethods
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
                .ToList();
            
            return new EventParticipantViewModel
            {
                Event = eventEntity,
                Participants = participants
            };
        }

    }
}
