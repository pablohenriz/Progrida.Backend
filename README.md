# Progrida — Backend

Backend reescrito seguindo **Clean Architecture + DDD leve** (monólito modular, sem microservices).

```
Backend
├── src
│   ├── Progrida.Domain          → entidades e regras de negócio puras
│   ├── Progrida.Application     → casos de uso (Tasks, Sections, Users)
│   ├── Progrida.Infrastructure  → EF Core + PostgreSQL, JWT, hashing
│   └── Progrida.API             → controllers, middleware, Program.cs
├── docker-compose.yml           → sobe um Postgres local
└── Progrida.Backend.sln
```

## Como rodar localmente

### 1. Subir o banco (Postgres via Docker)

```bash
cd Backend
docker compose up -d
```

### 2. Configurar os segredos (NUNCA no appsettings.json)

```bash
cd src/Progrida.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:ProgridaDb" "Host=localhost;Port=5432;Database=progrida;Username=progrida;Password=progrida_dev_only"
dotnet user-secrets set "Jwt:Secret" "uma-chave-bem-grande-e-aleatoria-com-no-minimo-32-caracteres"
```

### 3. Criar e aplicar as migrations

```bash
cd src/Progrida.API
dotnet tool install --global dotnet-ef   # se ainda não tiver
dotnet ef migrations add InitialCreate --project ../Progrida.Infrastructure --startup-project .
dotnet ef database update --project ../Progrida.Infrastructure --startup-project .
```

### 4. Rodar a API

```bash
dotnet run --project src/Progrida.API
```

Abre em `http://localhost:5080/swagger`.

## Testando o fluxo

```
POST /api/auth/register  { "name": "...", "email": "...", "password": "..." }
POST /api/auth/login     { "email": "...", "password": "..." }
      ↓ copie o accessToken
GET  /api/sections       Header: Authorization: Bearer {accessToken}
POST /api/sections       { "name": "Estudos" }
POST /api/tasks          { "title": "Estudar C#", "sectionId": "..." }
PATCH /api/tasks/{id}/complete
PATCH /api/tasks/reorder { "items": [{ "taskId": "...", "newPosition": 0, "newSectionId": null }] }
```

## O que falta (próximas fases do plano)

- `POST /api/auth/refresh` e `/logout` — precisam de uma tabela de refresh tokens revogáveis
- Testes automatizados (`Backend/tests/Progrida.Domain.Tests`, pasta já criada, vazia)
- Rate limiting nos endpoints de auth
- Entidades `Habit`, `Goal` e `DailyProgress` (fases 8–10 do plano de arquitetura)
- Frontend consumindo esta API (hoje o `Frontend/app.js` ainda usa dados locais/mock)

## Por que não usei microservices

O estágio atual do projeto não justifica a complexidade operacional de múltiplos serviços,
bancos e comunicação assíncrona. Um monólito modular com fronteiras internas bem definidas
(Domain / Application / Infrastructure / API) entrega os mesmos benefícios de organização
sem o custo de infraestrutura — e pode ser dividido em serviços no futuro, se necessário,
porque as camadas já estão desacopladas.
