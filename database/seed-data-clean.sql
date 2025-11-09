-- =================================================================
-- SEED DATA PARA O BANCO DE DADOS TaskOrganizerDb
-- =================================================================
-- Este script insere dados de exemplo para facilitar testes e demos
-- Execute este script APÓS aplicar as migrations
-- =================================================================

USE TaskOrganizerDb;
GO

-- Limpar dados existentes (opcional - descomente se quiser resetar)
-- DELETE FROM TaskHistories;
-- DELETE FROM TaskComments;
-- DELETE FROM TaskUsers;
-- DELETE FROM Tasks;
-- DELETE FROM ProjectUsers;
-- DELETE FROM Projects;
-- DELETE FROM Users;
-- GO

-- =================================================================
-- 1. USUÁRIOS (3 usuários com diferentes cargos)
-- =================================================================
INSERT INTO Users (Id, UserName, Position)
VALUES 
    ('11111111-1111-1111-1111-111111111111', N'João Silva', 0),      -- Manager
    ('22222222-2222-2222-2222-222222222222', N'Maria Santos', 1),    -- Analyst
    ('33333333-3333-3333-3333-333333333333', N'Carlos Souza', 2);    -- Specialist
GO

-- =================================================================
-- 2. PROJETOS (2 projetos criados por João)
-- =================================================================
INSERT INTO Projects (Id, Name, Description, UserId, TaskLimit)
VALUES 
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
        N'Sistema de Vendas Online',
        N'Desenvolvimento de plataforma e-commerce completa com integração de pagamentos',
        '11111111-1111-1111-1111-111111111111',
        20
    ),
    (
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
        N'Migração para Nuvem',
        N'Migração da infraestrutura on-premise para Azure Cloud',
        '11111111-1111-1111-1111-111111111111',
        20
    );
GO

-- =================================================================
-- 3. MEMBROS DOS PROJETOS (ProjectUsers - quem participa de cada projeto)
-- =================================================================
INSERT INTO ProjectUsers (ProjectId, UserId)
VALUES 
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '11111111-1111-1111-1111-111111111111'),  -- João no projeto 1
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '22222222-2222-2222-2222-222222222222'),  -- Maria no projeto 1
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '33333333-3333-3333-3333-333333333333'),  -- Carlos no projeto 1
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '11111111-1111-1111-1111-111111111111'),  -- João no projeto 2
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '33333333-3333-3333-3333-333333333333');  -- Carlos no projeto 2
GO

-- =================================================================
-- 4. TAREFAS (4 tarefas com diferentes status e prioridades)
-- =================================================================
INSERT INTO Tasks (Id, Title, Description, Status, Priority, DueDate, ProjectId)
VALUES 
    (
        'cccccccc-cccc-cccc-cccc-cccccccccccc',
        N'Implementar autenticação JWT',
        N'Desenvolver sistema de login com tokens JWT e refresh tokens',
        1,  -- InProgress
        2,  -- High
        '2025-11-15 23:59:59',
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'
    ),
    (
        'dddddddd-dddd-dddd-dddd-dddddddddddd',
        N'Criar catálogo de produtos',
        N'Desenvolver CRUD completo para gerenciamento do catálogo',
        2,  -- Completed
        1,  -- Medium
        '2025-11-10 23:59:59',
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'
    ),
    (
        'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee',
        N'Integração com gateway de pagamento',
        N'Implementar integração com Stripe/PagSeguro',
        0,  -- Pending
        2,  -- High
        '2025-11-20 23:59:59',
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'
    ),
    (
        'ffffffff-ffff-ffff-ffff-ffffffffffff',
        N'Configurar Azure App Service',
        N'Provisionar e configurar ambiente de produção no Azure',
        1,  -- InProgress
        2,  -- High
        '2025-11-12 23:59:59',
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'
    );
GO

-- =================================================================
-- 5. RESPONSÁVEIS PELAS TAREFAS (TaskUsers)
-- =================================================================
INSERT INTO TaskUsers (TaskId, UserId)
VALUES 
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', '22222222-2222-2222-2222-222222222222'),  -- Maria responsável por JWT
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', '33333333-3333-3333-3333-333333333333'),  -- Carlos responsável por catálogo
    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', '22222222-2222-2222-2222-222222222222'),  -- Maria responsável por pagamento
    ('ffffffff-ffff-ffff-ffff-ffffffffffff', '33333333-3333-3333-3333-333333333333');  -- Carlos responsável por Azure
GO

