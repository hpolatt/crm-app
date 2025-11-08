using MediatR;
using Microsoft.EntityFrameworkCore;
using CrmApp.Core.DTOs.Users;
using CrmApp.Domain.Entities;
using CrmApp.Infrastructure.Data;

namespace CrmApp.Application.Users.Commands;

public class UpdateUserCommand : IRequest<UserDto>
{
    public Guid Id { get; set; }
    public UpdateUserRequest Request { get; set; } = null!;
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly ApplicationDbContext _context;

    public UpdateUserCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == command.Id && !u.IsDeleted, cancellationToken);

        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        user.FirstName = command.Request.FirstName;
        user.LastName = command.Request.LastName;
        user.Phone = command.Request.Phone;
        user.IsActive = command.Request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        // Update roles
        var existingRoles = await _context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .ToListAsync(cancellationToken);

        _context.UserRoles.RemoveRange(existingRoles);

        foreach (var roleId in command.Request.RoleIds)
        {
            var userRole = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RoleId = roleId,
                CreatedAt = DateTime.UtcNow
            };
            _context.UserRoles.Add(userRole);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var roles = await _context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Role.Name)
            .ToListAsync(cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            Roles = roles
        };
    }
}
