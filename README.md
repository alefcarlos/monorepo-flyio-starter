# Flyio.Demo — Monolito Modular com .NET Aspire

Projeto de exemplo inspirado em [RiverBooks](https://github.com/ardalis/RiverBooks/), aplicando **monolito modular** com **.NET 10**, **Aspire**, **Keycloak** e deploy no **Fly.io**.


- https://riverbooks.ardalis.com/
- https://modularmonoliths.com/

## Arquitetura

```
┌─────────────────────────────────────────────────┐
│           Flyio.Demo.Web (Blazor SSR)           │
│   Autenticação OIDC + PKCE → Keycloak           │
│   Tokens gerenciados server-side                │
└──────────────────────┬──────────────────────────┘
                       │ HTTPS (autenticado)
┌──────────────────────▼──────────────────────────┐
│          Flyio.Demo.ApiService (Web API)        |
│   Minimal APIs com Mediator (CQRS)              │
│   JWT Bearer Auth → Keycloak                    │
│   Multi-tenancy por claim "organization"        │
│   Domain Events → Integration Events            │
│   OpenAPI / Scalar                              │
└───┬──────────────┬──────────────┬───────────────┘
    │              │              │
    ▼              ▼              ▼
┌─────────┐ ┌──────────┐ ┌──────────────┐
│  Heart  │ │  Todos   │ │ Service      │
│ Module  │ │  Module  │ │ Defaults     │
│ (SSE    │ │ (EF Core,│ │ (Steeltoe    │
│  heart  │ │  Npgsql) │ │  Config,     │
│  rate)  │ │          │ │  Placeholder)│
└─────────┘ └──────────┘ └──────────────┘
                │
     ┌──────────┴──────────┐
     ▼                     ▼
┌──────────────┐ ┌──────────────────┐
│ Todos        │ │ Module           │
│ Contracts    │ │ SharedKernel     │
│ (eventos,    │ │ (multi-tenant,   │
│  value obj)  │ │  auditing,       │
└──────────────┘ │  base entities)  │
                 └────────┬─────────┘
                          │
                 ┌────────▼─────────┐
                 │   SharedKernel   │
                 │ (domain events,  │
                 │  integration     │
                 │  events,         │
                 │  mediator)       │
                 └──────────────────┘
```

### Conceitos

- **Monolito Modular**: Cada módulo (`Todos`, `Heart`) é um projeto .NET com `IsModule=true`, possuindo seus próprios endpoints, use cases, domínio e persistência. Módulos se comunicam via **integration events**.
- **CQRS com Mediator**: Commands e Queries usando `Mediator.SourceGenerator` (ex.: `CreateTodoCommand` → `CreateTodoHandler`).
- **Domain Events → Integration Events**: Eventos de domínio (`TodoIsDoneEvent`) são convertidos em integration events (`TodoIsDoneIntegrationEvent`) e consumidos por outros módulos (ex.: `Heart`).
- **Multi-tenancy**: Baseada no claim `organization` do JWT, com `TenantRequirement` como authorization handler e `SetTenantIdInterceptor` no EF Core.
- **Auditing**: `UpdateAuditableEntitiesInterceptor` registra created-by/modified-by nas entidades e cria registros `AuditTrailEntity`.
- **Auth**: Blazor SSR faz OIDC + PKCE com Keycloak; API valida JWT Bearer.
- **Config Server**: Steeltoe busca configurações em um Spring Cloud Config Server (Git-backed) com suporte a placeholders.

## Estrutura do Projeto

| Projeto                          | Caminho                               | Responsabilidade                                                           |
| -------------------------------- | ------------------------------------- | -------------------------------------------------------------------------- |
| `Flyio.Demo.AppHost`             | `src/Flyio.Demo.AppHost/`             | Orquestrador Aspire (Keycloak, PostgreSQL, API, Web)                       |
| `Flyio.Demo.ApiService`          | `src/Flyio.Demo.ApiService/`          | Backend REST API                                                           |
| `Flyio.Demo.Web`                 | `src/Flyio.Demo.Web/`                 | Frontend Blazor SSR                                                        |
| `Flyio.Demo.ServiceDefaults`     | `src/Flyio.Demo.ServiceDefaults/`     | Configurações compartilhadas (OpenTelemetry, Steeltoe)                     |
| `Flyio.Demo.SharedKernel`        | `src/Flyio.Demo.SharedKernel/`        | Abstrações de domínio (eventos, mediator)                                  |
| `Flyio.Demo.Module.SharedKernel` | `src/Flyio.Demo.Module.SharedKernel/` | Infra compartilhada entre módulos (multi-tenancy, auditing, base entities) |
| `Flyio.Demo.Todos`               | `src/Flyio.Demo.Todos/`               | Módulo de Todos (EF Core + Npgsql)                                         |
| `Flyio.Demo.Todos.Contracts`     | `src/Flyio.Demo.Todos.Contracts/`     | Contratos e integration events do módulo Todos                             |
| `Flyio.Demo.Heart`               | `src/Flyio.Demo.Heart/`               | Módulo Heart (SSE de heartbeat, consome eventos do Todos)                  |
| `Flyio.Demo.ApiService.Tests`    | `tests/Flyio.Demo.ApiService.Tests/`  | Testes de integração da API                                                |

Outros diretórios:

| Diretório        | Finalidade                                             |
| ---------------- | ------------------------------------------------------ |
| `configs/`       | Configurações YAML para o Steeltoe Config Server       |
| `config-server/` | Config de deploy do Config Server no Fly.io            |
| `keycloak/`      | Dockerfile e realm de produção para Keycloak no Fly.io |
| `terraform/`     | Terraform para criar clients OIDC no Keycloak          |
| `.opencode/`     | Configuração do opencode AI agent                      |

## Como Executar

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Aspire CLI](https://github.com/microsoft/aspire#getting-started)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Terraform](https://developer.hashicorp.com/terraform/install) (opcional, para criar clients Keycloak)

### Executar com Aspire

```bash
aspire start
```

Isso inicia automaticamente:

| Serviço        | URL                           | Credenciais       |
| -------------- | ----------------------------- | ----------------- |
| **Keycloak**   | https://localhost:8080        | `admin` / `admin` |
| **PostgreSQL** | `localhost:5432` (via Docker) | —                 |
| **pgAdmin**    | http://localhost:5050         | —                 |
| **ApiService** | https://localhost:5001        | —                 |
| **Web**        | https://localhost:5002        | —                 |

### Criar Clients no Keycloak

Após o Keycloak estar rodando, execute:

```bash
terraform -chdir=terraform apply
```

Isso cria os clients OIDC (`apiservice`, `webfrontend`, `demo-confidential`) com suas roles e redirect URIs.

### Testar

Para acessar o recurso 'webfrontend' você pode fazer login com o usuário `alice@acme.com` ou `bob@bar.com` (senha `123`).

### Requests de Exemplo

Use o arquivo `run.http` (com [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) no VS Code) para testar os endpoints.

## EF Core Migrations

Utilize o Aspire para aplicar migrations utilizando o recurso `todos-migrations`.
