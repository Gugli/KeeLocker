set KeePass=%UserProfile%\KeePassDEV
set KeePassPlugins=%KeePass%\plugins
set BuildDir=%TEMP%\KeeLocker
set LocalDir=%~dp0

rmdir /S /Q "%BuildDir%"
mkdir "%BuildDir%"
mkdir "%BuildDir%\Forms"
mkdir "%BuildDir%\Properties"

copy "%LocalDir%KeeLocker\Forms\*.cs" "%BuildDir%\Forms"
copy "%LocalDir%KeeLocker\Forms\*.resx" "%BuildDir%\Forms"
copy "%LocalDir%KeeLocker\*.cs" "%BuildDir%"

copy "%LocalDir%KeeLocker\Properties\*.cs" "%BuildDir%\Properties"

copy "%LocalDir%KeeLocker\KeeLocker.csproj" "%BuildDir%\KeeLocker.csproj"


"%KeePass%\KeePass.exe" --plgx-create "%BuildDir%" --debug --plgx-prereq-os:Windows


del /Q "%KeePassPlugins%\KeeLocker.plgx"
copy "%TEMP%\KeeLocker.plgx" "%KeePassPlugins%\KeeLocker.plgx"

"%KeePass%\KeePass.exe" --debug

pause