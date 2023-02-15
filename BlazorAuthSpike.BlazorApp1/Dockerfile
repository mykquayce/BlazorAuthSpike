FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BlazorAuthSpike.BlazorApp1/BlazorAuthSpike.BlazorApp1.csproj", "BlazorAuthSpike.BlazorApp1/"]
RUN dotnet restore "BlazorAuthSpike.BlazorApp1/BlazorAuthSpike.BlazorApp1.csproj"
COPY . .
WORKDIR "/src/BlazorAuthSpike.BlazorApp1"
RUN dotnet build "BlazorAuthSpike.BlazorApp1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorAuthSpike.BlazorApp1.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorAuthSpike.BlazorApp1.dll"]
