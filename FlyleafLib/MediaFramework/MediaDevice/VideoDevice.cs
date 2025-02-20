﻿using System;
using System.Linq;

namespace FlyleafLib.MediaFramework.MediaDevice;

public class VideoDevice : DeviceBase<VideoDeviceStream>
{
    public VideoDevice(string friendlyName, string symbolicLink) : base(friendlyName, symbolicLink)
    {
        Streams = VideoDeviceStream.GetVideoFormatsForVideoDevice(friendlyName, symbolicLink);
        Url = Streams.Where(f => f.SubType.Contains("MJPG") && f.FrameRate >= 30).OrderByDescending(f => f.FrameSizeHeight).FirstOrDefault()?.Url;
    }

    public static void RefreshDevices()
    {
        //TODO: RefreshDevices
        //Utils.UIInvokeIfRequired(() =>
        //{
        //    Engine.Video.CapDevices.Clear();

        //    var devices = MediaFactory.MFEnumVideoDeviceSources();
        //        foreach (var device in devices)
        //        try { Engine.Video.CapDevices.Add(new VideoDevice(device.FriendlyName, device.SymbolicLink)); } catch(Exception) { }
        //});
    }
}
