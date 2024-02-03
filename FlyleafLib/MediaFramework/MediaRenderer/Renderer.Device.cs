using System;
using System.Numerics;
using System.Text.RegularExpressions;
using FlyleafLib.MediaFramework.MediaDecoder;

using static FlyleafLib.Logger;
using System.Runtime.InteropServices;

namespace FlyleafLib.MediaFramework.MediaRenderer;

public unsafe partial class Renderer
{

    PSBufferType        psBufferData = new();

    VSBufferType        vsBufferData = new();

    internal object     lockDevice = new();
    bool                isFlushing;


    public void Initialize(bool swapChain = true)
    {
        lock (lockDevice)
        {
            try
            {
                if (CanDebug) Log.Debug("Initializing");

                if (!Disposed)
                    Dispose();

                Disposed = false;


                //GPUAdapter = Engine.Video.GPUAdapters[adapter.Description1.Luid];
                //Config.Video.MaxVerticalResolutionAuto = GPUAdapter.MaxHeight;

                if (CanDebug)
                {
                    string dump = $"GPU Adapter\r\n{GPUAdapter}\r\n";

                    for (int i=0; i<GPUAdapter.Outputs.Count; i++)
                        dump += $"[Output #{i+1}] {GPUAdapter.Outputs[i]}\r\n";

                    Log.Debug(dump);
                }


                ReadOnlySpan<float> vertexBufferData = new float[]
                {
                    -1.0f,  -1.0f,  0,      0.0f, 1.0f,
                    -1.0f,   1.0f,  0,      0.0f, 0.0f,
                     1.0f,  -1.0f,  0,      1.0f, 1.0f,

                     1.0f,  -1.0f,  0,      1.0f, 1.0f,
                    -1.0f,   1.0f,  0,      0.0f, 0.0f,
                     1.0f,   1.0f,  0,      1.0f, 0.0f
                };
                //vertexBuffer = Device.CreateBuffer(vertexBufferData, new BufferDescription() { BindFlags = BindFlags.VertexBuffer });
                //context.IASetVertexBuffer(0, vertexBuffer, sizeof(float) * 5);

                InitPS();
                

                //ShaderVS = Device.CreateVertexShader(ShaderCompiler.VSBlob);
                //vertexLayout = Device.CreateInputLayout(inputElements, ShaderCompiler.VSBlob);

                //context.IASetInputLayout(vertexLayout);
                //context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
                //context.VSSetShader(ShaderVS);

                //psBuffer = Device.CreateBuffer(new BufferDescription()
                //{
                //    Usage           = ResourceUsage.Default,
                //    BindFlags       = BindFlags.ConstantBuffer,
                //    CPUAccessFlags  = CpuAccessFlags.None,
                //    ByteWidth       = sizeof(PSBufferType) + (16 - (sizeof(PSBufferType) % 16))
                //});
                //context.PSSetConstantBuffer(0, psBuffer);
                //psBufferData.hdrmethod = HDRtoSDRMethod.None;
                //context.UpdateSubresource(psBufferData, psBuffer);

                //vsBuffer = Device.CreateBuffer(new BufferDescription()
                //{
                //    Usage           = ResourceUsage.Default,
                //    BindFlags       = BindFlags.ConstantBuffer,
                //    CPUAccessFlags  = CpuAccessFlags.None,
                //    ByteWidth       = sizeof(VSBufferType) + (16 - (sizeof(VSBufferType) % 16))
                //});

                //context.VSSetConstantBuffer(0, vsBuffer);
                //vsBufferData.mat = Matrix4x4.Identity;
                //context.UpdateSubresource(vsBufferData, vsBuffer);
                
                //InitializeVideoProcessor();
                // TBR: Device Removal Event
                //ID3D11Device4 device4 = Device.QueryInterface<ID3D11Device4>(); device4.RegisterDeviceRemovedEvent(..);

                //if (CanInfo) Log.Info($"Initialized with Feature Level {(int)Device.FeatureLevel >> 12}.{((int)Device.FeatureLevel >> 8) & 0xf}");

            } catch (Exception e)
            {
                //if (string.IsNullOrWhiteSpace(Config.Video.GPUAdapter) || Config.Video.GPUAdapter.ToUpper() != "WARP")
                //{
                //    try { if (Device != null) Log.Warn($"Device Remove Reason = {Device.DeviceRemovedReason.Description}"); } catch { } // For troubleshooting

                //    Log.Warn($"Initialization failed ({e.Message}). Failling back to WARP device.");
                //    Config.Video.GPUAdapter = "WARP";
                //    Flush();
                //}
                //else
                //{
                //    Log.Error($"Initialization failed ({e.Message})");
                //    Dispose();
                //    return;
                //}
            }

            //if (swapChain)
            //{
            //    if (ControlHandle != IntPtr.Zero)
            //        InitializeSwapChain(ControlHandle);
            //    else if (SwapChainWinUIClbk != null)
            //        InitializeWinUISwapChain();
            //}

            InitializeChildSwapChain();
        }
    }
    public void InitializeChildSwapChain(bool swapChain = true)
    {
        //if (child == null )
        //    return;

        //lock (lockDevice)
        //{
        //    child.lockDevice    = lockDevice;
        //    child.VideoDecoder  = VideoDecoder;
        //    child.Device        = Device;
        //    child.context       = context;
        //    child.curRatio      = curRatio;
        //    child.VideoRect     = VideoRect;
        //    child.videoProcessor= videoProcessor;
        //    child.InitializeVideoProcessor(); // to use the same VP we need to set it's config in each present (means we don't update VP config as is different)

        //    if (swapChain)
        //    {
        //        if (child.ControlHandle != IntPtr.Zero)
        //            child.InitializeSwapChain(child.ControlHandle);
        //        else if (child.SwapChainWinUIClbk != null)
        //            child.InitializeWinUISwapChain();
        //    }

        //    child.SetViewport();
        //}
    }
    
