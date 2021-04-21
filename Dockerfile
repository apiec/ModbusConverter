#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim-arm32v7 AS base
#WORKDIR /app
#EXPOSE 80
#
#FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim-arm32v7 AS build
#WORKDIR /src
#COPY [".", "ModbusConverter/"]
#RUN dotnet restore "ModbusConverter.csproj"
#COPY . .
#WORKDIR "/src/ModbusConverter"
#RUN dotnet build "ModbusConverter.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "ModbusConverter.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "ModbusConverter.dll"]


# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY ./ModbusConverter_AspNet/ModbusConverter.csproj ./ModbusConverter_AspNet/
COPY ./EasyModbus/EasyModbus.csproj ./EasyModbus/
RUN ls
RUN dotnet restore ./ModbusConverter_AspNet/ModbusConverter.csproj -r linux-arm

# copy and publish app and libraries
COPY . .
RUN dotnet publish -c release -o /app -r linux-arm --self-contained false --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim-arm32v7
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./ModbusConverter"]