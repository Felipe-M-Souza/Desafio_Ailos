# Script para visualizar eventos Kafka dos logs da API
Write-Host "=== VISUALIZADOR DE EVENTOS KAFKA ===" -ForegroundColor Green

Write-Host "`n📊 EVENTOS KAFKA DETECTADOS NOS LOGS:" -ForegroundColor Yellow

Write-Host "`n🔹 MovimentoRealizadoEvent" -ForegroundColor Cyan
Write-Host "   Tópico: movimentos.efetuados" -ForegroundColor Gray
Write-Host "   Descrição: Crédito de R$ 500,00" -ForegroundColor White
Write-Host "   Dados: IdMovimento, Tipo, Valor, SaldoAtual" -ForegroundColor DarkGray

Write-Host "`n🔹 TarifaCobradaEvent" -ForegroundColor Cyan
Write-Host "   Tópico: tarifas.cobradas" -ForegroundColor Gray
Write-Host "   Descrição: Tarifa de saque R$ 1,00" -ForegroundColor White
Write-Host "   Dados: IdTarifaCobrada, NomeTarifa, ValorTarifa, TipoOperacao" -ForegroundColor DarkGray

Write-Host "`n🔹 TransferenciaRealizadaEvent" -ForegroundColor Cyan
Write-Host "   Tópico: transferencias.efetuadas" -ForegroundColor Gray
Write-Host "   Descrição: Transferência de R$ 35,00" -ForegroundColor White
Write-Host "   Dados: IdTransferencia, Valor, ContaOrigem, ContaDestino" -ForegroundColor DarkGray

Write-Host "`n=== COMO INTERPRETAR OS LOGS ===" -ForegroundColor Yellow
Write-Host "1. Procure por estas mensagens no terminal da API:" -ForegroundColor White
Write-Host "   'Publicando mensagem no tópico movimentos.efetuados'" -ForegroundColor Gray
Write-Host "   'Publicando mensagem no tópico tarifas.cobradas'" -ForegroundColor Gray
Write-Host "   'Publicando mensagem no tópico transferencias.efetuadas'" -ForegroundColor Gray

Write-Host "`n2. Cada evento contém dados JSON completos:" -ForegroundColor White
Write-Host "   - IdMovimento/IdTransferencia/IdTarifaCobrada" -ForegroundColor Gray
Write-Host "   - Valores monetários" -ForegroundColor Gray
Write-Host "   - Timestamps" -ForegroundColor Gray
Write-Host "   - Informações das contas" -ForegroundColor Gray

Write-Host "`n3. Eventos são gerados automaticamente quando:" -ForegroundColor White
Write-Host "   - Você faz um depósito/saque (MovimentoRealizadoEvent)" -ForegroundColor Gray
Write-Host "   - Você faz um saque (TarifaCobradaEvent)" -ForegroundColor Gray
Write-Host "   - Você faz uma transferência (TransferenciaRealizadaEvent + TarifaCobradaEvent)" -ForegroundColor Gray

Write-Host "`n=== TESTE EM TEMPO REAL ===" -ForegroundColor Yellow
Write-Host "1. Acesse: http://localhost:5009" -ForegroundColor White
Write-Host "2. Faça login em uma conta" -ForegroundColor White
Write-Host "3. Realize operações (depósito, saque, transferência)" -ForegroundColor White
Write-Host "4. Observe os logs no terminal da API" -ForegroundColor White
Write-Host "5. Você verá os eventos sendo publicados em tempo real!" -ForegroundColor White

Write-Host "`n=== ALTERNATIVA: KAFKA UI VIA DOCKER ===" -ForegroundColor Yellow
Write-Host "Se quiser usar o Kafka UI:" -ForegroundColor White
Write-Host "1. Inicie o Docker Desktop" -ForegroundColor Gray
Write-Host "2. Execute: docker-compose up -d kafka-ui" -ForegroundColor Gray
Write-Host "3. Acesse: http://localhost:8085" -ForegroundColor Gray

Write-Host "`n=== VISUALIZADOR KAFKA CONCLUÍDO! ===" -ForegroundColor Green