    public void Dispose()
    {
        lock (lockDevice)
        {
            if (Disposed)
                return;

            if (child != null)
                DisposeChild();
            
            Disposed = true;

            if (CanDebug) Log.Debug("Disposing");

            VideoDecoder.DisposeFrame(LastFrame);
            RefreshLayout();

            //DisposeVideoProcessor();

            //ShaderVS?.Dispose();
            //ShaderPS?.Dispose();
            //prevPSUniqueId = curPSUniqueId = ""; // Ensure we re-create ShaderPS for FlyleafVP on ConfigPlanes
            //psBuffer?.Dispose();
            //vsBuffer?.Dispose();
            //vertexLayout?.Dispose();
            //vertexBuffer?.Dispose();
            //DisposeSwapChain();

            //singleGpu?.Dispose();
            //singleStage?.Dispose();
            //singleGpuRtv?.Dispose();
            //singleStageDesc.Width = -1; // ensures re-allocation

            //if (rtv2 != null)
            //{
            //    for(int i = 0; i < rtv2.Length; i++)
            //        rtv2[i].Dispose();

            //    rtv2 = null;
            //}

            //if (backBuffer2 != null)
            //{
            //    for(int i = 0; i < backBuffer2.Length; i++)
            //        backBuffer2[i]?.Dispose();

            //    backBuffer2 = null;
            //}

            //if (Device != null)
            //{
            //    context.ClearState();
            //    context.Flush();
            //    context.Dispose();
            //    Device.Dispose();
            //    Device = null;
            //}

            #if DEBUG
            ReportLiveObjects();
            #endif

            curRatio = 1.0f;
            if (CanInfo) Log.Info("Disposed");
        }
    }
    public void DisposeChild()
    {
        if (child == null)
            return;

        lock (lockDevice)
        {
            child.DisposeSwapChain();
            //child.DisposeVideoProcessor();

            if (!isFlushing)
            {
                //child.Device        = null;
                //child.context       = null;
                child.VideoDecoder  = null;
                child.LastFrame     = null;
                child               = null;
            }
        }
    }

    public void Flush()
    {
        lock (lockDevice)
        {
            isFlushing = true;
            var controlHandle = ControlHandle;
            //var swapChainClbk = SwapChainWinUIClbk;

            //IntPtr controlHandleReplica = IntPtr.Zero;
            //Action<IDXGISwapChain2> swapChainClbkReplica = null;;
            //if (child != null)
            //{
            //    controlHandleReplica = child.ControlHandle;
            //    swapChainClbkReplica = child.SwapChainWinUIClbk;
            //}

            Dispose();
            ControlHandle = controlHandle;
            //SwapChainWinUIClbk = swapChainClbk;
            if (child != null)
            {
                //child.ControlHandle = controlHandleReplica;
                //child.SwapChainWinUIClbk = swapChainClbkReplica;
            }
            Initialize();
            isFlushing = false;
        }
    }

    #if DEBUG
    public static void ReportLiveObjects()
    {
    }
    #endif

    [StructLayout(LayoutKind.Sequential)]
    struct PSBufferType
    {
        public int coefsIndex;
        public HDRtoSDRMethod hdrmethod;

        public float brightness;
        public float contrast;

        public float g_luminance;
        public float g_toneP1;
        public float g_toneP2;

        public float texWidth;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct VSBufferType
    {
        public Matrix4x4 mat;
    }
}
