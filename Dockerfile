FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base

WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update && apt-get install -y --no-install-recommends \
    # WebKit
    libwoff1 \
    libopus0 \
    libwebp6 \
    libwebpdemux2 \
    libenchant1c2a \
    libgudev-1.0-0 \
    libsecret-1-0 \
    libhyphen0 \
    libgdk-pixbuf2.0-0 \
    libegl1 \
    libnotify4 \
    libxslt1.1 \
    libevent-2.1-6 \
    libgles2 \
    libvpx5 \
    libxcomposite1 \
    libatk1.0-0 \
    libatk-bridge2.0-0 \
    libepoxy0 \
    libgtk-3-0 \
    libharfbuzz-icu0 \
    # Chromium
    fonts-liberation \
    libnss3 \
    libxss1 \
    libasound2 \
    fonts-noto-color-emoji \
    libxtst6

COPY browsers/ browsers/

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build

ENV PLAYWRIGHT_SKIP_BROWSER_DOWNLOAD=1
RUN apt-get update -yq \
    && apt-get install -yq --no-install-recommends \
    curl \
    && curl -sL https://deb.nodesource.com/setup_15.x | bash \
    && apt-get install nodejs -yq

WORKDIR /src
COPY ["vaxalert.csproj", "./"]
RUN dotnet restore "vaxalert.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "vaxalert.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "vaxalert.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV DEBUG=pw:api
ENTRYPOINT ["dotnet", "vaxalert.dll"]
