FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 4445
EXPOSE 61010-61020

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY *.sln ./
COPY Slipka/Slipka.csproj Slipka/
RUN dotnet restore
COPY . .
WORKDIR /src/Slipka
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Slipka.dll"]
