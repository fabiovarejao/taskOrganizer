using Microsoft.AspNetCore.Mvc;
using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Interfaces.Repositories;

namespace TaskOrganizer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectUserController : ControllerBase
{
    private readonly IProjectUserRepository _projectUserRepository;

    public ProjectUserController(IProjectUserRepository projectUserRepository)
    {
        _projectUserRepository = projectUserRepository;
    }

    
    [HttpPost]
    [Route("/projects/{projectId}/users")]
    public async Task<IActionResult> AddUserToProject(Guid projectId, [FromQuery] Guid userId)
    {
        if (await _projectUserRepository.ExistsAsync(projectId, userId))
        {
            return BadRequest(new 
            { 
                success = false, 
                message = "Usuário já está associado a este projeto" 
            });
        }

        var projectUser = new ProjectUser(projectId, userId);
        await _projectUserRepository.AddAsync(projectUser);
        
        return Ok(new 
        { 
            success = true, 
            message = "Usuário adicionado ao projeto com sucesso", 
            projectId, 
            userId 
        });
    }

    
    [HttpDelete]
    [Route("/projects/{projectId}/users/{userId}")]
    public async Task<IActionResult> RemoveUserFromProject(Guid projectId, Guid userId)
    {
        var projectUser = await _projectUserRepository.GetByIdAsync(projectId, userId);
        
        if (projectUser is null)
        {
            return NotFound(new 
            { 
                success = false, 
                message = "Associação de usuário não encontrada" 
            });
        }

        await _projectUserRepository.DeleteAsync(projectUser);
        
        return Ok(new 
        { 
            success = true, 
            message = "Usuário removido do projeto com sucesso" 
        });
    }

    
    [HttpGet]
    [Route("/users/{userId}/projects")]
    public async Task<IActionResult> GetProjectsByUser(Guid userId)
    {
        var projectUsers = await _projectUserRepository.GetByUserIdAsync(userId);
        
        var projectsData = projectUsers.Select(pu => new
        {
            projectId = pu.ProjectId,
            projectName = pu.Project?.Name,
            projectDescription = pu.Project?.Description
        });
        
        return Ok(new 
        { 
            success = true, 
            data = projectsData 
        });
    }
}
