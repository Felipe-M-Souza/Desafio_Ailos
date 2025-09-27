# 🚀 BankMore - Kubernetes Deployment

Este diretório contém todos os manifests Kubernetes necessários para orquestrar o sistema BankMore.

## 📁 Estrutura

```
k8s/
├── configmaps/          # Configurações da aplicação
├── secrets/            # Secrets e chaves
├── deployments/        # Deployments dos microsserviços
├── services/          # Services para comunicação
├── ingress/           # Ingress para acesso externo
├── volumes/           # PersistentVolumes para dados
├── deploy.ps1         # Script de deploy
├── test-k8s.ps1       # Script de teste
├── cleanup.ps1        # Script de limpeza
└── README.md          # Este arquivo
```

## 🎯 Arquitetura Kubernetes

### **Microsserviços**
- **ContaCorrente** - 3 réplicas (HPA: 2-10)
- **Transferencias** - 2 réplicas (HPA: 1-5)
- **Tarifas** - 2 réplicas (HPA: 1-5)

### **Recursos**
- **ConfigMaps** - Configurações
- **Secrets** - Chaves e senhas
- **Services** - Comunicação interna
- **Ingress** - Acesso externo
- **PersistentVolumes** - Dados persistentes
- **HPA** - Auto-scaling

## 🚀 Como Deployar

### **1. Pré-requisitos**
```bash
# Instalar kubectl
# Instalar cluster Kubernetes (minikube, kind, etc.)
# Instalar NGINX Ingress Controller
```

### **2. Deploy Automático**
```powershell
# Executar script de deploy
.\deploy.ps1
```

### **3. Deploy Manual**
```bash
# Aplicar namespace
kubectl apply -f namespace.yaml

# Aplicar ConfigMaps
kubectl apply -f configmaps/app-config.yaml

# Aplicar Secrets
kubectl apply -f secrets/app-secrets.yaml

# Aplicar PersistentVolumes
kubectl apply -f volumes/persistent-volumes.yaml

# Aplicar Deployments
kubectl apply -f deployments/

# Aplicar Services
kubectl apply -f services/

# Aplicar Ingress
kubectl apply -f ingress/bankmore-ingress.yaml

# Aplicar HPA
kubectl apply -f hpa.yaml
```

## 🧪 Como Testar

### **1. Teste Automático**
```powershell
# Executar script de teste
.\test-k8s.ps1
```

### **2. Teste Manual**
```bash
# Verificar pods
kubectl get pods -n bankmore

# Verificar services
kubectl get services -n bankmore

# Verificar ingress
kubectl get ingress -n bankmore

# Verificar logs
kubectl logs -l app=bankmore -n bankmore

# Testar health check
kubectl port-forward -n bankmore service/conta-corrente-service 5009:5009
curl http://localhost:5009/health
```

## 🔧 Configurações

### **ConfigMaps**
- **app-config.yaml** - Configurações da aplicação
- **Connection Strings** - Bancos de dados
- **Kafka Settings** - Mensageria
- **Redis Settings** - Cache
- **JWT Settings** - Autenticação

### **Secrets**
- **jwt-secret** - Chave JWT
- **db-password** - Senha do banco
- **api-key** - Chave da API
- **kafka-credentials** - Credenciais Kafka

### **Deployments**
- **conta-corrente-deployment.yaml** - API principal
- **transferencias-deployment.yaml** - API de transferências
- **tarifas-deployment.yaml** - API de tarifas

### **Services**
- **conta-corrente-service** - Porta 5009
- **transferencias-service** - Porta 5010
- **tarifas-service** - Porta 5011

### **Ingress**
- **bankmore.local** - Acesso principal
- **api.bankmore.local** - API principal
- **transferencias.bankmore.local** - API transferências
- **tarifas.bankmore.local** - API tarifas

## 📊 Monitoramento

### **Health Checks**
```bash
# Verificar saúde dos pods
kubectl get pods -n bankmore

# Verificar logs
kubectl logs -l component=conta-corrente -n bankmore

# Verificar recursos
kubectl top pods -n bankmore
```

### **HPA (Horizontal Pod Autoscaler)**
```bash
# Verificar HPA
kubectl get hpa -n bankmore

# Verificar métricas
kubectl describe hpa conta-corrente-hpa -n bankmore
```

## 🧹 Limpeza

### **Limpeza Automática**
```powershell
# Executar script de limpeza
.\cleanup.ps1
```

### **Limpeza Manual**
```bash
# Remover tudo
kubectl delete namespace bankmore

# Ou remover recursos específicos
kubectl delete -f hpa.yaml
kubectl delete -f ingress/
kubectl delete -f services/
kubectl delete -f deployments/
kubectl delete -f volumes/
kubectl delete -f secrets/
kubectl delete -f configmaps/
kubectl delete -f namespace.yaml
```

## 🔗 Acessos

### **URLs de Acesso**
- **API Principal:** http://bankmore.local/api/contas
- **Transferências:** http://bankmore.local/api/transferencias
- **Tarifas:** http://bankmore.local/api/tarifas
- **Health Check:** http://bankmore.local/health
- **Swagger:** http://bankmore.local/swagger

### **Port Forward (Desenvolvimento)**
```bash
# Conta Corrente
kubectl port-forward -n bankmore service/conta-corrente-service 5009:5009

# Transferências
kubectl port-forward -n bankmore service/transferencias-service 5010:5010

# Tarifas
kubectl port-forward -n bankmore service/tarifas-service 5011:5011
```

## 📈 Auto-scaling

### **HPA Configuração**
- **CPU:** 70% utilization
- **Memory:** 80% utilization
- **Min Replicas:** 1-2
- **Max Replicas:** 5-10

### **Métricas**
```bash
# Verificar métricas
kubectl get hpa -n bankmore
kubectl describe hpa conta-corrente-hpa -n bankmore
```

## 🛠️ Troubleshooting

### **Problemas Comuns**
1. **Pods não iniciam** - Verificar logs e recursos
2. **Services não conectam** - Verificar selectors
3. **Ingress não funciona** - Verificar NGINX Ingress Controller
4. **PersistentVolumes não montam** - Verificar storage class

### **Comandos Úteis**
```bash
# Verificar eventos
kubectl get events -n bankmore

# Verificar logs
kubectl logs -l app=bankmore -n bankmore

# Verificar recursos
kubectl top pods -n bankmore
kubectl top nodes

# Descrever recursos
kubectl describe pod <pod-name> -n bankmore
kubectl describe service <service-name> -n bankmore
```

## 🎉 Pronto para Produção!

O sistema BankMore está configurado para:
- ✅ **Alta Disponibilidade** - Múltiplas réplicas
- ✅ **Auto-scaling** - HPA configurado
- ✅ **Monitoramento** - Health checks
- ✅ **Persistência** - Dados seguros
- ✅ **Segurança** - Secrets e RBAC
- ✅ **Acesso Externo** - Ingress configurado
