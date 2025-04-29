FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev llvm binutils \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/ProyectoFoo.API/ProyectoFoo.API.csproj", "src/ProyectoFoo.API/"]
COPY ["src/ProyectoFoo.Application/ProyectoFoo.Application.csproj", "src/ProyectoFoo.Application/"]
COPY ["src/ProyectoFoo.Domain/ProyectoFoo.Domain.csproj", "src/ProyectoFoo.Domain/"]
COPY ["src/ProyectoFoo.Infrastructure/ProyectoFoo.Infrastructure.csproj", "src/ProyectoFoo.Infrastructure/"]
COPY ["src/ProyectoFoo.Shared/ProyectoFoo.Shared.csproj", "src/ProyectoFoo.Shared/"]

RUN dotnet restore "src/ProyectoFoo.API/ProyectoFoo.API.csproj"

COPY . .

WORKDIR "/src/src/ProyectoFoo.API"

RUN dotnet publish "ProyectoFoo.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -r linux-x64 \
    --self-contained true \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["./ProyectoFoo.API"]