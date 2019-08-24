#!/bin/bash
attempt_counter=0
max_attempts=5

until $(curl --output /dev/null --silent --head --fail -X "DELETE" $1); do
    if [ ${attempt_counter} -eq ${max_attempts} ];then
      echo "Max attempts reached"
      exit 1
    fi

    printf '.'
    attempt_counter=$(($attempt_counter+1))
    sleep 5
done