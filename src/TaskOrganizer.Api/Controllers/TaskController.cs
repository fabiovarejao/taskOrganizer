using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using TaskOrganizer.Application.Dtos;
using TaskOrganizer.Application.Interfaces;
using TaskOrganizer.Application.Validators;
using TaskOrganizer.Domain.Enums;
using TaskOrganizer.Domain.Exceptions;
using TaskOrganizer.Domain.Interfaces.Repositories;
using TaskOrganizer.Infrastructure.Context;
using TaskStatus = TaskOrganizer.Domain.Enums.TaskStatus;

namespace TaskOrganizer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ITaskRepository _tasks;
    private readonly IMapper _mapper;
    private readonly AppDbContext _dbContext;

    public TaskController(
        ITaskService taskService, 
        ITaskRepository tasks,
        IMapper mapper,
        AppDbContext dbContext)
    {
        _taskService = taskService;
        _tasks = tasks;
        _mapper = mapper;
        _dbContext = dbContext;
    }

    
    [HttpGet]
    [Route("/projects/{projectId}/tasks")]
    public async Task<IActionResult> GetProjectTasks(Guid projectId)
    {
        var tasks = await _taskService.GetTasksForProjectAsync(projectId);
        return Ok(tasks);
    }

    
    [HttpGet]
    [Route("/tasks/{taskId}")]
    public async Task<IActionResult> GetTask(Guid taskId)
    {
        var task = await _tasks.GetByIdAsync(taskId);
        
        if (task is null)
            return NotFound();

        var taskDto = _mapper.Map<TaskDto>(task);
        return Ok(taskDto);
    }

    
    [HttpPost]
    [Route("/projects/{projectId}/tasks")]
    public async Task<IActionResult> CreateTask(Guid projectId, [FromBody] CreateTaskDto dto)
    {
        var validator = new CreateTaskDtoValidator();
        var validation = await validator.ValidateAsync(dto);
        
        if (!validation.IsValid)
        {
            return BadRequest(new { errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
        }

        try
        {
            var taskDto = await _taskService.CreateTaskAsync(projectId, dto);
            
            if (taskDto is null)
                return NotFound();

            return CreatedAtAction(nameof(GetTaskHistory), new { taskId = taskDto.Id }, taskDto);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    
    [HttpPut]
    [Route("/tasks/{taskId}")]
    public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] UpdateTaskDto dto, [FromQuery] Guid userId)
    {
        var validator = new UpdateTaskDtoValidator();
        var validation = await validator.ValidateAsync(dto);
        
        if (!validation.IsValid)
        {
            return BadRequest(new { errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
        }

        try
        {
            var taskDto = await _taskService.UpdateTaskAsync(taskId, dto, userId);
            
            if (taskDto is null)
                return NotFound();

            return Ok(taskDto);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    
    [HttpPut]
    [Route("/tasks/{taskId}/status")]
    public async Task<IActionResult> UpdateTaskStatus(Guid taskId, [FromQuery] TaskStatus newStatus, [FromQuery] Guid userId)
    {
        var taskDto = await _taskService.UpdateStatusAsync(taskId, newStatus, userId);
        
        if (taskDto is null)
            return NotFound();

        return Ok(taskDto);
    }

    
    [HttpPost]
    [Route("/tasks/{taskId}/comments")]
    public async Task<IActionResult> AddComment(Guid taskId, [FromBody] CreateCommentDto dto)
    {
        var validator = new CreateCommentDtoValidator();
        var validation = await validator.ValidateAsync(dto);
        
        if (!validation.IsValid)
        {
            return BadRequest(new { errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
        }

        try
        {
            await _taskService.AddCommentAsync(taskId, dto.Message, dto.UserId);
            return Created($"/api/task/{taskId}/comments", new { message = dto.Message, userId = dto.UserId });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    
    [HttpGet]
    [Route("/tasks/{taskId}/history")]
    public async Task<IActionResult> GetTaskHistory(Guid taskId)
    {
        var histories = await _dbContext.TaskHistories
            .Where(h => h.TaskItemId == taskId)
            .ToListAsync();
        
        var result = histories.Select(h => new 
        { 
            h.Id, 
            h.Field, 
            h.OldValue, 
            h.NewValue, 
            h.ChangedAt, 
            h.ChangedByUserId 
        });
        
        return Ok(result);
    }

    
    [HttpDelete]
    [Route("/tasks/{taskId}")]
    public async Task<IActionResult> DeleteTask(Guid taskId)
    {
        try
        {
            await _taskService.DeleteTaskAsync(taskId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
