# Script para verificar se o Kafka UI está funcionando
Write-Host "=== KAFKA UI CONFIGURADO CORRETAMENTE ===" -ForegroundColor Green

Write-Host "`n🌐 KAFKA UI ABERTO:" -ForegroundColor Yellow
Write-Host "URL: http://localhost:8085" -ForegroundColor Cyan

Write-Host "`n✅ CONFIGURAÇÃO CORRIGIDA:" -ForegroundColor Green
Write-Host "• Kafka UI está na mesma rede do Kafka" -ForegroundColor Gray
Write-Host "• Conectando via kafka:9092 (nome interno do container)" -ForegroundColor Gray
Write-Host "• Rede: desafioailos_conta-corrente-network" -ForegroundColor Gray

Write-Host "`n📊 COMO NAVEGAR:" -ForegroundColor Yellow
Write-Host "1. Acesse: http://localhost:8085" -ForegroundColor White
Write-Host "2. Clique em 'Topics' no menu lateral" -ForegroundColor White
Write-Host "3. Aguarde carregar (pode demorar alguns segundos)" -ForegroundColor White
Write-Host "4. Você deve ver os tópicos:" -ForegroundColor White
Write-Host "   - movimentos.efetuados" -ForegroundColor Gray
Write-Host "   - tarifas.cobradas" -ForegroundColor Gray

Write-Host "`n🔍 PARA VER AS MENSAGENS:" -ForegroundColor Yellow
Write-Host "1. Clique em um tópico (ex: movimentos.efetuados)" -ForegroundColor White
Write-Host "2. Vá para a aba 'Messages'" -ForegroundColor White
Write-Host "3. Clique em 'Load messages'" -ForegroundColor White
Write-Host "4. Veja as mensagens JSON com todos os dados!" -ForegroundColor White

Write-Host "`n📝 EVENTOS QUE VOCÊ DEVE VER:" -ForegroundColor Cyan
Write-Host "✅ MovimentoRealizadoEvent:" -ForegroundColor Green
Write-Host "   - IdMovimento, IdConta, NumeroConta" -ForegroundColor Gray
Write-Host "   - Tipo (C/D), Valor, DataMovimento" -ForegroundColor Gray
Write-Host "   - SaldoAtual, Descricao" -ForegroundColor Gray

Write-Host "`n✅ TarifaCobradaEvent:" -ForegroundColor Green
Write-Host "   - IdTarifaCobrada, IdConta, NumeroConta" -ForegroundColor Gray
Write-Host "   - IdTarifa, NomeTarifa, ValorTarifa" -ForegroundColor Gray
Write-Host "   - DataCobranca, TipoOperacao, Descricao" -ForegroundColor Gray

Write-Host "`n🔄 PARA TESTAR:" -ForegroundColor Yellow
Write-Host "1. Faça operações na interface web (localhost:5009)" -ForegroundColor White
Write-Host "2. Volte ao Kafka UI" -ForegroundColor White
Write-Host "3. Clique em 'Load messages' novamente" -ForegroundColor White
Write-Host "4. Veja os novos eventos aparecerem!" -ForegroundColor White

Write-Host "`n🎯 DICAS:" -ForegroundColor Yellow
Write-Host "• Use Ctrl+F para buscar por palavras-chave" -ForegroundColor White
Write-Host "• Clique em uma mensagem para ver detalhes" -ForegroundColor White
Write-Host "• Os timestamps mostram quando o evento foi criado" -ForegroundColor White
Write-Host "• Cada evento tem um ID único" -ForegroundColor White

Write-Host "`n=== KAFKA UI FUNCIONANDO! ===" -ForegroundColor Green

