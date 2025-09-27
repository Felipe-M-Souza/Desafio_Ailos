# Script para testar Kubernetes
Write-Host "🧪 Testando BankMore no Kubernetes..." -ForegroundColor Green

# Verificar status dos pods
Write-Host "`n📦 Status dos Pods..." -ForegroundColor Yellow
kubectl get pods -n bankmore -o wide

# Verificar status dos services
Write-Host "`n🌐 Status dos Services..." -ForegroundColor Yellow
kubectl get services -n bankmore

# Verificar status do ingress
Write-Host "`n🔗 Status do Ingress..." -ForegroundColor Yellow
kubectl get ingress -n bankmore

# Verificar status do HPA
Write-Host "`n📈 Status do HPA..." -ForegroundColor Yellow
kubectl get hpa -n bankmore

# Verificar logs da conta corrente
Write-Host "`n📋 Logs da Conta Corrente..." -ForegroundColor Yellow
kubectl logs -l component=conta-corrente -n bankmore --tail=10

# Verificar logs das transferências
Write-Host "`n📋 Logs das Transferências..." -ForegroundColor Yellow
kubectl logs -l component=transferencias -n bankmore --tail=10

# Verificar logs das tarifas
Write-Host "`n📋 Logs das Tarifas..." -ForegroundColor Yellow
kubectl logs -l component=tarifas -n bankmore --tail=10

# Testar health check
Write-Host "`n🏥 Testando Health Check..." -ForegroundColor Yellow
try {
    $healthResponse = kubectl port-forward -n bankmore service/conta-corrente-service 5009:5009 &
    Start-Sleep -Seconds 5
    $healthCheck = Invoke-RestMethod -Uri "http://localhost:5009/health" -Method GET
    Write-Host "✅ Health Check OK: $($healthCheck.status)" -ForegroundColor Green
    Stop-Job -Name "kubectl port-forward" -ErrorAction SilentlyContinue
} catch {
    Write-Host "❌ Erro ao testar Health Check: $($_.Exception.Message)" -ForegroundColor Red
}

# Verificar recursos utilizados
Write-Host "`n📊 Recursos Utilizados..." -ForegroundColor Yellow
kubectl top pods -n bankmore
kubectl top nodes

# Verificar eventos
Write-Host "`n📅 Eventos Recentes..." -ForegroundColor Yellow
kubectl get events -n bankmore --sort-by='.lastTimestamp' | Select-Object -Last 10

Write-Host "`n🎉 Teste concluído!" -ForegroundColor Green
Write-Host "📊 Para monitorar continuamente:" -ForegroundColor Cyan
Write-Host "   kubectl get pods -n bankmore -w" -ForegroundColor Cyan
Write-Host "   kubectl logs -f -l app=bankmore -n bankmore" -ForegroundColor Cyan
