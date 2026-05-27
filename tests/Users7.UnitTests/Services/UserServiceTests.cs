using FluentAssertions;
using FluentValidation;
using Moq;
using Users7.Application.DTOs;
using Users7.Application.Exceptions;
using Users7.Application.Interfaces;
using Users7.Application.Services;
using Users7.Application.Validators;
using Users7.Domain.Entities;
using Users7.UnitTests.Fakes;

namespace Users7.UnitTests.Services;

public sealed class UserServiceTests
{
    private readonly DateOnly today = new(2026, 5, 26);
    private readonly Mock<IUserRepository> userRepository = new();
    private readonly UserService service;

    public UserServiceTests()
    {
        var dateProvider = new FakeDateProvider(today);
        service = new UserService(
            userRepository.Object,
            dateProvider,
            new CreateUserRequestValidator(dateProvider),
            new UpdateUserRequestValidator(dateProvider),
            new UserQueryParametersValidator());
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUser_WhenRequestIsValid()
    {
        var request = new CreateUserRequest(" Maria Silva ", " MARIA@EXAMPLE.COM ", new DateOnly(1990, 5, 20));
        User? createdUser = null;

        userRepository
            .Setup(repository => repository.EmailExistsAsync("maria@example.com", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        userRepository
            .Setup(repository => repository.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => createdUser = user)
            .Returns(Task.CompletedTask);

        var response = await service.CreateAsync(request);

        response.Name.Should().Be("Maria Silva");
        response.Email.Should().Be("maria@example.com");
        response.CreatedAt.Should().Be(today);
        response.UpdatedAt.Should().Be(today);
        createdUser.Should().NotBeNull();
        userRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var request = new CreateUserRequest(string.Empty, "maria@example.com", new DateOnly(1990, 5, 20));

        var act = () => service.CreateAsync(request);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(exception => exception.Errors.Any(error => error.ErrorMessage == "Nome é obrigatório."));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenEmailIsInvalid()
    {
        var request = new CreateUserRequest("Maria Silva", "invalid-email", new DateOnly(1990, 5, 20));

        var act = () => service.CreateAsync(request);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(exception => exception.Errors.Any(error => error.ErrorMessage == "E-mail deve ser válido."));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenBirthDateIsFuture()
    {
        var request = new CreateUserRequest("Maria Silva", "maria@example.com", today.AddDays(1));

        var act = () => service.CreateAsync(request);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(exception => exception.Errors.Any(error => error.ErrorMessage == "Data de nascimento não pode ser futura."));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowConflictException_WhenEmailAlreadyExists()
    {
        var request = new CreateUserRequest("Maria Silva", "maria@example.com", new DateOnly(1990, 5, 20));

        userRepository
            .Setup(repository => repository.EmailExistsAsync("maria@example.com", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => service.CreateAsync(request);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Já existe um usuário cadastrado com este e-mail.");
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        var request = new UpdateUserRequest("Maria Silva", "maria@example.com", new DateOnly(1990, 5, 20));

        userRepository
            .Setup(repository => repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var response = await service.UpdateAsync(1, request);

        response.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser_WhenRequestIsValid()
    {
        var user = new User("Maria Silva", "maria@example.com", new DateOnly(1990, 5, 20), new DateOnly(2026, 1, 1));
        var request = new UpdateUserRequest("Maria Souza", "maria.souza@example.com", new DateOnly(1991, 6, 21));

        userRepository
            .Setup(repository => repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        userRepository
            .Setup(repository => repository.EmailExistsAsync("maria.souza@example.com", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var response = await service.UpdateAsync(1, request);

        response.Should().NotBeNull();
        response!.Name.Should().Be("Maria Souza");
        response.Email.Should().Be("maria.souza@example.com");
        response.BirthDate.Should().Be(new DateOnly(1991, 6, 21));
        response.UpdatedAt.Should().Be(today);
        userRepository.Verify(repository => repository.Update(user), Times.Once);
        userRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowConflictException_WhenEmailBelongsToAnotherUser()
    {
        var user = new User("Maria Silva", "maria@example.com", new DateOnly(1990, 5, 20), today);
        var request = new UpdateUserRequest("Maria Silva", "other@example.com", new DateOnly(1990, 5, 20));

        userRepository
            .Setup(repository => repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        userRepository
            .Setup(repository => repository.EmailExistsAsync("other@example.com", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => service.UpdateAsync(1, request);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Já existe um usuário cadastrado com este e-mail.");
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        userRepository
            .Setup(repository => repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var deleted = await service.DeleteAsync(1);

        deleted.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteUser_WhenUserExists()
    {
        var user = new User("Maria Silva", "maria@example.com", new DateOnly(1990, 5, 20), today);

        userRepository
            .Setup(repository => repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var deleted = await service.DeleteAsync(1);

        deleted.Should().BeTrue();
        userRepository.Verify(repository => repository.Delete(user), Times.Once);
        userRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedResult()
    {
        var parameters = new UserQueryParameters { PageNumber = 1, PageSize = 10 };
        var users = new[]
        {
            new User("Maria Silva", "maria@example.com", new DateOnly(1990, 5, 20), today)
        };

        userRepository
            .Setup(repository => repository.GetPagedAsync(parameters, It.IsAny<CancellationToken>()))
            .ReturnsAsync((users, 1));

        var result = await service.GetAllAsync(parameters);

        result.Items.Should().HaveCount(1);
        result.TotalItems.Should().Be(1);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
    }
}
