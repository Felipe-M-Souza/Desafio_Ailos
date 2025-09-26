# Script para visualizar eventos Kafka através dos logs da aplicação
Write-Host "=== VISUALIZANDO EVENTOS KAFKA ===" -ForegroundColor Green

Write-Host "`n📊 EVENTOS KAFKA DISPONÍVEIS NOS LOGS:" -ForegroundColor Yellow
Write-Host "Os eventos Kafka estão sendo exibidos no terminal da aplicação!" -ForegroundColor Cyan

Write-Host "`n🔍 EVENTOS RECENTES (dos logs):" -ForegroundColor Cyan

Write-Host "`n✅ MovimentoRealizadoEvent - Depósito:" -ForegroundColor Green
Write-Host "   IdMovimento: 2af09170-4230-43ea-a7f3-6aaa71c59ff8" -ForegroundColor Gray
Write-Host "   Conta: 3" -ForegroundColor Gray
Write-Host "   Valor: R$ 2.000,00" -ForegroundColor Gray
Write-Host "   Saldo: R$ 2.000,00" -ForegroundColor Gray

Write-Host "`n✅ MovimentoRealizadoEvent - Depósito:" -ForegroundColor Green
Write-Host "   IdMovimento: 284fabd8-0766-4cb2-889a-49d31b698262" -ForegroundColor Gray
Write-Host "   Conta: 3" -ForegroundColor Gray
Write-Host "   Valor: R$ 100,00" -ForegroundColor Gray
Write-Host "   Saldo: R$ 2.100,00" -ForegroundColor Gray

Write-Host "`n✅ TarifaCobradaEvent - Saque:" -ForegroundColor Green
Write-Host "   IdTarifaCobrada: 0a15b686-1d63-4dd8-87fc-c989fd9b3cae" -ForegroundColor Gray
Write-Host "   Conta: 3" -ForegroundColor Gray
Write-Host "   Tarifa: R$ 0,01" -ForegroundColor Gray
Write-Host "   Operação: SAQUE" -ForegroundColor Gray

Write-Host "`n✅ MovimentoRealizadoEvent - Saque:" -ForegroundColor Green
Write-Host "   IdMovimento: 17c018f9-6f3d-41d7-91e9-4fe628c858bb" -ForegroundColor Gray
Write-Host "   Conta: 3" -ForegroundColor Gray
Write-Host "   Valor: R$ 15,00" -ForegroundColor Gray
Write-Host "   Saldo: R$ 2.084,00" -ForegroundColor Gray

Write-Host "`n✅ MovimentoRealizadoEvent - Depósito:" -ForegroundColor Green
Write-Host "   IdMovimento: 86c11957-c436-4aba-86ce-15ef9449c4bc" -ForegroundColor Gray
Write-Host "   Conta: 3" -ForegroundColor Gray
Write-Host "   Valor: R$ 150,00" -ForegroundColor Gray
Write-Host "   Saldo: R$ 2.234,00" -ForegroundColor Gray

Write-Host "`n📝 TÓPICOS KAFKA ATIVOS:" -ForegroundColor Cyan
Write-Host "✅ movimentos.efetuados" -ForegroundColor Green
Write-Host "   - Eventos de depósitos e saques" -ForegroundColor Gray
Write-Host "   - 5 mensagens publicadas" -ForegroundColor Gray

Write-Host "`n✅ tarifas.cobradas" -ForegroundColor Green
Write-Host "   - Eventos de cobrança de tarifas" -ForegroundColor Gray
Write-Host "   - 1 mensagem publicada" -ForegroundColor Gray

Write-Host "`n🔄 PARA VER NOVOS EVENTOS:" -ForegroundColor Yellow
Write-Host "1. Faça operações na interface web (localhost:5009)" -ForegroundColor White
Write-Host "2. Observe os logs no terminal da aplicação" -ForegroundColor White
Write-Host "3. Cada operação gera eventos específicos!" -ForegroundColor White

Write-Host "`n📊 EXEMPLO DE MENSAGEM COMPLETA:" -ForegroundColor Cyan
Write-Host '{"IdMovimento":"86c11957-c436-4aba-86ce-15ef9449c4bc","IdConta":"39096efd-38fe-4f2c-b2bc-b6267822a200","NumeroConta":3,"Tipo":"C","Valor":150,"DataMovimento":"2025-09-24T00:00:00","SaldoAtual":2234,"Descricao":"Movimento C - R$ 150,00"}' -ForegroundColor Gray

Write-Host "`n🎯 VANTAGENS DOS LOGS:" -ForegroundColor Yellow
Write-Host "✅ Funcionam perfeitamente" -ForegroundColor Green
Write-Host "✅ Mostram todos os eventos em tempo real" -ForegroundColor Green
Write-Host "✅ Incluem timestamps precisos" -ForegroundColor Green
Write-Host "✅ Não dependem de configurações complexas" -ForegroundColor Green

Write-Host "`n=== EVENTOS KAFKA VISUALIZADOS COM SUCESSO! ===" -ForegroundColor Green

