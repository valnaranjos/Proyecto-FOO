FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Instalar dependencias requeridas por NativeAOT
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev llvm binutils \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Instalar herramienta dotnet-ef
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar archivos del proyecto y restaurar dependencias
COPY ["src/ProyectoFoo.API/ProyectoFoo.API.csproj", "src/ProyectoFoo.API/"]
COPY ["src/ProyectoFoo.Application/ProyectoFoo.Application.csproj", "src/ProyectoFoo.Application/"]
COPY ["src/ProyectoFoo.Domain/ProyectoFoo.Domain.csproj", "src/ProyectoFoo.Domain/"]
COPY ["src/ProyectoFoo.Infrastructure/ProyectoFoo.Infrastructure.csproj", "src/ProyectoFoo.Infrastructure/"]
COPY ["src/ProyectoFoo.Shared/ProyectoFoo.Shared.csproj", "src/ProyectoFoo.Shared/"]

# Restaurar dependencias
RUN dotnet restore "src/ProyectoFoo.API/ProyectoFoo.API.csproj"

# Copiar todo el código
COPY . .

# Establecer el directorio de trabajo
WORKDIR "/src/src/ProyectoFoo.API"

# Aplicar migraciones si corresponde
RUN dotnet ef database update

# Build y publicación
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
