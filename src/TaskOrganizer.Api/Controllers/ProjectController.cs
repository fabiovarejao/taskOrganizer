using Microsoft.AspNetCore.Mvc;
using TaskOrganizer.Application.Dtos;
using TaskOrganizer.Application.Interfaces;
using TaskOrganizer.Application.Validators;
using TaskOrganizer.Domain.Exceptions;

namespace TaskOrganizer.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }
    
    [HttpGet]
    [Route("/projects")]
    public async Task<IActionResult> GetProjects([FromQuery] Guid userId)
    {
        var projects = await _projectService.GetByUserAsync(userId);
        return Ok(projects);
    }

    
    [HttpPost]
    [Route("/projects")]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
    {
        var validator = new CreateProjectDtoValidator();
        var validation = await validator.ValidateAsync(dto);
        
        if (!validation.IsValid)
        {
            return BadRequest(new { errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
        }

        var result = await _projectService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetProjects), new { userId = result.UserId }, result);
    }

    
    [HttpDelete]
    [Route("/projects/{projectId}")]
    public async Task<IActionResult> DeleteProject(Guid projectId)
    {
        try
        {
            await _projectService.DeleteAsync(projectId);
            return NoContent();
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    
    [HttpGet]
    [Route("/projects/{projectId}")]
    public async Task<IActionResult> GetProject(Guid projectId)
    {
        var project = await _projectService.GetByIdAsync(projectId);
        return project is null ? NotFound() : Ok(project);
    }

    
    [HttpPut]
    [Route("/projects/{projectId}")]
    public async Task<IActionResult> UpdateProject(Guid projectId, [FromBody] UpdateProjectDto dto)
    {
        var validator = new UpdateProjectDtoValidator();
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(new { errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
        }

        var updated = await _projectService.UpdateAsync(projectId, dto);
        return updated is null ? NotFound() : Ok(updated);
    }
}
