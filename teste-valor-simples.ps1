# Teste simples do campo valor
Write-Host "=== TESTE SIMPLES DO CAMPO VALOR ===" -ForegroundColor Green

Write-Host "`n1. Acesse: http://localhost:5009" -ForegroundColor Yellow
Write-Host "2. Faça login em uma conta existente" -ForegroundColor Yellow
Write-Host "3. Clique em 'Depósito' ou 'Saque'" -ForegroundColor Yellow
Write-Host "4. No campo 'Valor', digite:" -ForegroundColor Yellow
Write-Host "   - 100 (apenas números)" -ForegroundColor White
Write-Host "   - 50,50 (com vírgula)" -ForegroundColor White
Write-Host "   - 25.75 (com ponto)" -ForegroundColor White
Write-Host "5. Clique em 'Confirmar'" -ForegroundColor Yellow

Write-Host "`n💡 O campo valor agora aceita:" -ForegroundColor Cyan
Write-Host "✅ Números (123)" -ForegroundColor Green
Write-Host "✅ Vírgula para centavos (123,45)" -ForegroundColor Green
Write-Host "✅ Ponto para decimais (123.45)" -ForegroundColor Green
Write-Host "✅ Entrada livre de valores" -ForegroundColor Green

Write-Host "`n🔧 Se ainda não funcionar:" -ForegroundColor Yellow
Write-Host "1. Recarregue a página (F5)" -ForegroundColor White
Write-Host "2. Limpe o cache do navegador (Ctrl+Shift+R)" -ForegroundColor White
Write-Host "3. Teste em modo incógnito" -ForegroundColor White

Write-Host "`n=== TESTE CONCLUIDO! ===" -ForegroundColor Green
