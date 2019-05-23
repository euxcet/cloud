#!/bin/sh
set -ex

sudo ifconfig over6 down
g++ main.cpp -o main -g -lpthread
sudo ./main
