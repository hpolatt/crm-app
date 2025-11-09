#!/bin/bash

# Test Coverage Raporu Oluşturma Script'i
# Kullanım: ./generate-coverage.sh

echo "🧪 Test Coverage Raporu Oluşturuluyor..."
echo ""

# Renk kodları
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# 1. Unit Tests Coverage
echo "${BLUE}📊 Unit Tests için coverage hesaplanıyor...${NC}"
dotnet test tests/CrmApp.UnitTests/CrmApp.UnitTests.csproj \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=opencover \
  /p:CoverletOutput=./TestResults/ \
  /p:Exclude="[xunit.*]*%2c[*.Tests]*" \
  --logger "console;verbosity=minimal"

echo ""

# 2. Integration Tests Coverage (API projesini de dahil et)
echo "${BLUE}📊 Integration Tests için coverage hesaplanıyor (API dahil)...${NC}"
dotnet test tests/CrmApp.IntegrationTests/CrmApp.IntegrationTests.csproj \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=opencover \
  /p:CoverletOutput=./TestResults/ \
  /p:MergeWith="../CrmApp.UnitTests/TestResults/coverage.opencover.xml" \
  /p:Include="[CrmApp.*]*" \
  /p:Exclude="[xunit.*]*%2c[*.Tests]*%2c[*]*.Program%2c[*]*.Startup" \
  --logger "console;verbosity=minimal"

echo ""

# 3. ReportGenerator ile HTML rapor oluştur
echo "${BLUE}📄 HTML raporu oluşturuluyor...${NC}"
reportgenerator \
  -reports:"tests/CrmApp.UnitTests/TestResults/coverage.opencover.xml;tests/CrmApp.IntegrationTests/TestResults/coverage.opencover.xml" \
  -targetdir:"TestResults/CoverageReport" \
  -reporttypes:"Html;HtmlSummary;Badges;TextSummary" \
  -verbosity:Warning

echo ""
echo "${GREEN}✅ Coverage raporu oluşturuldu!${NC}"
echo ""
echo "${YELLOW}📂 Rapor konumu: backend/TestResults/CoverageReport/index.html${NC}"
echo ""
echo "Raporu açmak için:"
echo "  ${BLUE}open TestResults/CoverageReport/index.html${NC}"
echo ""

# Özet bilgi göster
if [ -f "TestResults/CoverageReport/Summary.txt" ]; then
    echo "${BLUE}📈 Coverage Özeti:${NC}"
    cat TestResults/CoverageReport/Summary.txt
fi
