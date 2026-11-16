# Contexto e recorte

O grupo definiu a Quorum como uma usina virtual: software que coordena recursos de energia distribuídos. O material da ideia indica como primeiro MVP o monitoramento de demanda e o controle de carregadores por OCPP. Para a entrega acadêmica, a parte de C# representa a decisão de alocar potência, com dados fictícios e sem ligação a um carregador físico.

O cenário de demonstração é um condomínio fictício, com dois pontos de recarga e limite de 50 kW. O grupo optou por dados simulados nesta etapa. Não há cidade/bairro reais associados nem entrevista concluída; a etapa de campo continua pendente.

A demanda-base registrada na API exclui os carregadores controlados pelo plano. Essa separação evita contar a mesma carga duas vezes. O algoritmo usa a leitura mais recente e distribui a folga entre carregadores disponíveis, em ordem de cadastro. É uma regra simples e previsível; não representa otimização de preços, prioridade comercial ou justiça de distribuição entre moradores.

No exemplo inicial, o local tem limite de 50 kW, demanda-base de 32 kW e dois carregadores de 11 e 22 kW. A folga de 18 kW resulta em alocações de 11 e 7 kW. A soma chega a 50 kW. Não confundir potência (kW) com energia acumulada (kWh).

Em Cybersecurity, os cenários mostram como informações de um participante ou uma credencial de carregador podem chegar a logs ou saídas de terceiros. O motor acompanha as etiquetas de sensibilidade durante o fluxo e impede saídas incompatíveis com a política. O cenário de saúde atende à categoria solicitada pelo enunciado sem adicioná-la ao cadastro energético.

## Impacto e ODS

As relações abaixo são propostas de contribuição. O trabalho não mede impacto social real, economia financeira, emissões evitadas ou universalização do acesso.

- **ODS 7, meta 7.1:** a meta inclui serviços energéticos confiáveis e modernos. Coordenar a demanda dos recursos é uma proposta para apoiar a confiabilidade do uso da infraestrutura existente. O teste comprova apenas que a potência adicional simulada respeita a folga; não comprova menos apagões. [ONU — ODS 7](https://sdgs.un.org/goals/goal7).
- **ODS 9, meta 9.4:** a meta trata da modernização da infraestrutura e do uso eficiente de recursos. A Quorum propõe uma camada de coordenação sobre equipamentos existentes. O protótipo demonstra o cálculo de distribuição, sem medir eficiência energética ou substituição de infraestrutura. [ONU — ODS 9](https://sdgs.un.org/goals/goal9).

Indicadores que podem ser medidos em um piloto futuro: quantidade de períodos acima do limite antes/depois da coordenação, potência máxima no período e disponibilidade de recarga por participante. A economia em reais depende de dados reais, tarifas e condições contratuais específicas, ausentes neste protótipo.

## Origem dos dados

Os exemplos operacionais foram definidos manualmente para exercitar regras e resultados conhecidos. Não foram obtidos de medidores, contas de energia ou pessoas reais. Os dados sintéticos destas disciplinas não substituem os dados reais e públicos exigidos na disciplina de Machine Learning.

## Fontes do projeto

- Enunciado: `GS CitizenTech - 3ES Fev - FIAP 2026 -.pdf`, fornecido pelo grupo, páginas 14, 17–23 e 28–30.
- [Quorum — manual de marca](https://claude.ai/artifact/GmjuVzB8LHxshcc3AE8Mjm#readme).
- [Usina Virtual — resumo da ideia](https://claude.ai/artifact/3mj4mHC5FbkX4hGMvCGHWM#755aa4aa-86d4), material conceitual fornecido pelo grupo.
- [Microsoft — API com controllers](https://learn.microsoft.com/aspnet/core/tutorials/first-web-api).
- [Microsoft — EF Core com SQLite](https://learn.microsoft.com/ef/core/get-started/netcore/new-db-sqlite).
- [Ollama — geração de respostas](https://docs.ollama.com/api/generate).

O texto conceitual inclui projeções comerciais e afirmações regulatórias. Elas permanecem como contexto do grupo e não são repetidas aqui como fatos verificados.
