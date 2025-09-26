# Script para testar Docker Compose com Kafka
Write-Host "=== TESTANDO DOCKER COMPOSE COM KAFKA ===" -ForegroundColor Green

Write-Host "`n1. Parando containers existentes..." -ForegroundColor Yellow
docker-compose down

Write-Host "`n2. Iniciando todos os serviços..." -ForegroundColor Yellow
docker-compose up -d

Write-Host "`n3. Aguardando serviços iniciarem..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host "`n4. Verificando status dos containers..." -ForegroundColor Yellow
docker-compose ps

Write-Host "`n5. Verificando logs do Kafka..." -ForegroundColor Yellow
Write-Host "Logs do Kafka:" -ForegroundColor Cyan
docker-compose logs kafka | Select-Object -Last 10

Write-Host "`n6. Verificando logs da API..." -ForegroundColor Yellow
Write-Host "Logs da API:" -ForegroundColor Cyan
docker-compose logs api | Select-Object -Last 10

Write-Host "`n=== SERVIÇOS DISPONÍVEIS ===" -ForegroundColor Green
Write-Host "🌐 API Principal: http://localhost:8080" -ForegroundColor Cyan
Write-Host "🌐 API Transferências: http://localhost:8081" -ForegroundColor Cyan
Write-Host "🌐 API Tarifas: http://localhost:8082" -ForegroundColor Cyan
Write-Host "🌐 Kafka UI: http://localhost:8085" -ForegroundColor Cyan
Write-Host "🌐 Redis: localhost:6379" -ForegroundColor Cyan

Write-Host "`n=== COMO TESTAR KAFKA ===" -ForegroundColor Yellow
Write-Host "1. Acesse http://localhost:8085 (Kafka UI)" -ForegroundColor White
Write-Host "2. Navegue para 'Topics'" -ForegroundColor White
Write-Host "3. Você verá os tópicos:" -ForegroundColor White
Write-Host "   - movimentos.efetuados" -ForegroundColor Gray
Write-Host "   - transferencias.efetuadas" -ForegroundColor Gray
Write-Host "   - tarifas.cobradas" -ForegroundColor Gray
Write-Host "4. Clique em um tópico para ver as mensagens" -ForegroundColor White
Write-Host "5. Execute operações na API para gerar eventos" -ForegroundColor White

Write-Host "`n=== COMANDOS ÚTEIS ===" -ForegroundColor Yellow
Write-Host "Ver logs em tempo real:" -ForegroundColor White
Write-Host "  docker-compose logs -f kafka" -ForegroundColor Gray
Write-Host "  docker-compose logs -f api" -ForegroundColor Gray
Write-Host "Parar todos os serviços:" -ForegroundColor White
Write-Host "  docker-compose down" -ForegroundColor Gray
Write-Host "Reiniciar um serviço:" -ForegroundColor White
Write-Host "  docker-compose restart api" -ForegroundColor Gray

Write-Host "`n=== TESTE DOCKER KAFKA CONCLUÍDO! ===" -ForegroundColor Green

