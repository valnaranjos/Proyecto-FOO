FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Instalar dependencias necesarias
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev llvm binutils \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Variables
ARG BUILD_CONFIGURATION=Release
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# Directorio de trabajo
WORKDIR /src

# Copiar proyectos
COPY ["src/ProyectoFoo.API/ProyectoFoo.API.csproj", "src/ProyectoFoo.API/"]
COPY ["src/ProyectoFoo.Application/ProyectoFoo.Application.csproj", "src/ProyectoFoo.Application/"]
COPY ["src/ProyectoFoo.Domain/ProyectoFoo.Domain.csproj", "src/ProyectoFoo.Domain/"]
COPY ["src/ProyectoFoo.Infrastructure/ProyectoFoo.Infrastructure.csproj", "src/ProyectoFoo.Infrastructure/"]
COPY ["src/ProyectoFoo.Shared/ProyectoFoo.Shared.csproj", "src/ProyectoFoo.Shared/"]

# Restaurar dependencias
RUN dotnet restore "src/ProyectoFoo.API/ProyectoFoo.API.csproj"

# Copiar el resto de los archivos
COPY . .

# Cambiar al directorio del proyecto
WORKDIR "/src/src/ProyectoFoo.API"

# Publicar la aplicación
RUN dotnet publish "ProyectoFoo.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -r linux-x64 \
    --self-contained true \
    -o /app/publish

# Imagen final
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["./ProyectoFoo.API"]
