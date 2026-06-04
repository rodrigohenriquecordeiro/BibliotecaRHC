FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BibliotecaRHC.API/BibliotecaRHC.API.csproj", "BibliotecaRHC.API/"]
RUN dotnet restore "BibliotecaRHC.API/BibliotecaRHC.API.csproj"

COPY . .

WORKDIR "/src/BibliotecaRHC.API"
RUN dotnet build "BibliotecaRHC.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BibliotecaRHC.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
EXPOSE 8081
ENTRYPOINT ["dotnet", "BibliotecaRHC.API.dll"]