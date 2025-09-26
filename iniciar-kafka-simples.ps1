# Script para iniciar Kafka e Kafka UI de forma simples
Write-Host "=== INICIANDO KAFKA E KAFKA UI ===" -ForegroundColor Green

Write-Host "`n🛑 Parando containers existentes..." -ForegroundColor Yellow
docker stop $(docker ps -q --filter ancestor=provectuslabs/kafka-ui:latest) 2>$null
docker stop $(docker ps -q --filter name=kafka) 2>$null
docker stop $(docker ps -q --filter name=zookeeper) 2>$null

Write-Host "`n🗑️ Removendo containers antigos..." -ForegroundColor Yellow
docker rm $(docker ps -aq --filter ancestor=provectuslabs/kafka-ui:latest) 2>$null
docker rm $(docker ps -aq --filter name=kafka) 2>$null
docker rm $(docker ps -aq --filter name=zookeeper) 2>$null

Write-Host "`n🚀 Iniciando Zookeeper..." -ForegroundColor Cyan
docker run -d --name zookeeper -p 2181:2181 zookeeper:3.7

Write-Host "`n⏳ Aguardando Zookeeper inicializar..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host "`n🚀 Iniciando Kafka..." -ForegroundColor Cyan
docker run -d --name kafka -p 9092:9092 --link zookeeper:zookeeper -e KAFKA_ZOOKEEPER_CONNECT=zookeeper:2181 -e KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://localhost:9092 -e KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR=1 confluentinc/cp-kafka:latest

Write-Host "`n⏳ Aguardando Kafka inicializar..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

Write-Host "`n🚀 Iniciando Kafka UI..." -ForegroundColor Cyan
docker run -d --name kafka-ui -p 8085:8080 -e KAFKA_CLUSTERS_0_NAME=dev -e KAFKA_CLUSTERS_0_BOOTSTRAPSERVERS=host.docker.internal:9092 provectuslabs/kafka-ui:latest

Write-Host "`n⏳ Aguardando Kafka UI inicializar..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host "`n✅ Verificando status dos containers..." -ForegroundColor Green
docker ps --filter name=kafka --filter name=zookeeper --filter name=kafka-ui

Write-Host "`n🌐 Kafka UI disponível em: http://localhost:8085" -ForegroundColor Cyan
Write-Host "`n📊 Para ver os tópicos:" -ForegroundColor Yellow
Write-Host "1. Acesse http://localhost:8085" -ForegroundColor White
Write-Host "2. Clique em 'Topics' no menu lateral" -ForegroundColor White
Write-Host "3. Aguarde carregar (pode demorar alguns segundos)" -ForegroundColor White
Write-Host "4. Você verá os tópicos: movimentos.efetuados, transferencias.efetuadas, tarifas.cobradas" -ForegroundColor White

Write-Host "`n=== KAFKA INICIADO COM SUCESSO! ===" -ForegroundColor Green

