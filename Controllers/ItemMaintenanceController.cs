﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using VipcoMaintenance.Services;
using VipcoMaintenance.ViewModels;
using VipcoMaintenance.Models.Machines;
using VipcoMaintenance.Models.Maintenances;

using AutoMapper;
using System.Dynamic;

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
        private IRepositoryMaintenance<RequireMaintenance> repositoryRequire;
        private IRepositoryMachine<ProjectCodeMaster> repositoryProject;
        private IRepositoryMachine<Employee> repositoryEmp;
        private IRepositoryMachine<EmployeeGroupMis> repositoryEmpGroup;
        // Controller
        public ItemMaintenanceController(
            IRepositoryMaintenance<ItemMaintenance> repo,
            IRepositoryMaintenance<RequisitionStockSp> repoRequistion,
            IRepositoryMaintenance<MovementStockSp> repoMovement,
            IRepositoryMaintenance<ItemMainHasEmployee> repoItemMainEmp,
            IRepositoryMaintenance<RequireMaintenance> repoRequire,
            IRepositoryMachine<ProjectCodeMaster> repoProject,
            IRepositoryMachine<Employee> repoEmp,
            IRepositoryMachine<EmployeeGroupMis> repoEmpGroup,
            IMapper map
            ): base(repo) {
            // Repositiory
            this.repositoryItemMainEmp = repoItemMainEmp;
            this.repositoryMovement = repoMovement;
            this.repositoryRequisition = repoRequistion;
            this.repositoryProject = repoProject;
            this.repositoryRequire = repoRequire;
            this.repositoryEmp = repoEmp;
            this.repositoryEmpGroup = repoEmpGroup;
            // Mapper
            this.mapper = map;
        }
        
        private async Task<bool> UpdateRequireMaintenance(int RequireMaintenanceId,
            string ByEmp,
            RequireStatus status = RequireStatus.InProcess)
        {
            var RequireData = await this.repositoryRequire.GetAsync(RequireMaintenanceId);
            if (RequireData != null)
            {
                RequireData.MaintenanceApply = RequireData.MaintenanceApply == null ? DateTime.Now : RequireData.MaintenanceApply;
                RequireData.RequireStatus = status;
                RequireData.ModifyDate = DateTime.Now;
                RequireData.Modifyer = ByEmp;

                return await this.repositoryRequire.UpdateAsync(RequireData, RequireData.RequireMaintenanceId) != null;
            }
            else
                return false;
        }

        private StatusMaintenance ChangeStatus(ItemMaintenance itemMaintenance)
        {
            if (itemMaintenance != null)
            {
                if (itemMaintenance.StatusMaintenance == StatusMaintenance.TakeAction || 
                    itemMaintenance.StatusMaintenance == StatusMaintenance.InProcess)
                {
                    // Actual start is set
                    if (itemMaintenance.ActualStartDate.HasValue)
                    {
                        // Actual end is set
                        if (itemMaintenance.ActualEndDate.HasValue)
                            return StatusMaintenance.Complate;
                        else // Actual end is not set
                            return StatusMaintenance.InProcess;
                    }
                    else
                        return StatusMaintenance.TakeAction;
                }
            }
            else
                return StatusMaintenance.TakeAction;

            return itemMaintenance.StatusMaintenance.Value;
        }

        // GET: api/ItemMaintenance/5
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

        // GET: api/ItemMaintenance/ItemMaintenanceReport
        [HttpGet("ItemMaintenanceReport")]
        public async Task<IActionResult> ItemMaintenanceReport(int key)
        {
            if (key > 0)
            {
                var ItemMain = await this.repository.GetAllAsQueryable()
                                                     .Include(x => x.ItemMainHasEmployees)
                                                     .Include(x => x.RequireMaintenance.Item)
                                                     .Include(x => x.RequireMaintenance.Branch)
                                                     .Include(x => x.WorkGroupMaintenance)
                                                     .Include(x => x.RequisitionStockSps)
                                                        .ThenInclude(x => x.SparePart)
                                                     .FirstOrDefaultAsync(x => x.ItemMaintenanceId == key);

                if (ItemMain != null)
                {
                    var WorkGroupName = (await this.repositoryEmpGroup.GetAsync(ItemMain.RequireMaintenance.GroupMIS))?.GroupDesc ?? "";
                    var RequireMaintenBy = (await this.repositoryEmp.GetAsync(ItemMain.RequireMaintenance.RequireEmp))?.NameThai ?? "";
                    // Get ReportOverTimeMaster
                    var ReportItemMaintenance = new
                    {
                        RequireMaintenanceNo = ItemMain.RequireMaintenance.RequireNo,
                        RequireDate = ItemMain?.RequireMaintenance?.RequireDate.ToString("dd/MM/yyyy  HH:mm น."),
                        BranchName = ItemMain?.RequireMaintenance?.Branch?.Name ?? "-",
                        RequireMaintenBy = RequireMaintenBy,
                        WorkGroupName = WorkGroupName,
                        GroupMainten = ItemMain?.WorkGroupMaintenance.Name,
                        ItemName = $"{ItemMain?.RequireMaintenance?.Item?.ItemCode ?? "-" } / {ItemMain?.RequireMaintenance?.Item?.Name ?? "-" }", 
                        DescRequireMainten = ItemMain?.RequireMaintenance.Description ?? "-",
                        // =========================================================== //
                        DescMainten = ItemMain.Description,
                        StartActual = ItemMain.ActualStartDate != null ? ItemMain.ActualStartDate.Value.ToString("dd/MM/yyyy  HH:mm น.") : "",
                        EndActual = ItemMain.ActualEndDate != null ? ItemMain.ActualEndDate.Value.ToString("dd/MM/yyyy  HH:mm น.") : "",
                        // Lists
                        MaintenBy = new[] { new { EmpCode = "",NameThai = "" } }.ToList(),
                        SparePartes = new[] { new { SpareName = "",Quantity = 0D } }.ToList(),
                    };

                    foreach(var MainBy in ItemMain.ItemMainHasEmployees)
                    {
                        var EmpString = (await this.repositoryEmp.GetAsync(MainBy.EmpCode)).NameThai;
                        ReportItemMaintenance.MaintenBy.Add(new
                        {
                            EmpCode = MainBy.EmpCode,
                            NameThai = EmpString
                        });
                    }

                    foreach(var Requisition in ItemMain.RequisitionStockSps)
                    {
                        ReportItemMaintenance.SparePartes.Add(new
                        {
                            SpareName = Requisition?.SparePart?.Name ?? "-",
                            Quantity = Requisition.Quantity
                        });
                    }

                    // Get ReportOverTimeDetail
                    return new JsonResult(ReportItemMaintenance, this.DefaultJsonSettings);
                }
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

        // POST: api/ItemMaintenance/
        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] ItemMaintenance record)
        {
            // Set date for CrateDate Entity
            if (record == null)
                return BadRequest();

            record.CreateDate = DateTime.Now;
            record.StatusMaintenance = this.ChangeStatus(record);
            // +7 Hour
            record = this.helper.AddHourMethod(record);
            var RunNumber = (await this.repository.GetAllAsQueryable().CountAsync(x => x.CreateDate.Value.Year == record.CreateDate.Value.Year)) + 1;
            record.ItemMaintenanceNo = $"M/{record.CreateDate.Value.ToString("yy")}-{RunNumber.ToString("0000")}";

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

            // Update Status RequireMaintenance
            await this.UpdateRequireMaintenance(record.RequireMaintenanceId.Value, record.Creator);

            if (record.RequireMaintenance != null)
                record.RequireMaintenance = null;

            return new JsonResult(record, this.DefaultJsonSettings);
        }

        // POST: api/ItemMaintenance/Schedule
        [HttpPost("Schedule")]
        public async Task<IActionResult> Schedule([FromBody] OptionItemMaintananceSchedule Schedule)
        {
            var message = "Data not found.";
            try
            {
                var QueryData = this.repository.GetAllAsQueryable()
                                               .Include(x => x.RequireMaintenance.Item)
                                               .Include(x => x.TypeMaintenance)
                                               .Include(x => x.WorkGroupMaintenance)
                                               .Where(x => x.PlanStartDate != null &&
                                                           x.PlanEndDate != null &&
                                                           x.StatusMaintenance != StatusMaintenance.Cancel)
                                               .AsQueryable();
                int TotalRow;

                if (Schedule != null)
                {
                    // Option Filter
                    if (!string.IsNullOrEmpty(Schedule.Filter))
                    {
                        var filters = string.IsNullOrEmpty(Schedule.Filter) ? new string[] { "" }
                                   : Schedule.Filter.ToLower().Split(null);
                        foreach (var keyword in filters)
                        {
                            QueryData = QueryData.Where(x => x.Description.ToLower().Contains(keyword) ||
                                                             x.Remark.ToLower().Contains(keyword) ||
                                                             x.ItemMaintenanceNo.ToLower().Contains(keyword) ||
                                                             x.TypeMaintenance.Name.ToLower().Contains(keyword) ||
                                                             x.WorkGroupMaintenance.Name.ToLower().Contains(keyword) ||
                                                             x.RequireMaintenance.Item.ItemCode.ToLower().Contains(keyword) ||
                                                             x.RequireMaintenance.Item.Name.ToLower().Contains(keyword));
                        }
                    }
                    // Option Mode
                    if (Schedule.Mode.HasValue)
                    {
                        if (Schedule.Mode == 1)
                            QueryData = QueryData.OrderByDescending(x => x.PlanStartDate);
                        else
                            QueryData = QueryData.Where(x => x.StatusMaintenance == StatusMaintenance.InProcess ||
                                                             x.StatusMaintenance == StatusMaintenance.TakeAction)
                                                 .OrderBy(x => x.PlanStartDate);
                    }
                    // Option ProjectMasterId
                    if (Schedule.ProjectMasterId.HasValue)
                        QueryData = QueryData.Where(x => x.RequireMaintenance.ProjectCodeMasterId == Schedule.ProjectMasterId);
                    // Option Create
                    if (!string.IsNullOrEmpty(Schedule.Creator))
                        QueryData = QueryData.Where(x => x.RequireMaintenance.RequireEmp == Schedule.Creator);
                    // Option ItemMaintenance
                    if (Schedule.ItemMaintenanceId.HasValue)
                        QueryData = QueryData.Where(x => x.ItemMaintenanceId == Schedule.ItemMaintenanceId);
                    // Option RequireMaintenance
                    if (Schedule.RequireMaintenanceId.HasValue)
                        QueryData = QueryData.Where(x => x.RequireMaintenanceId == Schedule.RequireMaintenanceId);
                    // Option WorkGroupMaintenance
                    if (Schedule.GroupMaintenanceId.HasValue)
                        QueryData = QueryData.Where(x => x.WorkGroupMaintenanceId == Schedule.GroupMaintenanceId);
                    // Option TypeMaintenance
                    if (Schedule.TypeMaintenanceId.HasValue)
                        QueryData = QueryData.Where(x => x.TypeMaintenanceId == Schedule.TypeMaintenanceId);

                    TotalRow = await QueryData.CountAsync();
                    // Option Skip and Task
                    // if (Scehdule.Skip.HasValue && Scehdule.Take.HasValue)
                    QueryData = QueryData.Skip(Schedule.Skip ?? 0).Take(Schedule.Take ?? 20);
                }
                else
                    TotalRow = await QueryData.CountAsync();

                var GetData = await QueryData.ToListAsync();
                if (GetData.Any())
                {
                    IDictionary<string, int> ColumnGroupTop = new Dictionary<string, int>();
                    IDictionary<DateTime, string> ColumnGroupBtm = new Dictionary<DateTime, string>();
                    List<string> ColumnsAll = new List<string>();
                    // PlanDate
                    List<DateTime?> ListDate = new List<DateTime?>()
                    {
                        //START Date
                        GetData.Min(x => x.PlanStartDate),
                        GetData.Min(x => x.ActualStartDate) ?? null,
                        GetData.Min(x => x.RequireMaintenance.MaintenanceApply) ?? null,
                        //END Date
                        GetData.Max(x => x.PlanEndDate),
                        GetData.Max(x => x.ActualEndDate) ?? null,
                        GetData.Max(x => x.RequireMaintenance.MaintenanceApply) ?? null,
                    };

                    DateTime? MinDate = ListDate.Min();
                    DateTime? MaxDate = ListDate.Max();

                    if (MinDate == null && MaxDate == null)
                        return NotFound(new { Error = "Data not found" });

                    int countCol = 1;
                    // add Date to max
                    MaxDate = MaxDate.Value.AddDays(2);
                    MinDate = MinDate.Value.AddDays(-2);

                    // If Range of date below then 15 day add more
                    var RangeDay = (MaxDate.Value - MinDate.Value).Days;
                    if (RangeDay < 15)
                    {
                        MaxDate = MaxDate.Value.AddDays((15 - RangeDay) /2);
                        MinDate = MinDate.Value.AddDays((((15 - RangeDay) /2) * -1));
                    }

                    // EachDay
                    var EachDate = new Helper.LoopEachDate();
                    // Foreach Date
                    foreach (DateTime day in EachDate.EachDate(MinDate.Value, MaxDate.Value))
                    {
                        // Get Month
                        if (ColumnGroupTop.Any(x => x.Key == day.ToString("MMMM")))
                            ColumnGroupTop[day.ToString("MMMM")] += 1;
                        else
                            ColumnGroupTop.Add(day.ToString("MMMM"), 1);

                        ColumnGroupBtm.Add(day.Date, $"Col{countCol.ToString("00")}");
                        countCol++;
                    }

                    var DataTable = new List<IDictionary<String, Object>>();
                    // OrderBy(x => x.Machine.TypeMachineId).ThenBy(x => x.Machine.MachineCode)
                    foreach (var Data in GetData.OrderBy(x => x.PlanStartDate).ThenBy(x => x.PlanEndDate))
                    {
                        IDictionary<String, Object> rowData = new ExpandoObject();
                        var Progress = Data.StatusMaintenance != null ? System.Enum.GetName(typeof(StatusMaintenance), Data.StatusMaintenance) : "NoAction";
                        var ProjectMaster = "NoData";
                        if (Data?.RequireMaintenance?.ProjectCodeMasterId != null)
                        {
                            var ProjectData = await this.repositoryProject.
                                        GetAsync(Data.RequireMaintenance.ProjectCodeMasterId ?? 0);
                            ProjectMaster = ProjectData != null ? $"{ProjectData.ProjectCode}/{ProjectData.ProjectName}" : "-";
                        }

                        // add column time
                        rowData.Add("ProjectMaster", ProjectMaster);
                        rowData.Add("GroupMaintenance", Data?.WorkGroupMaintenance?.Name ?? "-");
                        rowData.Add("Item", (Data.RequireMaintenance == null ? "" : $"{Data.RequireMaintenance.Item.ItemCode}/{Data.RequireMaintenance.Item.Name}"));
                        rowData.Add("Progress", Progress);
                        rowData.Add("ItemMainStatus", Data.StatusMaintenance);
                        rowData.Add("ItemMaintenanceId", Data.ItemMaintenanceId);
                        // Add new
                        if (Data.RequireMaintenance.MaintenanceApply.HasValue)
                        {
                            if (ColumnGroupBtm.Any(x => x.Key == Data.RequireMaintenance.MaintenanceApply.Value.Date))
                                rowData.Add("Response", ColumnGroupBtm.FirstOrDefault(x => x.Key == Data.RequireMaintenance.MaintenanceApply.Value.Date).Value);
                        }
                        // End new

                        // Data is 1:Plan,2:Actual,3:PlanAndActual
                        // For Plan1
                        if (Data.PlanStartDate != null && Data.PlanEndDate != null)
                        {
                            // If Same Date can't loop
                            if (Data.PlanStartDate.Date == Data.PlanEndDate.Date)
                            {
                                if(ColumnGroupBtm.Any(x => x.Key == Data.PlanStartDate.Date))
                                        rowData.Add(ColumnGroupBtm.FirstOrDefault(x => x.Key == Data.PlanStartDate.Date).Value, 1);
                            }
                            else
                            {
                                foreach (DateTime day in EachDate.EachDate(Data.PlanStartDate, Data.PlanEndDate))
                                {
                                    if (ColumnGroupBtm.Any(x => x.Key == day.Date))
                                        rowData.Add(ColumnGroupBtm.FirstOrDefault(x => x.Key == day.Date).Value, 1);
                                }
                            }
                        }

                        //For Actual
                        if (Data.ActualStartDate != null)
                        {
                            var EndDate = Data.ActualEndDate ?? (MaxDate > DateTime.Today ? DateTime.Today : MaxDate);
                            if (Data.ActualStartDate.Value.Date > EndDate.Value.Date)
                                EndDate = Data.ActualStartDate;
                            // If Same Date can't loop 
                            if (Data.ActualStartDate.Value.Date == EndDate.Value.Date)
                            {
                                if (ColumnGroupBtm.Any(x => x.Key == Data.ActualStartDate.Value.Date))
                                {
                                    var Col = ColumnGroupBtm.FirstOrDefault(x => x.Key == Data.ActualStartDate.Value.Date);
                                    // if Have Plan change value to 3
                                    if (rowData.Keys.Any(x => x == Col.Value))
                                        rowData[Col.Value] = 3;
                                    else // else Don't have plan value is 2
                                        rowData.Add(Col.Value, 2);
                                }
                            }
                            else
                            {
                                foreach (DateTime day in EachDate.EachDate(Data.ActualStartDate.Value, EndDate.Value))
                                {
                                    if (ColumnGroupBtm.Any(x => x.Key == day.Date))
                                    {
                                        var Col = ColumnGroupBtm.FirstOrDefault(x => x.Key == day.Date);

                                        // if Have Plan change value to 3
                                        if (rowData.Keys.Any(x => x == Col.Value))
                                            rowData[Col.Value] = 3;
                                        else // else Don't have plan value is 2
                                            rowData.Add(Col.Value, 2);
                                    }
                                }
                            }
                        }

                        DataTable.Add(rowData);
                    }

                    if (DataTable.Any())
                        ColumnGroupBtm.OrderBy(x => x.Key.Date).Select(x => x.Value)
                            .ToList().ForEach(item => ColumnsAll.Add(item));

                    return new JsonResult(new
                    {
                        TotalRow = TotalRow,
                        ColumnsTop = ColumnGroupTop.Select(x => new
                        {
                            Name = x.Key,
                            Value = x.Value
                        }),
                        ColumnsLow = ColumnGroupBtm.OrderBy(x => x.Key.Date).Select(x => x.Key.Day),
                        ColumnsAll = ColumnsAll,
                        DataTable = DataTable
                    }, this.DefaultJsonSettings);
                } 
            }
            catch(Exception ex)
            {
                message = $"Has error with message has {ex.ToString()}.";
            }
            return BadRequest(new { Error = message });
        }
              
        // PUT: api/ItemMaintenance/
        [HttpPut]
        public override async Task<IActionResult> Update(int key, [FromBody] ItemMaintenance record)
        {
            if (key < 1)
                return BadRequest();
            if (record == null)
                return BadRequest();

            // Set date for CrateDate Entity
            record.ModifyDate = DateTime.Now;
            record.StatusMaintenance = this.ChangeStatus(record);
            // +7 Hour
            record = this.helper.AddHourMethod(record);
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

            // Update Status RequireMaintenance
            RequireStatus status = record.StatusMaintenance == StatusMaintenance.Cancel ? RequireStatus.Waiting :
                (record.StatusMaintenance == StatusMaintenance.Complate ? RequireStatus.Complate : RequireStatus.InProcess);

            await this.UpdateRequireMaintenance(record.RequireMaintenanceId.Value, record.Creator,status);

            if (record.RequireMaintenance != null)
                record.RequireMaintenance = null;

            return new JsonResult(record, this.DefaultJsonSettings);
        }
    }
}
