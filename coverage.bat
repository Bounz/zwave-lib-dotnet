.\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe -register:user "-filter:+[ZWaveLib]* -[*Test]*" "-target:.\packages\NUnit.ConsoleRunner.3.6.1\tools\nunit3-console.exe" "-targetargs: .\ZWaveLibTests\bin\Debug\ZWaveLibTests.dll" -output:"coverage.xml"

.\packages\ReportGenerator.2.5.6\tools\ReportGenerator.exe "-reports:coverage.xml" "-targetdir:.\coverage"
