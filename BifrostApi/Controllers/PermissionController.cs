using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BifrostApi.Models;
using BifrostApi.Models.Attributes;

namespace BifrostApi.Controllers
{
    [ApiController]
    public class PermissionController : Controller
    {
        // Be aware there are two routes in this controller,
        // one is for generating permissions in the database.
        // The other one is for adding permissions to usergroups

        private readonly bifrostContext _context;

        public PermissionController(bifrostContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Property")]
        [RequiredPermission("PropertyAdmin")]
        public async Task<ActionResult> PropertyCreate(string name)
        {

            PermissionProperty property = new PermissionProperty
            {
                Name = name,
                Deleted = false
            };

            int existingproperties = _context.PermissionProperties.Where(x => x.Name == name).Count();

            if (existingproperties > 0)
                return BadRequest("Permission already exists");

            _context.PermissionProperties.Add(property);
            await _context.SaveChangesAsync();

            return Ok(property);
        }

        [HttpGet]
        [Route("Property")]
        public ActionResult PropertyGet(string name)
        {
            // TODO: Test if an empty response returns an empty list;

            List<PermissionProperty> properties = _context.PermissionProperties.Where(x => x.Name == name).ToList();

            return Ok(properties);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Route("Property")]
        public async Task<ActionResult> PropertyDelete(string name)
        {
            // TODO: Permissions to permanently delete permissions (Superuser)

            var properties = _context.PermissionProperties.Where(x => x.Name == name);

            _context.PermissionProperties.RemoveRange(properties);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch]
        [ValidateAntiForgeryToken]
        [Route("Property")]
        public async Task<ActionResult> PropertyPatch(Guid uid, string name)
        {
            List<PermissionProperty> properties = _context.PermissionProperties.Where(x => x.Name == name).ToList();

            if (properties.Count == 0)
                return BadRequest("No permission found");

            if (properties.Count > 1)
                return BadRequest("multiple entries found");

            PermissionProperty property = properties.FirstOrDefault();
            property.Name = name;

            await _context.SaveChangesAsync();

            return Ok(property);
        }

        [HttpGet]
        [Route("Group")]
        [RequireHierarchy("groupUid", false, RequireHierarchyAttribute.HierarchySearchType.Usergroup)]
        public ActionResult GroupGet(Guid groupUid, string name)
        {
            List<GroupPermission> permissions = _context.GroupPermissions.Where(x => x.GroupUid == groupUid).ToList();

            // TODO: Check if an empty list is returned in response

            // early exit when nothing was found
            if (permissions.Count == 0)
                return Ok();

            List<GroupPermission> foundpermission = permissions.Where(x => x.GroupNavigation.Name == name).ToList();

            if (foundpermission.Count == 0)
            {
                return BadRequest("Group does not have permission");
            }

            return Ok(foundpermission.FirstOrDefault());
        }

        [HttpGet]
        [Route("Group/CheckPermission")]
        [RequireHierarchy("groupUid", false, RequireHierarchyAttribute.HierarchySearchType.Usergroup)]
        public ActionResult CheckPermission(Guid groupUid, string name)
        {
            List<GroupPermission> permissions = _context.GroupPermissions.Where(x => x.GroupUid == groupUid).ToList();

            // TODO: Check if an empty list is returned in response

            // early exit when nothing was found
            if (permissions.Count == 0)
                return Ok(false);

            List<GroupPermission> foundpermission = permissions.Where(x => x.GroupNavigation.Name == name).ToList();

            if (foundpermission.Count == 0)
            {
                return Ok(false);
            }

            return Ok(true);
        }

        [HttpPost]
        [Route("Group")]
        [RequireHierarchy("groupUid", false, RequireHierarchyAttribute.HierarchySearchType.Usergroup)]
        public async Task<ActionResult> GroupCreate(Guid groupUid, Guid permissionUid)
        {

            GroupPermission permission = new GroupPermission
            {
                GroupUid = groupUid,
                PermissionPropertyUid = permissionUid
            };

            _context.GroupPermissions.Add(permission);
            await _context.SaveChangesAsync();

            return Ok(permission);
        }

        [HttpDelete]
        [Route("Group")]
        [RequireHierarchy("groupUid", false, RequireHierarchyAttribute.HierarchySearchType.Usergroup)]
        public async Task<ActionResult> GroupDelete(Guid groupUid, Guid permissionUid)
        {

            List<GroupPermission> permission = _context.GroupPermissions.Where(x => x.GroupUid == groupUid && x.PermissionPropertyUid == permissionUid).ToList();

            _context.GroupPermissions.RemoveRange(permission);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
