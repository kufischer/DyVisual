set FIVESHOME=c:\DyVisual\FIVES
set WEBAPPS=C:\apache-tomcat-8.0.24\webapps

echo %FIVESHOME%
echo %WEBAPPS%

mkdir %FIVESHOME%\Plugins\DeviationMaps
xcopy DeviationMaps %FIVESHOME%\Plugins\DeviationMaps /s /y

mkdir %WEBAPPS%\WebClientWp
xcopy WebClientWp %WEBAPPS%\WebClientWp /s /y

copy FiVES.sln %FIVESHOME%


