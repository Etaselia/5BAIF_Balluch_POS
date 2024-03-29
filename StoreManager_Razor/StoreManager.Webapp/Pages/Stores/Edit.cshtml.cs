﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Infrastructure;
using StoreManager.Application.Model;
using StoreManager.Webapp.Dto;

namespace StoreManager.Webapp.Pages.Stores
{
	public class EditModel : PageModel
    {

        private readonly StoreContext _db;

        private readonly IMapper _mapper;

        public EditModel(StoreContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [BindProperty]
        public StoreDto Store { get; set; } = null!;

        public IActionResult OnPost(Guid guid)
        {
            if (!ModelState.IsValid)
            {

                return Page();

            }
            var store = _db.Stores
              .FirstOrDefault(s => s.Guid == guid);
            if (store is null)
            {
                return RedirectToPage("/Stores/Index");
            }
            _mapper.Map(Store, store);
            _db.Entry(store).State = EntityState.Modified;
            store.Name = Store.Name;
            store.CloseDate = Store.CloseDate;
            store.Url = Store.Url;
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException)
            {

                ModelState.AddModelError("", "Fehler beim schreiben in die Datenbank");
                return Page();
            }
            return RedirectToPage("/Stores/Index");

        }

        public IActionResult OnGet(Guid guid)
        {
            var store = _db.Stores
              .FirstOrDefault(s => s.Guid == guid);
            if (store is null)
            {
                return RedirectToPage("/Stores/Index");
            }
            Store = _mapper.Map<StoreDto>(store);
            return Page();
        }
    }
}
