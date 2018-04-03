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
                QueryData = QueryData.Where(x => x.RequireEmp == Scroll.Where);

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
            //record = this.helper.AddHourMethod(record);
            var RunNumber = (await this.repository.GetAllAsQueryable().CountAsync(x => x.RequireDate.Year == record.RequireDate.Year)) + 1;
            record.RequireNo = $"{record.RequireDate.ToString("MM/yy")}-{RunNumber.ToString("0000")}";
            record.CreateDate = DateTime.Now;

            if (await this.repository.AddAsync(record) == null)
                return BadRequest();
            return new JsonResult(record, this.DefaultJsonSettings);
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
                            QueryData = QueryData.Where(x => x.RequireStatus == RequireStatus.Waiting || x.RequireStatus == RequireStatus.InProcess);
                        else if (option.Status == 2)
                            QueryData = QueryData.Where(x => x.RequireStatus == RequireStatus.InProcess);
                        else
                            QueryData = QueryData.Where(x => x.RequireStatus != RequireStatus.Cancel);
                    }
                    else
                        QueryData = QueryData.Where(x => x.RequireStatus == RequireStatus.Waiting || 
                                                         x.RequireStatus == RequireStatus.InProcess);

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
