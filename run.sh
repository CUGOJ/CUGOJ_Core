#! /bin/bash

if [ $(basename "$PWD") = "output" ];then 
    dotnet ./CUGOJ_Core.dll
else
    dotnet run
fi

