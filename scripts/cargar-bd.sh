#!/usr/bin/env bash
# Carga el script SQL en la base MySQL local (contenedor taller-mysql).
set -euo pipefail

cd "$(dirname "$0")/.."

docker exec -i taller-mysql mysql -uroot -pRoot123! --default-character-set=utf8mb4 < db/TallerDB.sql

echo "Script db/TallerDB.sql ejecutado en taller-mysql"
