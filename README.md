# Users7

API REST desenvolvida em C#/.NET 8 para o gerenciamento de usuários.

## Objetivo

Implementar uma API para criação, leitura, atualização, remoção e listagem de usuários, garantindo validações de entrada, e-mail único e persistência em PostgreSQL.

## Tecnologias

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Docker Compose
- Swagger/OpenAPI
- FluentValidation
- xUnit, Moq e FluentAssertions

## Arquitetura

O projeto segue Clean Architecture:

```text
src/
  Users7.Domain          Regras e entidade de domínio
  Users7.Application     DTOs, interfaces, validações e serviços
  Users7.Infrastructure  EF Core, PostgreSQL e repositories
  Users7.Api             Controllers, middleware, Swagger e DI
tests/
  Users7.UnitTests       Testes unitários dos serviços e validações
```

Dependências principais:

- `Domain` não depende de nenhuma camada.
- `Application` depende apenas de `Domain`.
- `Infrastructure` implementa interfaces da `Application`.
- `Api` compõe as dependências e expõe os endpoints.

## Modelo de usuário

| Campo | Tipo | Regra |
| --- | --- | --- |
| Id | integer | Gerenciado pelo banco |
| Name | varchar(50) | Obrigatório |
| Email | varchar(254) | Obrigatório e único |
| BirthDate | date | Não pode ser futura |
| CreatedAt | date | Gerenciado pela API |
| UpdatedAt | date | Gerenciado pela API |

## Como executar o PostgreSQL

```bash
docker compose up -d
```

Banco local:

- Host: `localhost`
- Porta: `5437`
- Database: `users7`
- Usuário: `users7`
- Senha: `users7_password`

## Como aplicar migrations

```bash
dotnet ef database update --project "src/Users7.Infrastructure/Users7.Infrastructure.csproj" --startup-project "src/Users7.Api/Users7.Api.csproj"
```

## Como executar a API

```bash
dotnet run --project "src/Users7.Api/Users7.Api.csproj"
```

A documentação Swagger fica disponível em:

```text
/swagger
```

## Endpoints

### Criar usuário

```http
POST /users
Content-Type: application/json

{
  "name": "Maria Silva",
  "email": "maria@example.com",
  "birthDate": "1990-05-20"
}
```

Respostas esperadas:

- `201 Created` quando criado.
- `400 Bad Request` quando os dados são inválidos.
- `409 Conflict` quando já existe usuário com o mesmo e-mail.

### Buscar usuário por id

```http
GET /users/1
```

Respostas esperadas:

- `200 OK` quando encontrado.
- `404 Not Found` quando não encontrado.

### Atualizar usuário

```http
PUT /users/1
Content-Type: application/json

{
  "name": "Maria Souza",
  "email": "maria.souza@example.com",
  "birthDate": "1990-05-20"
}
```

Respostas esperadas:

- `200 OK` quando atualizado.
- `400 Bad Request` quando os dados são inválidos.
- `404 Not Found` quando não encontrado.
- `409 Conflict` quando o e-mail pertence a outro usuário.

### Remover usuário

```http
DELETE /users/1
```

Respostas esperadas:

- `204 No Content` quando removido.
- `404 Not Found` quando não encontrado.

### Listar usuários

```http
GET /users?pageNumber=1&pageSize=10
```

Também é possível filtrar e ordenar:

```http
GET /users?name=maria&email=example.com&sortBy=createdAt&sortDirection=desc&pageNumber=1&pageSize=10
```

Campos de ordenação aceitos:

- `id`
- `name`
- `email`
- `birthDate`
- `createdAt`
- `updatedAt`

## Testes

```bash
dotnet test
```
