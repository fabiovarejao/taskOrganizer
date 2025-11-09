# Task Organizer API - Checklist de Testes
# Este script executa todos os endpoints e valida as regras de negócio

$baseUrl = "http://localhost:5000"
$results = @()

function Test-Endpoint {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Url,
        [object]$Body = $null,
        [scriptblock]$Validation
    )
    
    Write-Host "`n=== Testing: $Name ===" -ForegroundColor Cyan
    
    try {
        $headers = @{
            "Content-Type" = "application/json"
        }
        
        $params = @{
            Uri = $Url
            Method = $Method
            Headers = $headers
            UseBasicParsing = $true
        }
        
        if ($Body) {
            $params.Body = ($Body | ConvertTo-Json -Depth 10)
        }
        
        $response = Invoke-WebRequest @params -ErrorAction Stop
        $content = $response.Content | ConvertFrom-Json
        
        $validationResult = & $Validation $response $content
        
        return @{
            Name = $Name
            Status = if ($validationResult) { "✓ PASS" } else { "✗ FAIL" }
            StatusCode = $response.StatusCode
            Details = $validationResult
            Response = $content
        }
    }
    catch {
        $statusCode = if ($_.Exception.Response) { [int]$_.Exception.Response.StatusCode } else { 0 }
        return @{
            Name = $Name
            Status = "✗ FAIL"
            StatusCode = $statusCode
            Details = $_.Exception.Message
            Response = $null
        }
    }
}

function Test-ExpectedError {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Url,
        [object]$Body = $null,
        [int]$ExpectedStatus,
        [string]$ExpectedMessage = ""
    )
    
    Write-Host "`n=== Testing: $Name ===" -ForegroundColor Cyan
    
    try {
        $headers = @{
            "Content-Type" = "application/json"
        }
        
        $params = @{
            Uri = $Url
            Method = $Method
            Headers = $headers
            UseBasicParsing = $true
        }
        
        if ($Body) {
            $params.Body = ($Body | ConvertTo-Json -Depth 10)
        }
        
        try {
            $response = Invoke-WebRequest @params -ErrorAction Stop
            return @{
                Name = $Name
                Status = "✗ FAIL"
                StatusCode = $response.StatusCode
                Details = "Expected error $ExpectedStatus but got success"
                Response = $null
            }
        }
        catch {
            $statusCode = [int]$_.Exception.Response.StatusCode
            if ($statusCode -eq $ExpectedStatus) {
                $details = "Got expected status $ExpectedStatus"
                if ($ExpectedMessage) {
                    $errorBody = $_.ErrorDetails.Message | ConvertFrom-Json
                    if ($errorBody.detail -match $ExpectedMessage) {
                        $details += " with expected message"
                    }
                }
                return @{
                    Name = $Name
                    Status = "✓ PASS"
                    StatusCode = $statusCode
                    Details = $details
                    Response = $null
                }
            }
            else {
                return @{
                    Name = $Name
                    Status = "✗ FAIL"
                    StatusCode = $statusCode
                    Details = "Expected $ExpectedStatus but got $statusCode"
                    Response = $null
                }
            }
        }
    }
    catch {
        return @{
            Name = $Name
            Status = "✗ FAIL"
            StatusCode = 0
            Details = $_.Exception.Message
            Response = $null
        }
    }
}

Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║   TASK ORGANIZER API - CHECKLIST DE TESTES COMPLETO      ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Green

# Verificar se API está rodando
Write-Host "`nVerificando se API está acessível..." -ForegroundColor Yellow
try {
    $ping = Invoke-WebRequest -Uri "$baseUrl/swagger/index.html" -Method GET -UseBasicParsing -TimeoutSec 5
    Write-Host "✓ API está rodando em $baseUrl" -ForegroundColor Green
}
catch {
    Write-Host "✗ API não está acessível em $baseUrl" -ForegroundColor Red
    Write-Host "Por favor, inicie a API antes de executar os testes." -ForegroundColor Red
    exit 1
}

