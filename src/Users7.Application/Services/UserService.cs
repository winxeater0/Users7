using FluentValidation;
using Users7.Application.DTOs;
using Users7.Application.Exceptions;
using Users7.Application.Interfaces;
using Users7.Domain.Entities;

namespace Users7.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository userRepository;
    private readonly IDateProvider dateProvider;
    private readonly IValidator<CreateUserRequest> createValidator;
    private readonly IValidator<UpdateUserRequest> updateValidator;
    private readonly IValidator<UserQueryParameters> queryValidator;

    public UserService(
        IUserRepository userRepository,
        IDateProvider dateProvider,
        IValidator<CreateUserRequest> createValidator,
        IValidator<UpdateUserRequest> updateValidator,
        IValidator<UserQueryParameters> queryValidator)
    {
        this.userRepository = userRepository;
        this.dateProvider = dateProvider;
        this.createValidator = createValidator;
        this.updateValidator = updateValidator;
        this.queryValidator = queryValidator;
    }

    public async Task<PagedResult<UserResponse>> GetAllAsync(UserQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        await queryValidator.ValidateAndThrowAsync(parameters, cancellationToken);

        var (items, totalItems) = await userRepository.GetPagedAsync(parameters, cancellationToken);
        var responses = items.Select(MapToResponse).ToArray();

        return new PagedResult<UserResponse>(responses, parameters.PageNumber, parameters.PageSize, totalItems);
    }

    public async Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdReadOnlyAsync(id, cancellationToken);
        return user is null ? null : MapToResponse(user);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        await createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var normalizedEmail = NormalizeEmail(request.Email);
        if (await userRepository.EmailExistsAsync(normalizedEmail, cancellationToken: cancellationToken))
        {
            throw new ConflictException("Já existe um usuário cadastrado com este e-mail.");
        }

        var today = dateProvider.Today;
        var user = new User(request.Name, normalizedEmail, request.BirthDate, today);

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(user);
    }

    public async Task<UserResponse?> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        await updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return null;
        }

        var normalizedEmail = NormalizeEmail(request.Email);
        if (await userRepository.EmailExistsAsync(normalizedEmail, id, cancellationToken))
        {
            throw new ConflictException("Já existe um usuário cadastrado com este e-mail.");
        }

        user.Update(request.Name, normalizedEmail, request.BirthDate, dateProvider.Today);
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(user);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return false;
        }

        userRepository.Delete(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse(user.Id, user.Name, user.Email, user.BirthDate, user.CreatedAt, user.UpdatedAt);
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}
