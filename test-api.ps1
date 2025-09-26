# Script para testar a API Conta Corrente
$baseUrl = "http://localhost:5009"

Write-Host "=== TESTANDO API CONTA CORRENTE ===" -ForegroundColor Green

# 1. Criar conta
Write-Host "`n1. Criando conta..." -ForegroundColor Yellow
$contaResponse = Invoke-RestMethod -Uri "$baseUrl/api/contas" -Method POST -ContentType "application/json" -Body '{"numero": 12345, "nome": "Felipe Teste", "senha": "MinhaSenh@123"}'
Write-Host "Conta criada: $($contaResponse.id)" -ForegroundColor Green

# 2. Ativar conta
Write-Host "`n2. Ativando conta..." -ForegroundColor Yellow
$contaAtivada = Invoke-RestMethod -Uri "$baseUrl/api/contas/$($contaResponse.id)/ativar" -Method PATCH -ContentType "application/json" -Body '{"ativo": true}'
Write-Host "Conta ativada: $($contaAtivada.ativo)" -ForegroundColor Green

# 3. Login
Write-Host "`n3. Fazendo login..." -ForegroundColor Yellow
$loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -ContentType "application/json" -Body '{"numero": 12345, "senha": "MinhaSenh@123"}'
$token = $loginResponse.token
Write-Host "Login realizado com sucesso!" -ForegroundColor Green

# 4. Lançar crédito
Write-Host "`n4. Lançando crédito de R$ 1000,00..." -ForegroundColor Yellow
$headers = @{
    "Authorization" = "Bearer $token"
    "Idempotency-Key" = "11111111-1111-1111-1111-111111111111"
}
$movimentoResponse = Invoke-RestMethod -Uri "$baseUrl/api/contas/$($contaResponse.id)/movimentos" -Method POST -ContentType "application/json" -Headers $headers -Body '{"data": "24/09/2025", "tipo": "C", "valor": 1000.00}'
Write-Host "Crédito lançado! Saldo atual: R$ $($movimentoResponse.saldoAtual)" -ForegroundColor Green

# 5. Lançar débito
Write-Host "`n5. Lançando débito de R$ 250,00..." -ForegroundColor Yellow
$headers["Idempotency-Key"] = "22222222-2222-2222-2222-222222222222"
$movimentoResponse2 = Invoke-RestMethod -Uri "$baseUrl/api/contas/$($contaResponse.id)/movimentos" -Method POST -ContentType "application/json" -Headers $headers -Body '{"data": "24/09/2025", "tipo": "D", "valor": 250.00}'
Write-Host "Débito lançado! Saldo atual: R$ $($movimentoResponse2.saldoAtual)" -ForegroundColor Green

# 6. Consultar saldo
Write-Host "`n6. Consultando saldo..." -ForegroundColor Yellow
$saldoResponse = Invoke-RestMethod -Uri "$baseUrl/api/contas/$($contaResponse.id)/saldo" -Method GET -Headers @{"Authorization" = "Bearer $token"}
Write-Host "Saldo atual: R$ $($saldoResponse.saldoAtual)" -ForegroundColor Green

# 7. Consultar extrato
Write-Host "`n7. Consultando extrato..." -ForegroundColor Yellow
$extratoResponse = Invoke-RestMethod -Uri "$baseUrl/api/contas/$($contaResponse.id)/extrato" -Method GET -Headers @{"Authorization" = "Bearer $token"}
Write-Host "Extrato com $($extratoResponse.total) movimentos:" -ForegroundColor Green
foreach ($item in $extratoResponse.itens) {
    $tipo = if ($item.tipo -eq "C") { "Crédito" } else { "Débito" }
    Write-Host "  $($item.data) - $tipo - R$ $($item.valor)" -ForegroundColor Cyan
}

Write-Host "`n=== TESTES CONCLUÍDOS COM SUCESSO! ===" -ForegroundColor Green


