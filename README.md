# MaiDXMR

Work in progress.

Aim to support hand tracking as input in quest.

Since this project uses OpenXR, it should be easy to port to other headset with hand tracking.

~~Currently not very playable due to high latency in encoding.~~

With lantecy adjustment it's able to achieve 80% acc on a lv9 map.

Use assets from [MaiDXR](https://github.com/xiaopeng12138/MaiDXR).

This front-end part just does receive h264 stream while send input back to the computer.

Uses [PopH264](https://github.com/NewChromantics/PopH264) to decode h264 stream.

TODO:

- [ ] find the pc IP address automatically
- [ ] sync to pc on frame send
