using FFmpeg.AutoGen;

namespace FlyleafLib.MediaFramework.MediaFrame;

public unsafe class VideoFrame : FrameBase
{
    // Zero-Copy
    public int                          subresource;    // FFmpeg texture's array index
    public AVBufferRef*                 bufRef;         // Lets ffmpeg to know that we still need it
}
