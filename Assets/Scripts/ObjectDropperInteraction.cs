using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObjectDropperInteraction : MonoBehaviour
{
    [field: Header("Cube Properties")]
    public GameObject MainObject {private set; get; }
    [SerializeField] private Transform _objectSpawnTransform;

    // [Header("Cube Spawn Button Settings")]
    // [SerializeField] private LayerMask _playerLayer;
    // [SerializeField] private Transform _buttonTransform;
    // [SerializeField] private float _castRadius = 10f;

    private void Start()
    {
        MainObject = GetComponentInChildren<ObjectInteraction>().gameObject;
        //_buttonCollider.center = _buttonTransform.localPosition;
    }
    
    public void ResetCubeTransform()
    {
        MainObject.transform.position = _objectSpawnTransform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
    }
}
