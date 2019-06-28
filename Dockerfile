FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine AS builder

FROM builder AS build

WORKDIR /src

COPY src/Slipka/Slipka.csproj ./Slipka/Slipka.csproj
COPY src/Microsoft.AspNetCore.Proxy/Microsoft.AspNetCore.Proxy.csproj ./Microsoft.AspNetCore.Proxy/Microsoft.AspNetCore.Proxy.csproj

RUN dotnet restore ./Slipka/Slipka.csproj

COPY src/. .

WORKDIR /src/Slipka
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
EXPOSE 4445
EXPOSE 61710-61920
WORKDIR /app
COPY --from=build /src/Slipka/out ./
ENTRYPOINT ["dotnet", "Slipka.dll"]

