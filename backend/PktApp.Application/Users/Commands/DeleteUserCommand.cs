using MediatR;
using Microsoft.EntityFrameworkCore;
using PktApp.Infrastructure.Data;

namespace PktApp.Application.Users.Commands;

public class DeleteUserCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly ApplicationDbContext _context;

    public DeleteUserCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);

        if (user == null)
        {
            return false;
        }

        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
