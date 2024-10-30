# Usa la imagen oficial de .NET SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copia el archivo de solución y los proyectos para restaurar dependencias
COPY ssptb.pe.tdlt.transaction.service.sln ./
COPY ssptb.pe.tdlt.transaction.api/ssptb.pe.tdlt.transaction.api.csproj ./ssptb.pe.tdlt.transaction.api/
COPY ssptb.pe.tdlt.transaction.command/ssptb.pe.tdlt.transaction.command.csproj ./ssptb.pe.tdlt.transaction.command/
COPY ssptb.pe.tdlt.transaction.commandhandler/ssptb.pe.tdlt.transaction.commandhandler.csproj ./ssptb.pe.tdlt.transaction.commandhandler/
COPY ssptb.pe.tdlt.transaction.commandvalidator/ssptb.pe.tdlt.transaction.commandvalidator.csproj ./ssptb.pe.tdlt.transaction.commandvalidator/
COPY ssptb.pe.tdlt.transaction.common/ssptb.pe.tdlt.transaction.common.csproj ./ssptb.pe.tdlt.transaction.common/
COPY ssptb.pe.tdlt.transaction.data/ssptb.pe.tdlt.transaction.data.csproj ./ssptb.pe.tdlt.transaction.data/
COPY ssptb.pe.tdlt.transaction.dto/ssptb.pe.tdlt.transaction.dto.csproj ./ssptb.pe.tdlt.transaction.dto/
COPY ssptb.pe.tdlt.transaction.entities/ssptb.pe.tdlt.transaction.entities.csproj ./ssptb.pe.tdlt.transaction.entities/
COPY ssptb.pe.tdlt.transaction.infraestructure/ssptb.pe.tdlt.transaction.infraestructure.csproj ./ssptb.pe.tdlt.transaction.infraestructure/
COPY ssptb.pe.tdlt.transaction.internalservices/ssptb.pe.tdlt.transaction.internalservices.csproj ./ssptb.pe.tdlt.transaction.internalservices/
COPY ssptb.pe.tdlt.transaction.redis/ssptb.pe.tdlt.transaction.redis.csproj ./ssptb.pe.tdlt.transaction.redis/
COPY ssptb.pe.tdlt.transaction.secretsmanager/ssptb.pe.tdlt.transaction.secretsmanager.csproj ./ssptb.pe.tdlt.transaction.secretsmanager/

# Restaura las dependencias
RUN dotnet restore

# Copia el código fuente y compílalo en modo Release
COPY . ./
RUN dotnet publish ssptb.pe.tdlt.transaction.api/ -c Release -o /app/out

# Usa una imagen de runtime más ligera para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copia los archivos publicados desde la fase de construcción
COPY --from=build-env /app/out .

# Copia el archivo de configuración para producción
COPY ssptb.pe.tdlt.transaction.api/appsettings.Production.json ./appsettings.Production.json

# Configura el entorno de producción y la URL
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:80

# Expone el puerto 80 para la aplicación
EXPOSE 80

# Comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "ssptb.pe.tdlt.transaction.api.dll"]
