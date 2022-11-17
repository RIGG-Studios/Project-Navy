using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class WaveHeightMap : MonoBehaviour
{
    public ComputeShader computeShader;
    public Mesh heightDataMesh;

    
    private ComputeBuffer _buffer;
    private TemporaryRenderTexture _heightDataTexture;

    private float _waterHeight;
    private float _currentHeightDataScale;
    private int _lastSize;

    private bool _heightDataUpdated;

    private NativeArray<FFTHeightData> _rawHeightData;

    public delegate void HeightDataHandler();
    public event HeightDataHandler IsDataReadComplete;

    Material dataHeightMaterial;

    
    
    public class KW_WaterSurfaceData
    {
        public bool IsActualDataReady;
        public Vector3 Position;
        public Vector3 Normal;
    }
    
    public struct FFTHeightData
    {
        public float height;
        public Vector3 normal;
    }

    public void Release()
    {
        Utility.SafeDestroy(dataHeightMaterial, heightDataMesh);
        
        _heightDataTexture.Release();
        
        if(_buffer != null) _buffer.Release();
        _lastSize = 0;


        _heightDataUpdated = true;
    }


    private void InitializeResources(int size)
    {
        _heightDataTexture.Alloc("HeighDataRT", size, size, 0, GraphicsFormat.R16_SFloat);
        
        var bufferSize = sizeof(float) * 4; //height float + normal float x3
        _buffer = new ComputeBuffer(size * size, bufferSize);
        
        computeShader.SetTexture(0, "RawHeightDataTex", _heightDataTexture.rt);
        computeShader.SetBuffer(0, "RawHeightData", _buffer);


        _heightDataUpdated = false;
        _lastSize = size;
    }
    
    private void OnCompleteGPUReadback(AsyncGPUReadbackRequest request)
    {
        if (request.hasError)
        {
            Debug.Log("FFT HeightData GPU readback error detected.");
            return;
        }
        if (request.done)
        {
            _rawHeightData = request.GetData<FFTHeightData>();
            _heightDataUpdated = true;
            IsDataReadComplete?.Invoke();
        }
    }
}

public class TemporaryRenderTexture
{
    public RenderTextureDescriptor descriptor;
    public RenderTexture rt;
    string _name;
    public bool isInitialized;

    public TemporaryRenderTexture()
    {

    }

    public TemporaryRenderTexture(string name, TemporaryRenderTexture source)
    {
        descriptor = source.descriptor;
        rt = RenderTexture.GetTemporary(descriptor);
        rt.name = name;
        _name = name;
        if (!rt.IsCreated()) rt.Create();
        isInitialized = true;
    }

    public void Alloc(string name, int width, int height, int depth, GraphicsFormat format)
    {
        if (rt == null)
        {
#if UNITY_2019_2_OR_NEWER
            descriptor = new RenderTextureDescriptor(width, height, format, depth);
#else
                    descriptor =
 new RenderTextureDescriptor(width, height, GraphicsFormatUtility.GetRenderTextureFormat(format), depth);
#endif

            descriptor.sRGB = false;
            descriptor.useMipMap = false;
            descriptor.autoGenerateMips = false;

            rt = RenderTexture.GetTemporary(descriptor);
            rt.name = name;
            _name = name;
            if (!rt.IsCreated()) rt.Create();
            isInitialized = true;

        }
        else if (rt.width != width || rt.height != height || !isInitialized || _name != name)
        {
            if (isInitialized) Release();

            descriptor.width = width;
            descriptor.height = height;

            rt = RenderTexture.GetTemporary(descriptor);
            rt.name = name;
            _name = name;
            if (!rt.IsCreated()) rt.Create();
            isInitialized = true;
        }
    }

    public void Release(bool unlink = false)
    {
        if (rt != null)
        {
            RenderTexture.ReleaseTemporary(rt);
            isInitialized = false;
            if (unlink) rt = null;
        }
    }
}
