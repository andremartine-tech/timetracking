# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar o certificado SSL
# COPY aspnetapp.pfx /https/aspnetapp.pfx

# ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
# ENV ASPNETCORE_Kestrel__Certificates__Default__Password=k4hvd891

# Copiar arquivos de projeto e restaurar dependências
COPY *.csproj ./
RUN dotnet restore

# Copiar todo o código e compilar
COPY . ./
RUN dotnet publish -c Release -o out

# Etapa de runtime com instalação temporária do SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /app

COPY --from=build /app/out .

# Variáveis de ambiente para o ASP.NET Core
ENV ASPNETCORE_URLS=https://+:5001;http://+:5000
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

# Expõe a porta da aplicação
EXPOSE 5000

# Comando de entrada com suporte a migrations
ENTRYPOINT ["dotnet", "VialoginTimeTrackingAPI.dll"]
