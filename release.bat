@ECHO OFF
ECHO Building Hemogenesis Weaponry 1.5 - Started
@ECHO ON
dotnet restore 1.5/Source/Hemogenesis_Weaponry.sln
dotnet build 1.5/Source/Hemogenesis_Weaponry.sln /p:Configuration=Release
@ECHO OFF
ECHO Building Hemogenesis Weaponry 1.5 - Complete
ECHO Press any key to exit...
PAUSE > NUL
