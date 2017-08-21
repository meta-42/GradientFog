using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GradientFog : MonoBehaviour
{
    public int gradientTextureWidth = 128;

    [Header("DepthFog")]

    public float depthFogDensity = 1.0f;
    public float depthFogStartDistance = 0.0f;
    public float depthFogEndDistance = 100.0f;
    public Gradient depthFogGradient = new Gradient();
    Texture2D depthFogGradientTexture;

    [Header("Vertical Fog")]
    public float verticalFogDensity = 1.0f;
    public float verticalFogBaseHight = 0.0f;
    public Gradient verticalFogGradient = new Gradient();
    Texture2D verticalFogGradientTexture;


    void Start ()
    {
        UpdateConstants();
    }

    void OnEnable()
    {
        Shader.EnableKeyword("GRADIENT_FOG");
    }

    void OnDisable()
    {
        Shader.DisableKeyword("GRADIENT_FOG");
    }

    private void UpdateConstants()
    {
        GenerateTexture();

        float scope = depthFogEndDistance - depthFogStartDistance;
        float delta = 1.0f / scope;
        //calculate start distance add
        float startAdd = -depthFogStartDistance / scope;

        Shader.SetGlobalFloat("depthFogGradientDelta", delta);
        Shader.SetGlobalFloat("depthFogGradientStartAdd", startAdd);
        Shader.SetGlobalFloat("depthFogDensity", depthFogDensity);
        Shader.SetGlobalFloat("verticalFogDensity", verticalFogDensity);
        Shader.SetGlobalFloat("verticalFogBaseHight", verticalFogBaseHight);
    }

    public void GenerateTexture()
    {
        depthFogGradientTexture = new Texture2D(gradientTextureWidth, 1, TextureFormat.ARGB32, false);
        depthFogGradientTexture.wrapMode = TextureWrapMode.Clamp;

        float delta = 1.0f / (gradientTextureWidth - 1);
        float value = 0.0f;
        for (int i = 0; i < gradientTextureWidth; i++)
        {
            depthFogGradientTexture.SetPixel(i, 0, depthFogGradient.Evaluate(value));
            value += delta;
        }
        depthFogGradientTexture.Apply();

        Shader.SetGlobalTexture("depthFogGradientTexture", depthFogGradientTexture);

        verticalFogGradientTexture = new Texture2D(gradientTextureWidth, 1, TextureFormat.RGB24, false);
        verticalFogGradientTexture.wrapMode = TextureWrapMode.Clamp;

        delta = 1.0f / (gradientTextureWidth - 1);
        value = 0.0f;
        for (int i = 0; i < gradientTextureWidth; i++)
        {
            verticalFogGradientTexture.SetPixel(i, 0, verticalFogGradient.Evaluate(value));
            value += delta;
        }
        verticalFogGradientTexture.Apply();

        Shader.SetGlobalTexture("verticalFogGradientTexture", verticalFogGradientTexture);

    }


#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying)
        {
            UpdateConstants();
        }
    }
#endif
}

