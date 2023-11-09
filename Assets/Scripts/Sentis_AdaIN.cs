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
    public Texture2D _style;
    public Texture2D _content;
    public RenderTexture result;
    Dictionary<string, Tensor> InputTensors;

    //Tensors
    TensorFloat t_style;
    TensorFloat t_content;

    //Worker
    Model m_RuntimeModel;
    IWorker m_Engine;

    //UI
    public GameObject _rawstyle;
    RawImage rawImageTexture;

    public enum style_list
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
    public style_list set_style = style_list.antimonocromatismo;
    Texture2D[] textures;
    style_list _prev_state;

    private void Awake()
    {
        m_RuntimeModel = ModelLoader.Load(_AdaINModel);
        m_Engine = WorkerFactory.CreateWorker(BackendType.GPUCompute, m_RuntimeModel);
        rawImageTexture = _rawstyle.GetComponent<RawImage>();
        LoadResources();
        Sentis_Execute();
    }
    // Start is called before the first frame update
    void Update()
    {
        Check_Style();
    }

    void Sentis_Execute()
    {
        //Reset
        InputTensors = new Dictionary<string, Tensor>();
        //Resize
        t_style = TextureConverter.ToTensor(textures[(int)set_style], 512, 512, 3); //이미지 크기 조절
        t_content = TextureConverter.ToTensor(_content);
        //Add
        InputTensors.Add("style", t_style);
        InputTensors.Add("content", t_content);
        //Output
        TensorFloat t_output = m_Engine.Execute(InputTensors).PeekOutput() as TensorFloat;
        TextureConverter.RenderToTexture(t_output, result);
        //Dispose
        t_style.Dispose();
        t_content.Dispose();
        t_output.Dispose();
    }

    void LoadResources()
    {
        int enumLength = Enum.GetValues(typeof(style_list)).Length;
        textures = new Texture2D[enumLength];
        for (int i = 0; i < enumLength; i++)
        {
            string _name = ((style_list)i).ToString();
            textures[i] = Resources.Load<Texture2D>(_name);
        }
    }

    void Check_Style()
    {
        if (set_style == _prev_state) { }
        _prev_state = set_style;
        rawImageTexture.texture = textures[(int)set_style];
        Sentis_Execute();
    }
}
