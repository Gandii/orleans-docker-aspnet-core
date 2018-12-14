FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY *.sln ./
COPY Silo/Silo.csproj Silo/
COPY Directory.Build.props .
RUN dotnet restore /p:RestoreUseSkipNonexistentTargets="false"
COPY . .
WORKDIR /src/Silo
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Silo.dll"]
