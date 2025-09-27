# Script para limpar recursos do Kubernetes
Write-Host "🧹 Limpando recursos do BankMore no Kubernetes..." -ForegroundColor Green

# Confirmar limpeza
$confirmation = Read-Host "Tem certeza que deseja remover todos os recursos? (y/N)"
if ($confirmation -ne "y" -and $confirmation -ne "Y") {
    Write-Host "❌ Limpeza cancelada" -ForegroundColor Yellow
    exit 0
}

# Remover HPA
Write-Host "`n📈 Removendo HPA..." -ForegroundColor Yellow
kubectl delete -f hpa.yaml --ignore-not-found=true
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ HPA removido" -ForegroundColor Green
}

# Remover Ingress
Write-Host "`n🔗 Removendo Ingress..." -ForegroundColor Yellow
kubectl delete -f ingress/bankmore-ingress.yaml --ignore-not-found=true
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Ingress removido" -ForegroundColor Green
}

# Remover Services
Write-Host "`n🌐 Removendo Services..." -ForegroundColor Yellow
kubectl delete -f services/conta-corrente-service.yaml --ignore-not-found=true
kubectl delete -f services/transferencias-service.yaml --ignore-not-found=true
kubectl delete -f services/tarifas-service.yaml --ignore-not-found=true
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Services removidos" -ForegroundColor Green
}

# Remover Deployments
Write-Host "`n🚀 Removendo Deployments..." -ForegroundColor Yellow
kubectl delete -f deployments/conta-corrente-deployment.yaml --ignore-not-found=true
kubectl delete -f deployments/transferencias-deployment.yaml --ignore-not-found=true
kubectl delete -f deployments/tarifas-deployment.yaml --ignore-not-found=true
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Deployments removidos" -ForegroundColor Green
}

# Remover PersistentVolumes
Write-Host "`n💾 Removendo PersistentVolumes..." -ForegroundColor Yellow
kubectl delete -f volumes/persistent-volumes.yaml --ignore-not-found=true
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ PersistentVolumes removidos" -ForegroundColor Green
}

# Remover Secrets
Write-Host "`n🔐 Removendo Secrets..." -ForegroundColor Yellow
kubectl delete -f secrets/app-secrets.yaml --ignore-not-found=true
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Secrets removidos" -ForegroundColor Green
}

# Remover ConfigMaps
Write-Host "`n⚙️ Removendo ConfigMaps..." -ForegroundColor Yellow
kubectl delete -f configmaps/app-config.yaml --ignore-not-found=true
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ ConfigMaps removidos" -ForegroundColor Green
}

# Remover Namespace (remove tudo)
Write-Host "`n📦 Removendo Namespace..." -ForegroundColor Yellow
kubectl delete namespace bankmore --ignore-not-found=true
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Namespace removido" -ForegroundColor Green
}

# Verificar se tudo foi removido
Write-Host "`n🔍 Verificando limpeza..." -ForegroundColor Yellow
$remainingPods = kubectl get pods -n bankmore 2>$null
if ($remainingPods) {
    Write-Host "⚠️ Ainda existem pods no namespace bankmore" -ForegroundColor Yellow
} else {
    Write-Host "✅ Todos os recursos foram removidos" -ForegroundColor Green
}

Write-Host "`n🎉 Limpeza concluída!" -ForegroundColor Green
Write-Host "📊 Para verificar:" -ForegroundColor Cyan
Write-Host "   kubectl get all -n bankmore" -ForegroundColor Cyan
Write-Host "   kubectl get namespaces" -ForegroundColor Cyan
