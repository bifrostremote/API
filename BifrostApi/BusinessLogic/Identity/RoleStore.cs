using BifrostApi.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BifrostApi.BusinessLogic.Identity
{
    public class RoleStore : IRoleStore<PermissionProperty>
    {

        private readonly bifrostContext _context;
        public RoleStore(bifrostContext context)
        {
            _context = context;
        }

        public Task<IdentityResult> CreateAsync(PermissionProperty role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(PermissionProperty role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<PermissionProperty> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            Guid translatedId = Guid.Parse(roleId);

            PermissionProperty property = _context.PermissionProperties.Where(x => x.Id == translatedId).FirstOrDefault();

            return property;
        }

        public Task<PermissionProperty> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(PermissionProperty role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetRoleIdAsync(PermissionProperty role, CancellationToken cancellationToken)
        {
            PermissionProperty property = _context.PermissionProperties.Find(role);

            return property.Id.ToString();
        }

        public Task<string> GetRoleNameAsync(PermissionProperty role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(PermissionProperty role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(PermissionProperty role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> UpdateAsync(PermissionProperty role, CancellationToken cancellationToken)
        {
            PermissionProperty foundProperty = _context.PermissionProperties.Find(role);

            if (foundProperty == null)
                return IdentityResult.Failed(new IdentityError { Description = "Role was not found" });

            foundProperty = role;

            await _context.SaveChangesAsync();

            return IdentityResult.Success;
        }
    }
}
