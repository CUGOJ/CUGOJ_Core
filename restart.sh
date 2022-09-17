#! /bin/bash
# arg[1] = last PID

pid=$1

while true
do 
    process=`ps aux | grep $pid | grep -v grep | grep -v restart.sh`;
    if [ "$process" != "" ];then
        echo "$process"
        sleep 1;
        echo "等待进程结束"
    else 
        echo "进程结束,正在重启"
        break;
    fi
done


if [ $(basename "$PWD") = "output" ];then 
    dotnet ./CUGOJ_Core.dll
else
    dotnet run -localhost
fi

