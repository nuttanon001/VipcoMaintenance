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
    public class ItemMaintenanceController : GenericController<ItemMaintenance>
    {
        public ItemMaintenanceController(IRepositoryMaintenance<ItemMaintenance> repo): base(repo) { }

        // POST: api/ItemMaintenance/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var QueryData = this.repository.GetAllAsQueryable()
                                            .AsQueryable()
                                            .Include(x => x.RequireMaintenance.Item)
                                            .AsNoTracking();

            if (!string.IsNullOrEmpty(Scroll.Where))
                QueryData = QueryData.Where(x => x.Creator == Scroll.Where);

            if (Scroll.WhereId.HasValue)
                QueryData = QueryData.Where(x => x.RequireMaintenance.Item.ItemTypeId == Scroll.WhereId);

            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.ToLower().Split(null);

            foreach (var keyword in filters)
            {
                QueryData = QueryData.Where(x => x.Description.ToLower().Contains(keyword) &&
                                                 x.ItemMaintenanceCode.ToLower().Contains(keyword) &&                                                 
                                                 x.Remark.ToLower().Contains(keyword) &&
                                                 x.RequireMaintenance.Item.Name.ToLower().Contains(keyword) &&
                                                 x.RequireMaintenance.Item.ItemCode.ToLower().Contains(keyword) &&
                                                 x.TypeMaintenance.Name.ToLower().Contains(keyword) &&
                                                 x.TypeMaintenance.Description.ToLower().Contains(keyword));
            }

            // Order
            switch (Scroll.SortField)
            {
                case "ItemCode":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.RequireMaintenance.Item.ItemCode);
                    else
                        QueryData = QueryData.OrderBy(e => e.RequireMaintenance.Item.ItemCode);
                    break;

                case "TypeMaintenanceName":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.TypeMaintenance.Name);
                    else
                        QueryData = QueryData.OrderBy(e => e.TypeMaintenance.Name);
                    break;

                case "ItemMaintenanceCode":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.ItemMaintenanceCode);
                    else
                        QueryData = QueryData.OrderBy(e => e.ItemMaintenanceCode);
                    break;

                default:
                    QueryData = QueryData.OrderByDescending(e => e.CreateDate);
                    break;
            }
            // Skip Take
            QueryData = QueryData.Skip(Scroll.Skip ?? 0).Take(Scroll.Take ?? 50);

            return new JsonResult(new ScrollDataViewModel<ItemMaintenance>(Scroll,
                await QueryData.AsNoTracking().ToListAsync()), this.DefaultJsonSettings);
        }
    }
}
