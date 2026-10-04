using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static PopH264;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance;
    private Material DisplayP1Mat;
    private PopH264.Decoder Decoder;
    private PopH264.DecoderParams param;
    private PopH264.FrameInput h264Frame;
    // Filled by the UDP receive thread, drained on the main thread in Update().
    private readonly ConcurrentQueue<byte[]> pendingPackets = new ConcurrentQueue<byte[]>();
    List<Texture2D> h264Textures = new();
    List<PopH264.PixelFormat> pixelFormats = new();
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        DisplayP1Mat = GetComponent<Renderer>().material;
        param.Decoder = "";
        param.VerboseDebug = false;
        param.AllowBuffering = false;
        param.LowPowerMode = false;
        param.DropBadFrames = true;
        Decoder = new PopH264.Decoder(param,true);
        h264Frame.FrameNumber = 0;
    }

    // Called from the UDP receive thread.
    public void EnqueuePacket(byte[] h264ScreenData)
    {
        pendingPackets.Enqueue(h264ScreenData);
    }

    // Update is called once per frame
    void Update()
    {
        // Feed everything that arrived since the last frame to the decoder.
        while (pendingPackets.TryDequeue(out var packet))
        {
            h264Frame.Bytes = packet;
            Decoder.PushFrameData(h264Frame);
            h264Frame.FrameNumber++;
        }

        // Drain ALL decoded frames and keep only the newest one. Popping just one
        // frame per pushed packet lets any decoder hiccup turn into a permanent
        // backlog (= permanent latency) that never shrinks.
        bool gotFrame = false;
        while (GetNextFrame(ref h264Textures, ref pixelFormats) != null)
        {
            gotFrame = true;
        }

        if (gotFrame && h264Textures.Count > 1)
        {
            DisplayP1Mat.SetTexture("_YTex", h264Textures[0]);
            DisplayP1Mat.SetTexture("_UVTex", h264Textures[1]);
        }
    }
    public int? GetNextFrame(ref List<Texture2D> Planes, ref List<PixelFormat> PixelFormats)
    {

        var Meta = Decoder.GetNextFrameAndMeta(ref Planes, ref PixelFormats);
        if (!Meta.HasValue)
            return null;

        return Meta.Value.FrameNumber;
    }
}
