#!/bin/sh

if [ "x$KSP_TEST" == "x" ]; then
    echo KSP_TEST has not been set!	
    exit 1
else
    KSP_TEST=`echo $KSP_TEST | tr '\\' '/'`
    cp -rvf GameData "$KSP_TEST/"
fi
