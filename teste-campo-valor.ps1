# Script para testar o campo valor
$baseUrl = "http://localhost:5009"

Write-Host "=== TESTE DO CAMPO VALOR ===" -ForegroundColor Green

# 1. Criar conta
Write-Host "`n1. Criando conta..." -ForegroundColor Yellow
$conta = Invoke-RestMethod -Uri "$baseUrl/api/contas" -Method POST -ContentType "application/json" -Body '{"numero": 44444, "nome": "Teste Valor", "cpf": "11122233344", "senha": "Senha123"}'
Write-Host "Conta criada: $($conta.id)" -ForegroundColor Green

# 2. Ativar conta
Write-Host "`n2. Ativando conta..." -ForegroundColor Yellow
Invoke-RestMethod -Uri "$baseUrl/api/contas/$($conta.id)/ativar" -Method PATCH -ContentType "application/json" -Body '{"ativo": true}' | Out-Null
Write-Host "Conta ativada!" -ForegroundColor Green

# 3. Login
Write-Host "`n3. Fazendo login..." -ForegroundColor Yellow
$login = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -ContentType "application/json" -Body '{"numero": 44444, "senha": "Senha123"}'
$token = $login.token
Write-Host "Login realizado!" -ForegroundColor Green

# 4. Testar diferentes formatos de valor
Write-Host "`n4. Testando diferentes formatos de valor..." -ForegroundColor Yellow

$valores = @(
    @{ Valor = 100.50; Descricao = "Valor com ponto decimal" },
    @{ Valor = 250; Descricao = "Valor inteiro" },
    @{ Valor = 15.75; Descricao = "Valor com centavos" }
)

foreach ($valor in $valores) {
    Write-Host "`n   Testando: $($valor.Descricao) - R$ $($valor.Valor)" -ForegroundColor Cyan
    
    $headers = @{
        "Authorization" = "Bearer $token"
        "Idempotency-Key" = "test-valor-$(Get-Random)"
    }
    
    try {
        $movimento = Invoke-RestMethod -Uri "$baseUrl/api/contas/$($conta.id)/movimentos" -Method POST -ContentType "application/json" -Headers $headers -Body "{\"data\": \"25/09/2025\", \"tipo\": \"C\", \"valor\": $($valor.Valor)}"
        Write-Host "   ✅ Sucesso! Saldo: R$ $($movimento.saldoAtual)" -ForegroundColor Green
    } catch {
        Write-Host "   ❌ Erro: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n=== TESTE DO CAMPO VALOR CONCLUÍDO! ===" -ForegroundColor Green
Write-Host "`n💡 DICAS PARA O CAMPO VALOR:" -ForegroundColor Yellow
Write-Host "1. Digite apenas números (ex: 1234)" -ForegroundColor White
Write-Host "2. Use vírgula para centavos (ex: 123,45)" -ForegroundColor White
Write-Host "3. Use ponto para decimais (ex: 123.45)" -ForegroundColor White
Write-Host "4. A formatação será aplicada automaticamente" -ForegroundColor White

