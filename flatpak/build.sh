#!/bin/sh

set -eu

# TODO: Copy libwebkit2gtk-4.1
# ls -l

# Copy App
cd ./KHID.UI/bin/Release/net8.0/linux-x64/publish/
cp -r . /app/bin/

# Debug
ls -l
echo "----------"
ls -l /app/bin