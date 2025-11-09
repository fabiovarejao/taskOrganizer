using Microsoft.AspNetCore.Mvc;
using TaskOrganizer.Application.Dtos;
using TaskOrganizer.Application.Interfaces;

namespace TaskOrganizer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportsService _reportsService;

    public ReportsController(IReportsService reportsService)
    {
        _reportsService = reportsService;
    }

    
    [HttpGet]
    [Route("/reports/completed-per-user")]
    public async Task<IActionResult> GetCompletedPerUser([FromQuery] Guid userId, [FromQuery] int? days)
    {
        var daysToAnalyze = days ?? 30;
        var userRepo = HttpContext.RequestServices.GetRequiredService<TaskOrganizer.Domain.Interfaces.Repositories.IUserRepository>();
        var user = await userRepo.GetByIdAsync(userId);
        if (user is null)
            return BadRequest(new { error = "userId inválido" });
        if (user.Position != TaskOrganizer.Domain.Enums.Position.Manager)
            return Forbid();

    var results = await _reportsService.GetCompletedTasksPerUserAsync(daysToAnalyze);
        
        var data = results.Select(r => new CompletedPerUserDto(
            r.UserId, 
            r.UserName, 
            r.CompletedCount, 
            r.AveragePerDay
        ));
        
        return Ok(data);
    }

    
    [HttpGet]
    [Route("/reports/manager/{userId}")]
    public async Task<IActionResult> GetManagerReport(Guid userId)
    {
        try
        {
            var userRepo = HttpContext.RequestServices.GetRequiredService<TaskOrganizer.Domain.Interfaces.Repositories.IUserRepository>();
            var user = await userRepo.GetByIdAsync(userId);
            if (user is null)
                return BadRequest(new { error = "userId Inválido" });
            if (user.Position != TaskOrganizer.Domain.Enums.Position.Manager)
                return StatusCode(403, new { error = "Somente gerentes podem acessar este relatório" });

            var results = await _reportsService.GetCompletedTasksPerUserAsync(30);
            
            var averageCompletedTasksLast30Days = results.Any() 
                ? results.Average(r => r.AveragePerDay) 
                : 0;

            return Ok(new
            {
                userId = userId,
                reportDate = DateTime.UtcNow,
                periodDays = 30,
                averageCompletedTasksLast30Days = averageCompletedTasksLast30Days,
                userDetails = results.Select(r => new
                {
                    userId = r.UserId,
                    userName = r.UserName,
                    completedCount = r.CompletedCount,
                    averagePerDay = r.AveragePerDay
                })
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
