dotnet test /p:CollectCoverage=true /p:CoverletOutput=TestResults/ /p:CoverletOutputFormat=lcov /p:Exclude="[*]openld.Migrations.*%2c[*]openld.Program*%2c[*]openld.Startup*%2c[*]openld.Pages.*"
reportgenerator -reports:TestResults/coverage.info -reporttypes:Html -targetdir:TestResults/coverage

xdg-open TestResults/coverage/index.htm