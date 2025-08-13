#!/bin/bash

if [ -z "$1" ]; then
  echo "Usage: ./migration.sh <MigrationName>"
  exit 1
fi

MIGRATION_NAME=$1

echo "Adding migration: $MIGRATION_NAME..."
dotnet ef migrations add $MIGRATION_NAME

echo "Updating database..."
dotnet ef database update

echo "Done."