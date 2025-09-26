# Teste da máscara automática de valor
Write-Host "=== TESTE DA MÁSCARA AUTOMÁTICA DE VALOR ===" -ForegroundColor Green

Write-Host "`n💰 COMO A MÁSCARA FUNCIONA:" -ForegroundColor Yellow
Write-Host "1. Digite apenas números no campo valor" -ForegroundColor White
Write-Host "2. A formatação será aplicada automaticamente" -ForegroundColor White
Write-Host "3. Exemplos de como funciona:" -ForegroundColor White

Write-Host "`n📝 EXEMPLOS DE ENTRADA:" -ForegroundColor Cyan
Write-Host "Digite: 1000000000" -ForegroundColor Gray
Write-Host "Resultado: R$ 10.000.000,00" -ForegroundColor Green

Write-Host "`nDigite: 12345" -ForegroundColor Gray
Write-Host "Resultado: R$ 123,45" -ForegroundColor Green

Write-Host "`nDigite: 500" -ForegroundColor Gray
Write-Host "Resultado: R$ 5,00" -ForegroundColor Green

Write-Host "`nDigite: 1000" -ForegroundColor Gray
Write-Host "Resultado: R$ 10,00" -ForegroundColor Green

Write-Host "`n🔧 COMO TESTAR:" -ForegroundColor Yellow
Write-Host "1. Acesse: http://localhost:5009" -ForegroundColor White
Write-Host "2. Faça login em uma conta" -ForegroundColor White
Write-Host "3. Clique em 'Depósito' ou 'Saque'" -ForegroundColor White
Write-Host "4. No campo 'Valor', digite apenas números" -ForegroundColor White
Write-Host "5. Veja a formatação automática acontecer!" -ForegroundColor White

Write-Host "`n💡 DICAS:" -ForegroundColor Yellow
Write-Host "✅ Digite apenas números (sem pontos ou vírgulas)" -ForegroundColor Green
Write-Host "✅ A formatação acontece automaticamente" -ForegroundColor Green
Write-Host "✅ Os últimos 2 dígitos são sempre centavos" -ForegroundColor Green
Write-Host "✅ Milhares são separados por pontos" -ForegroundColor Green

Write-Host "`n=== TESTE DA MÁSCARA CONCLUÍDO! ===" -ForegroundColor Green

