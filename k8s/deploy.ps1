# Script para deploy no Kubernetes
Write-Host "🚀 Deployando BankMore no Kubernetes..." -ForegroundColor Green

# Verificar se kubectl está disponível
try {
    $kubectlVersion = kubectl version --client --short
    Write-Host "✅ kubectl disponível: $kubectlVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ kubectl não encontrado. Instale o kubectl primeiro." -ForegroundColor Red
    exit 1
}

# Verificar se o cluster está acessível
try {
    $clusterInfo = kubectl cluster-info
    Write-Host "✅ Cluster Kubernetes acessível" -ForegroundColor Green
} catch {
    Write-Host "❌ Cluster Kubernetes não acessível. Verifique sua conexão." -ForegroundColor Red
    exit 1
}

# Criar namespace
Write-Host "`n📦 Criando namespace..." -ForegroundColor Yellow
kubectl apply -f namespace.yaml
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Namespace criado com sucesso" -ForegroundColor Green
} else {
    Write-Host "❌ Erro ao criar namespace" -ForegroundColor Red
    exit 1
}

# Aplicar ConfigMaps
Write-Host "`n⚙️ Aplicando ConfigMaps..." -ForegroundColor Yellow
kubectl apply -f configmaps/app-config.yaml
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ ConfigMaps aplicados com sucesso" -ForegroundColor Green
} else {
    Write-Host "❌ Erro ao aplicar ConfigMaps" -ForegroundColor Red
    exit 1
}

# Aplicar Secrets
Write-Host "`n🔐 Aplicando Secrets..." -ForegroundColor Yellow
kubectl apply -f secrets/app-secrets.yaml
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Secrets aplicados com sucesso" -ForegroundColor Green
} else {
    Write-Host "❌ Erro ao aplicar Secrets" -ForegroundColor Red
    exit 1
}

# Aplicar PersistentVolumes
Write-Host "`n💾 Aplicando PersistentVolumes..." -ForegroundColor Yellow
kubectl apply -f volumes/persistent-volumes.yaml
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ PersistentVolumes aplicados com sucesso" -ForegroundColor Green
} else {
    Write-Host "❌ Erro ao aplicar PersistentVolumes" -ForegroundColor Red
    exit 1
}

# Aplicar Deployments
Write-Host "`n🚀 Aplicando Deployments..." -ForegroundColor Yellow
kubectl apply -f deployments/conta-corrente-deployment.yaml
kubectl apply -f deployments/transferencias-deployment.yaml
kubectl apply -f deployments/tarifas-deployment.yaml
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Deployments aplicados com sucesso" -ForegroundColor Green
} else {
    Write-Host "❌ Erro ao aplicar Deployments" -ForegroundColor Red
    exit 1
}

# Aplicar Services
Write-Host "`n🌐 Aplicando Services..." -ForegroundColor Yellow
kubectl apply -f services/conta-corrente-service.yaml
kubectl apply -f services/transferencias-service.yaml
kubectl apply -f services/tarifas-service.yaml
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Services aplicados com sucesso" -ForegroundColor Green
} else {
    Write-Host "❌ Erro ao aplicar Services" -ForegroundColor Red
    exit 1
}

# Aplicar Ingress
Write-Host "`n🔗 Aplicando Ingress..." -ForegroundColor Yellow
kubectl apply -f ingress/bankmore-ingress.yaml
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Ingress aplicado com sucesso" -ForegroundColor Green
} else {
    Write-Host "❌ Erro ao aplicar Ingress" -ForegroundColor Red
    exit 1
}

# Aplicar HPA
Write-Host "`n📈 Aplicando HPA..." -ForegroundColor Yellow
kubectl apply -f hpa.yaml
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ HPA aplicado com sucesso" -ForegroundColor Green
} else {
    Write-Host "❌ Erro ao aplicar HPA" -ForegroundColor Red
    exit 1
}

# Aguardar pods ficarem prontos
Write-Host "`n⏳ Aguardando pods ficarem prontos..." -ForegroundColor Yellow
kubectl wait --for=condition=ready pod -l app=bankmore -n bankmore --timeout=300s
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Todos os pods estão prontos" -ForegroundColor Green
} else {
    Write-Host "⚠️ Alguns pods podem não estar prontos ainda" -ForegroundColor Yellow
}

# Verificar status
Write-Host "`n📊 Status dos recursos..." -ForegroundColor Yellow
kubectl get pods -n bankmore
kubectl get services -n bankmore
kubectl get ingress -n bankmore
kubectl get hpa -n bankmore

Write-Host "`n🎉 Deploy concluído com sucesso!" -ForegroundColor Green
Write-Host "📊 Para verificar o status:" -ForegroundColor Cyan
Write-Host "   kubectl get pods -n bankmore" -ForegroundColor Cyan
Write-Host "   kubectl get services -n bankmore" -ForegroundColor Cyan
Write-Host "   kubectl get ingress -n bankmore" -ForegroundColor Cyan
Write-Host "   kubectl logs -f deployment/conta-corrente-deployment -n bankmore" -ForegroundColor Cyan
