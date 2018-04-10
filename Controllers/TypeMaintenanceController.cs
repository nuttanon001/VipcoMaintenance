using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using VipcoMaintenance.Services;
using VipcoMaintenance.ViewModels;
using VipcoMaintenance.Models.Maintenances;

namespace VipcoMaintenance.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TypeMaintenanceController : GenericController<TypeMaintenance>
    {
        private IRepositoryMaintenance<Item> repositoryItem;
        public TypeMaintenanceController(IRepositoryMaintenance<TypeMaintenance> repo,
            IRepositoryMaintenance<Item> repoItem
            ):base(repo) {
            this.repositoryItem = repoItem;
        }

        // GET: api/TypeMaintenance/GetTypeMaintenanceByItem/5
        [HttpGet("GetTypeMaintenanceByItem")]
        public async Task<IActionResult> GetTypeMaintenanceByItem(int ItemId)
        {
            if (ItemId > 0)
            {
                var ItemData = (await this.repositoryItem.GetAsync(ItemId));
                if (ItemData != null)
                {
                    var QueryData = this.repository.GetAllAsQueryable()
                                        .Where(x => x.ItemTypeId == ItemData.ItemTypeId)
                                        .AsQueryable().AsNoTracking();
                    var HasData = await QueryData.ToListAsync();
                    return new JsonResult(HasData, this.DefaultJsonSettings);
                }
                else
                {
                    var QueryData = await this.repository.GetAllAsync();
                    return new JsonResult(QueryData, this.DefaultJsonSettings);
                }
            }

            return BadRequest();
        }

        // POST: api/TypeMaintenance/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var QueryData = this.repository.GetAllAsQueryable().AsQueryable().AsNoTracking();

            if (Scroll.WhereId.HasValue)
                QueryData = QueryData.Where(x => x.ItemType.WorkGroupId == Scroll.WhereId);

            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.ToLower().Split(null);

            foreach (var keyword in filters)
            {
                QueryData = QueryData.Where(x => x.Name.ToLower().Contains(keyword) ||
                                                 x.Description.ToLower().Contains(keyword));
            }

            // Order
            switch (Scroll.SortField)
            {
                case "Name":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.Name);
                    else
                        QueryData = QueryData.OrderBy(e => e.Name);
                    break;
                case "Description":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.Description);
                    else
                        QueryData = QueryData.OrderBy(e => e.Description);
                    break;

                default:
                    QueryData = QueryData.OrderByDescending(e => e.Name);
                    break;
            }
            // Get TotalRow
            Scroll.TotalRow = await QueryData.CountAsync();
            // Skip Take
            QueryData = QueryData.Skip(Scroll.Skip ?? 0).Take(Scroll.Take ?? 50);

            return new JsonResult(new ScrollDataViewModel<TypeMaintenance>(Scroll,
                await QueryData.AsNoTracking().ToListAsync()), this.DefaultJsonSettings);
        }
    }
}
