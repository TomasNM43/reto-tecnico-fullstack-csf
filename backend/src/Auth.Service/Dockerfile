# El contexto de construcción es la carpeta backend.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/BuildingBlocks/ BuildingBlocks/
COPY src/Auth.Service/ Auth.Service/
RUN dotnet publish Auth.Service/Auth.Service.Api/Auth.Service.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Auth.Service.Api.dll"]
