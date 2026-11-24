#!/bin/bash

ARCH=$(uname -m)
APP_ARCH=""
case $ARCH in
    x86_64)
        APP_ARCH="linux-x64"
        ;;
    aarch64)
        APP_ARCH="linux-arm64"
        ;;
    *)
        echo "Unsupported architecture: $ARCH"
        exit 1
        ;;
esac

CONFIG_PATH="$HOME/.docker_service_config"

export APP_ARCH
export CONFIG_PATH
docker compose build