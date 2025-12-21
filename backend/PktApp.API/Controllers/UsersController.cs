using Microsoft.AspNetCore.Mvc;
using PktApp.Core.DTOs.Common;
using PktApp.Core.DTOs.Users;
using PktApp.Core.Interfaces;
using PktApp.Domain.Entities;

namespace PktApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseController
{
    private readonly IRepository<User> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UsersController(IRepository<User> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAll()
    {
        // Sadece Admin erişebilir
        if (!IsAdmin())
        {
            return ForbiddenResponse<IEnumerable<UserDto>>();
        }

        var users = await _repository.GetAllAsync();
        
        var dtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Role = u.Role,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt
        });

        return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(dtos));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetById(Guid id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
            return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));

        var dto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };

        return Ok(ApiResponse<UserDto>.SuccessResponse(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] CreateUserDto createDto)
    {
        // Sadece Admin kullanıcı ekleyebilir
        if (!IsAdmin())
        {
            return ForbiddenResponse<UserDto>();
        }

        // Check if username exists
        var users = await _repository.GetAllAsync();
        if (users.Any(u => u.Username == createDto.Username))
            return BadRequest(ApiResponse<UserDto>.ErrorResponse("Username already exists"));

        var user = new User
        {
            Username = createDto.Username,
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Email = createDto.Email,
            Role = createDto.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Hash password using BCrypt with prefix and CreatedAt
        var passwordWithKey = "huseyinpolat_" + user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss") + createDto.Password;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordWithKey);

        await _repository.AddAsync(user);
        await _unitOfWork.CommitAsync();

        var dto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<UserDto>.SuccessResponse(dto));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Update(Guid id, [FromBody] UpdateUserDto updateDto)
    {
        // Sadece Admin düzenleyebilir
        if (!IsAdmin())
        {
            return ForbiddenResponse<UserDto>();
        }

        var user = await _repository.GetByIdAsync(id);
        if (user == null)
            return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));

        user.FirstName = updateDto.FirstName;
        user.LastName = updateDto.LastName;
        user.Email = updateDto.Email;
        user.Role = updateDto.Role;
        user.IsActive = updateDto.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        _repository.Update(user);
        await _unitOfWork.CommitAsync();

        var dto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };

        return Ok(ApiResponse<UserDto>.SuccessResponse(dto));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        // Sadece Admin silebilir
        if (!IsAdmin())
        {
            return ForbiddenResponse<bool>();
        }

        var user = await _repository.GetByIdAsync(id);
        if (user == null)
            return NotFound(ApiResponse<bool>.ErrorResponse("User not found"));

        // Prevent deleting the last admin
        var users = await _repository.GetAllAsync();
        var adminCount = users.Count(u => u.Role == Domain.Enums.UserRole.Admin && u.IsActive);
        if (user.Role == Domain.Enums.UserRole.Admin && adminCount <= 1)
            return BadRequest(ApiResponse<bool>.ErrorResponse("Cannot delete the last admin user"));

        _repository.Remove(user);
        await _unitOfWork.CommitAsync();

        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }

    [HttpPost("{id}/change-password")]
    public async Task<ActionResult<ApiResponse<bool>>> ChangePassword(Guid id, [FromBody] ChangePasswordDto dto)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
            return NotFound(ApiResponse<bool>.ErrorResponse("User not found"));

        // Foreman kendi şifresini değiştiremez
        if (IsForeman())
        {
            return ForbiddenResponse<bool>("Şifre değiştirme yetkiniz yok");
        }

        // Admin herkesin şifresini mevcut şifre olmadan değiştirebilir
        // Diğer kullanıcılar sadece kendi şifrelerini mevcut şifreyi girerek değiştirebilir
        if (!IsAdmin())
        {
            // Admin değilse mevcut şifre gerekli
            if (string.IsNullOrEmpty(dto.CurrentPassword))
                return BadRequest(ApiResponse<bool>.ErrorResponse("Mevcut şifre gereklidir"));

            // Verify current password with prefix and CreatedAt
            var currentPasswordWithKey = "huseyinpolat_" + user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss") + dto.CurrentPassword;
            if (!BCrypt.Net.BCrypt.Verify(currentPasswordWithKey, user.PasswordHash))
                return BadRequest(ApiResponse<bool>.ErrorResponse("Mevcut şifre yanlış"));
        }

        // Hash new password with prefix and CreatedAt
        var newPasswordWithKey = "huseyinpolat_" + user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss") + dto.NewPassword;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPasswordWithKey);
        user.UpdatedAt = DateTime.UtcNow;

        _repository.Update(user);
        await _unitOfWork.CommitAsync();

        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }
}
