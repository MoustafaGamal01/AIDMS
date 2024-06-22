using AIDMS.DTOs;
using AIDMS.Entities;
using AIDMS.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Google.Cloud.Vision.V1.ProductSearchResults.Types;

namespace AIDMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = ("Admin"))]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            this._roleManager = roleManager;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateRole([FromBody] AddRoleDto role)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result =  await _roleManager.CreateAsync(new IdentityRole(role.Name));

                if (result.Succeeded)
                {
                    return Ok("Role Created Successfully!");
                }
                else return BadRequest(result.Errors);
            }
            return BadRequest("Error Occured while creating a role");
        }
        
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(roles);
        }
    }
}
