#!/usr/bin/env bash
# Genera el modelo (entidades + DbContext) desde la base local con Scaffold.
# Requisitos: docker compose up -d  y  dotnet tool restore
set -euo pipefail

cd "$(dirname "$0")/.."

CADENA="${1:-Server=localhost;Port=3307;Database=TallerDB;Uid=root;Pwd=Root123!;CharSet=utf8mb4;}"

dotnet tool restore
cd TallerMecanico.Api
dotnet ef dbcontext scaffold "$CADENA" MySql.EntityFrameworkCore -o Models -c TallerContext -f

echo "Modelo regenerado en TallerMecanico.Api/Models"
