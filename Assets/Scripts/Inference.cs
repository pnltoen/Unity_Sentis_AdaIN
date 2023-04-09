using System.Collections;
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
        set_style = style_list.antimonocromatismo;
        excute();
    }

    void excute()
    {
        var worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, m_RuntimeModel);
        Inputs = new Dictionary<string, Tensor>();
        Tensor t_content = new Tensor(content, 3);
        Tensor t_style = new Tensor(rawImageTexture.texture, 3);
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
        Texture2D antimonocromatismo = Resources.Load<Texture2D>("antimonocromatismo");
        Texture2D asheville = Resources.Load<Texture2D>("asheville");
        Texture2D brushstrokes = Resources.Load<Texture2D>("brushstrokes");
        Texture2D contrast_of_forms = Resources.Load<Texture2D>("contrast_of_forms");

        if (set_style == prev_condition)
        {
            Debug.Log("Not updated!");
            return;
        }
        else if (set_style == style_list.antimonocromatismo)
        {
            rawImageTexture.texture = antimonocromatismo;
            prev_condition = style_list.antimonocromatismo;
            excute();
            Debug.Log("Updated!");
        }
        else if (set_style == style_list.asheville)
        {
            rawImageTexture.texture = asheville;
            prev_condition = style_list.asheville;
            excute();
            Debug.Log("Updated!");
        }
        else if (set_style == style_list.brushstrokes)
        {
            rawImageTexture.texture = brushstrokes;
            prev_condition = style_list.brushstrokes;
            excute();
            Debug.Log("Updated!");
        }
        else if (set_style == style_list.contrast_of_forms)
        {
            rawImageTexture.texture = contrast_of_forms;
            prev_condition = style_list.contrast_of_forms;
            excute();
            Debug.Log("Updated!");
        }
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
