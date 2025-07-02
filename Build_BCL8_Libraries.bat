@ECHO off
SETLOCAL

@REM ------------------------------------------------------------
@REM Build_BCL8_Libraries.bat
@REM Dieses Skript baut die SmartExpert BCL8 Libraries mit .NET 8.
@REM Optionales Argument: 1 = Build-Konfiguration (z. B. Debug oder Release)
@REM ------------------------------------------------------------

@ECHO.
@ECHO ========================================================
@ECHO   Build BCL8 Libraries - .NET 8
@ECHO ========================================================
@ECHO.

REM Standardwerte setzen
set buildConfig=Debug
set solutionFile=SmartExpert.BCL8.CoreLt.sln

REM Eingabeargument prüfen
IF NOT "%1"=="" SET buildConfig=%1

REM Prüfen, ob dotnet vorhanden ist
WHERE dotnet >nul 2>nul
IF ERRORLEVEL 1 (
    ECHO [FEHLER] .NET 8 SDK wurde nicht gefunden. Bitte installieren Sie es von:
    ECHO https://dotnet.microsoft.com/en-us/download/dotnet/8.0
    GOTO END
)

REM Prüfen, ob die Solution-Datei vorhanden ist
IF NOT EXIST "%solutionFile%" (
    ECHO [FEHLER] Die Datei "%solutionFile%" wurde nicht gefunden.
    GOTO END
)

REM Build-Befehl ausführen
ECHO [INFO] Baue Lösung mit Konfiguration "%buildConfig%" ...
dotnet build "%solutionFile%" -c %buildConfig%
IF ERRORLEVEL 1 (
    ECHO [FEHLER] Der Build-Vorgang ist fehlgeschlagen.
    GOTO END
)

ECHO.
ECHO [INFO] Build abgeschlossen.

:END
IF EXIST "%ComSpec%" PAUSE
ENDLOCAL
