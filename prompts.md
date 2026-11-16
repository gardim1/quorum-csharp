# Registro de uso de IA

O enunciado pede o registro dos prompts e que os integrantes saibam explicar o código. Houve apoio de IA no planejamento, na implementação, na revisão e na criação de testes e documentação. Os registros abaixo resumem os pedidos de trabalho; não são uma transcrição integral da conversa. As instruções técnicas ficam disponíveis junto ao código e aos testes. Os integrantes devem revisar e estudar a entrega antes de apresentá-la.

## Pedidos e decisões

1. Ler o enunciado da GS, identificar todos os requisitos de C# e Cybersecurity e montar um plano antes de implementar. Usar a ideia de usina virtual e manter escopo adequado a um trabalho de faculdade.
2. Conferir o branding da Quorum e identificar se as disciplinas exigem telas.
3. Correção do contexto: não existe projeto-base. Começar do zero, com C#/.NET básico, usando o resumo da ideia e os nomes/RMs fornecidos pelo grupo.
4. Implementar API com unidades consumidoras, carregadores e medições. Distribuir a folga de demanda entre carregadores disponíveis. Incluir POO, interfaces, DI, EF Core, LINQ, async/await, documentação e testes.
5. Implementar módulo de segurança para detectar CPF, saúde e senhas, propagar etiquetas em cópias e combinações, destruir conteúdo sensível no mascaramento e bloquear saídas inseguras. Usar fluxos JSON declarativos, sem executar código arbitrário.
6. Integrar um LLM local para explicar os achados e sugerir remediação sem receber valores brutos de dados sensíveis. Mostrar indisponibilidade se o modelo falhar.
7. Criar painel funcional de Risco Cidadão com as cores básicas da marca, estados de carregamento e erro, trilha dos dados e explicação da IA. O layout poderá ser substituído pelo design do grupo.
8. Conferir código e requisitos com revisão independente, executar testes reais e produzir evidências de funcionamento.
9. Definição posterior do grupo: “Segue com exemplo simulado ou deixa pendência, mas eu acho que exemplo simulado é melhor por enquanto.” Foi adotado um condomínio fictício; a validação de campo não foi inventada.

## Uso de IA dentro do produto

O módulo Cybersecurity chama um modelo local pelo Ollama. O prompt é montado no cliente de IA do módulo com categorias, destinos e contexto de remediação. Não inclui os valores sensíveis de entrada. A resposta é identificada no painel como explicação gerada por IA. O rastreamento, a decisão de bloqueio e o cálculo de risco são regras determinísticas, independentes do LLM.

O uso do LLM no produto é parte da exigência específica de Cybersecurity. O uso de IA como ferramenta de desenvolvimento está documentado separadamente neste arquivo.
