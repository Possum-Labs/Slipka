FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 4445
EXPOSE 61010-61020

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Slipka.Docker.sln ./
COPY docker-compose* ./
COPY Microsoft.AspNetCore.Proxy/Microsoft.AspNetCore.Proxy.csproj Microsoft.AspNetCore.Proxy/
COPY Slipka/Slipka.csproj Slipka/
RUN dotnet restore Slipka.Docker.sln
COPY . .
WORKDIR /src/Slipka
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Slipka.dll"]
