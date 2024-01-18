using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

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
        param.VerboseDebug = true;
        param.AllowBuffering = true;
        param.LowPowerMode = false;
        param.DropBadFrames = true;
        Decoder = new PopH264.Decoder(param,true);
        h264Frame.FrameNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        while (jobs.Count > 0)
        {
            jobs.Dequeue().Invoke();
        }
        Decoder.GetNextFrame(ref h264Textures, ref pixelFormats);

        Debug.Log("Got " + h264Textures.Count + " Frame");

        if (h264Textures.Count > 0)
        {
            DisplayP1Mat.SetTexture("_YTex", h264Textures[0]);
            DisplayP1Mat.SetTexture("_UVTex", h264Textures[1]);
            //DisplayP1Mat.mainTexture = h264Textures[0];
            Debug.Log("Updated! Width is " + h264Textures[0].width + " height is " + h264Textures[0].height);
        }
    }
    public void UpdateScreen(byte[] h264ScreenData)
    {
        Debug.Log("UpdateScreen frame "+h264Frame.FrameNumber);
        h264Frame.Bytes = h264ScreenData;
        Decoder.PushFrameData(h264Frame);
        h264Frame.FrameNumber++;
        
    }
    Texture2D MergeTextures(Texture2D rTexture, Texture2D gbTexture)
    {
        int width = rTexture.width;
        int height = rTexture.height;

        Texture2D rgbTexture = new Texture2D(width, height, TextureFormat.RGB24, false);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color rColor = rTexture.GetPixel(x, y);
                Color gbColor = gbTexture.GetPixel(x, y);

                // ´´½¨ RGB ÑÕÉ«
                Color rgbColor = new Color(rColor.r, gbColor.g, gbColor.b);
                rgbTexture.SetPixel(x, y, rgbColor);
            }
        }

        rgbTexture.Apply();
        return rgbTexture;
    }
}
