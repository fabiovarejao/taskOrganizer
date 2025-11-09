# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj files and restore (cache)
COPY src/TaskOrganizer.Api/TaskOrganizer.Api.csproj src/TaskOrganizer.Api/
COPY src/TaskOrganizer.Application/TaskOrganizer.Application.csproj src/TaskOrganizer.Application/
COPY src/TaskOrganizer.Domain/TaskOrganizer.Domain.csproj src/TaskOrganizer.Domain/
COPY src/TaskOrganizer.Infrastructure/TaskOrganizer.Infrastructure.csproj src/TaskOrganizer.Infrastructure/
RUN dotnet restore src/TaskOrganizer.Api/TaskOrganizer.Api.csproj

# copy the rest and publish
COPY . .
WORKDIR /src/src/TaskOrganizer.Api
RUN dotnet publish -c Release -o /app/publish

# runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
# run on http port 80 (matches docker-compose mapping)
ENV ASPNETCORE_URLS=http://+:80
ENV TZ=America/Sao_Paulo
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 80
ENTRYPOINT ["dotnet", "TaskOrganizer.Api.dll"]