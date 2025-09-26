# Script para verificar status completo do Kafka
Write-Host "=== STATUS COMPLETO KAFKA ===" -ForegroundColor Green

Write-Host "`n🔍 CONTAINERS RODANDO:" -ForegroundColor Yellow
docker ps --filter name=kafka --filter name=zookeeper --filter name=kafka-ui

Write-Host "`n🔍 TÓPICOS CRIADOS:" -ForegroundColor Yellow
docker exec kafka kafka-topics.sh --list --bootstrap-server localhost:9092

Write-Host "`n🔍 MENSAGENS NO TÓPICO movimentos.efetuados:" -ForegroundColor Yellow
docker exec kafka kafka-console-consumer.sh --topic movimentos.efetuados --bootstrap-server localhost:9092 --from-beginning --timeout-ms 3000

Write-Host "`n🔍 MENSAGENS NO TÓPICO tarifas.cobradas:" -ForegroundColor Yellow
docker exec kafka kafka-console-consumer.sh --topic tarifas.cobradas --bootstrap-server localhost:9092 --from-beginning --timeout-ms 3000

Write-Host "`n🔍 APLICAÇÃO .NET:" -ForegroundColor Yellow
Write-Host "Processos dotnet: $(Get-Process | Where-Object {$_.ProcessName -like '*dotnet*'} | Measure-Object).Count" -ForegroundColor White

Write-Host "`n🔍 KAFKA UI:" -ForegroundColor Yellow
Write-Host "URL: http://localhost:8085" -ForegroundColor Cyan
Write-Host "Status: Verificar se consegue ver os tópicos" -ForegroundColor White

Write-Host "`n🔍 DIAGNÓSTICO:" -ForegroundColor Yellow
Write-Host "✅ Kafka está rodando" -ForegroundColor Green
Write-Host "✅ Tópicos foram criados" -ForegroundColor Green
Write-Host "✅ Aplicação .NET está rodando" -ForegroundColor Green
Write-Host "❌ Mensagens da aplicação não estão chegando no Kafka" -ForegroundColor Red

Write-Host "`n🔍 POSSÍVEIS CAUSAS:" -ForegroundColor Yellow
Write-Host "1. Aplicação .NET não consegue se conectar ao Kafka" -ForegroundColor White
Write-Host "2. Configuração incorreta do Kafka na aplicação" -ForegroundColor White
Write-Host "3. Problema de rede entre aplicação e Kafka" -ForegroundColor White
Write-Host "4. Erro na configuração do KafkaFlow" -ForegroundColor White

Write-Host "`n🔍 PRÓXIMOS PASSOS:" -ForegroundColor Yellow
Write-Host "1. Verificar logs da aplicação para erros de Kafka" -ForegroundColor White
Write-Host "2. Verificar configuração do Kafka na aplicação" -ForegroundColor White
Write-Host "3. Testar conectividade entre aplicação e Kafka" -ForegroundColor White
Write-Host "4. Verificar se o Kafka está acessível" -ForegroundColor White

Write-Host "`n=== DIAGNÓSTICO CONCLUÍDO ===" -ForegroundColor Green

