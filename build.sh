#! /bin/bash

if [ -d "./output" ];then
    rm -rf ./output
fi
mkdir output

dotnet build -o ./output/ --configuration Release

cp ./run.sh ./output/run.sh
