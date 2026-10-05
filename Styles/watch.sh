#!/usr/bin/env bash
# ponytail: el target de MSBuild que compila Tailwind solo corre en rebuilds reales; el Hot Reload
# de Blazor aplica cambios de Razor en el proceso ya corriendo y NO pasa por MSBuild. Este script
# mantiene un `tailwindcss --watch` vivo mientras `dotnet watch run` esté activo (dotnet watch ya
# detecta por su cuenta cuando wwwroot/app.css cambia en disco y refresca el CSS en el navegador),
# y lo apaga solo cuando dotnet watch termina. Si algún día esto no basta (ej. soporte Windows),
# subir de nivel a un supervisor real.
set -e
cd "$(dirname "$0")/.."

watch_pid=$(pgrep -f dotnet-watch | head -1) || exit 0

pidfile=.tailwind-watch.pid
if [ -f "$pidfile" ] && kill -0 "$(cat "$pidfile")" 2>/dev/null; then
  exit 0
fi

nohup bash -c "
  pnpm exec tailwindcss -i ./Styles/tailwind.css -o ./wwwroot/app.css --watch=always &
  tw_pid=\$!
  echo \$tw_pid > '$pidfile'
  while kill -0 $watch_pid 2>/dev/null; do sleep 2; done
  kill \$tw_pid 2>/dev/null
  rm -f '$pidfile'
" > /tmp/tailwind-watch.log 2>&1 &
disown
