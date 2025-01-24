#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base

WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN curl -sL https://deb.nodesource.com/setup_18.x | bash -
RUN apt-get install -y nodejs
RUN npm i -g @angular/cli
RUN npm update
RUN ng version

ENV PATH "$PATH:/usr/bin/node"

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

WORKDIR /src
COPY ["DocuSign.MyHR/*.sln", "DocuSign.MyHR/"]
COPY ["DocuSign.MyHR/DocuSign.MyHR/DocuSign.MyHR.csproj", "DocuSign.MyHR/DocuSign.MyHR/"]
COPY ["DocuSign.MyHR/DocuSign.MyHR.UnitTests/DocuSign.MyHR.UnitTests.csproj", "DocuSign.MyHR.UnitTests/"]
COPY ["DocuSign.MyHR/DocuSign.MyHR.UnitTests/DocuSign.MyHR.UnitTests.csproj", "DocuSign.MyHR/DocuSign.MyHR.UnitTests/"]
WORKDIR /src/DocuSign.MyHR
RUN dotnet restore
COPY . .

RUN dotnet build "DocuSign.MyHR/DocuSign.MyHR/DocuSign.MyHR.csproj" -c Release -o /app/build

FROM build AS publish
RUN curl -sL https://deb.nodesource.com/setup_18.x | bash -
RUN apt-get install -y nodejs
RUN npm i -g @angular/cli
RUN npm update
RUN ng version
ENV PATH "$PATH:/usr/bin/node"


RUN dotnet publish "DocuSign.MyHR/DocuSign.MyHR/DocuSign.MyHR.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DocuSign.MyHR.dll"]