using Users7.Application.DTOs;
using Users7.Domain.Entities;

namespace Users7.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdReadOnlyAsync(int id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyCollection<User> Items, int TotalItems)> GetPagedAsync(UserQueryParameters parameters, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, int? exceptUserId = null, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    void Update(User user);
    void Delete(User user);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
