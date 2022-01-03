using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDropperInteraction : MonoBehaviour
{
    [field: Header("Cube Properties")]
    public GameObject MainObject {private set; get; }
    [SerializeField] private Transform _objectSpawnTransform;

    [SerializeField] private float _fadeTimer;

    private Renderer _renderer;

    private Color[] _originalColorArray;
    //[SerializeField]private Material[] _matArray;

    private void Start()
    {
        MainObject = GetComponentInChildren<ObjectInteraction>().gameObject;
        _renderer = MainObject.GetComponentInChildren<Renderer>();
        _originalColorArray = new Color[_renderer.materials.Length];
        for (int i = 0; i < _renderer.materials.Length; ++i)
        {
            _originalColorArray[i] = _renderer.materials[i].color;
        }
    }   
    
    public void ResetCubeTransform()
    {
        //foreach (Material mat in _renderer.materials) { ToTransparentMode(mat); }

        StartCoroutine(CubeFade(Time.time));
        MainObject.GetComponent<Rigidbody>().useGravity = false;
    }

    private IEnumerator CubeFade(float startTime)
    {
        //Color _black = Color.black;
        float alpha = (Time.time - startTime) / _fadeTimer;
        while (alpha <= 1)
        {
            foreach (Material mat in _renderer.materials)
            {
                mat.color = Color.Lerp(mat.color, Color.black, alpha);
                alpha = (Time.time - startTime) / _fadeTimer;
            }
            yield return null;
        }
        
        //foreach (Material mat in _renderer.materials) { ToOpaqueMode(mat); }
        MainObject.transform.position = _objectSpawnTransform.position;
        MainObject.GetComponent<Rigidbody>().useGravity = true;
        for (int i = 0; i < _renderer.materials.Length; ++i)
        {
            _renderer.materials[i].color = _originalColorArray[i];
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




