using Microsoft.AspNetCore.Mvc;
using Users7.Api.Constants;
using Users7.Api.Responses;
using Users7.Application.DTOs;
using Users7.Application.Interfaces;

namespace Users7.Api.Controllers;

[ApiController]
[Route("users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService userService;

    public UsersController(IUserService userService)
    {
        this.userService = userService;
    }

    /// <summary>
    /// Lista usuários com suporte a paginação, filtros e ordenação.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<UserResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<UserResponse>>>> GetAllAsync(
        [FromQuery] UserQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var result = await userService.GetAllAsync(parameters, cancellationToken);
        var response = new ApiResponse<IReadOnlyCollection<UserResponse>>(
            ApiCodes.UsersListed,
            ApiMessages.UsersListed,
            result.Items,
            Meta: new
            {
                result.PageNumber,
                result.PageSize,
                result.TotalItems,
                result.TotalPages
            });

        return Ok(response);
    }

    /// <summary>
    /// Busca um usuário pelo identificador.
    /// </summary>
    [HttpGet("{id:int}", Name = "GetUserById")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var user = await userService.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return NotFound(new ApiResponse<object>(ApiCodes.UserNotFound, ApiMessages.UserNotFound, TraceId: HttpContext.TraceIdentifier));
        }

        return Ok(new ApiResponse<UserResponse>(ApiCodes.UserFound, ApiMessages.UserFound, user));
    }

    /// <summary>
    /// Cria um usuário.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> CreateAsync(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userService.CreateAsync(request, cancellationToken);
        var response = new ApiResponse<UserResponse>(ApiCodes.UserCreated, ApiMessages.UserCreated, user);

        return CreatedAtRoute("GetUserById", new { id = user.Id }, response);
    }

    /// <summary>
    /// Atualiza um usuário pelo identificador.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateAsync(
        int id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userService.UpdateAsync(id, request, cancellationToken);
        if (user is null)
        {
            return NotFound(new ApiResponse<object>(ApiCodes.UserNotFound, ApiMessages.UserNotFound, TraceId: HttpContext.TraceIdentifier));
        }

        return Ok(new ApiResponse<UserResponse>(ApiCodes.UserUpdated, ApiMessages.UserUpdated, user));
    }

    /// <summary>
    /// Remove um usuário pelo identificador.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var deleted = await userService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(new ApiResponse<object>(ApiCodes.UserNotFound, ApiMessages.UserNotFound, TraceId: HttpContext.TraceIdentifier));
        }

        return Ok(new ApiResponse<object>(ApiCodes.UserDeleted, ApiMessages.UserDeleted));
    }
}
