using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Sentis;
using UnityEngine;
using UnityEngine.Rendering;

public class TTS : MonoBehaviour
{
    public ModelAsset _AdaINModel;
    public RenderTexture _content;
    public _Style_List updated_style = _Style_List.antimonocromatismo;
    public RenderTexture _result;
    public Camera _cam;
    Dictionary<string, Tensor> _InputTensors;
    Texture2D[] _textures;
    //Tensors
    TensorFloat t_style;
    TensorFloat t_content;
    TensorFloat t_output;

    //Worker
    Model _RuntimeModel;
    IWorker _Engine;

    public enum _Style_List
    {
        antimonocromatismo,
        asheville,
        brushstrokes,
        contrast_of_forms,
        en_campo_gris,
        flower_of_life,
        goeritz,
        impronte_d_artista,
        la_muse,
        mondrian,
        mondrian_cropped,
        picasso_seated_nude_hr,
        picasso_self_portrait,
        scene_de_rue,
        sketch,
        the_resevoir_at_poitiers,
        trial,
        woman_in_peasant_dress,
        woman_in_peasant_dress_cropped,
        woman_with_hat_matisse
    }

    void Start()
    {
        _RuntimeModel = ModelLoader.Load(_AdaINModel);
        _cam = GetComponent<Camera>();
        _Engine = WorkerFactory.CreateWorker(BackendType.GPUCompute, _RuntimeModel);
        LoadResources();
        RenderPipelineManager.endFrameRendering += OnEndFrameRendering;
    }
    void LoadResources()
    {
        int enumLength = Enum.GetValues(typeof(_Style_List)).Length;
        _textures = new Texture2D[enumLength];
        for (int i = 0; i < enumLength; i++)
        {
            string _name = ((_Style_List)i).ToString();
            _textures[i] = Resources.Load<Texture2D>(_name);
        }
    }

    void OnEndFrameRendering(ScriptableRenderContext context, Camera[] cameras)
    {
        Exctue_Sents();
    }

    void Exctue_Sents()
    {
        _cam.targetTexture = _content;
        //Reset
        _InputTensors = new Dictionary<string, Tensor>();
        //Resize
        t_content = TextureConverter.ToTensor(_content, 512, 512 ,3);
        t_style = TextureConverter.ToTensor(_textures[(int)updated_style], 512, 512, 3);
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
