using Microsoft.EntityFrameworkCore;
using Users7.Application.DTOs;
using Users7.Application.Interfaces;
using Users7.Domain.Entities;
using Users7.Infrastructure.Persistence;

namespace Users7.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly UsersDbContext context;

    public UserRepository(UsersDbContext context)
    {
        this.context = context;
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return context.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public Task<User?> GetByIdReadOnlyAsync(int id, CancellationToken cancellationToken = default)
    {
        return context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyCollection<User> Items, int TotalItems)> GetPagedAsync(
        UserQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(parameters.Name))
        {
            query = query.Where(user => EF.Functions.ILike(user.Name, $"%{parameters.Name.Trim()}%"));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Email))
        {
            query = query.Where(user => EF.Functions.ILike(user.Email, $"%{parameters.Email.Trim()}%"));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var orderedQuery = ApplySorting(query, parameters.SortBy, parameters.SortDirection);
        var items = await orderedQuery
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalItems);
    }

    public Task<bool> EmailExistsAsync(string email, int? exceptUserId = null, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return context.Users.AnyAsync(
            user => user.Email == normalizedEmail && (!exceptUserId.HasValue || user.Id != exceptUserId.Value),
            cancellationToken);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        return context.Users.AddAsync(user, cancellationToken).AsTask();
    }

    public void Update(User user)
    {
        context.Users.Update(user);
    }

    public void Delete(User user)
    {
        context.Users.Remove(user);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<User> ApplySorting(IQueryable<User> query, string sortBy, string sortDirection)
    {
        var descending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);
        var normalizedSortBy = sortBy.Trim().ToLowerInvariant();

        return normalizedSortBy switch
        {
            "name" => descending ? query.OrderByDescending(user => user.Name) : query.OrderBy(user => user.Name),
            "email" => descending ? query.OrderByDescending(user => user.Email) : query.OrderBy(user => user.Email),
            "birthdate" => descending ? query.OrderByDescending(user => user.BirthDate) : query.OrderBy(user => user.BirthDate),
            "createdat" => descending ? query.OrderByDescending(user => user.CreatedAt) : query.OrderBy(user => user.CreatedAt),
            "updatedat" => descending ? query.OrderByDescending(user => user.UpdatedAt) : query.OrderBy(user => user.UpdatedAt),
            _ => descending ? query.OrderByDescending(user => user.Id) : query.OrderBy(user => user.Id)
        };
    }
}
