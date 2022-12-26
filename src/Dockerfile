FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN curl -sL https://deb.nodesource.com/setup_10.x |  bash -
RUN apt-get install -y nodejs

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
RUN curl -sL https://deb.nodesource.com/setup_10.x |  bash -
RUN apt-get install -y nodejs
WORKDIR /src
COPY . ./
COPY ./Web/ClientApp/.npmrc ./Web/ClientApp/
FROM build AS publish
ARG pat

RUN touch ./Web/ClientApp/.npmrc
RUN echo "; begin auth token" >> ./Web/ClientApp/.npmrc
RUN echo "//pkgs.dev.azure.com/suporte0667/NoCond/_packaging/NoCond/npm/registry/:username=suporte0667" >> ./Web/ClientApp/.npmrc
RUN echo "//pkgs.dev.azure.com/suporte0667/NoCond/_packaging/NoCond/npm/registry/:_password=$pat" >> ./Web/ClientApp/.npmrc
RUN echo "//pkgs.dev.azure.com/suporte0667/NoCond/_packaging/NoCond/npm/registry/:email=npm requires email to be set but doesn't use the value" >> ./Web/ClientApp/.npmrc
RUN echo "//pkgs.dev.azure.com/suporte0667/NoCond/_packaging/NoCond/npm/:username=suporte0667" >> ./Web/ClientApp/.npmrc
RUN echo "//pkgs.dev.azure.com/suporte0667/NoCond/_packaging/NoCond/npm/:_password=$pat" >> ./Web/ClientApp/.npmrc
RUN echo "//pkgs.dev.azure.com/suporte0667/NoCond/_packaging/NoCond/npm/:email=npm requires email to be set but doesn't use the value" >> ./Web/ClientApp/.npmrc
RUN echo "; end auth token" >> ./Web/ClientApp/.npmrc

RUN dotnet publish "Web/Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NoCond.Identity.Web.dll"]