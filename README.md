# Conta Corrente API - Desafio Ailos

Sistema de gerenciamento de conta corrente implementado com .NET 8, seguindo arquitetura limpa com CQRS, MediatR, Dapper, KafkaFlow e Swagger.

## 🏗️ Arquitetura

O projeto segue uma arquitetura em camadas bem definida:

- **ContaCorrente.Api**: Camada de apresentação com Controllers e Swagger
- **ContaCorrente.Application**: Camada de aplicação com Commands, Queries e Handlers (MediatR)
- **ContaCorrente.Domain**: Camada de domínio com entidades e interfaces
- **ContaCorrente.Infrastructure**: Camada de infraestrutura com repositórios Dapper e KafkaFlow

## 🚀 Tecnologias Utilizadas

- **.NET 8**
- **MediatR** (CQRS)
- **Dapper** (ORM)
- **SQLite** (banco de dados)
- **KafkaFlow** (mensageria)
- **Swagger** (documentação)
- **JWT** (autenticação)
- **BCrypt** (hash de senhas)
- **xUnit** (testes)

## 📋 Funcionalidades

### ✅ Implementadas

- ✅ Criação de contas correntes
- ✅ Ativação/desativação de contas
- ✅ Autenticação JWT
- ✅ Lançamento de movimentos (crédito/débito)
- ✅ Consulta de saldo
- ✅ Extrato com filtros e paginação
- ✅ Idempotência via header `Idempotency-Key`
- ✅ Cache em memória para consultas
- ✅ Mensageria Kafka para auditoria
- ✅ Documentação Swagger completa
- ✅ Testes unitários e de integração
- ✅ Docker Compose com Kafka e Redis

### 🔒 Regras de Negócio

- Contas são criadas inativas por padrão
- Débitos não podem deixar saldo negativo
- Validação de formato de data (DD/MM/YYYY)
- Hash de senha com BCrypt + salt
- JWT com expiração de 30 minutos
- Cache de 10 segundos para consultas

## 🛠️ Como Executar

### Pré-requisitos

- .NET 8 SDK
- Docker e Docker Compose

### Execução Local

1. **Clone o repositório**
```bash
git clone <repository-url>
cd ContaCorrente
```

2. **Restaure as dependências**
```bash
dotnet restore
```

3. **Execute os testes**
```bash
dotnet test
```

4. **Execute a aplicação**
```bash
dotnet run --project src/ContaCorrente.Api
```

A API estará disponível em: `https://localhost:7000`
Swagger UI: `https://localhost:7000/swagger`

### Execução com Docker

1. **Execute o Docker Compose**
```bash
docker-compose up -d --build
```

2. **Acesse os serviços**
- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`
- Kafka UI: `http://localhost:8085`
- Redis: `localhost:6379`

## 📚 Documentação da API

### Endpoints Principais

#### Contas
- `POST /api/contas` - Criar conta
- `PATCH /api/contas/{id}/ativar` - Ativar/desativar conta

#### Autenticação
- `POST /api/auth/login` - Login

#### Movimentos
- `POST /api/contas/{id}/movimentos` - Lançar movimento
- `GET /api/contas/{id}/saldo` - Consultar saldo
- `GET /api/contas/{id}/extrato` - Consultar extrato

### Exemplos de Uso

#### 1. Criar Conta
```bash
curl -X POST http://localhost:8080/api/contas \
  -H "Content-Type: application/json" \
  -d '{
    "numero": 123,
    "nome": "Felipe",
    "senha": "Segredo@123"
  }'
```

#### 2. Ativar Conta
```bash
curl -X PATCH http://localhost:8080/api/contas/{id}/ativar \
  -H "Content-Type: application/json" \
  -d '{"ativo": true}'
```

#### 3. Login
```bash
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "numero": 123,
    "senha": "Segredo@123"
  }'
```

#### 4. Lançar Crédito (com idempotência)
```bash
curl -X POST http://localhost:8080/api/contas/{id}/movimentos \
  -H "Authorization: Bearer {token}" \
  -H "Idempotency-Key: 11111111-1111-1111-1111-111111111111" \
  -H "Content-Type: application/json" \
  -d '{
    "data": "24/09/2025",
    "tipo": "C",
    "valor": 250.00
  }'
```

#### 5. Consultar Saldo
```bash
curl -H "Authorization: Bearer {token}" \
  http://localhost:8080/api/contas/{id}/saldo
```

#### 6. Consultar Extrato
```bash
curl -H "Authorization: Bearer {token}" \
  "http://localhost:8080/api/contas/{id}/extrato?data_inicio=01/09/2025&data_fim=30/09/2025&page=1&pageSize=10"
```

## 🧪 Testes

### Executar Testes Unitários
```bash
dotnet test tests/ContaCorrente.UnitTests
```

### Executar Testes de Integração
```bash
dotnet test tests/ContaCorrente.IntegrationTests
```

### Executar Todos os Testes
```bash
dotnet test
```

## 📊 Monitoramento

### Kafka UI
Acesse `http://localhost:8085` para monitorar:
- Tópicos Kafka
- Mensagens em tempo real
- Consumidores e produtores

### Logs
A aplicação gera logs estruturados para:
- Operações de movimentação
- Autenticação
- Erros e exceções
- Performance de queries

## 🔧 Configuração

### Variáveis de Ambiente

```bash
# Banco de Dados
ConnectionStrings__Sqlite=Data Source=app.db

# Kafka
Kafka__BootstrapServers=localhost:9092
Kafka__TopicMovimentos=movimentos.efetuados

# JWT
Jwt__Issuer=ContaCorrente
Jwt__Audience=ContaCorrente
Jwt__Key=super_secret_key_here_change

# Redis (opcional)
Redis__Enabled=false
Redis__ConnectionString=localhost:6379
```

## 🏗️ Estrutura do Projeto

```
src/
├── ContaCorrente.Api/           # API e Controllers
├── ContaCorrente.Application/   # Commands, Queries, Handlers
├── ContaCorrente.Domain/        # Entidades e Interfaces
└── ContaCorrente.Infrastructure/ # Repositórios e Kafka

tests/
├── ContaCorrente.UnitTests/     # Testes unitários
└── ContaCorrente.IntegrationTests/ # Testes de integração
```

## 🚀 Próximos Passos

- [ ] Implementar health checks
- [ ] Adicionar métricas com OpenTelemetry
- [ ] Implementar retry policy para Kafka
- [ ] Adicionar validação com FluentValidation
- [ ] Implementar rate limiting
- [ ] Adicionar suporte a Oracle

## 📝 Licença

Este projeto foi desenvolvido como parte do desafio técnico da Ailos.

---

**Desenvolvido com ❤️ usando .NET 8**