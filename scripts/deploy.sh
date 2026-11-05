#!/usr/bin/env bash
# Publica la API en MonsterASP.NET por FTP.
#
# Uso:
#   export FTP_HOST="site105061.siteasp.net"
#   export FTP_USER="site105061"
#   export FTP_PASS="********"
#   export DB_CONNECTION="Server=...;Database=...;Uid=...;Pwd=...;CharSet=utf8mb4;"
#   export SITE_URL="http://taller-mecanico.runasp.net"
#   ./scripts/deploy.sh
#
# La cadena de conexión NO se guarda en el repositorio: solo se escribe en el
# web.config del servidor en el momento de publicar.
set -euo pipefail

cd "$(dirname "$0")/.."

: "${FTP_HOST:?Falta FTP_HOST}"
: "${FTP_USER:?Falta FTP_USER}"
: "${FTP_PASS:?Falta FTP_PASS}"
: "${DB_CONNECTION:?Falta DB_CONNECTION}"
SITE_URL="${SITE_URL:-http://taller-mecanico.runasp.net}"

echo "1/4 Compilando en Release..."
rm -rf publish
dotnet publish TallerMecanico.Api -c Release -o publish

echo "2/4 Generando web.config con variables de entorno..."
cat > publish/web.config <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" arguments=".\TallerMecanico.Api.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" hostingModel="outofprocess">
        <environmentVariables>
          <environmentVariable name="ConnectionStrings__TallerDB" value="${DB_CONNECTION}" />
          <environmentVariable name="Cors__Origenes__0" value="${SITE_URL}" />
        </environmentVariables>
      </aspNetCore>
    </system.webServer>
  </location>
</configuration>
EOF

echo "3/4 Subiendo por FTP..."
echo "mantenimiento" > publish/app_offline.htm
lftp -u "${FTP_USER},${FTP_PASS}" "${FTP_HOST}" -e "set ftp:ssl-allow no; put publish/app_offline.htm -o wwwroot/app_offline.htm; bye" >/dev/null
sleep 5
lftp -u "${FTP_USER},${FTP_PASS}" "${FTP_HOST}" -e "set ftp:ssl-allow no; mirror -R --delete --parallel=1 --exclude-glob logs/ publish wwwroot; bye"
lftp -u "${FTP_USER},${FTP_PASS}" "${FTP_HOST}" -e "set ftp:ssl-allow no; rm wwwroot/app_offline.htm; bye" >/dev/null
rm -f publish/app_offline.htm

echo "4/4 Verificando..."
sleep 8
curl -s -o /dev/null -w "Swagger: %{http_code}\n" --max-time 90 "${SITE_URL}/swagger/index.html"
curl -s -o /dev/null -w "Clientes: %{http_code}\n" --max-time 60 "${SITE_URL}/api/clientes"
