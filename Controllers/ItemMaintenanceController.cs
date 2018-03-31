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
using AutoMapper;

namespace VipcoMaintenance.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ItemMaintenanceController : GenericController<ItemMaintenance>
    {
        private IMapper mapper;
        public ItemMaintenanceController(
            IRepositoryMaintenance<ItemMaintenance> repo,
            IMapper map
            ): base(repo) {
            // Mapper
            this.mapper = map;
        }

        // POST: api/ItemMaintenance/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var QueryData = this.repository.GetAllAsQueryable()
                                            .Include(x => x.RequireMaintenance.Item)
                                            .Include(x => x.TypeMaintenance)
                                            .AsQueryable()
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
                QueryData = QueryData.Where(x => x.Description.ToLower().Contains(keyword) ||
                                                 x.ItemMaintenanceNo.ToLower().Contains(keyword) ||                                                 
                                                 x.Remark.ToLower().Contains(keyword) ||
                                                 x.RequireMaintenance.Item.Name.ToLower().Contains(keyword) ||
                                                 x.RequireMaintenance.Item.ItemCode.ToLower().Contains(keyword) ||
                                                 x.TypeMaintenance.Name.ToLower().Contains(keyword) ||
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

                case "TypeMaintenanceString":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.TypeMaintenance.Name);
                    else
                        QueryData = QueryData.OrderBy(e => e.TypeMaintenance.Name);
                    break;

                case "StatusMaintenanceString":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.StatusMaintenance);
                    else
                        QueryData = QueryData.OrderBy(e => e.StatusMaintenance);
                    break;

                default:
                    QueryData = QueryData.OrderByDescending(e => e.CreateDate);
                    break;
            }
            // Get TotalRow
            Scroll.TotalRow = await QueryData.CountAsync();
            // Skip Take
            QueryData = QueryData.Skip(Scroll.Skip ?? 0).Take(Scroll.Take ?? 50);

            var HasData = await QueryData.AsNoTracking().ToListAsync();
            var listData = new List<ItemMaintenanceViewModel>();
            foreach (var item in HasData)
                listData.Add(this.mapper.Map<ItemMaintenance, ItemMaintenanceViewModel>(item));

            return new JsonResult(new ScrollDataViewModel<ItemMaintenanceViewModel>(Scroll, listData), this.DefaultJsonSettings);
        }

        // POST: api/RequireMaintenance/
        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] ItemMaintenance record)
        {
            // Set date for CrateDate Entity
            if (record == null)
                return BadRequest();
            // +7 Hour
            record = this.helper.AddHourMethod(record);
            var RunNumber = (await this.repository.GetAllAsQueryable().CountAsync(x => x.CreateDate.Value.Year == record.CreateDate.Value.Year)) + 1;
            record.ItemMaintenanceNo = $"M/{record.CreateDate.Value.ToString("MM/yy")}-{RunNumber.ToString("0000")}";
            record.CreateDate = DateTime.Now;

            if (record.RequisitionStockSps != null)
            {
                foreach(var item in record.RequisitionStockSps)
                {
                    if (item == null)
                        continue;

                    item.CreateDate = record.CreateDate;
                    item.Creator = record.Creator;
                    item.RequisitionEmp = record.MaintenanceEmp;
                    item.PaperNo = record.ItemMaintenanceNo;

                    if (item.MovementStockSp == null)
                    {
                        item.MovementStockSp = new MovementStockSp()
                        {
                            CreateDate = item.CreateDate,
                            Creator = item.Creator,
                            MovementDate = item.RequisitionDate,
                            MovementStatus = MovementStatus.ReceiveStock,
                            Quantity = item.Quantity,
                            SparePartId = item.SparePartId,
                        };
                    }
                }
            }

            if (await this.repository.AddAsync(record) == null)
                return BadRequest();
            return new JsonResult(record, this.DefaultJsonSettings);
        }

        [HttpPut]
        public virtual async Task<IActionResult> Update(int key, [FromBody] ItemMaintenance record)
        {
            if (key < 1)
                return BadRequest();
            if (record == null)
                return BadRequest();

            // +7 Hour
            record = this.helper.AddHourMethod(record);

            // Set date for CrateDate Entity
            record.ModifyDate = DateTime.Now;
            if (await this.repository.UpdateAsync(record, key) != null)
                return BadRequest();
            else
            {

            }
            return new JsonResult(record, this.DefaultJsonSettings);
        }
    }
}
