FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app
COPY *.sln .
COPY src/ ./src/
COPY tests/ ./tests/
RUN dotnet restore && dotnet publish -o /app/publish -c Release --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/publish .
RUN adduser -D -H appuser
USER appuser
ENTRYPOINT [ "dotnet", "/app/API.dll" ]

ARG GIT_COMMIT_HASH=unknown
LABEL maintainer="hikashi <hikashimlbb@gmail.com>" \
      vcs_ref="${GIT_COMMIT_HASH}"