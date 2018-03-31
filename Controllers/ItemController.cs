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

namespace VipcoMaintenance.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ItemController : GenericController<Item>
    {
        // IRepository
        private IRepositoryMachine<Employee> repositoryEmp;
        private IRepositoryMaintenance<ItemType> repositoryType;
        // Mapper
        private IMapper mapper;

        public ItemController(IRepositoryMaintenance<Item> repo, 
            IRepositoryMachine<Employee> repoEmp,
            IRepositoryMaintenance<ItemType> repoType,
            IMapper map) : base(repo) {
            // Repository
            this.repositoryEmp = repoEmp;
            this.repositoryType = repoType;
            // Mapper
            this.mapper = map;
        }

        // GET: api/controller/5
        [HttpGet("GetKeyNumber")]
        public override async Task<IActionResult> Get(int key)
        {
            var HasItem = await this.repository.GetAsynvWithIncludes(key, "ItemId",new List<string> { "Branch", "ItemType" });
            if (HasItem != null)
            {
                var MapItem = this.mapper.Map<Item, ItemViewModel>(HasItem);
                if (!string.IsNullOrEmpty(MapItem.EmpResponsible))
                    MapItem.EmpResposibleString = (await this.repositoryEmp.GetAsync(MapItem.EmpResponsible)).NameThai;
                return new JsonResult(MapItem, this.DefaultJsonSettings);
            }
            return BadRequest();
        }

        // POST: api/Item/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
            {
                return BadRequest();
            }

            var QueryData = this.repository.GetAllAsQueryable()
                                            //.Include(x => x.Branch)
                                            .Include(x => x.ItemType)
                                            .AsQueryable().AsNoTracking();

            if (Scroll.WhereId.HasValue)
                QueryData = QueryData.Where(x => x.ItemTypeId == Scroll.WhereId);

            if (!string.IsNullOrEmpty(Scroll.Where))
                QueryData = QueryData.Where(x => x.Creator == Scroll.Where);

            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.ToLower().Split(null);

            foreach (var keyword in filters)
            {
                QueryData = QueryData.Where(x => x.Branch.Name.ToLower().Contains(keyword) ||
                                                 x.Description.ToLower().Contains(keyword) ||
                                                 x.ItemCode.ToLower().Contains(keyword) ||
                                                 x.Name.ToLower().Contains(keyword) ||
                                                 x.ItemType.Name.ToLower().Contains(keyword));
            }

            // Order
            switch (Scroll.SortField)
            {
                case "ItemCode":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.ItemCode);
                    else
                        QueryData = QueryData.OrderBy(e => e.ItemCode);
                    break;
                case "Name":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.Name);
                    else
                        QueryData = QueryData.OrderBy(e => e.Name);
                    break;
                case "ItemTypeString":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.ItemType.Name);
                    else
                        QueryData = QueryData.OrderBy(e => e.ItemType.Name);
                    break;

                default:
                    QueryData = QueryData.OrderBy(e => e.ItemCode);
                    break;
            }
            // Get TotalRow
            Scroll.TotalRow = await QueryData.CountAsync();
            // Skip Take
            QueryData = QueryData.Skip(Scroll.Skip ?? 0).Take(Scroll.Take ?? 50);
            var HasData = await QueryData.AsNoTracking().ToListAsync();

            var listData = new List<ItemViewModel>();
            foreach (var item in HasData)
                listData.Add(this.mapper.Map<Item, ItemViewModel>(item));

            return new JsonResult(new ScrollDataViewModel<ItemViewModel>(Scroll, listData), this.DefaultJsonSettings);
        }
    }
}
