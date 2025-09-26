# Script para testar Kafka
Write-Host "=== TESTANDO KAFKA ===" -ForegroundColor Green

Write-Host "`n🔍 VERIFICANDO TÓPICOS:" -ForegroundColor Yellow
docker exec kafka kafka-topics.sh --list --bootstrap-server localhost:9092

Write-Host "`n🔍 TESTANDO PUBLICAÇÃO DE MENSAGEM:" -ForegroundColor Yellow
Write-Host "Publicando mensagem de teste..." -ForegroundColor White

# Criar uma mensagem de teste
$testMessage = '{"IdMovimento":"test-123","IdConta":"test-account","NumeroConta":999,"Tipo":"C","Valor":100,"DataMovimento":"2025-09-25T00:00:00","SaldoAtual":100,"Descricao":"Teste Kafka"}'

# Publicar mensagem
echo $testMessage | docker exec -i kafka kafka-console-producer.sh --topic movimentos.efetuados --bootstrap-server localhost:9092

Write-Host "`n🔍 VERIFICANDO SE A MENSAGEM FOI PUBLICADA:" -ForegroundColor Yellow
Write-Host "Consumindo mensagens do tópico..." -ForegroundColor White

# Consumir mensagens
docker exec kafka kafka-console-consumer.sh --topic movimentos.efetuados --bootstrap-server localhost:9092 --from-beginning --timeout-ms 5000

Write-Host "`n🔍 VERIFICANDO KAFKA UI:" -ForegroundColor Yellow
Write-Host "Acesse: http://localhost:8085" -ForegroundColor Cyan
Write-Host "1. Clique em 'Topics'" -ForegroundColor White
Write-Host "2. Clique em 'movimentos.efetuados'" -ForegroundColor White
Write-Host "3. Vá para a aba 'Messages'" -ForegroundColor White
Write-Host "4. Clique em 'Load messages'" -ForegroundColor White
Write-Host "5. Você deve ver a mensagem de teste!" -ForegroundColor White

Write-Host "`n=== TESTE CONCLUÍDO ===" -ForegroundColor Green

