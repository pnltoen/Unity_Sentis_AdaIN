using System.Collections;
using System.Collections.Generic;
using Unity.Sentis;
using UnityEngine;
using UnityEngine.Rendering;

public class TTS : MonoBehaviour
{
    public ModelAsset _AdaINModel;
    public Texture2D _style;
    public RenderTexture _content;
    public RenderTexture _result;
    public Camera _cam;
    Dictionary<string, Tensor> _InputTensors;
    //Tensors
    TensorFloat t_style;
    TensorFloat t_content;
    TensorFloat t_output;

    //Worker
    Model _RuntimeModel;
    IWorker _Engine;

    // Start is called before the first frame update
    void Start()
    {
        _RuntimeModel = ModelLoader.Load(_AdaINModel);
        _cam = GetComponent<Camera>();
        _Engine = WorkerFactory.CreateWorker(BackendType.GPUCompute, _RuntimeModel);
        RenderPipelineManager.endFrameRendering += OnEndFrameRendering;
    }

    void OnEndFrameRendering(ScriptableRenderContext context, Camera[] cameras)
    {
        Exctue_Sents();
    }

    void Exctue_Sents()
    {
        //Rotated the Camera
        transform.eulerAngles = new Vector3(0, 0, 180);
        _cam.targetTexture = _content;
        //Reset
        _InputTensors = new Dictionary<string, Tensor>();
        //Resize
        t_content = TextureConverter.ToTensor(_content);
        t_style = TextureConverter.ToTensor(_style, 512, 512, 3); //??©ö??? ??¡¾? ?¢Ò??
        //Add
        _InputTensors.Add("style", t_style);
        _InputTensors.Add("content", t_content);
        //Output
        t_output = _Engine.Execute(_InputTensors).PeekOutput() as TensorFloat;
        TextureConverter.RenderToTexture(t_output, _result);

        // Create a tensor operation to divide the output values by 255, to remap to the (0-1) color range
        Ops ops = WorkerFactory.CreateOps(BackendType.GPUCompute, new TensorCachingAllocator());
        TensorFloat divideAmount = new TensorFloat(255f);
        TensorFloat tensorScaled = ops.Div(t_output, divideAmount);

        // Copy the rescaled tensor to the screen as a texture
        TextureConverter.RenderToScreen(t_output);

        t_content.Dispose();
        t_style.Dispose();
        t_output.Dispose();
    }

    private void OnDisable()
    {
        //Dispose
        _Engine.Dispose();
        RenderPipelineManager.endFrameRendering -= OnEndFrameRendering;
    }
}
