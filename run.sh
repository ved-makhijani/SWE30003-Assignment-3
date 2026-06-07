#!/bin/bash
cd "$(dirname "$0")"
dotnet publish BookStoreApp/BookStoreApp.csproj -c Debug -o BookStoreApp/publish
cd BookStoreApp/publish && dotnet BookStoreApp.dll
