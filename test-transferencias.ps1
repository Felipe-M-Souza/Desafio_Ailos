# Script para testar o microsserviço de Transferências
$baseUrl = "http://localhost:8081"
$apiBaseUrl = "http://localhost:8080"

Write-Host "=== TESTANDO MICROSSERVIÇO DE TRANSFERÊNCIAS ===" -ForegroundColor Green

# 1. Criar conta origem
Write-Host "`n1. Criando conta origem..." -ForegroundColor Yellow
$contaOrigem = Invoke-RestMethod -Uri "$apiBaseUrl/api/contas" -Method POST -ContentType "application/json" -Body '{"numero": 11111, "nome": "João Origem", "cpf": "11111111111", "senha": "MinhaSenh@123"}'
Write-Host "Conta origem criada: $($contaOrigem.id)" -ForegroundColor Green

# 2. Ativar conta origem
Write-Host "`n2. Ativando conta origem..." -ForegroundColor Yellow
$contaAtivada = Invoke-RestMethod -Uri "$apiBaseUrl/api/contas/$($contaOrigem.id)/ativar" -Method PATCH -ContentType "application/json" -Body '{"ativo": true}'
Write-Host "Conta origem ativada: $($contaAtivada.ativo)" -ForegroundColor Green

# 3. Criar conta destino
Write-Host "`n3. Criando conta destino..." -ForegroundColor Yellow
$contaDestino = Invoke-RestMethod -Uri "$apiBaseUrl/api/contas" -Method POST -ContentType "application/json" -Body '{"numero": 22222, "nome": "Maria Destino", "cpf": "22222222222", "senha": "MinhaSenh@123"}'
Write-Host "Conta destino criada: $($contaDestino.id)" -ForegroundColor Green

# 4. Ativar conta destino
Write-Host "`n4. Ativando conta destino..." -ForegroundColor Yellow
$contaDestinoAtivada = Invoke-RestMethod -Uri "$apiBaseUrl/api/contas/$($contaDestino.id)/ativar" -Method PATCH -ContentType "application/json" -Body '{"ativo": true}'
Write-Host "Conta destino ativada: $($contaDestinoAtivada.ativo)" -ForegroundColor Green

# 5. Login na conta origem
Write-Host "`n5. Fazendo login na conta origem..." -ForegroundColor Yellow
$loginResponse = Invoke-RestMethod -Uri "$apiBaseUrl/api/auth/login" -Method POST -ContentType "application/json" -Body '{"numero": 11111, "senha": "MinhaSenh@123"}'
$token = $loginResponse.token
Write-Host "Login realizado com sucesso!" -ForegroundColor Green

# 6. Lançar crédito na conta origem
Write-Host "`n6. Lançando crédito de R$ 1000,00 na conta origem..." -ForegroundColor Yellow
$headers = @{
    "Authorization" = "Bearer $token"
    "Idempotency-Key" = "11111111-1111-1111-1111-111111111111"
}
$movimentoResponse = Invoke-RestMethod -Uri "$apiBaseUrl/api/contas/$($contaOrigem.id)/movimentos" -Method POST -ContentType "application/json" -Headers $headers -Body '{"data": "25/09/2025", "tipo": "C", "valor": 1000.00}'
Write-Host "Crédito lançado! Saldo atual: R$ $($movimentoResponse.saldoAtual)" -ForegroundColor Green

# 7. Realizar transferência
Write-Host "`n7. Realizando transferência de R$ 300,00..." -ForegroundColor Yellow
$headers["Idempotency-Key"] = "22222222-2222-2222-2222-222222222222"
$transferenciaResponse = Invoke-RestMethod -Uri "$baseUrl/api/transferencias" -Method POST -ContentType "application/json" -Headers $headers -Body '{"numeroContaDestino": 22222, "valor": 300.00, "data": "25/09/2025", "descricao": "Transferência teste"}'
Write-Host "Transferência realizada! Saldo atual: R$ $($transferenciaResponse.saldoAtual)" -ForegroundColor Green

# 8. Consultar histórico de transferências
Write-Host "`n8. Consultando histórico de transferências..." -ForegroundColor Yellow
$historicoResponse = Invoke-RestMethod -Uri "$baseUrl/api/transferencias/historico" -Method GET -Headers @{"Authorization" = "Bearer $token"}
Write-Host "Histórico obtido com sucesso!" -ForegroundColor Green

Write-Host "`n=== TESTES DO MICROSSERVIÇO DE TRANSFERÊNCIAS CONCLUÍDOS! ===" -ForegroundColor Green
