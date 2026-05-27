using Users7.Application.DTOs;

namespace Users7.Application.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserResponse>> GetAllAsync(UserQueryParameters parameters, CancellationToken cancellationToken = default);
    Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse?> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
