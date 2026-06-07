#!/bin/bash
cd "$(dirname "$0")"
dotnet publish BookStoreApp/BookStoreApp.csproj -c Debug -o BookStoreApp/publish --nologo -q
cd BookStoreApp/publish && dotnet BookStoreApp.dll
