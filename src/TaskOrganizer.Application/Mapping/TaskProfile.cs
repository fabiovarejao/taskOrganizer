using AutoMapper;
using TaskOrganizer.Application.Dtos;
using TaskOrganizer.Domain.Entities;

namespace TaskOrganizer.Application.Mapping;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<TaskItem, TaskDto>()
            .ConstructUsing(src => new TaskDto(
                src.Id,
                src.Title,
                src.Description,
                src.DueDate,
                src.Status,
                src.Priority,
                src.ProjectId,
                src.ResponsibleUserId,
                src.History.Select(h => new TaskHistoryDto(
                    h.Id,
                    h.TaskItemId,
                    h.Field,
                    h.OldValue,
                    h.NewValue,
                    h.ChangedAt,
                    h.ChangedByUserId)).ToList(),
                src.Comments.Select(c => new TaskCommentDto(
                    c.Id,
                    c.TaskItemId,
                    c.Message,
                    c.UserId,
                    c.CreatedAt)).ToList()));
        CreateMap<CreateTaskDto, TaskItem>();
        CreateMap<TaskHistory, TaskHistoryDto>()
            .ConstructUsing(src => new TaskHistoryDto(
                src.Id,
                src.TaskItemId,
                src.Field,
                src.OldValue,
                src.NewValue,
                src.ChangedAt,
                src.ChangedByUserId));
        CreateMap<TaskComment, TaskCommentDto>()
            .ConstructUsing(src => new TaskCommentDto(
                src.Id,
                src.TaskItemId,
                src.Message,
                src.UserId,
                src.CreatedAt));
    }
}
