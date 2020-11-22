#!/bin/bash

echo "Restoring .NET Core tools"
dotnet tool restore

echo "Running Build"
dotnet cake recipe.cake "$@"
