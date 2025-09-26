# Teste do campo CPF
$baseUrl = "http://localhost:5009"

Write-Host "=== TESTE DO CAMPO CPF ===" -ForegroundColor Green

# 1. Criar conta com CPF formatado
Write-Host "`n1. Criando conta com CPF..." -ForegroundColor Yellow
$conta = Invoke-RestMethod -Uri "$baseUrl/api/contas" -Method POST -ContentType "application/json" -Body '{"numero": 77777, "nome": "Teste CPF", "cpf": "12345678909", "senha": "Senha123"}'
Write-Host "Conta criada: $($conta.id)" -ForegroundColor Green

# 2. Tentar criar conta com CPF inválido
Write-Host "`n2. Testando CPF inválido..." -ForegroundColor Yellow
try {
    $contaInvalida = Invoke-RestMethod -Uri "$baseUrl/api/contas" -Method POST -ContentType "application/json" -Body '{"numero": 77778, "nome": "Teste CPF Inválido", "cpf": "12345678900", "senha": "Senha123"}'
    Write-Host "ERRO: CPF inválido foi aceito!" -ForegroundColor Red
} catch {
    Write-Host "CPF inválido rejeitado corretamente: $($_.Exception.Message)" -ForegroundColor Green
}

# 3. Tentar criar conta com CPF duplicado
Write-Host "`n3. Testando CPF duplicado..." -ForegroundColor Yellow
try {
    $contaDuplicada = Invoke-RestMethod -Uri "$baseUrl/api/contas" -Method POST -ContentType "application/json" -Body '{"numero": 77779, "nome": "Teste CPF Duplicado", "cpf": "12345678909", "senha": "Senha123"}'
    Write-Host "ERRO: CPF duplicado foi aceito!" -ForegroundColor Red
} catch {
    Write-Host "CPF duplicado rejeitado corretamente: $($_.Exception.Message)" -ForegroundColor Green
}

Write-Host "`n=== TESTE DO CPF CONCLUÍDO! ===" -ForegroundColor Green

