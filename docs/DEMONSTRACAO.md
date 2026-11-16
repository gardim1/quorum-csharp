# Demonstração técnica

## C#

1. Iniciar a API e abrir `/swagger`.
2. Consultar a unidade de demonstração e seus dois carregadores.
3. Mostrar a medição-base de 32 kW e o limite de 50 kW.
4. Consultar o plano: os carregadores de 11 e 22 kW recebem 11 e 7 kW.
5. Cadastrar uma nova medição e consultar o novo plano. Explicar que o cálculo usa a medição mais recente pelo horário, não simplesmente o último ID.
6. Tornar um carregador indisponível e mostrar alocação zero para ele.
7. Enviar uma entrada inválida e mostrar o erro de validação.
8. Mostrar consulta por período e o resultado dos testes.

O serviço simula a decisão. Não envia comandos a carregadores nem representa um contrato real de energia.
