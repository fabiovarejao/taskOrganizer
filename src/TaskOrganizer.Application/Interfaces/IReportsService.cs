using TaskOrganizer.Application.Dtos;

namespace TaskOrganizer.Application.Interfaces;

public interface IReportsService
{
    Task<List<CompletedPerUserResult>> GetCompletedTasksPerUserAsync(int days = 30);
}
