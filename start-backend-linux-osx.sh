#!/bin/bash

echo -e "\033[36m=========================================\033[0m"
echo -e "\033[36m   OMDB Terminal - Infrastructure Setup  \033[0m"
echo -e "\033[36m=========================================\033[0m"
echo ""

default_key="47c4ba47"
read -p "Please enter your OMDb API Key (Press Enter to use the default demo key): " api_key

if [ -z "$api_key" ]; then
    api_key=$default_key
    echo -e "\n\033[90m[+] No key provided. Using the built-in demo key.\033[0m"
fi

echo -e "\n\033[33m[+] Ensuring .NET User Secrets are initialized...\033[0m"
dotnet user-secrets init --project "OmdbTerminal/OmdbTerminal.ApiService/OmdbTerminal.ApiService.csproj"

echo -e "\n\033[33m[+] Setting API Key securely in .NET User Secrets...\033[0m"
dotnet user-secrets set "Omdb:ApiKey" "$api_key" --project "OmdbTerminal/OmdbTerminal.ApiService/OmdbTerminal.ApiService.csproj"
echo -e "\033[32m[+] Key saved successfully.\033[0m"

echo -e "\n\033[33m[+] Booting up .NET Aspire Orchestrator (API + MySQL)...\033[0m"
echo -e "\033[90m    (Keep this terminal open to keep the backend running)\033[0m\n"

dotnet run --project "OmdbTerminal/OmdbTerminal.AppHost/OmdbTerminal.AppHost.csproj" --configuration Release