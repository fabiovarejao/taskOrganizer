# TaskOrganizer – Guia Rápido

Este README é direto ao ponto para subir e testar a Task Organizer API localmente, dentro de um container Docker. 

## Requisitos

- .NET 8 SDK
- Docker Desktop (para SQL Server e, opcionalmente, a própria API) – Download: https://www.docker.com/products/docker-desktop/


# Clonando o Repositório

1. Clone o repositório do projeto para sua máquina local:

```powershell
git clone https://github.com/fabiovarejao/taskOrganizer.git
```

2. Acesse o diretório do projeto:

```powershell
cd taskOrganizer
```

## Como rodar com Docker Compose

1) Na raiz do projeto, garanta o arquivo `.env` (existe um exemplo `.env.example`). O `.env` padrão já contém:

```
SA_PASSWORD=saIron74Ds!#
CONNECTION_STRING=Server=sqlserver,1433;Database=TaskOrganizerDb;User Id=sa;Password=saIron74Ds!#;TrustServerCertificate=True;MultipleActiveResultSets=true
APPLY_MIGRATIONS=true
```

2) Execute o build da aplicação:

```powershell
docker compose build --no-cache
```

3) Suba os containers (SQL Server + API):

```powershell
docker compose up -d
```

4) Acesse a API:

- Swagger: http://localhost:5000/swagger

**Observações:**
- O `docker-compose.yml` mapeia 5000:80 para a API
- Com `APPLY_MIGRATIONS=true`, as migrations são aplicadas automaticamente na subida

## (Opcional) Carregar dados de exemplo no banco (seed)

Para popular o banco com usuários, projetos e tarefas de teste:

```powershell
# Criar versão UTF-8 limpa do seed (executar uma vez)
[System.IO.File]::WriteAllText("$PWD\database\seed-data-clean.sql", (Get-Content .\database\seed-data.sql -Raw -Encoding UTF8), (New-Object System.Text.UTF8Encoding $false))

# Copiar para o container e aplicar seed
docker cp .\database\seed-data-clean.sql tasks-sql:/tmp/seed.sql
docker exec tasks-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "saIron74Ds!#" -C -i /tmp/seed.sql
```

**Observação:** A senha padrão do `.env` é `saIron74Ds!#`. Se alterou, ajuste o comando acima.

**Dados criados:**
- 3 Usuários: João Silva (Manager), Maria Santos (Analyst), Carlos Souza (Specialist)
- 2 Projetos: Sistema de Vendas Online, Migração para Nuvem
- 4 Tarefas com diferentes status e prioridades
- Comentários e histórico de mudanças

Os IDs fixos para testes estão documentados no próprio arquivo `database/seed-data.sql`.

## Como rodar local (API via dotnet run)

Se preferir rodar a API no host (e usar só o SQL Server em Docker):

1) Suba apenas o SQL Server do compose:

```powershell
docker compose up -d sqlserver
```

2) Confirme a `DefaultConnection` em `src/TaskOrganizer.Api/appsettings.json` (ou use `ConnectionStrings__DefaultConnection` via variável de ambiente) apontando para `localhost,1433`.

3) Rode a API expondo na porta 5000:

```powershell
cd src/TaskOrganizer.Api
$env:ASPNETCORE_URLS = "http://localhost:5000"; dotnet run
```

4) Acesse: http://localhost:5000/swagger

## Testes

Na raiz do repositório:

```powershell
dotnet test
```

## Endpoints (resumo)

- **Projetos**: `GET /projects?userId=...`, `POST /projects`, `DELETE /projects/{projectId}`
- **Tarefas**: `GET /projects/{projectId}/tasks`, `POST /projects/{projectId}/tasks`, `PUT /tasks/{taskId}/status?newStatus=..&userId=..`, `DELETE /tasks/{taskId}`, `POST /tasks/{taskId}/comments`, `GET /tasks/{taskId}/history`
- **Relatórios**: `GET /reports/completed-per-user?userId=...`

## Notas rápidas

- Porta pública: 5000
- Banco: SQL Server 2022 em container (`tasks-sql`)
- Migrations automáticas quando `APPLY_MIGRATIONS=true`
- Para encoding correto (acentos): use o método de seed via docker cp + sqlcmd -i