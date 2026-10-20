FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
COPY --from=node:22-slim /usr/local/ /usr/local/
RUN corepack enable pnpm
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:8080
ENV Api__BaseUrl=http://localhost:8080/
EXPOSE 8080
VOLUME /app/Data
ENTRYPOINT ["dotnet", "sharpness-sharp.dll"]
