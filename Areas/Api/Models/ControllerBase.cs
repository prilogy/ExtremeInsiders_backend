using System;
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
        public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string orderByDate)
        {
            return await Paging(_db.Set<T>().AsQueryable(), page, pageSize, orderByDate);
        }

        [HttpGet("recommended")]
        public async Task<IActionResult> GetRecommended([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string orderByDate)
        {
            var queryable = GetRecommendedQueryable();

            if (queryable == null)
                return NotFound();
            
            return await Paging(queryable, page, pageSize, orderByDate);
        }

        protected virtual IQueryable<T> GetRecommendedQueryable() => null;

        [HttpPost]
        public async Task<IActionResult> GetByIds(int[] ids, [FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string orderByDate = null)
        {
            var list = _db.Set<T>().Where(x => ids.Contains(x.Id));
            return await Paging(list, page, pageSize, orderByDate);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _db.Set<T>().FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity.OfFormat(_userService));
        }

        [HttpPost("{id}/search")]
        public virtual async Task<IActionResult> Search([FromBody] string query, [FromRoute] int id)
        {
            return await Task.Run(() => { return NotFound(); });
        }

        protected async Task<IActionResult> Paging(IQueryable<T> q, int page, int pageSize, string orderByDate)
        {
            if (orderByDate == "asc")
                q = q.OrderBy(x => x.DateCreated);
            else if (orderByDate == "desc")
                q = q.OrderByDescending(x => x.DateCreated);

            return Ok((page == 0
                ? await q.ToListAsync()
                : await q.Page(page, pageSize == 0 ? PAGE_SIZE : pageSize).ToListAsync()).OfFormat(_userService));
        }
    }
}