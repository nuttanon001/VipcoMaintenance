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
        private IRepositoryMaintenance<RequisitionStockSp> repositoryRequisition;
        private IRepositoryMaintenance<MovementStockSp> repositoryMovement;
        private IRepositoryMaintenance<ItemMainHasEmployee> repositoryItemMainEmp;
        public ItemMaintenanceController(
            IRepositoryMaintenance<ItemMaintenance> repo,
            IRepositoryMaintenance<RequisitionStockSp> repoRequistion,
            IRepositoryMaintenance<MovementStockSp> repoMovement,
            IRepositoryMaintenance<ItemMainHasEmployee> repoItemMainEmp,
            IMapper map
            ): base(repo) {
            // Repositiory
            this.repositoryItemMainEmp = repoItemMainEmp;
            this.repositoryMovement = repoMovement;
            this.repositoryRequisition = repoRequistion;
            // Mapper
            this.mapper = map;
        }

        // GET: api/RequireMaintenance/5
        [HttpGet("GetKeyNumber")]
        public override async Task<IActionResult> Get(int key)
        {
            var HasItem = await this.repository.GetAsynvWithIncludes(key, "ItemMaintenanceId", new List<string> { "TypeMaintenance", "WorkGroupMaintenance" });
            if (HasItem != null)
            {
                var MapItem = this.mapper.Map<ItemMaintenance, ItemMaintenanceViewModel>(HasItem);
                return new JsonResult(MapItem, this.DefaultJsonSettings);
            }
            return BadRequest();
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

            record.CreateDate = DateTime.Now;
            // +7 Hour
            //record = this.helper.AddHourMethod(record);
            var RunNumber = (await this.repository.GetAllAsQueryable().CountAsync(x => x.CreateDate.Value.Year == record.CreateDate.Value.Year)) + 1;
            record.ItemMaintenanceNo = $"M/{record.CreateDate.Value.ToString("MM/yy")}-{RunNumber.ToString("0000")}";

            // Set ItemMainHasEmployees
            if (record.ItemMainHasEmployees != null)
            {
                foreach(var item in record.ItemMainHasEmployees)
                {
                    if (item == null)
                        continue;

                    item.CreateDate = record.CreateDate;
                    item.Creator = record.Creator;
                }
            }
            // Set RequisitionStockSps
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
        public override async Task<IActionResult> Update(int key, [FromBody] ItemMaintenance record)
        {
            if (key < 1)
                return BadRequest();
            if (record == null)
                return BadRequest();

            // Set date for CrateDate Entity
            record.ModifyDate = DateTime.Now;
            // +7 Hour
            //record = this.helper.AddHourMethod(record);
            foreach (var item in record.ItemMainHasEmployees)
            {
                if (item == null)
                    continue;

                if (item.ItemMainHasEmployeeId > 0)
                {
                    item.ModifyDate = record.ModifyDate;
                    item.Modifyer = record.Modifyer;
                }
                else
                {
                    item.CreateDate = record.ModifyDate;
                    item.Creator = record.Modifyer;
                }
            }
            // Update Requisiton
            foreach (var item in record.RequisitionStockSps)
            {
                if (item == null)
                    continue;
                // If Already have in database
                if (item.RequisitionStockSpId > 0)
                {
                    item.ModifyDate = record.ModifyDate;
                    item.Modifyer = record.Modifyer;
                    item.RequisitionEmp = record.MaintenanceEmp;
                }
                else // if do't have add new to database
                {
                    item.CreateDate = item.ModifyDate;
                    item.Creator = item.Modifyer;
                    item.RequisitionEmp = record.MaintenanceEmp;
                    item.PaperNo = record.ItemMaintenanceNo;
                }
            }


            if (await this.repository.UpdateAsync(record, key) == null)
                return BadRequest();
            else
            {
                // Find requisition of item maintenance
                Expression<Func<RequisitionStockSp, bool>> condition = r => r.ItemMaintenanceId == key;
                var dbRequisition = await this.repositoryRequisition.FindAllAsync(condition);

                Expression<Func<ItemMainHasEmployee, bool>> condition2 = e => e.ItemMaintenanceId == key;
                var dbItemMainHasEmp = await this.repositoryItemMainEmp.FindAllAsync(condition2);

                //Remove requisition if edit remove it
                foreach (var item in dbRequisition)
                {
                    if (!record.RequisitionStockSps.Any(x => x.RequisitionStockSpId == item.RequisitionStockSpId))
                    {
                        if (item.MovementStockSpId.HasValue)
                        {
                            var hasMovement = await this.repositoryMovement.GetAsync(item.MovementStockSpId.Value);
                            if (hasMovement != null)
                            {
                                // Cancel Status
                                hasMovement.MovementStatus = MovementStatus.Cancel;
                                hasMovement.ModifyDate = record.ModifyDate;
                                hasMovement.Modifyer = record.Modifyer;
                                // Update
                                await this.repositoryMovement.UpdateAsync(hasMovement, hasMovement.MovementStockSpId);
                            }
                        }
                        await this.repositoryRequisition.DeleteAsync(item.RequisitionStockSpId);
                    }
                }

                foreach(var item in dbItemMainHasEmp)
                {
                    if (!record.ItemMainHasEmployees.Any(x => x.ItemMainHasEmployeeId == item.ItemMainHasEmployeeId))
                        await this.repositoryItemMainEmp.DeleteAsync(item.ItemMainHasEmployeeId);
                }

                //Update ItemMainHasEmployee or New ItemMainHasEmployee
                foreach(var item in record.ItemMainHasEmployees)
                {
                    if (item == null)
                        continue;

                    if (item.ItemMainHasEmployeeId > 0)
                        await this.repositoryItemMainEmp.UpdateAsync(item, item.ItemMainHasEmployeeId);
                    else
                    {
                        if (item.ItemMaintenanceId is null || item.ItemMaintenanceId < 1)
                            item.ItemMaintenanceId = record.ItemMaintenanceId;
                      
                        await this.repositoryItemMainEmp.AddAsync(item);
                    }
                }

                //Update RequisitionStockSps or New RequisitionStockSps
                foreach (var item in record.RequisitionStockSps)
                {
                    if (item == null)
                        continue;

                    if (item.RequisitionStockSpId > 0)
                    {
                        // Update movement
                        var editMovement = await this.repositoryMovement.GetAsync(item.MovementStockSpId.Value);
                        if (editMovement != null)
                        {
                            editMovement.ModifyDate = item.ModifyDate;
                            editMovement.Modifyer = item.Modifyer;
                            editMovement.MovementDate = item.RequisitionDate;
                            editMovement.Quantity = item.Quantity;
                            editMovement.SparePartId = item.SparePartId;

                            await this.repositoryMovement.UpdateAsync(editMovement, editMovement.MovementStockSpId);
                        }
                        await this.repositoryRequisition.UpdateAsync(item, item.RequisitionStockSpId);
                    }
                    else
                    {
                        if (item.ItemMaintenanceId is null || item.ItemMaintenanceId < 1)
                            item.ItemMaintenanceId = record.ItemMaintenanceId;

                        item.MovementStockSp = new MovementStockSp()
                        {
                            CreateDate = item.CreateDate,
                            Creator = item.Creator,
                            MovementDate = item.RequisitionDate,
                            MovementStatus = MovementStatus.RequisitionStock,
                            Quantity = item.Quantity,
                            SparePartId = item.SparePartId,
                        };
                        await this.repositoryRequisition.AddAsync(item);
                    }
                }
            }
            return new JsonResult(record, this.DefaultJsonSettings);
        }
    }
}
