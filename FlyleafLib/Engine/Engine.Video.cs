using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using FlyleafLib.MediaFramework.MediaDevice;

namespace FlyleafLib;

public class VideoEngine
{
    /// <summary>
    /// List of Video Capture Devices
    /// </summary>
    public ObservableCollection<VideoDevice>
                            CapDevices          { get; set; } = new();
    public void             RefreshCapDevices() => VideoDevice.RefreshDevices();

    /// <summary>
    /// List of GPU Adpaters <see cref="Config.VideoConfig.GPUAdapter"/>
    /// </summary>
    public Dictionary<long, GPUAdapter>
                            GPUAdapters         { get; private set; }

    /// <summary>
    /// List of GPU Outputs from default GPU Adapter (Note: will no be updated on screen connect/disconnect)
    /// </summary>
    public List<GPUOutput>  Screens             { get; private set; } = new List<GPUOutput>();

    public GPUOutput GetScreenFromPosition(int top, int left)
    {
        foreach(var screen in Screens)
        {
            if (top >= screen.Top && top <= screen.Bottom && left >= screen.Left && left <= screen.Right)
                return screen;
        }

        return null;
    }

    private static string VendorIdStr(int vendorId)
    {
        switch (vendorId)
        {
            case 0x1002:
                return "ATI";
            case 0x10DE:
                return "NVIDIA";
            case 0x1106:
                return "VIA";
            case 0x8086:
                return "Intel";
            case 0x5333:
                return "S3 Graphics";
            case 0x4D4F4351:
                return "Qualcomm";
            default:
                return "Unknown";
        }
    }
}
