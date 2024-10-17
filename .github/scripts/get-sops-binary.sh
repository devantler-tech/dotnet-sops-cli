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
    unzip "$target_dir/$target_name.zip" -d "$target_dir"
    mv "$target_dir/$binary" "${target_dir}/$target_name"
    rm "$target_dir/$target_name.zip"
  elif [ "$archiveType" = false ]; then
    curl -LJ "$url" -o "$target_dir/$target_name"
  fi
  chmod +x "$target_dir/$target_name"
}

get "https://getbin.io/getsops/sops?os=darwin&arch=amd64" "sops" "Devantler.SOPSCLI/assets/binaries" "sops-darwin-amd64" false
get "https://getbin.io/getsops/sops?os=darwin&arch=arm64" "sops" "Devantler.SOPSCLI/assets/binaries" "sops-darwin-arm64" false
get "https://getbin.io/getsops/sops?os=linux&arch=amd64" "sops" "Devantler.SOPSCLI/assets/binaries" "sops-linux-amd64" false
get "https://getbin.io/getsops/sops?os=linux&arch=arm64" "sops" "Devantler.SOPSCLI/assets/binaries" "sops-linux-arm64" false
get "https://getbin.io/getsops/sops?os=windows&arch=amd64" "sops.exe" "Devantler.SOPSCLI/assets/binaries" "sops-windows-amd64.exe" false
