#!/bin/bash

for x in *.mp3
do
    ffmpeg -i "$x" -acodec pcm_s16le -ar 44100 "${x%.mp3}.wav"
done
