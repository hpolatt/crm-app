using MediatR;
using Microsoft.EntityFrameworkCore;
using PktApp.Core.DTOs.Users;
using PktApp.Infrastructure.Data;

namespace PktApp.Application.Users.Queries;

public class GetUserByIdQuery : IRequest<UserDto?>
{
    public Guid Id { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly ApplicationDbContext _context;

    public GetUserByIdQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Where(u => u.Id == request.Id && !u.IsDeleted)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Phone = u.Phone,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return user;
    }
}
