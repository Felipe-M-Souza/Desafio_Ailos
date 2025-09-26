# Script para verificar logs no Kafka
Write-Host "=== VERIFICANDO LOGS NO KAFKA ===" -ForegroundColor Green

Write-Host "`n🌐 KAFKA UI ABERTO:" -ForegroundColor Yellow
Write-Host "URL: http://localhost:8085" -ForegroundColor Cyan

Write-Host "`n📊 COMO NAVEGAR NO KAFKA UI:" -ForegroundColor Yellow
Write-Host "1. No menu lateral, clique em 'Topics'" -ForegroundColor White
Write-Host "2. Você verá os tópicos:" -ForegroundColor White
Write-Host "   - movimentos.efetuados" -ForegroundColor Gray
Write-Host "   - transferencias.efetuadas" -ForegroundColor Gray
Write-Host "   - tarifas.cobradas" -ForegroundColor Gray

Write-Host "`n🔍 COMO VER AS MENSAGENS:" -ForegroundColor Yellow
Write-Host "1. Clique em um tópico (ex: movimentos.efetuados)" -ForegroundColor White
Write-Host "2. Vá para a aba 'Messages'" -ForegroundColor White
Write-Host "3. Clique em 'Load messages'" -ForegroundColor White
Write-Host "4. Veja as mensagens JSON com todos os dados" -ForegroundColor White

Write-Host "`n📝 EVENTOS QUE VOCÊ DEVE VER:" -ForegroundColor Cyan
Write-Host "✅ MovimentoRealizadoEvent:" -ForegroundColor Green
Write-Host "   - IdMovimento, IdConta, NumeroConta" -ForegroundColor Gray
Write-Host "   - Tipo (C/D), Valor, DataMovimento" -ForegroundColor Gray
Write-Host "   - SaldoAtual, Descricao" -ForegroundColor Gray

Write-Host "`n✅ TarifaCobradaEvent:" -ForegroundColor Green
Write-Host "   - IdTarifaCobrada, IdConta, NumeroConta" -ForegroundColor Gray
Write-Host "   - IdTarifa, NomeTarifa, ValorTarifa" -ForegroundColor Gray
Write-Host "   - DataCobranca, TipoOperacao, Descricao" -ForegroundColor Gray

Write-Host "`n✅ TransferenciaRealizadaEvent:" -ForegroundColor Green
Write-Host "   - IdTransferencia, IdContaOrigem, IdContaDestino" -ForegroundColor Gray
Write-Host "   - NumeroContaOrigem, NumeroContaDestino" -ForegroundColor Gray
Write-Host "   - Valor, DataTransferencia, Descricao" -ForegroundColor Gray
Write-Host "   - IdMovimentoOrigem, IdMovimentoDestino" -ForegroundColor Gray
Write-Host "   - SaldoContaOrigem, SaldoContaDestino" -ForegroundColor Gray

Write-Host "`n🔄 PARA VER NOVOS EVENTOS:" -ForegroundColor Yellow
Write-Host "1. Faça operações na interface (depósito, saque, transferência)" -ForegroundColor White
Write-Host "2. Volte ao Kafka UI" -ForegroundColor White
Write-Host "3. Clique em 'Load messages' novamente" -ForegroundColor White
Write-Host "4. Veja os novos eventos aparecerem!" -ForegroundColor White

Write-Host "`n📊 EXEMPLO DE MENSAGEM (MovimentoRealizadoEvent):" -ForegroundColor Cyan
Write-Host '{"IdMovimento":"f1bd9e5d-90a1-41ae-90d6-0381a5061570","IdConta":"3797b410-455e-4bae-a554-891288e899d7","NumeroConta":1,"Tipo":"D","Valor":15000000,"DataMovimento":"2025-09-24T00:00:00","SaldoAtual":85000034,"Descricao":"Movimento D - R$ 15.000.000,00"}' -ForegroundColor Gray

Write-Host "`n🎯 DICAS:" -ForegroundColor Yellow
Write-Host "• Use Ctrl+F para buscar por palavras-chave" -ForegroundColor White
Write-Host "• Clique em uma mensagem para ver detalhes" -ForegroundColor White
Write-Host "• Os timestamps mostram quando o evento foi criado" -ForegroundColor White
Write-Host "• Cada evento tem um ID único" -ForegroundColor White

Write-Host "`n=== VERIFICAÇÃO DE LOGS CONCLUÍDA! ===" -ForegroundColor Green

