using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDropperInteraction : MonoBehaviour
{
    [field: Header("Cube Properties")]
    public GameObject MainObject {private set; get; }
    public Transform _objectSpawnTransform;
    
    [Header("Sound fx")]
    [SerializeField] private AudioClip _buttonSound;
    private AudioSource _audioSource;
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
    }

    public void OnButtonInteract()
    {
        _audioSource.clip = _buttonSound;
        _audioSource.loop = false;
        AudioSource.PlayClipAtPoint(_buttonSound, transform.position);
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




