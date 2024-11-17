#!/bin/bash
set -e

get() {
  local url=$1
  local binary=$2
  local target_dir=$3
  local target_name=$4
  local archiveType=$5

  echo "Downloading $target_name from $url"
  if [ "$archiveType" = "tar" ]; then
    curl -LJ "$url" | tar xvz -C "$target_dir" "$binary"
    mv "$target_dir/$binary" "${target_dir}/$target_name"
  elif [ "$archiveType" = "zip" ]; then
    curl -LJ "$url" -o "$target_dir/$target_name.zip"
    unzip -o "$target_dir/$target_name.zip" -d "$target_dir"
    mv "$target_dir/$binary" "${target_dir}/$target_name"
    rm "$target_dir/$target_name.zip"
  elif [ "$archiveType" = false ]; then
    curl -LJ "$url" -o "$target_dir/$target_name"
  fi
  chmod +x "$target_dir/$target_name"
}

get "https://getbin.io/getsops/sops?os=darwin&arch=amd64" "sops" "Devantler.SOPSCLI/runtimes/osx-x64/native" "sops-osx-x64" false
get "https://getbin.io/getsops/sops?os=darwin&arch=arm64" "sops" "Devantler.SOPSCLI/runtimes/osx-arm64/native" "sops-osx-arm64" false
get "https://getbin.io/getsops/sops?os=linux&arch=amd64" "sops" "Devantler.SOPSCLI/runtimes/linux-x64/native" "sops-linux-x64" false
get "https://getbin.io/getsops/sops?os=linux&arch=arm64" "sops" "Devantler.SOPSCLI/runtimes/linux-arm64/native" "sops-linux-arm64" false
version=$(curl -s https://api.github.com/repos/getsops/sops/releases/latest | grep '"tag_name":' | sed -E 's/.*"([^"]+)".*/\1/')
get "https://github.com/getsops/sops/releases/download/$version/sops-$version.exe" "sops.exe" "Devantler.SOPSCLI/runtimes/win-x64/native" "sops-windows-x64.exe" false