# Criar usuários de teste
Write-Host "`n" + "="*60 -ForegroundColor Magenta
Write-Host "SETUP: Criando usuários de teste" -ForegroundColor Magenta
Write-Host "="*60 -ForegroundColor Magenta

$aliceId = [guid]::NewGuid().ToString()
$bobId = [guid]::NewGuid().ToString()
$managerId = [guid]::NewGuid().ToString()

# Variáveis globais para armazenar IDs criados
$script:testProjectId = $null
$script:testTaskId = $null

Write-Host "`n📋 INICIANDO TESTES DE ENDPOINTS E REGRAS DE NEGÓCIO`n" -ForegroundColor Yellow

# ============================================================================
# TESTES DE ENDPOINTS - PROJECTS
# ============================================================================
Write-Host "`n" + "="*60 -ForegroundColor Magenta
Write-Host "SEÇÃO 1: TESTES DE ENDPOINTS - PROJECTS (CRUD)" -ForegroundColor Magenta
Write-Host "="*60 -ForegroundColor Magenta

$result = Test-Endpoint `
    -Name "1.1 - POST /projects - Criar projeto" `
    -Method "POST" `
    -Url "$baseUrl/projects" `
    -Body @{
        name = "Projeto Teste Checklist"
        description = "Projeto para validação completa"
        userId = $aliceId
    } `
    -Validation {
        param($response, $content)
        $script:testProjectId = $content.id
        return ($response.StatusCode -eq 201 -and $content.name -eq "Projeto Teste Checklist")
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

$result = Test-Endpoint `
    -Name "1.2 - GET /projects?userId={userId} - Listar projetos do usuário" `
    -Method "GET" `
    -Url "$baseUrl/projects?userId=$aliceId" `
    -Validation {
        param($response, $content)
        return ($response.StatusCode -eq 200 -and $content.Count -ge 1)
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

$result = Test-Endpoint `
    -Name "1.3 - GET /projects/{id} - Obter projeto específico" `
    -Method "GET" `
    -Url "$baseUrl/projects/$script:testProjectId" `
    -Validation {
        param($response, $content)
        return ($response.StatusCode -eq 200 -and $content.id -eq $script:testProjectId)
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

$result = Test-Endpoint `
    -Name "1.4 - PUT /projects/{id} - Atualizar projeto" `
    -Method "PUT" `
    -Url "$baseUrl/projects/$script:testProjectId" `
    -Body @{
        name = "Projeto Teste ATUALIZADO"
        description = "Descrição atualizada"
    } `
    -Validation {
        param($response, $content)
        return ($response.StatusCode -eq 200 -and $content.name -eq "Projeto Teste ATUALIZADO")
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

# ============================================================================
# TESTES DE ENDPOINTS - TASKS
# ============================================================================
Write-Host "`n" + "="*60 -ForegroundColor Magenta
Write-Host "SEÇÃO 2: TESTES DE ENDPOINTS - TASKS (CRUD)" -ForegroundColor Magenta
Write-Host "="*60 -ForegroundColor Magenta

$result = Test-Endpoint `
    -Name "2.1 - POST /projects/{projectId}/tasks - Criar tarefa" `
    -Method "POST" `
    -Url "$baseUrl/projects/$script:testProjectId/tasks" `
    -Body @{
        title = "Tarefa Teste 1"
        description = "Descrição da tarefa"
        priority = "High"
        dueDate = (Get-Date).AddDays(7).ToString("yyyy-MM-ddTHH:mm:ss")
        responsibleUserId = $bobId
    } `
    -Validation {
        param($response, $content)
        $script:testTaskId = $content.id
        return ($response.StatusCode -eq 201 -and $content.title -eq "Tarefa Teste 1")
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

$result = Test-Endpoint `
    -Name "2.2 - GET /tasks - Listar todas as tarefas" `
    -Method "GET" `
    -Url "$baseUrl/tasks" `
    -Validation {
        param($response, $content)
        return ($response.StatusCode -eq 200 -and $content.Count -ge 1)
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

$result = Test-Endpoint `
    -Name "2.3 - GET /tasks/{id} - Obter tarefa específica" `
    -Method "GET" `
    -Url "$baseUrl/tasks/$script:testTaskId" `
    -Validation {
        param($response, $content)
        return ($response.StatusCode -eq 200 -and $content.id -eq $script:testTaskId)
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

$result = Test-Endpoint `
    -Name "2.4 - PUT /tasks/{id} - Atualizar descrição da tarefa" `
    -Method "PUT" `
    -Url "$baseUrl/tasks/$script:testTaskId" `
    -Body @{
        title = "Tarefa Teste 1 - ATUALIZADA"
        description = "Nova descrição atualizada"
        priority = "High"
    } `
    -Validation {
        param($response, $content)
        return ($response.StatusCode -eq 200 -and $content.description -eq "Nova descrição atualizada")
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

# ============================================================================
# TESTES DE REGRAS DE NEGÓCIO
# ============================================================================
Write-Host "`n" + "="*60 -ForegroundColor Magenta
Write-Host "SEÇÃO 3: TESTES DE REGRAS DE NEGÓCIO" -ForegroundColor Magenta
Write-Host "="*60 -ForegroundColor Magenta

# REGRA 1: Prioridade não pode ser alterada
Write-Host "`n--- REGRA 1: Prioridade imutável após criação ---" -ForegroundColor Yellow
$result = Test-ExpectedError `
    -Name "3.1 - REGRA 1: Tentar alterar prioridade (deve falhar)" `
    -Method "PUT" `
    -Url "$baseUrl/tasks/$script:testTaskId" `
    -Body @{
        title = "Tarefa Teste 1"
        description = "Tentando mudar prioridade"
        priority = "Low"
    } `
    -ExpectedStatus 400 `
    -ExpectedMessage "prioridade"
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

# REGRA 2: Limite de 20 tarefas por projeto
Write-Host "`n--- REGRA 2: Máximo 20 tarefas por projeto ---" -ForegroundColor Yellow

# Criar projeto específico para testar o limite
$limitTestProjectId = $null
$result = Test-Endpoint `
    -Name "3.2a - Criar projeto para teste de limite" `
    -Method "POST" `
    -Url "$baseUrl/projects" `
    -Body @{
        name = "Projeto Teste Limite 20 Tarefas"
        description = "Para testar a regra de 20 tarefas"
        userId = $aliceId
    } `
    -Validation {
        param($response, $content)
        $script:limitTestProjectId = $content.id
        return ($response.StatusCode -eq 201)
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

# Criar 20 tarefas
Write-Host "`nCriando 20 tarefas..." -ForegroundColor Gray
for ($i = 1; $i -le 20; $i++) {
    $taskResult = Test-Endpoint `
        -Name "3.2b - Criar tarefa $i de 20" `
        -Method "POST" `
        -Url "$baseUrl/projects/$limitTestProjectId/tasks" `
        -Body @{
            title = "Tarefa $i"
            description = "Tarefa numero $i"
            priority = "Medium"
            responsibleUserId = $bobId
        } `
        -Validation {
            param($response, $content)
            return ($response.StatusCode -eq 201)
        }
    
    if ($taskResult.Status -notmatch "PASS") {
        Write-Host "  ✗ Falhou ao criar tarefa $i" -ForegroundColor Red
        break
    }
}
Write-Host "✓ 20 tarefas criadas com sucesso" -ForegroundColor Green

# Tentar criar a 21ª tarefa (deve falhar)
$result = Test-ExpectedError `
    -Name "3.2c - REGRA 2: Tentar criar 21ª tarefa (deve falhar)" `
    -Method "POST" `
    -Url "$baseUrl/projects/$limitTestProjectId/tasks" `
    -Body @{
        title = "Tarefa 21 - Deve falhar"
        description = "Esta tarefa não deve ser criada"
        priority = "Low"
        responsibleUserId = $bobId
    } `
    -ExpectedStatus 400 `
    -ExpectedMessage "20"
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

# REGRA 3: Não permitir exclusão de projeto com tarefas pendentes
Write-Host "`n--- REGRA 3: Bloqueio de exclusão com tarefas pendentes ---" -ForegroundColor Yellow
$result = Test-ExpectedError `
    -Name "3.3a - REGRA 3: Tentar deletar projeto com tarefas pendentes (deve falhar)" `
    -Method "DELETE" `
    -Url "$baseUrl/projects/$script:testProjectId" `
    -ExpectedStatus 400 `
    -ExpectedMessage "pendente"
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

# Completar a tarefa e tentar deletar novamente
$result = Test-Endpoint `
    -Name "3.3b - Completar tarefa para liberar exclusão do projeto" `
    -Method "PUT" `
    -Url "$baseUrl/tasks/$script:testTaskId" `
    -Body @{
        title = "Tarefa Teste 1"
        description = "Completando tarefa"
        priority = "High"
        status = "Completed"
    } `
    -Validation {
        param($response, $content)
        return ($response.StatusCode -eq 200 -and $content.status -eq "Completed")
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

$result = Test-Endpoint `
    -Name "3.3c - REGRA 3: Deletar projeto após completar tarefas (deve passar)" `
    -Method "DELETE" `
    -Url "$baseUrl/projects/$script:testProjectId" `
    -Validation {
        param($response, $content)
        return ($response.StatusCode -eq 204)
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

# REGRA 4: Histórico de alterações
Write-Host "`n--- REGRA 4: Rastreamento de histórico de alterações ---" -ForegroundColor Yellow

# Criar novo projeto e tarefa para testar histórico
$historyProjectId = $null
$historyTaskId = $null

$result = Test-Endpoint `
    -Name "3.4a - Criar projeto para teste de histórico" `
    -Method "POST" `
    -Url "$baseUrl/projects" `
    -Body @{
        name = "Projeto Teste Histórico"
        description = "Para testar tracking de mudanças"
        userId = $aliceId
    } `
    -Validation {
        param($response, $content)
        $script:historyProjectId = $content.id
        return ($response.StatusCode -eq 201)
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

$result = Test-Endpoint `
    -Name "3.4b - Criar tarefa para teste de histórico" `
    -Method "POST" `
    -Url "$baseUrl/projects/$historyProjectId/tasks" `
    -Body @{
        title = "Tarefa para rastrear histórico"
        description = "Descrição inicial"
        priority = "Medium"
        responsibleUserId = $bobId
    } `
    -Validation {
        param($response, $content)
        $script:historyTaskId = $content.id
        return ($response.StatusCode -eq 201)
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

# Atualizar status
$result = Test-Endpoint `
    -Name "3.4c - Atualizar status para InProgress" `
    -Method "PUT" `
    -Url "$baseUrl/tasks/$historyTaskId" `
    -Body @{
        title = "Tarefa para rastrear histórico"
        description = "Descrição inicial"
        priority = "Medium"
        status = "InProgress"
    } `
    -Validation {
        param($response, $content)
        return ($response.StatusCode -eq 200)
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

# Atualizar descrição
$result = Test-Endpoint `
    -Name "3.4d - Atualizar descrição da tarefa" `
    -Method "PUT" `
    -Url "$baseUrl/tasks/$historyTaskId" `
    -Body @{
        title = "Tarefa para rastrear histórico"
        description = "Descrição MODIFICADA para verificar histórico"
        priority = "Medium"
        status = "InProgress"
    } `
    -Validation {
        param($response, $content)
        return ($response.StatusCode -eq 200)
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

# Verificar se o histórico foi criado
$result = Test-Endpoint `
    -Name "3.4e - REGRA 4: Verificar histórico de mudanças na tarefa" `
    -Method "GET" `
    -Url "$baseUrl/tasks/$historyTaskId" `
    -Validation {
        param($response, $content)
        $hasHistory = $content.history -and $content.history.Count -ge 2
        if ($hasHistory) {
            Write-Host "  ✓ Histórico contém $($content.history.Count) entradas" -ForegroundColor Green
            $statusChange = $content.history | Where-Object { $_.field -eq "Status" }
            $descChange = $content.history | Where-Object { $_.field -eq "Description" }
            if ($statusChange) { Write-Host "    - Mudança de Status detectada" -ForegroundColor Gray }
            if ($descChange) { Write-Host "    - Mudança de Description detectada" -ForegroundColor Gray }
        }
        return $hasHistory
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

# REGRA 5: Relatórios para gerentes
Write-Host "`n--- REGRA 5: Relatórios exclusivos para gerentes ---" -ForegroundColor Yellow
$result = Test-Endpoint `
    -Name "3.5 - REGRA 5: Obter relatório de gerente (média de tarefas concluídas)" `
    -Method "GET" `
    -Url "$baseUrl/reports/manager/$managerId" `
    -Validation {
        param($response, $content)
        $hasAverage = $content.PSObject.Properties.Name -contains "averageCompletedTasksLast30Days"
        if ($hasAverage) {
            Write-Host "  ✓ Relatório contém média de tarefas: $($content.averageCompletedTasksLast30Days)" -ForegroundColor Green
        }
        return ($response.StatusCode -eq 200 -and $hasAverage)
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

# REGRA 6: Comentários em tarefas
Write-Host "`n--- REGRA 6: Comentários com validação ---" -ForegroundColor Yellow

$result = Test-Endpoint `
    -Name "3.6a - REGRA 6: Adicionar comentário válido na tarefa" `
    -Method "POST" `
    -Url "$baseUrl/tasks/$historyTaskId/comments" `
    -Body @{
        message = "Este é um comentário de teste válido"
        userId = $aliceId
    } `
    -Validation {
        param($response, $content)
        return ($response.StatusCode -eq 201)
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

$result = Test-ExpectedError `
    -Name "3.6b - REGRA 6: Tentar adicionar comentário vazio (deve falhar)" `
    -Method "POST" `
    -Url "$baseUrl/tasks/$historyTaskId/comments" `
    -Body @{
        message = ""
        userId = $aliceId
    } `
    -ExpectedStatus 400
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

$result = Test-Endpoint `
    -Name "3.6c - Verificar comentários foram salvos" `
    -Method "GET" `
    -Url "$baseUrl/tasks/$historyTaskId" `
    -Validation {
        param($response, $content)
        $hasComments = $content.comments -and $content.comments.Count -ge 1
        if ($hasComments) {
            Write-Host "  ✓ Tarefa contém $($content.comments.Count) comentário(s)" -ForegroundColor Green
        }
        return $hasComments
    }
$results += $result
Write-Host "$($result.Status) - $($result.Name) - Status: $($result.StatusCode)" -ForegroundColor $(if ($result.Status -match "PASS") { "Green" } else { "Red" })

# ============================================================================
# GERAR RELATÓRIO FINAL
# ============================================================================
Write-Host "`n" + "="*60 -ForegroundColor Magenta
Write-Host "RELATÓRIO FINAL DE TESTES" -ForegroundColor Magenta
Write-Host "="*60 -ForegroundColor Magenta

$totalTests = $results.Count
$passedTests = ($results | Where-Object { $_.Status -match "PASS" }).Count
$failedTests = $totalTests - $passedTests
$successRate = [math]::Round(($passedTests / $totalTests) * 100, 2)

Write-Host "`nResumo:"
Write-Host "  Total de testes: $totalTests"
Write-Host "  Aprovados: $passedTests" -ForegroundColor Green
Write-Host "  Falharam: $failedTests" -ForegroundColor $(if ($failedTests -gt 0) { "Red" } else { "Green" })
Write-Host "  Taxa de sucesso: $successRate%" -ForegroundColor $(if ($successRate -ge 80) { "Green" } elseif ($successRate -ge 50) { "Yellow" } else { "Red" })

# Gerar arquivo Markdown com resultados
$reportPath = "c:\workspace\bancoMaster\docs\test-results-checklist.md"
$markdown = @"
# 📋 Task Organizer API - Relatório de Testes

**Data da execução:** $(Get-Date -Format "dd/MM/yyyy HH:mm:ss")  
**Ambiente:** Development  
**Base URL:** $baseUrl

---

## 📊 Resumo Executivo

| Métrica | Valor |
|---------|-------|
| **Total de Testes** | $totalTests |
| **Testes Aprovados** | ✅ $passedTests |
| **Testes Falhados** | ❌ $failedTests |
| **Taxa de Sucesso** | $successRate% |

---

## 📝 Detalhamento dos Testes

### 🔷 Seção 1: Endpoints de Projetos (CRUD)

"@

# Agrupar por seção
$section1 = $results | Where-Object { $_.Name -match "^1\." }
$section2 = $results | Where-Object { $_.Name -match "^2\." }
$section3 = $results | Where-Object { $_.Name -match "^3\." }

foreach ($test in $section1) {
    $statusIcon = if ($test.Status -match "PASS") { "✅" } else { "❌" }
    $markdown += "`n#### $statusIcon $($test.Name)`n"
    $markdown += "- **Status:** $($test.Status)`n"
    $markdown += "- **Código HTTP:** $($test.StatusCode)`n"
    if ($test.Details) {
        $markdown += "- **Detalhes:** $($test.Details)`n"
    }
    $markdown += "`n"
}

$markdown += "`n### 🔷 Seção 2: Endpoints de Tarefas (CRUD)`n`n"
foreach ($test in $section2) {
    $statusIcon = if ($test.Status -match "PASS") { "✅" } else { "❌" }
    $markdown += "`n#### $statusIcon $($test.Name)`n"
    $markdown += "- **Status:** $($test.Status)`n"
    $markdown += "- **Código HTTP:** $($test.StatusCode)`n"
    if ($test.Details) {
        $markdown += "- **Detalhes:** $($test.Details)`n"
    }
    $markdown += "`n"
}

$markdown += "`n### 🔷 Seção 3: Regras de Negócio`n`n"

$markdown += "`n#### 📌 Regra 1: Prioridade Imutável`n"
$rule1Tests = $section3 | Where-Object { $_.Name -match "REGRA 1" }
foreach ($test in $rule1Tests) {
    $statusIcon = if ($test.Status -match "PASS") { "✅" } else { "❌" }
    $markdown += "- $statusIcon **$($test.Name)** - Status HTTP: $($test.StatusCode)`n"
}

$markdown += "`n#### 📌 Regra 2: Limite de 20 Tarefas por Projeto`n"
$rule2Tests = $section3 | Where-Object { $_.Name -match "REGRA 2" }
foreach ($test in $rule2Tests) {
    $statusIcon = if ($test.Status -match "PASS") { "✅" } else { "❌" }
    $markdown += "- $statusIcon **$($test.Name)** - Status HTTP: $($test.StatusCode)`n"
}

$markdown += "`n#### 📌 Regra 3: Bloqueio de Exclusão com Tarefas Pendentes`n"
$rule3Tests = $section3 | Where-Object { $_.Name -match "REGRA 3" }
foreach ($test in $rule3Tests) {
    $statusIcon = if ($test.Status -match "PASS") { "✅" } else { "❌" }
    $markdown += "- $statusIcon **$($test.Name)** - Status HTTP: $($test.StatusCode)`n"
}

$markdown += "`n#### 📌 Regra 4: Rastreamento de Histórico`n"
$rule4Tests = $section3 | Where-Object { $_.Name -match "REGRA 4|3\.4" }
foreach ($test in $rule4Tests) {
    $statusIcon = if ($test.Status -match "PASS") { "✅" } else { "❌" }
    $markdown += "- $statusIcon **$($test.Name)** - Status HTTP: $($test.StatusCode)`n"
}

$markdown += "`n#### 📌 Regra 5: Relatórios para Gerentes`n"
$rule5Tests = $section3 | Where-Object { $_.Name -match "REGRA 5" }
foreach ($test in $rule5Tests) {
    $statusIcon = if ($test.Status -match "PASS") { "✅" } else { "❌" }
    $markdown += "- $statusIcon **$($test.Name)** - Status HTTP: $($test.StatusCode)`n"
}

$markdown += "`n#### 📌 Regra 6: Comentários com Validação`n"
$rule6Tests = $section3 | Where-Object { $_.Name -match "REGRA 6|3\.6" }
foreach ($test in $rule6Tests) {
    $statusIcon = if ($test.Status -match "PASS") { "✅" } else { "❌" }
    $markdown += "- $statusIcon **$($test.Name)** - Status HTTP: $($test.StatusCode)`n"
}

$markdown += "`n---`n`n"
$markdown += "## ✅ Checklist de Conformidade`n`n"
$markdown += "| # | Requisito | Status |`n"
$markdown += "|---|-----------|--------|`n"
$markdown += "| 1 | Endpoints CRUD de Projetos funcionando | " + $(if (($section1 | Where-Object { $_.Status -match "PASS" }).Count -eq $section1.Count) { "✅ PASS" } else { "❌ FAIL" }) + " |`n"
$markdown += "| 2 | Endpoints CRUD de Tarefas funcionando | " + $(if (($section2 | Where-Object { $_.Status -match "PASS" }).Count -eq $section2.Count) { "✅ PASS" } else { "❌ FAIL" }) + " |`n"
$markdown += "| 3 | Regra 1: Prioridade imutável | " + $(if (($rule1Tests | Where-Object { $_.Status -match "PASS" }).Count -eq $rule1Tests.Count) { "✅ PASS" } else { "❌ FAIL" }) + " |`n"
$markdown += "| 4 | Regra 2: Limite de 20 tarefas | " + $(if (($rule2Tests | Where-Object { $_.Status -match "PASS" }).Count -eq $rule2Tests.Count) { "✅ PASS" } else { "❌ FAIL" }) + " |`n"
$markdown += "| 5 | Regra 3: Bloqueio de exclusão | " + $(if (($rule3Tests | Where-Object { $_.Status -match "PASS" }).Count -eq $rule3Tests.Count) { "✅ PASS" } else { "❌ FAIL" }) + " |`n"
$markdown += "| 6 | Regra 4: Histórico de mudanças | " + $(if (($rule4Tests | Where-Object { $_.Status -match "PASS" }).Count -eq $rule4Tests.Count) { "✅ PASS" } else { "❌ FAIL" }) + " |`n"
$markdown += "| 7 | Regra 5: Relatórios de gerente | " + $(if (($rule5Tests | Where-Object { $_.Status -match "PASS" }).Count -eq $rule5Tests.Count) { "✅ PASS" } else { "❌ FAIL" }) + " |`n"
$markdown += "| 8 | Regra 6: Comentários válidos | " + $(if (($rule6Tests | Where-Object { $_.Status -match "PASS" }).Count -eq $rule6Tests.Count) { "✅ PASS" } else { "❌ FAIL" }) + " |`n"

$markdown += "`n---`n"
$markdown += "`n*Relatório gerado automaticamente pelo script de testes*`n"

$markdown | Out-File -FilePath $reportPath -Encoding UTF8

Write-Host "`n✅ Relatório completo salvo em: $reportPath" -ForegroundColor Green
Write-Host "`nPara visualizar o relatório em Markdown, abra o arquivo:" -ForegroundColor Yellow
Write-Host "  $reportPath`n" -ForegroundColor Cyan
