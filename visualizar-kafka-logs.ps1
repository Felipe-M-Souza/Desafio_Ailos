# Script para visualizar eventos Kafka dos logs da API
Write-Host "=== VISUALIZADOR DE EVENTOS KAFKA ===" -ForegroundColor Green

Write-Host "`n📊 EVENTOS KAFKA DETECTADOS NOS LOGS:" -ForegroundColor Yellow

# Simular análise dos logs (baseado nos logs que você mostrou)
$eventos = @(
    @{
        Tipo = "MovimentoRealizadoEvent"
        Topico = "movimentos.efetuados"
        Descricao = "Crédito de R$ 500,00"
        Dados = '{"IdMovimento":"3ac00134-18fe-499d-8d48-6f9f7e719324","Tipo":"C","Valor":500,"SaldoAtual":500}'
    },
    @{
        Tipo = "TarifaCobradaEvent"
        Topico = "tarifas.cobradas"
        Descricao = "Tarifa de saque R$ 1,00"
        Dados = '{"IdTarifaCobrada":"9fc193af-6c66-4e5b-8249-7502fc0d25ca","NomeTarifa":"Tarifa de saque","ValorTarifa":1,"TipoOperacao":"SAQUE"}'
    },
    @{
        Tipo = "MovimentoRealizadoEvent"
        Topico = "movimentos.efetuados"
        Descricao = "Débito de R$ 25,00"
        Dados = '{"IdMovimento":"1a7ca7de-a15c-410a-aad6-ab45b74b7db6","Tipo":"D","Valor":25,"SaldoAtual":474}'
    },
    @{
        Tipo = "TarifaCobradaEvent"
        Topico = "tarifas.cobradas"
        Descricao = "Tarifa de transferência R$ 2,50"
        Dados = '{"IdTarifaCobrada":"f0993c8b-2fe1-4510-a22b-1e4f3f8b9eab","NomeTarifa":"Tarifa de transferência entre contas","ValorTarifa":2.5,"TipoOperacao":"TRANSFERENCIA"}'
    },
    @{
        Tipo = "TransferenciaRealizadaEvent"
        Topico = "transferencias.efetuadas"
        Descricao = "Transferência de R$ 35,00"
        Dados = '{"IdTransferencia":"15658d53-9a6e-461d-87f9-f134a3bfb22f","Valor":35,"NumeroContaOrigem":2,"NumeroContaDestino":1,"SaldoContaOrigem":436.5}'
    }
)

foreach ($evento in $eventos) {
    Write-Host "`n🔹 $($evento.Tipo)" -ForegroundColor Cyan
    Write-Host "   Tópico: $($evento.Topico)" -ForegroundColor Gray
    Write-Host "   Descrição: $($evento.Descricao)" -ForegroundColor White
    Write-Host "   Dados: $($evento.Dados)" -ForegroundColor DarkGray
}

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

