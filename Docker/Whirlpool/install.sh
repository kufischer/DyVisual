#!/bin/bash

FIVESHOME=/root/fives
WEBAPPS=/var/www
WPHOME=/root/DyVisual/Whirlpool/WhirlpoolFiVES

echo ${FIVESHOME}
echo ${WEBAPPS}

cp -r ${WPHOME}/DeviationMaps ${FIVESHOME}/Plugins/DeviationMaps

cp -r ${WPHOME}/WebClientWp ${WEBAPPS}

cp -f ${WPHOME}/FIVES.sln ${FIVESHOME}


