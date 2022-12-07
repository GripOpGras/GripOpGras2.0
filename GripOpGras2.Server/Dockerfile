#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GripOpGras2.Server/GripOpGras2.Server.csproj", "GripOpGras2.Server/"]
COPY ["GripOpGras2.Domain/GripOpGras2.Domain.csproj", "GripOpGras2.Domain/"]
COPY ["GripOpGras2.Client/GripOpGras2.Client.csproj", "GripOpGras2.Client/"]
RUN dotnet restore "GripOpGras2.Server/GripOpGras2.Server.csproj"
COPY . .
WORKDIR "/src/GripOpGras2.Server"
RUN dotnet build "GripOpGras2.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GripOpGras2.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GripOpGras2.Server.dll"]