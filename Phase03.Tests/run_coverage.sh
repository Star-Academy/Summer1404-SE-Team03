#!/bin/bash

# To Run:
# chmod +x run_coverage.sh
# ./run_coverage.sh

set -e

REPORT_DIR="CoverageReport"
TEST_RESULT_DIR="TestResults"
SERVER_ADDRESS="127.0.0.1:8000"

echo "✅ Step 1: Cleaning up old report directory..."
rm -rf "$REPORT_DIR"
rm -rf "$TEST_RESULT_DIR"
echo "Done."

echo "✅ Step 2: Running tests and collecting coverage..."
dotnet test --collect:"XPlat Code Coverage"
echo "Done."

echo "✅ Step 3: Finding the coverage result file..."

COVERAGE_FILE=$(find . -name "coverage.cobertura.xml" | head -n 1)

if [ -z "$COVERAGE_FILE" ]; then
    echo "❌ Error: Could not find the coverage.cobertura.xml file."
    echo "Make sure that 'dotnet test' ran successfully and that 'coverlet.collector' is installed."
    exit 1
fi

echo "Found coverage file: $COVERAGE_FILE"

echo "✅ Step 4: Generating the HTML report..."
reportgenerator \
    "-reports:$COVERAGE_FILE" \
    "-targetdir:$REPORT_DIR" \
    -reporttypes:Html
echo "Report generated in '$REPORT_DIR' directory."

echo "✅ Step 5: Serving the report locally..."
echo "============================================================"
echo "  Starting local web server..."
echo "  Your report is available at: http://$SERVER_ADDRESS"
echo "  Press Ctrl+C to stop the server and exit the script."
echo "============================================================"

(cd "$REPORT_DIR" && php -S "$SERVER_ADDRESS")

echo "✅ Server stopped. Script finished."