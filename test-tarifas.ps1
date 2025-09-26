# Script para testar o microsserviço de Tarifas
$baseUrl = "http://localhost:8082"
$apiBaseUrl = "http://localhost:8080"

Write-Host "=== TESTANDO MICROSSERVIÇO DE TARIFAS ===" -ForegroundColor Green

# 1. Login para obter token
Write-Host "`n1. Fazendo login..." -ForegroundColor Yellow
$loginResponse = Invoke-RestMethod -Uri "$apiBaseUrl/api/auth/login" -Method POST -ContentType "application/json" -Body '{"numero": 12345, "senha": "MinhaSenh@123"}'
$token = $loginResponse.token
Write-Host "Login realizado com sucesso!" -ForegroundColor Green

# 2. Criar tarifa de transferência
Write-Host "`n2. Criando tarifa de transferência..." -ForegroundColor Yellow
$tarifaTransferencia = Invoke-RestMethod -Uri "$baseUrl/api/tarifas" -Method POST -ContentType "application/json" -Headers @{"Authorization" = "Bearer $token"} -Body '{"tipoOperacao": "TRANSFERENCIA", "valor": 5.00, "descricao": "Tarifa de transferência entre contas"}'
Write-Host "Tarifa de transferência criada: $($tarifaTransferencia.id)" -ForegroundColor Green

# 3. Criar tarifa de saque
Write-Host "`n3. Criando tarifa de saque..." -ForegroundColor Yellow
$tarifaSaque = Invoke-RestMethod -Uri "$baseUrl/api/tarifas" -Method POST -ContentType "application/json" -Headers @{"Authorization" = "Bearer $token"} -Body '{"tipoOperacao": "SAQUE", "valor": 2.50, "descricao": "Tarifa de saque"}'
Write-Host "Tarifa de saque criada: $($tarifaSaque.id)" -ForegroundColor Green

# 4. Listar todas as tarifas
Write-Host "`n4. Listando todas as tarifas..." -ForegroundColor Yellow
$tarifas = Invoke-RestMethod -Uri "$baseUrl/api/tarifas" -Method GET -Headers @{"Authorization" = "Bearer $token"}
Write-Host "Total de tarifas: $($tarifas.Count)" -ForegroundColor Green
foreach ($tarifa in $tarifas) {
    Write-Host "  - $($tarifa.tipoOperacao): R$ $($tarifa.valor) - $($tarifa.descricao)" -ForegroundColor Cyan
}

# 5. Obter tarifa específica
Write-Host "`n5. Obtendo tarifa específica..." -ForegroundColor Yellow
$tarifaEspecifica = Invoke-RestMethod -Uri "$baseUrl/api/tarifas/$($tarifaTransferencia.id)" -Method GET -Headers @{"Authorization" = "Bearer $token"}
Write-Host "Tarifa obtida: $($tarifaEspecifica.tipoOperacao) - R$ $($tarifaEspecifica.valor)" -ForegroundColor Green

# 6. Obter tarifas cobradas (simular com uma conta existente)
Write-Host "`n6. Obtendo tarifas cobradas..." -ForegroundColor Yellow
try {
    $tarifasCobradas = Invoke-RestMethod -Uri "$baseUrl/api/tarifas/cobradas/teste-conta-id" -Method GET -Headers @{"Authorization" = "Bearer $token"}
    Write-Host "Tarifas cobradas obtidas com sucesso!" -ForegroundColor Green
} catch {
    Write-Host "Nenhuma tarifa cobrada encontrada (esperado para conta de teste)" -ForegroundColor Yellow
}

Write-Host "`n=== TESTES DO MICROSSERVIÇO DE TARIFAS CONCLUÍDOS! ===" -ForegroundColor Green

