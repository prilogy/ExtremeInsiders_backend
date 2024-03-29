﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Models;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExtremeInsiders.Areas.Api.Models
{
    [Authorize(Policy = SubscriptionHandler.POLICY_NAME)]
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ControllerBase<T, TR> : Controller
        where T : EntityBase, ITranslatableEntity<T, TR>
        where TR : TranslatableEntityTranslation<T>
    {
        protected readonly ApplicationContext _db;
        protected readonly UserService _userService;

        private readonly int PAGE_SIZE = 10;

        public ControllerBase(ApplicationContext db, UserService userService)
        {
            _db = db;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<T>>> GetAll([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string orderByDate)
        {
            return await Paging(_db.Set<T>().OrderByDescending(x => x.Id).AsQueryable(), page, pageSize, orderByDate);
        }

        [HttpGet("recommended")]
        public async Task<ActionResult<List<T>>> GetRecommended([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string orderByDate)
        {
            var queryable = GetRecommendedQueryable();

            if (queryable == null)
                return NotFound();
            
            return await Paging(queryable, page, pageSize, orderByDate);
        }

        protected virtual IQueryable<T> GetRecommendedQueryable() => null;

        [HttpPost]
        public async Task<ActionResult<List<T>>> GetByIds(int[] ids, [FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string orderByDate = null)
        {
            var list = _db.Set<T>().OrderByDescending(x => x.Id).Where(x => ids.Contains(x.Id));
            var r = await Paging(list, page, pageSize, orderByDate);
            return r;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _db.Set<T>().FindAsync(id);

            // if (entity == null || FormatExtensions.IsPlaylistAndOnlyLocalization(entity, _userService.Culture))
            // {
            //     return NotFound();
            // }

            if (FormatExtensions.IsTranslatableEntityHasTranslation(entity))
                return Ok(entity.OfFormat(_userService));
            return NoContent();
        }

        [HttpPost("{id}/search")]
        public virtual async Task<IActionResult> Search([FromBody] string query, [FromRoute] int id)
        {
            return await Task.Run(NotFound);
        }

        protected async Task<ActionResult<List<T>>> Paging(IQueryable<T> q, int page, int pageSize, string orderByDate=null)
        {
            q = orderByDate switch
            {
                "asc" => q.OrderBy(x => x.DateCreated),
                "desc" => q.OrderByDescending(x => x.DateCreated),
                _ => q
            };

            return (page == 0
                ? await q.ToListAsync()
                : await q.Page(page, pageSize == 0 ? PAGE_SIZE : pageSize).ToListAsync()).OfFormat(_userService);
        }
    }
}