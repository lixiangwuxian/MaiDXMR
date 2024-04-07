using System;
using System.Collections;
using System.Collections.Generic;
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
    public Queue<Action> jobs = new Queue<Action>();
    List<Texture2D> h264Textures = new();
    List<PopH264.PixelFormat> pixelFormats = new();
    private Thread updateThread = null;
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
        updateThread = new Thread(new ThreadStart(FixedUpdate));
        updateThread.IsBackground = true;
        updateThread.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (jobs.Count == 0)
        {
            //sleep 1ms

        }
        while (jobs.Count > 0)
        {
            jobs.Dequeue().Invoke();
        }
    }
    public void UpdateScreen(byte[] h264ScreenData)
    {
        Debug.Log("UpdateScreen frame "+h264Frame.FrameNumber);
        h264Frame.Bytes = h264ScreenData;
        Decoder.PushFrameData(h264Frame);
        h264Frame.FrameNumber++;

        GetNextFrame(ref h264Textures, ref pixelFormats);
        //Debug.Log("Got " + h264Textures.Count + " Frame");

        if (h264Textures.Count > 0)
        {
            DisplayP1Mat.SetTexture("_YTex", h264Textures[0]);
            DisplayP1Mat.SetTexture("_UVTex", h264Textures[1]);
            //DisplayP1Mat.mainTexture = h264Textures[0];
            //Debug.Log("Updated! Width is " + h264Textures[0].width + " height is " + h264Textures[0].height);
        }
        //while (GetNextFrame(ref h264Textures, ref pixelFormats) != null) { }

    }
    public int? GetNextFrame(ref List<Texture2D> Planes, ref List<PixelFormat> PixelFormats)
    {

        var Meta = Decoder.GetNextFrameAndMeta(ref Planes, ref PixelFormats);
        if (!Meta.HasValue)
            return null;

        return Meta.Value.FrameNumber;
    }
}
