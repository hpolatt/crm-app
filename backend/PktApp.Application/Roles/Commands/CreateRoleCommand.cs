using MediatR;
using Microsoft.EntityFrameworkCore;
using PktApp.Core.DTOs.Roles;
using PktApp.Domain.Entities;
using PktApp.Infrastructure.Data;

namespace PktApp.Application.Roles.Commands;

public class CreateRoleCommand : IRequest<RoleDto>
{
    public CreateRoleRequest Request { get; set; } = null!;
}

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
{
    private readonly ApplicationDbContext _context;

    public CreateRoleCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RoleDto> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var existingRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == command.Request.Name, cancellationToken);

        if (existingRole != null)
        {
            throw new InvalidOperationException("Role with this name already exists");
        }

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = command.Request.Name,
            Description = command.Request.Description,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt
        };
    }
}
