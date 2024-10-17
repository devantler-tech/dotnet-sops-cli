#!/bin/bash
set -e

get() {
  local url=$1
  local binary=$2
  local target_dir=$3
  local target_name=$4
  local isTar=$5

  echo "Downloading $target_name from $url"
  if [ "$isTar" = true ]; then
    curl -LJ "$url" | tar xvz -C "$target_dir" "$binary"
    mv "$target_dir/$binary" "${target_dir}/$target_name"
  elif [ "$isTar" = false ]; then
    curl -LJ "$url" -o "$target_dir/$target_name"
  fi
  chmod +x "$target_dir/$target_name"
}

get "https://getbin.io/getsops/sops?os=darwin&arch=amd64" "sops" "Devantler.SOPSCLI/assets/binaries" "sops-darwin-amd64" false
get "https://getbin.io/getsops/sops?os=darwin&arch=arm64" "sops" "Devantler.SOPSCLI/assets/binaries" "sops-darwin-arm64" false
get "https://getbin.io/getsops/sops?os=linux&arch=amd64" "sops" "Devantler.SOPSCLI/assets/binaries" "sops-linux-amd64" false
get "https://getbin.io/getsops/sops?os=linux&arch=arm64" "sops" "Devantler.SOPSCLI/assets/binaries" "sops-linux-arm64" false
curl -s "https://api.github.com/repos/getsops/sops/releases" | grep "browser_download.*.exe" | cut -d '"' -f 4 | sort -V| tail -n 2 | head -1 | xargs curl -LJ -o Devantler.SOPSCLI/assets/binaries/sops-windows-amd64.exe
