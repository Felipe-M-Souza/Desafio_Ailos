# Script para testar funcionalidades básicas
$baseUrl = "http://localhost:5009"

Write-Host "=== TESTANDO FUNCIONALIDADES BÁSICAS ===" -ForegroundColor Green

try {
    # 1. Criar conta
    Write-Host "`n1. Criando conta..." -ForegroundColor Yellow
    $conta = Invoke-RestMethod -Uri "$baseUrl/api/contas" -Method POST -ContentType "application/json" -Body '{"numero": 12345, "nome": "Felipe Teste", "cpf": "12345678909", "senha": "MinhaSenh@123"}'
    Write-Host "Conta criada: $($conta.id)" -ForegroundColor Green

    # 2. Ativar conta
    Write-Host "`n2. Ativando conta..." -ForegroundColor Yellow
    $contaAtivada = Invoke-RestMethod -Uri "$baseUrl/api/contas/$($conta.id)/ativar" -Method PATCH -ContentType "application/json" -Body '{"ativo": true}'
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
    $movimentoResponse = Invoke-RestMethod -Uri "$baseUrl/api/contas/$($conta.id)/movimentos" -Method POST -ContentType "application/json" -Headers $headers -Body '{"data": "25/09/2025", "tipo": "C", "valor": 1000.00}'
    Write-Host "Crédito lançado! Saldo atual: R$ $($movimentoResponse.saldoAtual)" -ForegroundColor Green

    # 5. Consultar saldo
    Write-Host "`n5. Consultando saldo..." -ForegroundColor Yellow
    $saldoResponse = Invoke-RestMethod -Uri "$baseUrl/api/contas/$($conta.id)/saldo" -Method GET -Headers @{"Authorization" = "Bearer $token"}
    Write-Host "Saldo atual: R$ $($saldoResponse.saldoAtual)" -ForegroundColor Green

    Write-Host "`n=== TESTES BÁSICOS CONCLUÍDOS COM SUCESSO! ===" -ForegroundColor Green

} catch {
    Write-Host "`nErro durante os testes: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Verifique se a API está rodando na porta 5009" -ForegroundColor Yellow
}

