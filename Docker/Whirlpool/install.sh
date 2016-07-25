#!/bin/bash

FIVESHOME=/root/fives
WEBAPPS=/var/www
WPHOME=/root/DyVisual/Whirlpool/WhirlpoolFiVES

echo ${FIVESHOME}
echo ${WEBAPPS}

cp -r ${WPHOME}/DeviationMaps ${FIVESHOME}/Plugins/

mkdir ${WEBAPPS}/WebClientWp

cp -r ${WPHOME}/WebClientWp ${WEBAPPS}/WebClientWp

cp -f ${WPHOME}/FIVES.sln ${FIVESHOME}


