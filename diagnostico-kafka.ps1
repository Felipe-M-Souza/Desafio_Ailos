# Script para diagnosticar problemas com Kafka
Write-Host "=== DIAGNÓSTICO KAFKA ===" -ForegroundColor Green

Write-Host "`n🔍 VERIFICANDO CONTAINERS:" -ForegroundColor Yellow
docker ps --filter name=kafka --filter name=zookeeper --filter name=kafka-ui

Write-Host "`n🔍 VERIFICANDO TÓPICOS:" -ForegroundColor Yellow
docker exec kafka kafka-topics.sh --list --bootstrap-server localhost:9092

Write-Host "`n🔍 VERIFICANDO CONECTIVIDADE:" -ForegroundColor Yellow
Write-Host "Testando conexão com Kafka..." -ForegroundColor White

try {
    $result = Test-NetConnection -ComputerName localhost -Port 9092 -WarningAction SilentlyContinue
    if ($result.TcpTestSucceeded) {
        Write-Host "✅ Conexão com Kafka OK" -ForegroundColor Green
    } else {
        Write-Host "❌ Conexão com Kafka FALHOU" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Erro ao testar conexão: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🔍 VERIFICANDO MENSAGENS NOS TÓPICOS:" -ForegroundColor Yellow
Write-Host "Testando tópico movimentos.efetuados..." -ForegroundColor White

try {
    $output = docker exec kafka kafka-console-consumer.sh --topic movimentos.efetuados --bootstrap-server localhost:9092 --from-beginning --timeout-ms 3000 2>&1
    if ($output -match "Processed a total of 0 messages") {
        Write-Host "❌ Nenhuma mensagem encontrada no tópico movimentos.efetuados" -ForegroundColor Red
    } else {
        Write-Host "✅ Mensagens encontradas no tópico movimentos.efetuados" -ForegroundColor Green
    }
} catch {
    Write-Host "❌ Erro ao verificar mensagens: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🔍 VERIFICANDO CONFIGURAÇÃO DA APLICAÇÃO:" -ForegroundColor Yellow
Write-Host "Kafka BootstrapServers: localhost:9092" -ForegroundColor White
Write-Host "Tópicos configurados:" -ForegroundColor White
Write-Host "  - movimentos.efetuados" -ForegroundColor Gray
Write-Host "  - transferencias.efetuadas" -ForegroundColor Gray
Write-Host "  - tarifas.cobradas" -ForegroundColor Gray

Write-Host "`n🔍 POSSÍVEIS SOLUÇÕES:" -ForegroundColor Yellow
Write-Host "1. Verificar se a aplicação está rodando" -ForegroundColor White
Write-Host "2. Verificar logs da aplicação para erros de Kafka" -ForegroundColor White
Write-Host "3. Reiniciar a aplicação" -ForegroundColor White
Write-Host "4. Verificar se o Kafka está acessível" -ForegroundColor White

Write-Host "`n=== DIAGNÓSTICO CONCLUÍDO ===" -ForegroundColor Green

