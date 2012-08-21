@echo off
rem Input parameters
rem DATABASE_NAME should NOT contain spaces
set DATABASE_NAME=LocalDatabase
set DATABASE_PASSWORD=
set NAMESPACE=Yaaf.WirePlugin.WinFormGui.Database

rem Set full path to SqlMetal.exe
rem For 64-bit Windows
set SQLMETAL="%ProgramW6432%\Microsoft SDKs\Windows\v7.0\Bin\SqlMetal.exe"
rem For 32-bit Windows
if "%ProgramW6432%"=="" set SQLMETAL="%ProgramFiles%\Microsoft SDKs\Windows\v6.0A\Bin\SqlMetal.exe"

rem Other parameters
set DATABASE_PATH="%CD%\%DATABASE_NAME%.sdf"
set OUTPUT_CS_PATH="%CD%\%DATABASE_NAME%.cs"
set OUTPUT_DBML_PATH="%CD%\%DATABASE_NAME%.dbml"
set CONNECTION_STRING="Data Source=\"%DATABASE_PATH%\"; Password=\"%DATABASE_PASSWORD%\";"

rem Check and run sqlmetal
if not exist %SQLMETAL% goto error-missing-sqlmetal
%SQLMETAL% /conn:%CONNECTION_STRING% /dbml:%OUTPUT_DBML_PATH% /namespace:%NAMESPACE% /context:%DATABASE_NAME%DataContext /pluralize
rem %SQLMETAL% /conn:%CONNECTION_STRING% /code:%OUTPUT_CS_PATH% /namespace:%NAMESPACE% /context:%DATABASE_NAME%DataContext /pluralize
goto end

:error-missing-sqlmetal
echo Error: cannot find sqlmetal.exe
echo Search location: %SQLMETAL%
goto end

:end