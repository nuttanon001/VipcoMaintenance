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

namespace VipcoMaintenance.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ItemController : GenericController<Item>
    {
        // IRepository
        private IRepositoryMachine<Employee> repositoryEmp;
        private IRepositoryMachine<EmployeeGroupMis> repositoryGroupMis;
        private IRepositoryMaintenance<ItemType> repositoryType;
        // Mapper
        private IMapper mapper;

        public ItemController(IRepositoryMaintenance<Item> repo, 
            IRepositoryMachine<Employee> repoEmp,
            IRepositoryMachine<EmployeeGroupMis> repoGroupMis,
            IRepositoryMaintenance<ItemType> repoType,
            IMapper map) : base(repo) {
            // Repository
            this.repositoryEmp = repoEmp;
            this.repositoryGroupMis = repoGroupMis;
            this.repositoryType = repoType;
            // Mapper
            this.mapper = map;
        }

        // GET: api/Item/5
        [HttpGet("GetKeyNumber")]
        public override async Task<IActionResult> Get(int key)
        {
            var HasItem = await this.repository.GetAsynvWithIncludes(key, "ItemId",new List<string> { "Branch", "ItemType" });
            if (HasItem != null)
            {
                var MapItem = this.mapper.Map<Item, ItemViewModel>(HasItem);
                if (!string.IsNullOrEmpty(MapItem.EmpResponsible))
                    MapItem.EmpResposibleString = (await this.repositoryEmp.GetAsync(MapItem.EmpResponsible)).NameThai;
                if (!string.IsNullOrEmpty(MapItem.GroupMis))
                    MapItem.GroupMisString = (await this.repositoryGroupMis.GetAsync(MapItem.GroupMis)).GroupDesc ?? "-";
                return new JsonResult(MapItem, this.DefaultJsonSettings);
            }
            return BadRequest();
        }

        // GET: api/Item/ItemByGroup
        [HttpGet("ItemByGroup")]
        public async Task<IActionResult> ItemByGroup(string key)
        {
            var HasData = await this.repository.GetAllAsQueryable()
                                    .Where(x => x.GroupMis == key)
                                    .AsNoTracking().ToListAsync();
            if (HasData.Any()){
                var listData = new List<ItemViewModel>();
                foreach(var item in HasData){
                    var MapData = this.mapper.Map<Item, ItemViewModel>(item);
                    if (!string.IsNullOrEmpty(MapData.GroupMis))
                        MapData.GroupMisString = (await this.repositoryGroupMis.GetAsync(MapData.GroupMis)).GroupDesc ?? "-";
                    listData.Add(MapData);
                }
                return new JsonResult(listData,this.DefaultJsonSettings);
            }
            return BadRequest();
        }

        [HttpPost("ItemByGroupWithScroll")]
        // POST: api/Item/ItemByGroupWithScroll
        public async Task<IActionResult> GetItemByGroupWithScroll([FromBody] ScrollViewModel Scroll)
        {
            var Message = "";
            try
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
                {
                    if (Scroll.WhereId > 0)
                        QueryData = QueryData.Where(x => x.ItemTypeId == Scroll.WhereId);
                }

                if (!string.IsNullOrEmpty(Scroll.Where))
                    QueryData = QueryData.Where(x => x.Creator == Scroll.Where);

                var QueryData2 = await QueryData.GroupBy(x => x.GroupMis).Select(x => new ItemByGroupViewModel()
                                        {
                                            GroupMis = string.IsNullOrEmpty(x.Key) ? "-" : x.Key,
                                            GroupMisString = string.IsNullOrEmpty(x.Key) ? "ไม่ระบุ" : this.repositoryGroupMis.Get(x.Key).GroupDesc,
                                            ItemCount = x.Count()
                                        }).ToListAsync();

                // Filter
                var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                    : Scroll.Filter.ToLower().Split(null);

                foreach (var keyword in filters)
                {
                    QueryData2 = QueryData2.Where(x => x.GroupMis.ToLower().Contains(keyword) ||
                                                       x.GroupMisString.ToLower().Contains(keyword)).ToList();
                }

                // Order
                switch (Scroll.SortField)
                {
                    case "GroupMisString":
                        if (Scroll.SortOrder == -1)
                            QueryData2 = QueryData2.OrderByDescending(e => e.GroupMis).ToList();
                        else
                            QueryData2 = QueryData2.OrderBy(e => e.GroupMis).ToList();
                        break;

                    case "ItemCount":
                        if (Scroll.SortOrder == -1)
                            QueryData2 = QueryData2.OrderByDescending(e => e.ItemCount).ToList();
                        else
                            QueryData2 = QueryData2.OrderBy(e => e.ItemCount).ToList();
                        break;

                    default:
                        QueryData2 = QueryData2.OrderBy(e => e.GroupMis).ToList();
                        break;
                }
                // Get TotalRow
                Scroll.TotalRow = QueryData2.Count();
                // Skip Take
                QueryData2 = QueryData2.Skip(Scroll.Skip ?? 0).Take(Scroll.Take ?? 50).ToList();
                // var HasData = await QueryData2.AsNoTracking().ToListAsync();
                return new JsonResult(new ScrollDataViewModel<ItemByGroupViewModel>(Scroll, QueryData2), this.DefaultJsonSettings);
            }
            catch(Exception ex)
            {
                Message = $"Has error {ex.ToString()}";
            }

            return BadRequest(new { Error = Message });
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
            {
                if (Scroll.WhereId > 0)
                    QueryData = QueryData.Where(x => x.ItemTypeId == Scroll.WhereId);
            }

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
            foreach (var item in HasData){
                var MapData = this.mapper.Map<Item, ItemViewModel>(item);
                if (!string.IsNullOrEmpty(MapData.GroupMis))
                    MapData.GroupMisString = (await this.repositoryGroupMis.GetAsync(MapData.GroupMis)).GroupDesc ?? "-";
                listData.Add(MapData);
            }
            return new JsonResult(new ScrollDataViewModel<ItemViewModel>(Scroll, listData), this.DefaultJsonSettings);
        }

        [HttpPut("ChangeGroupOfItem")]
        public async Task<IActionResult> ChangeGroupOfItem(string Group,string ByEmp,[FromBody] List<Item> records)
        {
            if (records != null)
            {
                List<int> excluse = new List<int>();
                // Update group
                foreach(var record in records)
                {
                    record.ModifyDate = DateTime.Now;
                    record.Modifyer = ByEmp;
                    record.GroupMis = Group;
                    if (await this.repository.UpdateAsync(record, record.ItemId) == null)
                        excluse.Add(record.ItemId);
                }

                Expression<Func<Item, bool>> match = i => i.GroupMis == Group;
                var dbItems = await this.repository.FindAllAsync(match);
                if (dbItems != null)
                {
                    // Remove group if not pick
                    foreach (var dbItem in dbItems)
                    {
                        if (excluse.Any(x => x == dbItem.ItemId))
                            continue;

                        if (!records.Any(x => x.ItemId == dbItem.ItemId))
                        {
                            // If item of this group don't pick
                            dbItem.ModifyDate = DateTime.Now;
                            dbItem.Modifyer = ByEmp;
                            dbItem.GroupMis = "";

                            await this.repository.UpdateAsync(dbItem, dbItem.ItemId);
                        }
                    }
                }

                return new JsonResult(new ItemByGroupViewModel()
                {
                    GroupMis = Group,
                    ItemCount = records.Count
                }, this.DefaultJsonSettings);
            }
            return BadRequest();
        }
    }
}
