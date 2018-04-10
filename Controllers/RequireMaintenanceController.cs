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
using VipcoMaintenance.Models.Machines;
using VipcoMaintenance.Models.Maintenances;
using AutoMapper;
using System.Dynamic;

namespace VipcoMaintenance.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class RequireMaintenanceController : GenericController<RequireMaintenance>
    {
        // Repository
        private IRepositoryMachine<ProjectCodeMaster> repositoryProject;
        private IRepositoryMachine<Employee> repositoryEmployee;
        private IRepositoryMachine<EmployeeGroupMis> repositoryGroupMis;
        // Mapper
        private IMapper mapper;

        public RequireMaintenanceController(IRepositoryMaintenance<RequireMaintenance> repo,
            IRepositoryMachine<ProjectCodeMaster> repoPro,
            IRepositoryMachine<Employee> repoEmp,
            IRepositoryMachine<EmployeeGroupMis> repoGroupMis,
            IMapper map
            ) : base(repo) {
            // Repository Machine
            this.repositoryEmployee = repoEmp;
            this.repositoryProject = repoPro;
            this.repositoryGroupMis = repoGroupMis;
            // Mapper
            this.mapper = map;

        }

        #region Property
        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
        #endregion

        // GET: api/RequireMaintenance/5
        [HttpGet("GetKeyNumber")]
        public override async Task<IActionResult> Get(int key)
        {
            var HasItem = await this.repository.GetAsynvWithIncludes(key, "RequireMaintenanceId", new List<string> { "Item", "Branch" });
            if (HasItem != null)
            {
                var MapItem = this.mapper.Map<RequireMaintenance, RequireMaintenanceViewModel>(HasItem);
                if (!string.IsNullOrEmpty(MapItem.RequireEmp))
                    MapItem.RequireEmpString = (await this.repositoryEmployee.GetAsync(MapItem.RequireEmp)).NameThai;
                if (MapItem.ProjectCodeMasterId.HasValue)
                {
                   var HasProject = await this.repositoryProject.GetAsync(MapItem.ProjectCodeMasterId ?? 0);
                    MapItem.ProjectCodeMasterString = HasProject != null ? $"{HasProject.ProjectCode}/{HasProject.ProjectName}" : "-";
                }
                if (!string.IsNullOrEmpty(MapItem.GroupMIS))
                    MapItem.GroupMISString = (await this.repositoryGroupMis.GetAsync(MapItem.GroupMIS)).GroupDesc;

                return new JsonResult(MapItem, this.DefaultJsonSettings);
            }
            return BadRequest();
        }
        // GET: api/ActionRequireMaintenance/5
        [HttpGet("ActionRequireMaintenance")]
        public async Task<IActionResult> ActionRequireMaintenance(int key,string byEmp)
        {
            if (key > 0)
            {
                var HasData = await this.repository.GetAsync(key);
                if (HasData != null)
                {
                    HasData.MaintenanceApply = DateTime.Now;
                    HasData.ModifyDate = DateTime.Now;
                    HasData.Modifyer = byEmp;

                    return new JsonResult(await this.repository.UpdateAsync(HasData, key), this.DefaultJsonSettings);
                }
            }
            return BadRequest();
        }

        // POST: api/RequireMaintenance/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var QueryData = this.repository.GetAllAsQueryable().AsQueryable()
                                .Include(x => x.Item)
                                .AsNoTracking();

            if (!string.IsNullOrEmpty(Scroll.Where))
                QueryData = QueryData.Where(x => x.Creator == Scroll.Where);

            if (Scroll.WhereId.HasValue)
                QueryData = QueryData.Where(x => x.Item.ItemTypeId == Scroll.WhereId);

            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.ToLower().Split(null);

            foreach (var keyword in filters)
            {
                QueryData = QueryData.Where(x => x.RequireNo.ToLower().Contains(keyword) ||
                                                 x.Item.Name.ToLower().Contains(keyword) ||
                                                 x.Item.ItemCode.ToLower().Contains(keyword) ||
                                                 x.Remark.ToLower().Contains(keyword) ||
                                                 x.Description.ToLower().Contains(keyword));
            }

            // Order
            switch (Scroll.SortField)
            {
                case "RequireNo":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.RequireNo);
                    else
                        QueryData = QueryData.OrderBy(e => e.RequireNo);
                    break;
                case "ItemCode":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.Item.ItemCode);
                    else
                        QueryData = QueryData.OrderBy(e => e.Item.ItemCode);
                    break;
                case "RequireDate":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.RequireDate);
                    else
                        QueryData = QueryData.OrderBy(e => e.RequireDate);
                    break;
                default:
                    QueryData = QueryData.OrderByDescending(e => e.RequireDate);
                    break;
            }
            // Get TotalRow
            Scroll.TotalRow = await QueryData.CountAsync();
            // Skip Take
            QueryData = QueryData.Skip(Scroll.Skip ?? 0).Take(Scroll.Take ?? 50);

            var HasData = await QueryData.AsNoTracking().ToListAsync();
            var listData = new List<RequireMaintenanceViewModel>();
            foreach (var item in HasData)
            {
                // 
                var MapItem = this.mapper.Map<RequireMaintenance, RequireMaintenanceViewModel>(item);
                if (MapItem.RequireStatus == RequireStatus.Waiting && MapItem.MaintenanceApply.HasValue)
                {
                    MapItem.RequireStatus = RequireStatus.MaintenanceResponse;
                }
                listData.Add(MapItem);

            }

            return new JsonResult(new ScrollDataViewModel<RequireMaintenanceViewModel>(Scroll, listData), this.DefaultJsonSettings);
        }

        // POST: api/RequireMaintenance/
        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] RequireMaintenance record)
        {
            // Set date for CrateDate Entity
            if (record == null)
                return BadRequest();
            // +7 Hour
            record = this.helper.AddHourMethod(record);
            var RunNumber = (await this.repository.GetAllAsQueryable().CountAsync(x => x.RequireDate.Year == record.RequireDate.Year)) + 1;
            record.RequireNo = $"{record.RequireDate.ToString("yy")}-{RunNumber.ToString("0000")}";
            record.CreateDate = DateTime.Now;

            if (await this.repository.AddAsync(record) == null)
                return BadRequest();
            return new JsonResult(record, this.DefaultJsonSettings);
        }

        // POST: api/ItemMaintenance/ScheduleWithRequire
        [HttpPost("ScheduleWithRequire")]
        public async Task<IActionResult> ScheduleWithRequire([FromBody] OptionItemMaintananceSchedule Schedule)
        {
            var message = "Data not found.";
            try
            {
                var QueryData = this.repository.GetAllAsQueryable()
                                               .Include(x => x.Item)
                                               .Include(x => x.ItemMaintenance.TypeMaintenance)
                                               .Include(x => x.ItemMaintenance.WorkGroupMaintenance)
                                               .Where(x => x.RequireStatus != RequireStatus.Cancel)
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
                                                             x.ItemMaintenance.ItemMaintenanceNo.ToLower().Contains(keyword) ||
                                                             x.ItemMaintenance.TypeMaintenance.Name.ToLower().Contains(keyword) ||
                                                             x.Item.ItemType.Name.ToLower().Contains(keyword) ||
                                                             x.Item.ItemCode.ToLower().Contains(keyword) ||
                                                             x.Item.Name.ToLower().Contains(keyword));
                        }
                    }
                    // Option Mode
                    if (Schedule.Mode.HasValue)
                    {
                        if (Schedule.Mode == 1)
                            QueryData = QueryData.OrderByDescending(x => x.RequireDate);
                        else
                            QueryData = QueryData.Where(x => x.RequireStatus == RequireStatus.InProcess ||
                                                             x.RequireStatus == RequireStatus.Waiting ||
                                                             x.RequireStatus == RequireStatus.MaintenanceResponse)
                                                 .OrderBy(x => x.RequireDate);
                    }
                    // Option ProjectMasterId
                    if (Schedule.ProjectMasterId.HasValue)
                        QueryData = QueryData.Where(x => x.ProjectCodeMasterId == Schedule.ProjectMasterId);
                    // Option Create
                    if (!string.IsNullOrEmpty(Schedule.Creator))
                        QueryData = QueryData.Where(x => x.RequireEmp == Schedule.Creator);
                    // Option RequireMaintenance
                    if (Schedule.RequireMaintenanceId.HasValue)
                        QueryData = QueryData.Where(x => x.RequireMaintenanceId == Schedule.RequireMaintenanceId);
                    // Option WorkGroupMaintenance
                    if (Schedule.GroupMaintenanceId.HasValue)
                        QueryData = QueryData.Where(x => x.ItemMaintenance.WorkGroupMaintenanceId == Schedule.GroupMaintenanceId);
                    
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
                        GetData.Min(x => x.RequireDate),
                        GetData.Min(x => x?.ItemMaintenance?.PlanStartDate) ?? null,
                        GetData.Min(x => x?.ItemMaintenance?.ActualStartDate) ?? null,
                        GetData.Min(x => x?.MaintenanceApply) ?? null,
                        //END Date
                        GetData.Max(x => x.RequireDate),
                        GetData.Max(x => x?.ItemMaintenance?.PlanEndDate) ?? null,
                        GetData.Max(x => x?.ItemMaintenance?.ActualEndDate) ?? null,
                        GetData.Max(x => x?.MaintenanceApply) ?? null,
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
                        MaxDate = MaxDate.Value.AddDays((15 - RangeDay) / 2);
                        MinDate = MinDate.Value.AddDays((((15 - RangeDay) / 2) * -1));
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
                    foreach (var Data in GetData.OrderBy(x => x.RequireDate).ThenBy(x => x.CreateDate))
                    {
                        IDictionary<String, Object> rowData = new ExpandoObject();
                        var Progress = Data?.ItemMaintenance?.StatusMaintenance != null ? System.Enum.GetName(typeof(StatusMaintenance), Data.ItemMaintenance.StatusMaintenance) : "NoAction";
                        var ProjectMaster = "NoData";
                        if (Data?.ProjectCodeMasterId != null)
                        {
                            var ProjectData = await this.repositoryProject.
                                        GetAsync(Data.ProjectCodeMasterId ?? 0);
                            ProjectMaster = ProjectData != null ? ($"{ProjectData.ProjectCode}/{ProjectData.ProjectName}") : "-";
                            if (ProjectMaster.Length > 50)
                            {
                                ProjectMaster = ProjectMaster.Substring(0, 50) + "...";
                            }
                        }

                        // add column time
                        rowData.Add("ProjectMaster", ProjectMaster);
                        rowData.Add("GroupMaintenance", Data?.ItemMaintenance?.WorkGroupMaintenance?.Name ?? "Not-Assign");
                        rowData.Add("Item", (Data == null ? "Data not been found" : $"{Data.Item.ItemCode}/{Data.Item.Name}"));
                        rowData.Add("Progress", Progress);
                        rowData.Add("ItemMainStatus", Data?.ItemMaintenance != null ? Data.ItemMaintenance.StatusMaintenance : StatusMaintenance.Cancel);
                        rowData.Add("ItemMaintenanceId", Data?.ItemMaintenance != null ? Data.ItemMaintenance.ItemMaintenanceId : 0);
                        // Add new
                        if (Data.MaintenanceApply.HasValue)
                        {
                            if (ColumnGroupBtm.Any(x => x.Key == Data.MaintenanceApply.Value.Date))
                                rowData.Add("Response", ColumnGroupBtm.FirstOrDefault(x => x.Key == Data.MaintenanceApply.Value.Date).Value);
                        }
                        // End new

                        // Data is 1:Plan,2:Actual,3:PlanAndActual
                        // For Plan1
                        if (Data.ItemMaintenance != null)
                        {
                            if (Data?.ItemMaintenance?.PlanStartDate != null && Data?.ItemMaintenance?.PlanEndDate != null)
                            {
                                // If Same Date can't loop
                                if (Data?.ItemMaintenance?.PlanStartDate.Date == Data?.ItemMaintenance?.PlanEndDate.Date)
                                {
                                    if (ColumnGroupBtm.Any(x => x.Key == Data?.ItemMaintenance?.PlanStartDate.Date))
                                        rowData.Add(ColumnGroupBtm.FirstOrDefault(x => x.Key == Data?.ItemMaintenance?.PlanStartDate.Date).Value, 1);
                                }
                                else
                                {
                                    foreach (DateTime day in EachDate.EachDate(Data.ItemMaintenance.PlanStartDate, Data.ItemMaintenance.PlanEndDate))
                                    {
                                        if (ColumnGroupBtm.Any(x => x.Key == day.Date))
                                            rowData.Add(ColumnGroupBtm.FirstOrDefault(x => x.Key == day.Date).Value, 1);
                                    }
                                }
                            }

                            //For Actual
                            if (Data?.ItemMaintenance?.ActualStartDate != null)
                            {
                                var EndDate = Data?.ItemMaintenance?.ActualEndDate ?? (MaxDate > DateTime.Today ? DateTime.Today : MaxDate);
                                if (Data?.ItemMaintenance?.ActualStartDate.Value.Date > EndDate.Value.Date)
                                    EndDate = Data?.ItemMaintenance?.ActualStartDate;
                                // If Same Date can't loop 
                                if (Data?.ItemMaintenance?.ActualStartDate.Value.Date == EndDate.Value.Date)
                                {
                                    if (ColumnGroupBtm.Any(x => x.Key == Data?.ItemMaintenance?.ActualStartDate.Value.Date))
                                    {
                                        var Col = ColumnGroupBtm.FirstOrDefault(x => x.Key == Data?.ItemMaintenance?.ActualStartDate.Value.Date);
                                        // if Have Plan change value to 3
                                        if (rowData.Keys.Any(x => x == Col.Value))
                                            rowData[Col.Value] = 3;
                                        else // else Don't have plan value is 2
                                            rowData.Add(Col.Value, 2);
                                    }
                                }
                                else
                                {
                                    foreach (DateTime day in EachDate.EachDate(Data.ItemMaintenance.ActualStartDate.Value, EndDate.Value))
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
            catch (Exception ex)
            {
                message = $"Has error with message has {ex.ToString()}.";
            }
            return BadRequest(new { Error = message });
        }

        // POST: api/RequireMaintenance/MaintenanceWaiting
        [HttpPost("MaintenanceWaiting")]
        public async Task<IActionResult> MaintenanceWaiting([FromBody] OptionRequireMaintenace option)
        {
            string Message = "";
            try
            {
                var QueryData = this.repository.GetAllAsQueryable()
                                               .Where(x => x.RequireStatus != RequireStatus.Cancel)
                                               .Include(x => x.Item.ItemType)
                                               .AsNoTracking()
                                               .AsQueryable();
                int TotalRow;

                if (option != null)
                {
                    if (!string.IsNullOrEmpty(option.Filter))
                    {
                        // Filter
                        var filters = string.IsNullOrEmpty(option.Filter) ? new string[] { "" }
                                            : option.Filter.ToLower().Split(null);
                        foreach (var keyword in filters)
                        {
                            QueryData = QueryData.Where(x => x.Description.ToLower().Contains(keyword) ||
                                                             x.Remark.ToLower().Contains(keyword) ||
                                                             x.Item.ItemCode.ToLower().Contains(keyword) ||
                                                             x.Item.Name.ToLower().Contains(keyword));
                        }
                    }

                    // Option ProjectCodeMaster
                    if (option.ProjectId.HasValue)
                        QueryData = QueryData.Where(x => x.ProjectCodeMasterId == option.ProjectId);

                    // Option Status
                    if (option.Status.HasValue)
                    {
                        if (option.Status == 1)
                            QueryData = QueryData.Where(x => x.RequireStatus == RequireStatus.Waiting);
                        else if (option.Status == 2)
                            QueryData = QueryData.Where(x => x.RequireStatus == RequireStatus.InProcess);
                        else
                            QueryData = QueryData.Where(x => x.RequireStatus != RequireStatus.Cancel);
                    }
                    else
                        QueryData = QueryData.Where(x => x.RequireStatus == RequireStatus.Waiting);

                    TotalRow = await QueryData.CountAsync();

                    // Option Skip and Task
                    if (option.Skip.HasValue && option.Take.HasValue)
                        QueryData = QueryData.Skip(option.Skip ?? 0).Take(option.Take ?? 50);
                    else
                        QueryData = QueryData.Skip(0).Take(50);
                }
                else
                    TotalRow = await QueryData.CountAsync();

                var GetData = await QueryData.ToListAsync();
                if (GetData.Any())
                {
                    List<string> Columns = new List<string>();

                    var MinDate = GetData.Min(x => x.RequireDate);
                    var MaxDate = GetData.Max(x => x.RequireDate);

                    if (MinDate == null && MaxDate == null)
                    {
                        return NotFound(new { Error = "Data not found" });
                    }

                    foreach (DateTime day in EachDay(MinDate, MaxDate))
                    {
                        if (GetData.Any(x => x.RequireDate.Date == day.Date))
                            Columns.Add(day.Date.ToString("dd/MM/yy"));
                    }

                    var DataTable = new List<IDictionary<String, Object>>();

                    foreach (var Data in GetData.OrderBy(x => x.Item.ItemType.Name))
                    {
                        var ItemTypeName = $"{Data.Item.ItemType.Name ?? "No-Data"}";

                        IDictionary<String, Object> rowData;
                        bool update = false;
                        if (DataTable.Any(x => (string)x["ItemTypeName"] == ItemTypeName))
                        {
                            var FirstData = DataTable.FirstOrDefault(x => (string)x["ItemTypeName"] == ItemTypeName);
                            if (FirstData != null)
                            {
                                rowData = FirstData;
                                update = true;
                            }
                            else
                                rowData = new ExpandoObject();
                        }
                        else
                            rowData = new ExpandoObject();

                        //Get Employee Name
                        // var Employee = await this.repositoryEmp.GetAsync(Data.RequireEmp);
                        // var EmployeeReq = Employee != null ? $"คุณ{(Employee?.NameThai ?? "")}" : "No-Data";

                        var Key = Data.RequireDate.ToString("dd/MM/yy");
                        // New Data
                        var Master = new RequireMaintenanceViewModel()
                        {
                            RequireMaintenanceId = Data.RequireMaintenanceId,
                            MaintenanceApply = Data.MaintenanceApply != null ? Data.MaintenanceApply : null,
                            // RequireString = $"{EmployeeReq} | No.{Data.RequireNo}",
                            ItemCode = $"{Data.Item.ItemCode}/{Data.Item.Name}",
                            RequireEmpString = string.IsNullOrEmpty(Data.RequireEmp) ? "-" : "คุณ" + (await this.repositoryEmployee.GetAsync(Data.RequireEmp)).NameThai
                        };

                        if (rowData.Any(x => x.Key == Key))
                        {
                            // New Value
                            var ListMaster = (List<RequireMaintenanceViewModel>)rowData[Key];
                            ListMaster.Add(Master);
                            // add to row data
                            rowData[Key] = ListMaster;
                        }
                        else // add new
                            rowData.Add(Key, new List<RequireMaintenanceViewModel>() { Master });

                        if (!update)
                        {
                            rowData.Add("ItemTypeName", ItemTypeName);
                            DataTable.Add(rowData);
                        }
                    }

                    return new JsonResult(new
                    {
                        TotalRow = TotalRow,
                        Columns = Columns,
                        DataTable = DataTable
                    }, this.DefaultJsonSettings);
                }
            }
            catch (Exception ex)
            {
                Message = $"Has error {ex.ToString()}";
            }

            return NotFound(new { Error = Message });
        }
    }
}
