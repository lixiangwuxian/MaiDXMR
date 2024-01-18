# MaiDXMR

Aim to support hand tracking as input in quest.

Since this project uses OpenXR, it should be easy to port to other headset with hand tracking.

Work in progress.

Currently not very playable due to high latency in encoding.

Use assets from [MaiDXR](https://github.com/xiaopeng12138/MaiDXR).

This front-end part just does receive h264 stream and send input back to the computer.

Uses [PopH264](https://github.com/NewChromantics/PopH264) to decode h264 stream.
