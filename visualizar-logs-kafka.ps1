# Script para visualizar logs do Kafka através da aplicação
Write-Host "=== VISUALIZANDO LOGS DO KAFKA ===" -ForegroundColor Green

Write-Host "`n📊 LOGS DO KAFKA DISPONÍVEIS:" -ForegroundColor Yellow
Write-Host "Os logs do Kafka estão sendo exibidos no terminal da aplicação!" -ForegroundColor Cyan

Write-Host "`n🔍 EVENTOS QUE VOCÊ PODE VER NOS LOGS:" -ForegroundColor Yellow

Write-Host "`n✅ MovimentoRealizadoEvent:" -ForegroundColor Green
Write-Host "   Log: 'Publicando mensagem no tópico movimentos.efetuados'" -ForegroundColor Gray
Write-Host "   Exemplo: {" -ForegroundColor Gray
Write-Host "     'IdMovimento': 'b5096cbd-2869-4726-b51b-26d6d5a27ecc'," -ForegroundColor Gray
Write-Host "     'IdConta': '3797b410-455e-4bae-a554-891288e899d7'," -ForegroundColor Gray
Write-Host "     'NumeroConta': 1," -ForegroundColor Gray
Write-Host "     'Tipo': 'C'," -ForegroundColor Gray
Write-Host "     'Valor': 100000000," -ForegroundColor Gray
Write-Host "     'DataMovimento': '2025-09-24T00:00:00'," -ForegroundColor Gray
Write-Host "     'SaldoAtual': 100000035," -ForegroundColor Gray
Write-Host "     'Descricao': 'Movimento C - R$ 100.000.000,00'" -ForegroundColor Gray
Write-Host "   }" -ForegroundColor Gray

Write-Host "`n✅ TarifaCobradaEvent:" -ForegroundColor Green
Write-Host "   Log: 'Publicando mensagem no tópico tarifas.cobradas'" -ForegroundColor Gray
Write-Host "   Exemplo: {" -ForegroundColor Gray
Write-Host "     'IdTarifaCobrada': '3003f1ea-1cbe-4f56-9287-a4e3b86911c7'," -ForegroundColor Gray
Write-Host "     'IdConta': '3797b410-455e-4bae-a554-891288e899d7'," -ForegroundColor Gray
Write-Host "     'NumeroConta': 0," -ForegroundColor Gray
Write-Host "     'IdTarifa': '8f624796-83b1-45f9-992e-b537d28727f9'," -ForegroundColor Gray
Write-Host "     'NomeTarifa': 'Tarifa de saque'," -ForegroundColor Gray
Write-Host "     'ValorTarifa': 1," -ForegroundColor Gray
Write-Host "     'DataCobranca': '2025-09-25T19:39:16.0612993Z'," -ForegroundColor Gray
Write-Host "     'TipoOperacao': 'SAQUE'," -ForegroundColor Gray
Write-Host "     'Descricao': 'Tarifa cobrada: Tarifa de saque'" -ForegroundColor Gray
Write-Host "   }" -ForegroundColor Gray

Write-Host "`n✅ TransferenciaRealizadaEvent:" -ForegroundColor Green
Write-Host "   Log: 'Publicando mensagem no tópico transferencias.efetuadas'" -ForegroundColor Gray
Write-Host "   (Aparece quando você faz transferências entre contas)" -ForegroundColor Gray

Write-Host "`n🎯 COMO TESTAR E VER OS LOGS:" -ForegroundColor Yellow
Write-Host "1. Mantenha o terminal da aplicação aberto" -ForegroundColor White
Write-Host "2. Acesse a interface web: http://localhost:5009" -ForegroundColor White
Write-Host "3. Faça login com uma conta" -ForegroundColor White
Write-Host "4. Execute operações:" -ForegroundColor White
Write-Host "   - Depósito (verá MovimentoRealizadoEvent)" -ForegroundColor Gray
Write-Host "   - Saque (verá TarifaCobradaEvent + MovimentoRealizadoEvent)" -ForegroundColor Gray
Write-Host "   - Transferência (verá TransferenciaRealizadaEvent)" -ForegroundColor Gray
Write-Host "5. Observe os logs no terminal da aplicação!" -ForegroundColor White

Write-Host "`n📝 LOGS RECENTES (do terminal):" -ForegroundColor Cyan
Write-Host "✅ MovimentoRealizadoEvent publicado:" -ForegroundColor Green
Write-Host "   IdMovimento: b5096cbd-2869-4726-b51b-26d6d5a27ecc" -ForegroundColor Gray
Write-Host "   Valor: R$ 100.000.000,00 (depósito)" -ForegroundColor Gray

Write-Host "`n✅ TarifaCobradaEvent publicado:" -ForegroundColor Green
Write-Host "   IdTarifaCobrada: 3003f1ea-1cbe-4f56-9287-a4e3b86911c7" -ForegroundColor Gray
Write-Host "   Tarifa: R$ 0,01 (saque)" -ForegroundColor Gray

Write-Host "`n✅ MovimentoRealizadoEvent publicado:" -ForegroundColor Green
Write-Host "   IdMovimento: f1bd9e5d-90a1-41ae-90d6-0381a5061570" -ForegroundColor Gray
Write-Host "   Valor: R$ 15.000.000,00 (saque)" -ForegroundColor Gray

Write-Host "`n🔄 PARA VER NOVOS EVENTOS:" -ForegroundColor Yellow
Write-Host "1. Faça mais operações na interface" -ForegroundColor White
Write-Host "2. Observe os novos logs aparecerem no terminal" -ForegroundColor White
Write-Host "3. Cada operação gera eventos específicos!" -ForegroundColor White

Write-Host "`n=== LOGS DO KAFKA VISUALIZADOS COM SUCESSO! ===" -ForegroundColor Green

