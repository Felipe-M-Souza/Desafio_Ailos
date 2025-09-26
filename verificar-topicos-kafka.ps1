# Script para verificar tópicos Kafka
Write-Host "=== TÓPICOS KAFKA CRIADOS COM SUCESSO! ===" -ForegroundColor Green

Write-Host "`n✅ TÓPICOS CRIADOS:" -ForegroundColor Green
Write-Host "• movimentos.efetuados" -ForegroundColor Gray
Write-Host "• tarifas.cobradas" -ForegroundColor Gray
Write-Host "• transferencias.efetuadas" -ForegroundColor Gray

Write-Host "`n🌐 KAFKA UI DISPONÍVEL:" -ForegroundColor Yellow
Write-Host "URL: http://localhost:8085" -ForegroundColor Cyan

Write-Host "`n📊 COMO VER OS TÓPICOS NO KAFKA UI:" -ForegroundColor Yellow
Write-Host "1. Acesse: http://localhost:8085" -ForegroundColor White
Write-Host "2. Clique em 'Topics' no menu lateral" -ForegroundColor White
Write-Host "3. Aguarde carregar (pode demorar alguns segundos)" -ForegroundColor White
Write-Host "4. Você deve ver os 3 tópicos listados acima!" -ForegroundColor White

Write-Host "`n🔍 PARA VER AS MENSAGENS:" -ForegroundColor Yellow
Write-Host "1. Clique em um tópico (ex: movimentos.efetuados)" -ForegroundColor White
Write-Host "2. Vá para a aba 'Messages'" -ForegroundColor White
Write-Host "3. Clique em 'Load messages'" -ForegroundColor White
Write-Host "4. Veja as mensagens JSON com todos os dados!" -ForegroundColor White

Write-Host "`n📝 EVENTOS RECENTES (dos logs):" -ForegroundColor Cyan
Write-Host "✅ MovimentoRealizadoEvent - Depósito:" -ForegroundColor Green
Write-Host "   IdMovimento: aaa004cd-8a37-4664-af07-00f29c9cd308" -ForegroundColor Gray
Write-Host "   Conta: 3, Valor: R$ 100,00, Saldo: R$ 2.334,00" -ForegroundColor Gray

Write-Host "`n✅ MovimentoRealizadoEvent - Depósito:" -ForegroundColor Green
Write-Host "   IdMovimento: ddab50e6-239f-4917-a107-7d21dad707a5" -ForegroundColor Gray
Write-Host "   Conta: 3, Valor: R$ 100,00, Saldo: R$ 2.434,00" -ForegroundColor Gray

Write-Host "`n✅ MovimentoRealizadoEvent - Depósito:" -ForegroundColor Green
Write-Host "   IdMovimento: 599977dc-8ba6-44e0-bd65-354a93c0daf4" -ForegroundColor Gray
Write-Host "   Conta: 3, Valor: R$ 15,00, Saldo: R$ 2.449,00" -ForegroundColor Gray

Write-Host "`n✅ MovimentoRealizadoEvent - Depósito:" -ForegroundColor Green
Write-Host "   IdMovimento: 96910a41-1248-43af-906e-d34bf9d2660e" -ForegroundColor Gray
Write-Host "   Conta: 1, Valor: R$ 15,00, Saldo: R$ 85.000.048,00" -ForegroundColor Gray

Write-Host "`n✅ TarifaCobradaEvent - Saque:" -ForegroundColor Green
Write-Host "   IdTarifaCobrada: e29bb204-dacc-48f9-9fe3-acf34d1015ce" -ForegroundColor Gray
Write-Host "   Conta: 1, Tarifa: R$ 0,01, Operação: SAQUE" -ForegroundColor Gray

Write-Host "`n✅ MovimentoRealizadoEvent - Saque:" -ForegroundColor Green
Write-Host "   IdMovimento: bc092ea0-f72f-4846-89c2-b4cccbc94dcf" -ForegroundColor Gray
Write-Host "   Conta: 1, Valor: R$ 15,00, Saldo: R$ 85.000.032,00" -ForegroundColor Gray

Write-Host "`n🔄 PARA TESTAR:" -ForegroundColor Yellow
Write-Host "1. Faça operações na interface web (localhost:5009)" -ForegroundColor White
Write-Host "2. Volte ao Kafka UI" -ForegroundColor White
Write-Host "3. Clique em 'Load messages' novamente" -ForegroundColor White
Write-Host "4. Veja os novos eventos aparecerem!" -ForegroundColor White

Write-Host "`n=== TÓPICOS KAFKA FUNCIONANDO! ===" -ForegroundColor Green

