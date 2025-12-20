using MediatR;
using Microsoft.EntityFrameworkCore;
using PktApp.Core.DTOs.Users;
using PktApp.Domain.Entities;
using PktApp.Infrastructure.Data;

namespace PktApp.Application.Users.Commands;

public class CreateUserCommand : IRequest<UserDto>
{
    public CreateUserRequest Request { get; set; } = null!;
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly ApplicationDbContext _context;

    public CreateUserCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        // Check if user exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        // Assign roles
        foreach (var roleId in request.RoleIds)
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

        // Get role names
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
