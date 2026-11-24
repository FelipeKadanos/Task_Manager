#!/usr/bin/env bash
set -e

cd "$(dirname "$0")"

(cd TaskManager.Api && dotnet ef database update)
(cd task_manager.front && { [ -d node_modules ] || npm ci; })

(cd TaskManager.Api && exec dotnet run --launch-profile http) &
api_pid=$!
trap 'kill "$api_pid" 2>/dev/null || true' EXIT

cd task_manager.front
npm start
