#!/bin/bash

version=v1.0.0

echo "***********************************"
echo "* 3D PRINT CONSOLE .NET INSTALLER *"
echo "***********************************"
echo
echo "v1.0.0 for Linux (deb)"
echo

if [[ $EUID -ne 0 ]];
then
   echo "[!] This installer must be run as root, exiting..." 
   exit 1
fi

dotnet --version >> /dev/null

if [[ $? -ne 0 ]];
then
   echo "[!] .NET not installed, please install the .NET Runtime from Microsoft." 
   exit 1
fi

echo "Updating package lists..."
apt-get update >> /dev/null
echo "Installing dependencies..."
apt-get install -y wget systemd unzip >> /dev/null

echo "Downloading LICENSE.txt file..."
wget "https://raw.githubusercontent.com/Longridge-High-School/3d-print-console-dotnet/refs/tags/$version/LICENSE.txt" -O /tmp/LICENSE.txt -q --show-progress
less /tmp/LICENSE.txt
read -p "Do you agree to the license? [Y/N, default Y] " agreed

if [[ $agreed =~ ^[nN] ]];
then
   rm -f /tmp/LICENSE.txt
   echo "[!] You did not agree to the license, exiting..."  
   exit 0
fi

echo "Downloading 3D Print Console .NET from GitHub..."
wget "https://github.com/Longridge-High-School/3d-print-console-dotnet/releases/download/$version/3d-print-console-dotnet-$version.zip" -O /tmp/3d-print-console-dotnet.zip -q --show-progress
echo "Extracting package..."
unzip /tmp/3d-print-console-dotnet.zip -d /srv/ >> /dev/null
rm -f /tmp/3d-print-console-dotnet.zip
mv /tmp/LICENSE.txt /srv/3d-print-console-dotnet/LICENSE.txt

read -p "Which user will 3D Print Console .NET run as? " appUser
chown -R $appUser /srv/3d-print-console-dotnet/*

echo "Downloading systemd file..."
wget "https://raw.githubusercontent.com/Longridge-High-School/3d-print-console-dotnet/refs/tags/$version/utilities/systemd/3d-print-console.service" -O /etc/systemd/system/3d-print-console.service -q --show-progress
sed -i -e "s/YOUR_USER_HERE/$appUser/g" /etc/systemd/system/3d-print-console.service

read -p "Install Redis on this machine? [Y/N, default N] " shouldInstallRedis

if [[ $shouldInstallRedis =~ ^[yY] ]];
then
   echo "Installing Redis dependencies..." 
   apt-get install lsb-release curl gpg >> /dev/null
   echo "Adding Redis to package list..."
   curl -fsSL https://packages.redis.io/gpg | gpg --dearmor -o /usr/share/keyrings/redis-archive-keyring.gpg 
   chmod 644 /usr/share/keyrings/redis-archive-keyring.gpg
   echo "deb [signed-by=/usr/share/keyrings/redis-archive-keyring.gpg] https://packages.redis.io/deb $(lsb_release -cs) main" | tee /etc/apt/sources.list.d/redis.list
   echo "Updating package lists..."
   apt-get update >> /dev/null

   echo "Installing Redis..."
   apt-get install redis >> /dev/null

   echo "Setting up Redis service..."
   systemctl enable redis-server
   systemctl start redis-server

   echo "Redis installed and configured!"
fi

echo "Starting 3D Print Console .NET service..."
systemctl start 3d-print-console
systemctl enable 3d-print-console

echo "Install complete!"
exit 0