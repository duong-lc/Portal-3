using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectDropperInteraction : MonoBehaviour
{
    [field: Header("Cube Properties")]
    public GameObject MainObject {private set; get; }
    [SerializeField] private Transform _objectSpawnTransform;

    [SerializeField] private float _fadeTimer;

    private Renderer[] _rendererArray;

    private Color[] _originalColorArray;
    //[SerializeField]private Material[] _matArray;

    private void Start()
    {
        MainObject = GetComponentInChildren<ObjectInteraction>().gameObject;
        _rendererArray = MainObject.GetComponentsInChildren<Renderer>();
        int count = _rendererArray.Sum(ren => ren.materials.Length);
        _originalColorArray = new Color[count];
        int j = 0;
        foreach (Renderer ren in _rendererArray)
        {
            foreach (var t in ren.materials)
            {
                _originalColorArray[j] = t.color;
                j++;
            }
            
        }
    }   
    
    public void ResetCubeTransform()
    {
        //foreach (Material mat in _renderer.materials) { ToTransparentMode(mat); }

        StartCoroutine(CubeFade(Time.time));
        var rb = MainObject.GetComponent<Rigidbody>();
        rb.velocity *= 0.3f;
        rb.useGravity = false;
    }

    private IEnumerator CubeFade(float startTime)
    {
        //Color _black = Color.black;
        float alpha = (Time.time - startTime) / _fadeTimer;
        while (alpha <= 1)
        {
            foreach (Renderer ren in _rendererArray)
            {
                foreach (Material mat in ren.materials)
                {
                    mat.color = Color.Lerp(mat.color, Color.black, alpha);
                    alpha = (Time.time - startTime) / _fadeTimer;
                }
            }
            yield return null;
        }
        
        //foreach (Material mat in _renderer.materials) { ToOpaqueMode(mat); }
        MainObject.transform.position = _objectSpawnTransform.position;
        MainObject.GetComponent<Rigidbody>().useGravity = true;
        int j = 0;
        foreach (Renderer ren in _rendererArray)
        {
            foreach (var t in ren.materials)
            {
                t.color = _originalColorArray[j];
                j++;
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
    }
    
    // private static void ToOpaqueMode(Material material)
    // {
    //     material.SetOverrideTag("RenderType", "");
    //     material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
    //     material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.Zero);
    //     material.SetInt("_ZWrite", 1);
    //     material.DisableKeyword("_ALPHATEST_ON");
    //     material.DisableKeyword("_ALPHABLEND_ON");
    //     material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
    //     material.renderQueue = -1;
    // }
    //
    // private static void ToTransparentMode(Material material)
    // {
    //     material.SetOverrideTag("RenderType", "Transparent");
    //     material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
    //     material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
    //     material.SetInt("_ZWrite", 0);
    //     material.DisableKeyword("_ALPHATEST_ON");
    //     material.EnableKeyword("_ALPHABLEND_ON");
    //     material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
    //     material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
    // }
}




