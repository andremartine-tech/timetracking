# Controle de Registro de Ponto - Vialogin Time Tracking

## Descrição

Sistema simples para controle de registro de ponto, desenvolvido com **.NET 9** no backend, **React** no frontend, e **PostgreSQL 17** como banco de dados. 
Utiliza RabbitMq para messageria.
Utiliza **Docker** e **Docker Compose** para orquestração.

---

## Estrutura do Projeto

- **backend/VialoginTimeTrackingAPI**: API principal.
- **backend/VialoginTimeTrackingTests**: Testes automatizados.
- **frontend**: Aplicação React servida com Nginx.
- **frontend/Dockerfile**: Configuração de frontend para o Docker.
- **backend/VialoginTimeTrackingAPI/Dockerfile**: Configuração da API da solução para o Docker.
- **backend/VialoginTimeTrackingAPI/Dockerfile.migrations**: Configuração de migrations do banco de dados para o Docker.
- **backend/VialoginTimeTrackingTests/Dockerfile.tests**: Configuração dos testes da solução para o Docker.

---

## Requisitos

- **Docker** 20.10+
- **Docker Compose** 2.0+
- **Node.js** 16.x+
- **.NET 9 SDK**
- **Postgres** 17.x+

## Configuração e Inicialização

1. Clone o repositório:
    ```bash
    git clone https://github.com/andremartine-tech/timetracking.git
    cd timetracking

2. Construa e inicie os serviços:
    ```bash
    docker-compose up -d --build

3. Verifique se os serviços estão conforme os status abaixo:
    - ✔ Network timelog_default          Created
    - ✔ Container timelog-rabbitmq-1     Healthy
    - ✔ Container timelog-database-1     Healthy
    - ✔ Container timelog-migrations-1   Exited
    - ✔ Container timelog-api-1          Healthy
    - ✔ Container timelog-frontend-1     Started 

4. Acesse os serviços:    
    
    - Frontend: http://localhost:3000
    - API: http://localhost:5000
    - RabbitMQ Management: http://localhost:15672
        - Usuário: guest, Senha: guest

5. Monitoramento e Saúde
    - API Healthcheck: http://localhost:5000/health

## Rodar testes de desempenho

1. Teste de inclusão de usuários
    ```bash
    cd .\frontend\src\test\load\
    k6 run --out json=addUsersLoadTest.json addUsersLoadTest.js

2. Teste de busca de usuários
    ```bash
    cd .\frontend\src\test\load\
    k6 run --out json=getUsersLoadTest.js getUsersLoadTest.js

## Licença
Este projeto está licenciado sob a MIT License.

## Contato
Dúvidas ou sugestões? Entre em contato via [LinkedIn](https://www.linkedin.com/in/andresantos1983/) ou abra uma issue.