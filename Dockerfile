FROM --platform=${BUILDPLATFORM} mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build-env
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish Kyaru.csproj -c Release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine
LABEL com.centurylinklabs.watchtower.enable="true"
WORKDIR /app
COPY --from=build-env /out .
RUN apk add icu-libs --no-cache
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENTRYPOINT ["dotnet", "Kyaru.dll"]
