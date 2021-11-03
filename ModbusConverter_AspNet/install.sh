#!/bin/bash

INSTALL_PATH='/usr/local/bin/ModbusConverter'
SERVICE_FILE_NAME='modbus-converter.service'

echo Creating directory $INSTALL_PATH
mkdir $INSTALL_PATH

echo Copying contents of this file to $INSTALL_PATH
cp -r * $INSTALL_PATH

echo Copying $SERVICE_FILE_NAME to /etc/systemd/system
cp ./$SERVICE_FILE_NAME /etc/systemd/system/

echo Reloading systemctl daemon
systemctl daemon-reload

echo Enabling $SERVICE_FILE_NAME
systemctl enable $SERVICE_FILE_NAME
