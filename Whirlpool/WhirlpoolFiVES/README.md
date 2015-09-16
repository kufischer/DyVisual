# DyVisual\Whirlpool\WhirlpoolApp

Source Code of the DyVisual Web Client and FiVES plugin for displaying deviation maps.

INSTALLATION

If you do not have a Apache Tomcat server installed download and install Tomcat from http://tomcat.apache.org/
where install means that you uncompress the zip archive and put the uncompressed folder
not too deeple nested (Windows path length is restricted)into the Winows file system.

Download FiVES from https://github.com/fives-team/fives by cloning the git repository.
(You can also use the zip archive and the rest of the install should work, too, but some paths for the produced executables of FiVES differ from what is described here).
If you are not familiar with git we recommend to use https://git-scm.com/download/win and https://tortoisegit.org/download/.

To install the app use and editor to open install.bat and check whether the variables
%FIVESHOME% and %WEBAPS% point to the right folders. The former needs to point to the
root folder of FiVES and the latter needs to point to the webapps folder of the Apache Tomcat
Web server. If you want to use a different Web server you need to copy the data manually or
modify the install.bat script accordingly.

When the variables in install.bat are set correctly. execute .\install.bat in a command line window.

Before you can strat the app you need to compile FiVES with Visual Studio.
You can download visual studio from http://www.microsoft.com/en-us/download/details.aspx?id=44914
We recommend Visual Studio Express 2013 which should work out of the box.
If you want to use a different version, you need to find your own way how to use it.

With Visual Studio installed double click on FiVES.sln (be sure that you did run install.bat) and recreate the project.
The compile should report 29 successes and 0 errors.

RUN THE APP

1. Start Apache Tomcat: Double-click on startup.bat in the Tomcat bin folder
2. Start FiVES by double-clicking on %FIVESHOME%\Binaries\Debug\FiVES.exe (wait till comand prompt is shown)
3. Start Google Chrome or Firefox
4. Point the browser to localhost:8080/WebClientWp/client.html

You can access the Web server from remote. However, if you want to do that, you need to change in file:
%WEBAPS%\WebClientWp\kiara\fives.json "localhost" into "host-address" where host-address is either the host name
or the IP-address of the host on which the Web server and FiVES are started.



 


