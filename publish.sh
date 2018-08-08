#!/usr/bin/env bash

set -e

function getfield() {
  RE="<$2>(.*)</$2>"
  if [[ "$1" =~ $RE ]]; then
    echo "${BASH_REMATCH[1]}"
  fi
}

SOURCE=https://api.nuget.org/v3/index.json
SSOURCE=https://nuget.smbsrc.net/
SLN=$(basename $(pwd))
TESTPROJ="./$SLN.Tests"
PKGPATH="./$SLN/bin/Release"
CSPROJ=$(cat "./$SLN/$SLN.csproj")
VERSION=$(getfield "$CSPROJ" Version)
PKGNAME=$(getfield "$CSPROJ" PackageId)

dotnet pack -c Release

dotnet nuget push "$PKGPATH/$PKGNAME.$VERSION.nupkg" -s "$SOURCE" -k "$NUGET_KEY"
dotnet nuget push "$PKGPATH/$PKGNAME.$VERSION.symbols.nupkg" -s "$SSOURCE" -k "$NUGET_KEY"
