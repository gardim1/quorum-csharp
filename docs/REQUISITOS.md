# Conferência da entrega

## C# Software Development — página 30

| Critério | Pontos | Implementação e demonstração |
|---|---:|---|
| POO | 3,0 | Classes em `Quorum.Api/Dominio`, atributos encapsulados, validações e distribuição de potência. |
| Web API | 2,5 | Controllers de unidades, carregadores e medições; rotas REST e respostas de sucesso/erro. |
| EF Core + LINQ | 1,5 | `Dados/QuorumDbContext.cs`, SQLite persistente, relações, filtros, ordenação e agregações nas consultas. |
| Async/Await | 1,0 | Controllers e serviço fazem acesso assíncrono ao banco com `ToListAsync`, `SaveChangesAsync` e demais operações. |
| Qualidade | 2,0 | DTOs, separação de responsabilidades, instruções, testes de domínio e testes HTTP com SQLite real. |

Os objetivos adicionais aparecem em `Servicos/IControleDemandaService.cs` e no registro de DI em `Program.cs` (interfaces e injeção), e em `Quorum.Tests` (testes unitários e de integração). O README explica as regras e a execução. O Swagger documenta os endpoints.

Evidências: [requisições reais e respostas](../evidencias/api-demonstracao.json), [Swagger](../evidencias/swagger.png) e [testes](../evidencias/testes.txt).

**Formato exigido:** link do GitHub. Repositório: https://github.com/gardim1/quorum-csharp. O envio na tarefa e a concessão de acesso ao professor são etapas da entrega acadêmica.
