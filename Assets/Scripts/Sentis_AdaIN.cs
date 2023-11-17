using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Sentis;
using UnityEngine.UI;
using UnityEngine.Profiling;

public class Sentis_AdaIN : MonoBehaviour
{
    public ModelAsset _AdaINModel;
    public Texture2D _content;
    public Texture2D _style;
    public RenderTexture _result;
    Dictionary<string, Tensor> _InputTensors;

    //Tensors
    TensorFloat t_style;
    TensorFloat t_content;

    //Worker
    Model _RuntimeModel;
    IWorker _Engine;

    //UI
    public GameObject _rawcontent;
    RawImage _rawcontentImage;
    public GameObject _rawstyle;
    RawImage _rawstyleImage;

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
    public _Style_List updated_content = _Style_List.antimonocromatismo;
    public _Style_List updated_style = _Style_List.antimonocromatismo;

    Texture2D[] _textures;
    _Style_List _prev_style;
    _Style_List _prev_content;


    private void Awake()
    {
        _RuntimeModel = ModelLoader.Load(_AdaINModel);
        _Engine = WorkerFactory.CreateWorker(BackendType.GPUCompute, _RuntimeModel);
        _rawcontentImage = _rawcontent.GetComponent<RawImage>();
        _rawstyleImage = _rawstyle.GetComponent<RawImage>();
        LoadResources();
        Sentis_Execute();
    }
    // Start is called before the first frame update
    void Update()
    {
        Check_Conditions();
    }

    void Sentis_Execute()
    {
        //Reset
        _InputTensors = new Dictionary<string, Tensor>();
        //Resize
        t_style = TextureConverter.ToTensor(_textures[(int)updated_style], 512, 512, 3);
        t_content = TextureConverter.ToTensor(_textures[(int)updated_content], 512, 512, 3);
        //Add
        _InputTensors.Add("style", t_style);
        _InputTensors.Add("content", t_content);
        //Output
        TensorFloat t_output = _Engine.Execute(_InputTensors).PeekOutput() as TensorFloat;
        TextureConverter.RenderToTexture(t_output, _result);
        //Dispose
        t_style.Dispose();
        t_content.Dispose();
        t_output.Dispose();
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

    void Check_Conditions()
    {
        if (updated_style != _prev_style)
        {
            _prev_style = updated_style;
            _rawstyleImage.texture = _textures[(int)updated_style];
            Sentis_Execute();
        }

        if (updated_content != _prev_content)
        {
            _prev_content = updated_content;
            _rawcontentImage.texture = _textures[(int)updated_content];
            Sentis_Execute();
        }
    }
}
