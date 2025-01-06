# Etapa de build
FROM node:18 AS build
WORKDIR /app

# Copiar os arquivos de configuração e dependências
COPY package.json package-lock.json ./

# Instalar dependências
RUN npm install

# Copiar o restante do código
COPY . .

# Construir a aplicação
RUN npm run build

# Etapa de runtime
FROM nginx:1.23
WORKDIR /usr/share/nginx/html

# Remover arquivos padrão do Nginx
RUN rm -rf ./*

# Copiar os arquivos da aplicação para o Nginx
COPY --from=build /app/dist .

# Copiar arquivo de configuração do Nginx (opcional, para SPA)
COPY ./nginx.conf /etc/nginx/conf.d/default.conf

# Expõe a porta do Nginx
EXPOSE 80

# Iniciar o Nginx
CMD ["nginx", "-g", "daemon off;"]

