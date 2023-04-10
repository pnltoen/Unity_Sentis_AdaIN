using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;

public class Inference : MonoBehaviour
{
    public NNModel AdainModel;
    private Model m_RuntimeModel;
    public Texture2D content;
    public RenderTexture result;
    IWorker worker;
    public GameObject _rawstyle;
    RawImage rawImageTexture;
    Texture2D[] textures;
    Texture2D antimonocromatismo;
    Texture2D asheville;
    Texture2D brushstrokes;
    Texture2D contrast_of_forms;
    
        public enum style_list
    {
        antimonocromatismo,
        asheville,
        brushstrokes,
        contrast_of_forms
    }
    public style_list set_style = style_list.antimonocromatismo;
    style_list prev_condition;


    Dictionary<string, Tensor> Inputs = new Dictionary<string, Tensor>();
    // Start is called before the first frame update

    void Set()
    {
        m_RuntimeModel = ModelLoader.Load(AdainModel);
        rawImageTexture = _rawstyle.GetComponent<RawImage>();
        set_style = (style_list)0;
        LoadResources();
        excute();
    }

    void LoadResources()
    {
        int enumLength = Enum.GetValues(typeof(style_list)).Length;
    
        textures = new Texture2D[enumLength];

        for (int i=0; i < enumLength; i++)
        {
            string _name = ((style_list)i).ToString();
            textures[i] = Resources.Load<Texture2D>(_name);
        }
    }

    void excute()
    {
        var worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, m_RuntimeModel);
        Inputs = new Dictionary<string, Tensor>();
        Tensor t_content = new Tensor(content, 3);
        Tensor t_style = new Tensor(textures[(int)set_style], 3);
        Inputs.Add("content", t_content);
        Inputs.Add("style", t_style);

        worker.Execute(Inputs);

        Tensor output = worker.PeekOutput("output");
        output.ToRenderTexture(result);

        t_content.Dispose();
        t_style.Dispose();
        output.Dispose();
    }
    void StyleChanged()
    {    
        if (set_style == prev_condition)
        {
            Debug.Log("Not updated!");
            return;
        }
        
        rawImageTexture.texture = textures[(int)set_style];
        prev_condition = set_style;
        excute();
    }

    void Awake()
    {
        Set();
    }

    void Update()
    {
        StyleChanged();
    }

}
