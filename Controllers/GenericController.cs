﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using VipcoMaintenance.Services;

namespace VipcoMaintenance.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public abstract class GenericController<Entity> : Controller where Entity : class
    {
        private IRepositoryMaintenance<Entity> repository;
        private JsonSerializerSettings DefaultJsonSettings =>
            new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

        public GenericController(IRepositoryMaintenance<Entity> repo)
        {
            this.repository = repo;
        }

        // GET: api/controller
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return new JsonResult(await this.repository.GetAllAsync(), this.DefaultJsonSettings);
        }

        // GET: api/controller/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return new JsonResult(await this.repository.GetAsync(id),this.DefaultJsonSettings);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Entity record)
        {
            // Set date for CrateDate Entity
            if (record.GetType().GetProperty("CreateDate") != null)
                record.GetType().GetProperty("CreateDate").SetValue(record, DateTime.Now);

            if (await this.repository.AddAsync(record) == null)
                return BadRequest();

            return new JsonResult(record, this.DefaultJsonSettings);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Entity record)
        {
            if (id < 1)
                return BadRequest();
            // Set date for CrateDate Entity
            if (record.GetType().GetProperty("ModifyDate") != null)
                record.GetType().GetProperty("ModifyDate").SetValue(record, DateTime.Now);

            if (await this.repository.UpdateAsync(record,id) != null)
                return BadRequest();

            return new JsonResult(record, this.DefaultJsonSettings);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await this.repository.DeleteAsync(id) == 0)
                return BadRequest();

            return NoContent();
        }
    }
}