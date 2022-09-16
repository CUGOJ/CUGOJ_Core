#! /bin/bash
# args[1] = env
# args[2] = port


if [ $(basename "$PWD") = "output" ];then 
    dotnet ./CUGOJ_Core.dll -env $1 -port $2
else
    dotnet run -env $1 -port $2 -localhost
fi

