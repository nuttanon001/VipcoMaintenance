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
using VipcoMaintenance.Models.Maintenances;


namespace VipcoMaintenance.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PermissionController : GenericController<Permission>
    {
        public PermissionController(IRepositoryMaintenance<Permission> repo) : base(repo) { }

        [HttpGet("GetPermission/{UserId}")]
        public async Task<IActionResult> GetPermission(int UserId)
        {
            if (UserId < 1)
                return BadRequest();

            var HasPermission = await this.repository.GetAllAsQueryable().FirstOrDefaultAsync(x => x.UserId == UserId);
            if (HasPermission == null)
                return BadRequest();

            return new JsonResult(HasPermission, this.DefaultJsonSettings);
        }
    }
}
