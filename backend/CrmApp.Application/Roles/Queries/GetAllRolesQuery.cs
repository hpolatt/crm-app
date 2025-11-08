using MediatR;
using Microsoft.EntityFrameworkCore;
using CrmApp.Core.DTOs.Roles;
using CrmApp.Infrastructure.Data;

namespace CrmApp.Application.Roles.Queries;

public class GetAllRolesQuery : IRequest<List<RoleDto>>
{
}

public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleDto>>
{
    private readonly ApplicationDbContext _context;

    public GetAllRolesQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _context.Roles
            .Where(r => !r.IsDeleted)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return roles;
    }
}
