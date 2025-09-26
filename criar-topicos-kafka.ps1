# Script para criar tópicos Kafka através de operações na interface
Write-Host "=== CRIANDO TÓPICOS KAFKA ===" -ForegroundColor Green

Write-Host "`n🌐 INTERFACE ABERTA:" -ForegroundColor Yellow
Write-Host "URL: http://localhost:5009" -ForegroundColor Cyan

Write-Host "`n📊 KAFKA UI ABERTO:" -ForegroundColor Yellow
Write-Host "URL: http://localhost:8085" -ForegroundColor Cyan

Write-Host "`n🎯 COMO CRIAR OS TÓPICOS:" -ForegroundColor Yellow
Write-Host "1. Na interface web (localhost:5009):" -ForegroundColor White
Write-Host "   - Faça login com uma conta existente" -ForegroundColor Gray
Write-Host "   - OU crie uma nova conta" -ForegroundColor Gray

Write-Host "`n2. Execute operações para criar os tópicos:" -ForegroundColor White
Write-Host "   ✅ Depósito → Cria tópico 'movimentos.efetuados'" -ForegroundColor Green
Write-Host "   ✅ Saque → Cria tópicos 'tarifas.cobradas' + 'movimentos.efetuados'" -ForegroundColor Green
Write-Host "   ✅ Transferência → Cria tópico 'transferencias.efetuadas'" -ForegroundColor Green

Write-Host "`n3. Volte ao Kafka UI (localhost:8085):" -ForegroundColor White
Write-Host "   - Clique em 'Topics' no menu lateral" -ForegroundColor Gray
Write-Host "   - Aguarde alguns segundos" -ForegroundColor Gray
Write-Host "   - Os tópicos aparecerão automaticamente!" -ForegroundColor Gray

Write-Host "`n📝 TÓPICOS QUE SERÃO CRIADOS:" -ForegroundColor Cyan
Write-Host "✅ movimentos.efetuados" -ForegroundColor Green
Write-Host "   - Eventos de depósitos e saques" -ForegroundColor Gray
Write-Host "   - Contém: IdMovimento, IdConta, Tipo, Valor, etc." -ForegroundColor Gray

Write-Host "`n✅ tarifas.cobradas" -ForegroundColor Green
Write-Host "   - Eventos de cobrança de tarifas" -ForegroundColor Gray
Write-Host "   - Contém: IdTarifaCobrada, IdConta, ValorTarifa, etc." -ForegroundColor Gray

Write-Host "`n✅ transferencias.efetuadas" -ForegroundColor Green
Write-Host "   - Eventos de transferências entre contas" -ForegroundColor Gray
Write-Host "   - Contém: IdTransferencia, ContaOrigem, ContaDestino, etc." -ForegroundColor Gray

Write-Host "`n🔄 PROCESSO PASSO A PASSO:" -ForegroundColor Yellow
Write-Host "1. Interface: Faça login/cadastro" -ForegroundColor White
Write-Host "2. Interface: Execute um depósito (R$ 100,00)" -ForegroundColor White
Write-Host "3. Kafka UI: Clique em 'Topics' → Verá 'movimentos.efetuados'" -ForegroundColor White
Write-Host "4. Interface: Execute um saque (R$ 50,00)" -ForegroundColor White
Write-Host "5. Kafka UI: Atualize → Verá 'tarifas.cobradas' também" -ForegroundColor White
Write-Host "6. Interface: Execute uma transferência" -ForegroundColor White
Write-Host "7. Kafka UI: Atualize → Verá 'transferencias.efetuadas'" -ForegroundColor White

Write-Host "`n💡 DICA:" -ForegroundColor Yellow
Write-Host "Os tópicos são criados automaticamente quando a primeira mensagem é publicada!" -ForegroundColor White
Write-Host "Por isso precisamos fazer operações na interface primeiro." -ForegroundColor White

Write-Host "`n=== TÓPICOS SERÃO CRIADOS APÓS OPERAÇÕES! ===" -ForegroundColor Green

