@echo off

set ProjName=RomanG:@StarTeam02:49201/UltiPro.Net/06.12 December release working view/

set SrcFolder=TestApps/SWAT

echo Getting the latest SWAT files

echo.
echo.

rem echo StarTeamApp = %StarTeamApp%
rem pause


xcopy /Y /E C:\Projects\Ultipro.NET\AutomatedTests\FitNesse\fitnesse\FitNesseRoot C:\Projects\Ultipro.NET\AutomatedTests\FitNesse\fitnesse\FitNesseRoot(Backup)\


rem *-- Get latest files using Forced CheckOut:
"%StarTeamApp%\stcmd" co -p "%ProjName%/%SrcFolder%" -is -u -o -nologo "*.*"


set SrcFolder=AutomatedTests/FitNesse/fitnesse
"%StarTeamApp%\stcmd" co -p "%ProjName%/%SrcFolder%" -is -u -o -nologo "*.*"


set SrcFolder=VendorBin/Fitnesse
"%StarTeamApp%\stcmd" co -p "%ProjName%/%SrcFolder%" -is -u -o -nologo "*.*"

set SrcFolder=Assemblys/Interops
"%StarTeamApp%\stcmd" co -p "%ProjName%/%SrcFolder%" -is -u -o -nologo "*.*"

rem echo Press Ok to copy files to distrib
rem pause
xcopy /Y C:\Projects\Ultipro.NET\TestApps\SWAT\*.dll C:\Projects\Ultipro.NET\Distrib\

xcopy /Y C:\Projects\Ultipro.NET\VendorBin\Fitnesse\*.* C:\Projects\Ultipro.NET\Distrib\

xcopy /Y C:\Projects\Ultipro.NET\Assemblys\Interops\*.* C:\Projects\Ultipro.NET\Distrib\

rem *-- Show CheckOut cmd line options help:
rem "%StarTeamApp%\stcmd" co -?



echo.
echo.
echo.
echo End of Run
pause


