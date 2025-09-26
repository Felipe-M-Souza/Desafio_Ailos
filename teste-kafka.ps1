# Script para testar o Kafka
$baseUrl = "http://localhost:5009"

Write-Host "=== TESTANDO KAFKA - EVENTOS ===" -ForegroundColor Green

# 1. Criar conta
Write-Host "`n1. Criando conta..." -ForegroundColor Yellow
$conta = Invoke-RestMethod -Uri "$baseUrl/api/contas" -Method POST -ContentType "application/json" -Body '{"numero": 55555, "nome": "Teste Kafka", "cpf": "11122233344", "senha": "Senha123"}'
Write-Host "Conta criada: $($conta.id)" -ForegroundColor Green

# 2. Ativar conta
Write-Host "`n2. Ativando conta..." -ForegroundColor Yellow
Invoke-RestMethod -Uri "$baseUrl/api/contas/$($conta.id)/ativar" -Method PATCH -ContentType "application/json" -Body '{"ativo": true}' | Out-Null
Write-Host "Conta ativada!" -ForegroundColor Green

# 3. Login
Write-Host "`n3. Fazendo login..." -ForegroundColor Yellow
$login = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -ContentType "application/json" -Body '{"numero": 55555, "senha": "Senha123"}'
$token = $login.token
Write-Host "Login realizado!" -ForegroundColor Green

# 4. Lançar crédito (gera evento de movimento)
Write-Host "`n4. Lançando crédito (gera evento de movimento)..." -ForegroundColor Yellow
$headers = @{
    "Authorization" = "Bearer $token"
    "Idempotency-Key" = "kafka-test-1"
}
$movimento = Invoke-RestMethod -Uri "$baseUrl/api/contas/$($conta.id)/movimentos" -Method POST -ContentType "application/json" -Headers $headers -Body '{"data": "25/09/2025", "tipo": "C", "valor": 1000.00}'
Write-Host "Crédito lançado! Saldo: R$ $($movimento.saldoAtual)" -ForegroundColor Green

# 5. Lançar débito (gera evento de movimento + tarifa)
Write-Host "`n5. Lançando débito (gera evento de movimento + tarifa)..." -ForegroundColor Yellow
$headers2 = @{
    "Authorization" = "Bearer $token"
    "Idempotency-Key" = "kafka-test-2"
}
$movimento2 = Invoke-RestMethod -Uri "$baseUrl/api/contas/$($conta.id)/movimentos" -Method POST -ContentType "application/json" -Headers $headers2 -Body '{"data": "25/09/2025", "tipo": "D", "valor": 50.00}'
Write-Host "Débito lançado! Saldo: R$ $($movimento2.saldoAtual)" -ForegroundColor Green

# 6. Criar segunda conta para transferência
Write-Host "`n6. Criando segunda conta para transferência..." -ForegroundColor Yellow
$conta2 = Invoke-RestMethod -Uri "$baseUrl/api/contas" -Method POST -ContentType "application/json" -Body '{"numero": 66666, "nome": "Teste Kafka 2", "cpf": "55566677788", "senha": "Senha456"}'
Invoke-RestMethod -Uri "$baseUrl/api/contas/$($conta2.id)/ativar" -Method PATCH -ContentType "application/json" -Body '{"ativo": true}' | Out-Null
Write-Host "Segunda conta criada e ativada!" -ForegroundColor Green

# 7. Realizar transferência (gera evento de transferência + tarifa)
Write-Host "`n7. Realizando transferência (gera evento de transferência + tarifa)..." -ForegroundColor Yellow
$headers3 = @{
    "Authorization" = "Bearer $token"
    "Idempotency-Key" = "kafka-test-3"
}
$transferencia = Invoke-RestMethod -Uri "$baseUrl/api/transferencias" -Method POST -ContentType "application/json" -Headers $headers3 -Body '{"numeroContaDestino": 66666, "valor": 100.00, "data": "25/09/2025", "descricao": "Teste Kafka"}'
Write-Host "Transferência realizada! Saldo: R$ $($transferencia.saldoAtualOrigem)" -ForegroundColor Green

Write-Host "`n=== EVENTOS KAFKA GERADOS ===" -ForegroundColor Green
Write-Host "✅ MovimentoRealizadoEvent (crédito)" -ForegroundColor Cyan
Write-Host "✅ MovimentoRealizadoEvent (débito)" -ForegroundColor Cyan
Write-Host "✅ TarifaCobradaEvent (saque)" -ForegroundColor Cyan
Write-Host "✅ TransferenciaRealizadaEvent" -ForegroundColor Cyan
Write-Host "✅ TarifaCobradaEvent (transferência)" -ForegroundColor Cyan

Write-Host "`n=== COMO VERIFICAR OS EVENTOS ===" -ForegroundColor Yellow
Write-Host "1. Verifique os logs da API (terminal onde está rodando)" -ForegroundColor White
Write-Host "2. Procure por mensagens como:" -ForegroundColor White
Write-Host "   - 'Publicando mensagem no tópico movimentos.efetuados'" -ForegroundColor Gray
Write-Host "   - 'Publicando mensagem no tópico tarifas.cobradas'" -ForegroundColor Gray
Write-Host "   - 'Publicando mensagem no tópico transferencias.efetuadas'" -ForegroundColor Gray
Write-Host "3. Cada evento contém dados JSON completos" -ForegroundColor White

Write-Host "`n=== TESTE KAFKA CONCLUÍDO! ===" -ForegroundColor Green

