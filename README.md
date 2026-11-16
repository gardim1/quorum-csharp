# Quorum API

API acadêmica em C# para acompanhar a demanda de uma unidade consumidora e simular quanto de potência pode ser reservado aos seus carregadores de veículos elétricos. A simulação usa dados fictícios em kW. A futura integração OCPP é o ponto em que os valores poderiam ser traduzidos em perfis de carregamento para equipamentos reais.

## Equipe

| Integrante | RM |
|---|---|
| Vinicius Gardim | 556013 |
| Fabiano Zague | 555524 |
| Pedro Certo | 556268 |
| Lorran dos Santos | 558982 |

## Contexto e entregas

Este é o backend C# da Quorum, desenvolvido para a Global Solution CitizenTech FIAP 2026. O cenário é um condomínio fictício; a definição de cidade/bairro e a validação com cidadãos continuam pendentes.

- [Contexto, ODS e fontes](docs/CONTEXTO.md)
- [Conferência dos requisitos](docs/REQUISITOS.md)
- [Sequência de demonstração](docs/DEMONSTRACAO.md)
- [Registro de uso de IA](prompts.md)
- [Projeto de Cybersecurity](https://github.com/gardim1/quorum-cybersecurity)

O outro repositório contém `demonstrar_integracao.py`, que lê esta API e audita a exportação da telemetria. Os projetos devem estar rodando nas portas indicadas nos respectivos READMEs.

## Executar localmente

Com o SDK .NET 9, execute na raiz deste repositório:

```sh
dotnet restore Quorum.sln
dotnet run --project Quorum.Api
```

Abra o [Swagger](http://localhost:5080/swagger) para testar as rotas. O arquivo [exemplos.http](exemplos.http) contém uma sequência de requisições para o editor REST do Visual Studio ou do VS Code.

Na primeira execução, a API cria `Quorum.Api/quorum.db` com uma unidade fictícia de 50 kW, uma medição de 32 kW e dois carregadores disponíveis de 11 kW e 22 kW. O SQLite preserva as alterações nas execuções seguintes. Para usar outro arquivo de banco, informe `ConnectionStrings__Quorum` com uma conexão como `Data Source=outro-quorum.db`.

## Endpoints

| Método | Caminho | Resultado |
|---|---|---|
| GET | `/api/unidades` | Lista unidades por ID. |
| POST | `/api/unidades` | Cria unidade e retorna `201`. |
| GET | `/api/unidades/{id}` | Consulta a unidade. |
| PUT | `/api/unidades/{id}` | Substitui nome e limite da unidade e retorna `204`. |
| DELETE | `/api/unidades/{id}` | Exclui unidade, carregadores e medições vinculados. Retorna `204`. |
| GET | `/api/unidades/{id}/carregadores` | Lista os carregadores da unidade por ID. |
| POST | `/api/unidades/{id}/carregadores` | Cadastra carregador vinculado à unidade e retorna `201`. |
| GET | `/api/unidades/{id}/carregadores/{carregadorId}` | Consulta o carregador da unidade. |
| PUT | `/api/unidades/{id}/carregadores/{carregadorId}` | Substitui nome, potência máxima e disponibilidade. Retorna `204`. |
| DELETE | `/api/unidades/{id}/carregadores/{carregadorId}` | Exclui o carregador e retorna `204`. |
| GET | `/api/unidades/{id}/medicoes?inicio=...&fim=...` | Lista medições e calcula quantidade, média e máximo no período. |
| POST | `/api/unidades/{id}/medicoes` | Registra medição de demanda e retorna `201`. |
| GET | `/api/unidades/{id}/plano-carregamento` | Simula a alocação com a medição mais recente. |

| Cadastro | Campos do JSON |
|---|---|
| Unidade | `nome`, `limiteDemandaKw` maior que zero |
| Carregador | `nome`, `potenciaMaximaKw` maior que zero, `disponivel` booleano |
| Medição | `demandaBaseKw` maior ou igual a zero, `registradaEm` opcional |

Informe datas no padrão ISO 8601 com fuso explícito, por exemplo `2026-11-16T12:00:00Z`. A API converte as datas para UTC e usa a hora atual em UTC quando `registradaEm` é omitida. Os filtros `inicio` e `fim` são opcionais e inclusivos. A resposta contém `quantidade`, `mediaKw`, `maximaKw` e `medicoes`, em ordem da mais recente para a mais antiga. Para um período vazio, a média e a máxima ficam `null`.

Dados inválidos e período invertido retornam `400`. Uma unidade ou um carregador inexistente retorna `404`. Consultar o plano de uma unidade sem medições retorna `409`.

## Regra de alocação

A demanda base é a carga da unidade sem os carregadores gerenciados. A folga é o maior valor entre zero e a diferença entre o limite e a demanda base. O plano usa a medição de maior data e desempata pelo maior ID. Percorre os carregadores em ordem crescente de ID e distribui a menor potência entre a folga restante e o máximo de cada carregador disponível.

No exemplo inicial, a folga é **50 − 32 = 18 kW**. O primeiro carregador recebe **11 kW** e o segundo recebe os **7 kW** restantes. Carregadores indisponíveis recebem zero. Com demanda base igual ou superior ao limite, toda a alocação fica zerada. O campo `excedeLimite` é verdadeiro quando a demanda base supera o limite.

A ordem por ID é uma regra simples de prioridade para a demonstração e pode deixar os carregadores seguintes sem potência. O plano reflete a medição registrada mais recente e a disponibilidade cadastrada, sem validar a idade da medição ou transmitir comandos a equipamentos.

## Organização e testes

- `Dominio` reúne as classes com atributos encapsulados e o cálculo de distribuição.
- `Dados` configura as relações, a base fictícia inicial e a persistência com EF Core e SQLite.
- `Controllers` recebe as operações HTTP e usa DTOs de `Contratos`. Consultas LINQ filtram, ordenam e calculam as estatísticas.
- `Servicos` conecta a consulta ao cálculo do plano. O ASP.NET Core injeta `IControleDemandaService` e o contexto de banco nos controladores. Operações de banco usam `async`/`await`.
- `Quorum.Tests` contém testes xUnit da distribuição e testes HTTP com arquivos SQLite temporários, incluindo reabertura do banco.

```sh
dotnet build Quorum.sln
dotnet test Quorum.sln
```
