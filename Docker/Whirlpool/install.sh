#!/bin/bash

FIVESHOME=/root/fives
WEBAPPS=/var/www

echo ${FIVESHOME}
echo ${WEBAPPS}

cp -r DeviationMaps ${FIVESHOME}/Plugins/DeviationMaps

cp -r WebClientWp ${WEBAPPS}

cp -f FiVES.sln ${FIVESHOME}


