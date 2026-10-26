# ============================================================
# Backend image: ASP.NET Core Web API
# Multi-stage build: stage 1 compiles, stage 2 only runs.
# ============================================================

# ---------- Stage 1: build ----------
# The SDK image contains the compiler and the dotnet CLI (large, ~800 MB).
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy ONLY the project file first and restore NuGet packages.
# Docker caches each step, so packages are only re-downloaded when the .csproj changes.
COPY EmployeeManagement.Api.csproj ./
RUN dotnet restore

# Now copy the rest of the source code and publish a Release build.
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Stage 2: run ----------
# The ASP.NET runtime image can only RUN apps, not build them (small, ~250 MB).
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Take just the compiled output from stage 1; the SDK and source code are left behind.
COPY --from=build /app/publish .

# .NET container images listen on port 8080 by default.
EXPOSE 8080

ENTRYPOINT ["dotnet", "EmployeeManagement.Api.dll"]
