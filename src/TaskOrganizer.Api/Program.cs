using TaskOrganizer.Infrastructure.Context;
using TaskOrganizer.Application.Interfaces;
using TaskOrganizer.Domain.Interfaces.Repositories;
using TaskOrganizer.Infrastructure.Repositories;
using TaskOrganizer.Application.Services;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using TaskOrganizer.Domain.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddScoped<TaskOrganizer.Application.Interfaces.IAppDbContext>(provider => 
    provider.GetRequiredService<AppDbContext>());

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<TaskOrganizer.Application.Validators.CreateProjectDtoValidator>();

builder.Services.AddSingleton(provider =>
{
    var config = new AutoMapper.MapperConfiguration(cfg =>
    {
        cfg.AddMaps(typeof(TaskOrganizer.Application.Mapping.ProjectProfile).Assembly);
    });
    return config.CreateMapper();
});


builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectUserRepository, ProjectUserRepository>();
builder.Services.AddScoped<ITaskUserRepository, TaskUserRepository>();
builder.Services.AddScoped<ITaskHistoryRepository, TaskHistoryRepository>();
builder.Services.AddScoped<TaskOrganizer.Application.Interfaces.ITaskService, TaskService>();
builder.Services.AddScoped<TaskOrganizer.Application.Interfaces.IProjectService, ProjectService>();
builder.Services.AddScoped<TaskOrganizer.Application.Interfaces.IUserService, UserService>();

builder.Services.AddScoped<IReportsService, ReportsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/problem+json";
        var ex = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        if (ex is not null)
        {
            static string Flatten(Exception e)
            {
                var msgs = new List<string>();
                for (var cur = e; cur != null; cur = cur.InnerException)
                    msgs.Add(cur.Message);
                return string.Join(" | ", msgs);
            }

            var isDomain = ex is DomainException;
            var pd = new
            {
                type = "about:blank",
                title = isDomain ? "Violação de regra de negócio" : "Ocorreu um erro inesperado",
                status = isDomain ? 400 : 500,
                detail = Flatten(ex)
            };
            context.Response.StatusCode = pd.status;
            await context.Response.WriteAsJsonAsync(pd);
        }
    });
});

app.MapControllers();

try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var applyMigrations = app.Configuration.GetValue<bool>("ApplyMigrations", false);
        if (applyMigrations && db.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
        {
            var retries = 6;
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    db.Database.Migrate();
                    break;
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.Message.Contains("Relational-specific methods can only be used"))
                    {
                        break;
                    }
                    if (i == retries - 1) throw;
                    Thread.Sleep(3000);
                }
                catch (Exception)
                {
                    if (i == retries - 1) throw;
                    Thread.Sleep(3000);
                }
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Aviso: migrations automáticas falharam ou foram ignoradas: {ex.Message}");
}

app.Run();

public partial class Program { }
