# Script para verificar se a aplicação está conseguindo se conectar ao Kafka
Write-Host "=== VERIFICANDO APLICAÇÃO E KAFKA ===" -ForegroundColor Green

Write-Host "`n🔍 STATUS DOS CONTAINERS:" -ForegroundColor Yellow
docker ps --filter name=kafka --filter name=zookeeper --filter name=kafka-ui

Write-Host "`n🔍 VERIFICANDO TÓPICOS:" -ForegroundColor Yellow
docker exec kafka kafka-topics.sh --list --bootstrap-server localhost:9092

Write-Host "`n🔍 VERIFICANDO MENSAGENS DE TESTE:" -ForegroundColor Yellow
Write-Host "Consumindo mensagens do tópico movimentos.efetuados..." -ForegroundColor White
docker exec kafka kafka-console-consumer.sh --topic movimentos.efetuados --bootstrap-server localhost:9092 --from-beginning --timeout-ms 3000

Write-Host "`n🔍 VERIFICANDO APLICAÇÃO .NET:" -ForegroundColor Yellow
Write-Host "Processo dotnet rodando: $(Get-Process | Where-Object {$_.ProcessName -like '*dotnet*'} | Measure-Object).Count" -ForegroundColor White

Write-Host "`n🔍 POSSÍVEIS PROBLEMAS:" -ForegroundColor Yellow
Write-Host "1. Aplicação .NET não está conseguindo se conectar ao Kafka" -ForegroundColor White
Write-Host "2. Configuração incorreta do Kafka na aplicação" -ForegroundColor White
Write-Host "3. Problema de rede entre aplicação e Kafka" -ForegroundColor White
Write-Host "4. Erro na configuração do KafkaFlow" -ForegroundColor White

Write-Host "`n🔍 SOLUÇÕES:" -ForegroundColor Yellow
Write-Host "1. Verificar logs da aplicação para erros de Kafka" -ForegroundColor White
Write-Host "2. Reiniciar a aplicação" -ForegroundColor White
Write-Host "3. Verificar configuração do Kafka na aplicação" -ForegroundColor White
Write-Host "4. Verificar se o Kafka está acessível" -ForegroundColor White

Write-Host "`n🔍 KAFKA UI:" -ForegroundColor Yellow
Write-Host "Acesse: http://localhost:8085" -ForegroundColor Cyan
Write-Host "1. Clique em 'Topics'" -ForegroundColor White
Write-Host "2. Clique em 'movimentos.efetuados'" -ForegroundColor White
Write-Host "3. Vá para a aba 'Messages'" -ForegroundColor White
Write-Host "4. Clique em 'Load messages'" -ForegroundColor White
Write-Host "5. Você deve ver a mensagem de teste!" -ForegroundColor White

Write-Host "`n=== VERIFICAÇÃO CONCLUÍDA ===" -ForegroundColor Green

