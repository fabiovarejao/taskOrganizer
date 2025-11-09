# TaskOrganizer – Guia Rápido

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927)

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


### Testes Unitários

Na raiz do repositório:

```powershell
dotnet test
```

### Testes Manuais via Postman

1. Abra o **Postman**
2. Clique em **Import** → **File**
3. Selecione o arquivo: `docs/TaskOrganizer.postman_collection.json`
4. Configure a variável `baseUrl` para `http://localhost:5000`
5. Execute as requisições na ordem da collection

### Script de Validação Automatizada

Execute o checklist completo (certifique-se que a API está rodando):

```powershell
.\docs\test-checklist.ps1
```

**Se der erro de permissão**, execute antes:

```powershell
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process
```

## Notas rápidas

- Porta pública: 5000
- Banco: SQL Server 2022 em container (`tasks-sql`)
- Migrations automáticas quando `APPLY_MIGRATIONS=true`
- Para encoding correto (acentos): use o método de seed via docker cp + sqlcmd -i

---

## 📋 Fase 2: Refinamento (Questões para o PO)

### 1. Regras de Prioridade de Tarefas
**A prioridade é imutável por qual motivo de negócio?**

Existe algum cenário onde seria necessário ajustar a prioridade? Por exemplo, quando uma tarefa se torna mais urgente devido a mudanças no projeto?

### 2. Limite de 20 Tarefas por Projeto
**Este limite é fixo ou pode variar por tipo de projeto?**

Projetos maiores ou de longo prazo poderiam ter um limite maior? Devemos alertar o usuário quando estiver próximo do limite?

### 3. Hierarquia e Permissões de Usuários
**Devemos implementar controle de permissões baseado em cargos?**

Atualmente apenas Gerentes podem gerar relatórios. Devemos expandir esse modelo de permissões? Por exemplo:
- Apenas Gerentes podem criar/excluir projetos?
- Digamos que Analistas poderiam apenas visualizar e comentar tarefas?
- Em caso de termos Especialistas, teriam permissões específicas diferentes?

### 4. Status e Ciclo de Vida das Tarefas
**Quais são todos os status possíveis de uma tarefa?**

O que acha de acrescentar Bloqueada, Cancelada? 

### 5. Exclusão de Projetos e Dados Históricos
**A exclusão de projetos deve ser física ou lógica?**

É importante manter histórico de projetos excluídos para auditoria? Tarefas concluídas também impedem a exclusão do projeto ou apenas as pendentes?

### 6. Notificações e Alertas
**O sistema deve notificar usuários sobre alterações em suas tarefas?**

Devemos implementar notificações quando:
- Uma tarefa é atribuída ao usuário?
- Um comentário é adicionado à tarefa?
- O prazo está próximo do vencimento?

---

## 🚀 Fase 3: Melhorias Futuras

### 1. Substituir Lazy Loading por Eager Loading Estratégico
Objetivo de melhorar performance e evitar o problema de N+1 queries.

---

### 2. Implementar Paginação em Todas as Listagens
Objetivo de evitar sobrecarga quando há muitos registros, melhorar a experiência do usuário e menor consumo de memória.

---

### 3. Adicionar Sistema de Cache com Redis
**Objetivo:** Reduzir a quantidade de leituras repetidas no SQL para dados consultados com frequência.

Implementar cache para:
- Lista de projetos por usuário
- Detalhes de tarefas
- Estatísticas de dashboard

**Impacto esperado:** Pode reduzir significativamente a carga de leitura do banco de dados.

---

### 4. TaskHistory em MongoDB (Opcional – para alto volume)
Objetivo de manter dados transacionais (Projects/Tasks/Users) no SQL Server e mover apenas o histórico (TaskHistory) para MongoDB quando o volume de eventos crescer muito.

Só adotar quando o histórico começar a pesar em queries ou storage do SQL; até lá manter tudo no mesmo banco reduz complexidade.

---

### 5. Adicionar Health Checks
**Objetivo:** Monitorar saúde da aplicação e dependências (SQL Server, Redis, etc).

**Impacto:** Detecção proativa de problemas antes que afetem usuários.

---

### 6. Adicionar Autenticação e Autorização com JWT
**Objetivo:** Proteger a API e implementar controle de acesso por cargo.

**Impacto:** Segurança e controle granular de permissões (ex: apenas Managers podem deletar projetos).

---

### 7. Criar Sistema de Relatórios e Dashboards
**Objetivo:** Fornecer insights sobre produtividade e progresso.

Relatórios sugeridos:
- Tarefas concluídas por usuário/período
- Projetos próximos do limite de tarefas
- Média de tempo para conclusão de tarefas
- Usuários mais ativos/comentários

**Impacto:** Tomada de decisão baseada em dados.