FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["BSK_proj2/BSK_proj2.csproj", "BSK_proj2/"]
RUN dotnet restore "BSK_proj2/BSK_proj2.csproj"
COPY . .
WORKDIR "/src/BSK_proj2"
RUN dotnet build "BSK_proj2.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BSK_proj2.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BSK_proj2.dll"]