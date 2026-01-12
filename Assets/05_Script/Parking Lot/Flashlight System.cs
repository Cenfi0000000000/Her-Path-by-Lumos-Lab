using UnityEngine;
using System.Collections.Generic;

public class FlashlightController : MonoBehaviour
{
    [Header("Flashlight Light (Spot Light)")]
    public Light flashlightLight;

    [Header("Follow Camera")]
    public Transform cameraToFollow;

    [Header("Flashlight Settings")]
    public float maxIntensity = 5f;
    public float smoothSpeed = 10f;

    [Header("Global Fog Settings")]
    public bool enableFog = false;
    public float fogDensity = 0.1f;
    public Color fogColor = Color.black;

    [Header("Objects to Dim")]
    public List<GameObject> dimObjects = new List<GameObject>();
    [Range(0f, 1f)]
    public float dimMultiplier = 0.05f;

    private bool isOn = true;

    // 原本環境光
    private Color originalAmbientColor;
    private UnityEngine.Rendering.AmbientMode originalAmbientMode;
    private Material originalSkybox;

    // 其他光源
    private List<Light> otherLights = new List<Light>();
    private List<float> originalLightIntensity = new List<float>();

    // 材質
    private List<Material> dimMaterials = new List<Material>();
    private List<Color> originalEmissionColors = new List<Color>();
    private List<Color> originalColors = new List<Color>();

    // 貼圖
    private List<Texture2D> originalMainTex = new List<Texture2D>();
    private List<Texture2D> dimmedMainTex = new List<Texture2D>();


    void Start()
    {
        if (flashlightLight != null)
            flashlightLight.intensity = 0;

        originalAmbientColor = RenderSettings.ambientLight;
        originalAmbientMode = RenderSettings.ambientMode;
        originalSkybox = RenderSettings.skybox;

        foreach (Light l in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
        {
            if (l != flashlightLight)
            {
                otherLights.Add(l);
                originalLightIntensity.Add(l.intensity);
            }
        }

        foreach (GameObject obj in dimObjects)
        {
            if (obj == null) continue;

            foreach (Renderer r in obj.GetComponentsInChildren<Renderer>())
            {
                foreach (Material m in r.sharedMaterials)
                {
                    if (m == null) continue;

                    dimMaterials.Add(m);

                    if (m.HasProperty("_EmissionColor"))
                        originalEmissionColors.Add(m.GetColor("_EmissionColor"));
                    else
                        originalEmissionColors.Add(Color.black);

                    if (m.HasProperty("_Color"))
                        originalColors.Add(m.GetColor("_Color"));
                    else
                        originalColors.Add(Color.white);

                    if (m.HasProperty("_MainTex") && m.mainTexture != null)
                    {
                        Texture2D origTex = m.mainTexture as Texture2D;
                        originalMainTex.Add(origTex);

                        Texture2D newTex = CreateDimmedTexture(origTex, dimMultiplier);
                        dimmedMainTex.Add(newTex);
                    }
                    else
                    {
                        originalMainTex.Add(null);
                        dimmedMainTex.Add(null);
                    }
                }
            }
        }
    }

    void Update()
    {
        if (flashlightLight == null || cameraToFollow == null)
            return;

        flashlightLight.transform.position =
            cameraToFollow.position + cameraToFollow.forward * 0.1f;
        flashlightLight.transform.rotation = cameraToFollow.rotation;

        float targetIntensity = isOn ? maxIntensity : 0f;
        flashlightLight.intensity =
            Mathf.Lerp(flashlightLight.intensity, targetIntensity, Time.deltaTime * smoothSpeed);
    }

    /// <summary>
    /// ⭐ 對外呼叫：開啟手電筒
    /// </summary>
    public void OpenFlashlight()
    {
        SetFlashlight(true);
    }

    /// <summary>
    /// （可選）關閉手電筒
    /// </summary>
    public void CloseFlashlight()
    {
        SetFlashlight(false);
    }

    public void SetFlashlight(bool enable)
    {
        isOn = enable;

        if (enable)
        {
            RenderSettings.skybox = null;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = Color.black;

            if (enableFog)
            {
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Exponential;
                RenderSettings.fogDensity = fogDensity;
                RenderSettings.fogColor = fogColor;
            }

            for (int i = 0; i < otherLights.Count; i++)
                otherLights[i].intensity = 0f;

            for (int i = 0; i < dimMaterials.Count; i++)
            {
                Material m = dimMaterials[i];

                if (m.HasProperty("_EmissionColor"))
                {
                    m.SetColor("_EmissionColor", Color.black);
                    m.DisableKeyword("_EMISSION");
                }

                if (m.HasProperty("_Color"))
                    m.SetColor("_Color", originalColors[i] * dimMultiplier);

                if (m.HasProperty("_MainTex") && dimmedMainTex[i] != null)
                    m.mainTexture = dimmedMainTex[i];
            }
        }
        else
        {
            RenderSettings.skybox = originalSkybox;
            RenderSettings.ambientMode = originalAmbientMode;
            RenderSettings.ambientLight = originalAmbientColor;

            if (enableFog)
                RenderSettings.fog = false;

            for (int i = 0; i < otherLights.Count; i++)
                otherLights[i].intensity = originalLightIntensity[i];

            for (int i = 0; i < dimMaterials.Count; i++)
            {
                Material m = dimMaterials[i];

                if (m.HasProperty("_EmissionColor"))
                {
                    m.SetColor("_EmissionColor", originalEmissionColors[i]);
                    if (originalEmissionColors[i] != Color.black)
                        m.EnableKeyword("_EMISSION");
                }

                if (m.HasProperty("_Color"))
                    m.SetColor("_Color", originalColors[i]);

                if (m.HasProperty("_MainTex") && originalMainTex[i] != null)
                    m.mainTexture = originalMainTex[i];
            }
        }
    }

    private Texture2D CreateDimmedTexture(Texture2D source, float multiplier)
    {
        Texture2D newTex = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        Color[] pixels = source.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] *= multiplier;

        newTex.SetPixels(pixels);
        newTex.Apply();
        return newTex;
    }
}
