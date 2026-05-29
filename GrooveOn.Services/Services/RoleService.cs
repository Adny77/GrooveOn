using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Exceptions;
using GrooveOn.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace GrooveOn.Services.Services
{
    public class RoleService : BaseCRUDService<RoleResponse, BaseSearchObject, Role, RoleUpsertRequest, RoleUpsertRequest>, IRoleService
    {
        public RoleService(GrooveOnDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        protected override IQueryable<Role> ApplyFilter(IQueryable<Role> query, BaseSearchObject search)
        {
            return base.ApplyFilter(query, search);
        }

        protected override async Task BeforeDelete(Role entity)
        {
            var hasUsers = await _context.Set<UserRole>()
                .AnyAsync(x => x.RoleId == entity.Id);

            if (hasUsers)
                throw new UserException(
                    $"Role '{entity.Name}' cannot be deleted because it is assigned to one or more users. " +
                    "Remove the role from all users first.");

            await base.BeforeDelete(entity);
        }
    }
}
