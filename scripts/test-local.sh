#!/usr/bin/env bash
# Ejecuta los tests sin nuget.org: compila con el sustituto local de xUnit (LocalShim=true).
# En CI y en tu máquina usa simplemente: dotnet test Legado.sln
# Uso: scripts/test-local.sh [filtro-por-nombre]
set -euo pipefail
cd "$(dirname "$0")/.."
CFG=tools/offline.nuget.config
dotnet build tests/Legado.Core.Tests/Legado.Core.Tests.csproj -p:LocalShim=true --configfile "$CFG" -nologo -v q -clp:ErrorsOnly
dotnet build tools/ShimRunner/ShimRunner.csproj --configfile "$CFG" -nologo -v q -clp:ErrorsOnly
dotnet tools/ShimRunner/bin/Debug/net8.0/ShimRunner.dll tests/Legado.Core.Tests/bin/Debug/net8.0/Legado.Core.Tests.dll "${1:-}"
