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
    public class PaymentMethodsController : Controller
    {
        private readonly IAppUOW _uow;

        public PaymentMethodsController(IAppUOW uow)
        {
            _uow = uow;
        }

        // GET: PaymentMethods
        public async Task<IActionResult> Index()
        {
            var paymentMethods = await _uow.PaymentMethodRepository.AllAsync();
            return View(paymentMethods);
        }


        // GET: PaymentMethods/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var paymentMethod = await _uow.PaymentMethodRepository.FindAsync(id.Value);
            if (paymentMethod == null) return NotFound();

            return View(paymentMethod);
        }

        // GET: PaymentMethods/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PaymentMethods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,Id")] PaymentMethod paymentMethod)
        {
            if (ModelState.IsValid)
            {
                paymentMethod.Id = Guid.NewGuid();
                _uow.PaymentMethodRepository.Add(paymentMethod);
                await _uow.SaveChangesAsync();

                if (Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    return Created("", paymentMethod);
                }

                return RedirectToAction(nameof(Index));
            }
            return View(paymentMethod);
        }

        // GET: PaymentMethods/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var paymentMethod = await _uow.PaymentMethodRepository.FindAsync(id.Value);
            if (paymentMethod == null) return NotFound();

            return View(paymentMethod);
        }

        // POST: PaymentMethods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Id")] PaymentMethod paymentMethod)
        {
            if (id != paymentMethod.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _uow.PaymentMethodRepository.Update(paymentMethod);
                    await _uow.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await PaymentMethodExists(paymentMethod.Id))
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
            return View(paymentMethod);
        }

        // GET: PaymentMethods/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var paymentMethod = await _uow.PaymentMethodRepository.FindAsync(id.Value);
            if (paymentMethod == null) return NotFound();

            return View(paymentMethod);
        }

        // POST: PaymentMethods/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var paymentMethod = await _uow.PaymentMethodRepository.FindAsync(id);
            if (paymentMethod != null)
            {
                _uow.PaymentMethodRepository.Remove(paymentMethod);
                await _uow.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> PaymentMethodExists(Guid id)
        {
            var paymentMethod = await _uow.PaymentMethodRepository.FindAsync(id);
            return paymentMethod != null;
        }
    }
}