-- =================================================================
INSERT INTO TaskComments (Id, TaskItemId, Message, CreatedAt, UserId)
VALUES 
    (
        '10000000-0000-0000-0000-000000000001',
        'cccccccc-cccc-cccc-cccc-cccccccccccc',
        N'Iniciei a implementação do JWT. Estudando melhores práticas de segurança.',
        '2025-11-03 10:30:00',
        '22222222-2222-2222-2222-222222222222'
    ),
    (
        '10000000-0000-0000-0000-000000000002',
        'dddddddd-dddd-dddd-dddd-dddddddddddd',
        N'CRUD completo finalizado. Testes unitários passando 100%.',
        '2025-11-08 16:45:00',
        '33333333-3333-3333-3333-333333333333'
    ),
    (
        '10000000-0000-0000-0000-000000000003',
        'ffffffff-ffff-ffff-ffff-ffffffffffff',
        N'Azure App Service configurado com deploy automático via GitHub Actions.',
        '2025-11-06 14:20:00',
        '33333333-3333-3333-3333-333333333333'
    );
GO

-- =================================================================
INSERT INTO TaskHistories (Id, TaskItemId, Field, OldValue, NewValue, ChangedAt, ChangedByUserId, Message, UserId)
VALUES 
    (
        '20000000-0000-0000-0000-000000000001',
        'cccccccc-cccc-cccc-cccc-cccccccccccc',
        N'Status',
        N'Pending',
        N'InProgress',
        '2025-11-03 09:00:00',
        '22222222-2222-2222-2222-222222222222',
        N'Status alterado de Pending para InProgress',
        '22222222-2222-2222-2222-222222222222'
    ),
    (
        '20000000-0000-0000-0000-000000000002',
        'dddddddd-dddd-dddd-dddd-dddddddddddd',
        N'Status',
        N'InProgress',
        N'Completed',
        '2025-11-08 17:00:00',
        '33333333-3333-3333-3333-333333333333',
        N'Status alterado de InProgress para Completed',
        '33333333-3333-3333-3333-333333333333'
    ),
    (
        '20000000-0000-0000-0000-000000000003',
        'ffffffff-ffff-ffff-ffff-ffffffffffff',
        N'Status',
        N'Pending',
        N'InProgress',
        '2025-11-06 10:00:00',
        '33333333-3333-3333-3333-333333333333',
        N'Status alterado de Pending para InProgress',
        '33333333-3333-3333-3333-333333333333'
    );
GO

-- =================================================================
-- VERIFICAÇÃO DOS DADOS INSERIDOS
-- =================================================================
PRINT '======================================='
PRINT 'DADOS INSERIDOS COM SUCESSO!'
PRINT '======================================='
PRINT ''
PRINT 'Resumo:'
SELECT 'Usuários' AS Tabela, COUNT(*) AS Total FROM Users
UNION ALL
SELECT 'Projetos', COUNT(*) FROM Projects
UNION ALL
SELECT 'Membros em Projetos', COUNT(*) FROM ProjectUsers
UNION ALL
SELECT 'Tarefas', COUNT(*) FROM Tasks
UNION ALL
SELECT 'Responsáveis', COUNT(*) FROM TaskUsers
UNION ALL
SELECT 'Comentários', COUNT(*) FROM TaskComments
UNION ALL
SELECT 'Histórico', COUNT(*) FROM TaskHistories;
GO

PRINT ''
PRINT '======================================='
PRINT 'IDs IMPORTANTES PARA TESTES:'
PRINT '======================================='
PRINT 'Usuários:'
PRINT '  João Silva (Manager):   11111111-1111-1111-1111-111111111111'
PRINT '  Maria Santos (Analyst): 22222222-2222-2222-2222-222222222222'
PRINT '  Carlos Souza (Specialist): 33333333-3333-3333-3333-333333333333'
PRINT ''
PRINT 'Projetos:'
PRINT '  Sistema de Vendas:  aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'
PRINT '  Migração para Nuvem: bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'
PRINT ''
PRINT 'Tarefas:'
PRINT '  JWT Auth:     cccccccc-cccc-cccc-cccc-cccccccccccc (InProgress)'
PRINT '  Catálogo:     dddddddd-dddd-dddd-dddd-dddddddddddd (Completed)'
PRINT '  Pagamento:    eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee (Pending)'
PRINT '  Azure Config: ffffffff-ffff-ffff-ffff-ffffffffffff (InProgress)'
PRINT '======================================='
GO
