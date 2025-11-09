using Microsoft.AspNetCore.Mvc;
using TaskOrganizer.Application.Interfaces;

namespace TaskOrganizer.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    
    [HttpGet]
    [Route("/users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    
    [HttpGet]
    [Route("/users/{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }
}
