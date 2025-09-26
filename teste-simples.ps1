# Teste simples da API
$baseUrl = "http://localhost:5009"

Write-Host "=== TESTE SIMPLES DA API ===" -ForegroundColor Green

# 1. Criar conta
Write-Host "`n1. Criando conta..." -ForegroundColor Yellow
$conta = Invoke-RestMethod -Uri "$baseUrl/api/contas" -Method POST -ContentType "application/json" -Body '{"numero": 88888, "nome": "Teste API", "cpf": "98765432100", "senha": "Senha123"}'
Write-Host "Conta criada: $($conta.id)" -ForegroundColor Green

# 2. Ativar conta
Write-Host "`n2. Ativando conta..." -ForegroundColor Yellow
Invoke-RestMethod -Uri "$baseUrl/api/contas/$($conta.id)/ativar" -Method PATCH -ContentType "application/json" -Body '{"ativo": true}' | Out-Null
Write-Host "Conta ativada!" -ForegroundColor Green

# 3. Login
Write-Host "`n3. Fazendo login..." -ForegroundColor Yellow
$login = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -ContentType "application/json" -Body '{"numero": 88888, "senha": "Senha123"}'
$token = $login.token
Write-Host "Login realizado!" -ForegroundColor Green

# 4. Lançar crédito
Write-Host "`n4. Lançando crédito..." -ForegroundColor Yellow
$headers = @{
    "Authorization" = "Bearer $token"
    "Idempotency-Key" = "11111111-1111-1111-1111-111111111111"
}
$movimento = Invoke-RestMethod -Uri "$baseUrl/api/contas/$($conta.id)/movimentos" -Method POST -ContentType "application/json" -Headers $headers -Body '{"data": "25/09/2025", "tipo": "C", "valor": 500.00}'
Write-Host "Crédito lançado! Saldo: R$ $($movimento.saldoAtual)" -ForegroundColor Green

# 5. Consultar saldo
Write-Host "`n5. Consultando saldo..." -ForegroundColor Yellow
$saldo = Invoke-RestMethod -Uri "$baseUrl/api/contas/$($conta.id)/saldo" -Method GET -Headers @{"Authorization" = "Bearer $token"}
Write-Host "Saldo atual: R$ $($saldo.saldoAtual)" -ForegroundColor Green

Write-Host "`n=== TESTE CONCLUÍDO COM SUCESSO! ===" -ForegroundColor Green

