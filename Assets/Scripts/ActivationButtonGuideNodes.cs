using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class ActivationButtonGuideNodes : MonoBehaviour
{
    public enum DensityLevel
    {
        level1 = 1,
        level2 = 2,
        level3 = 3,
        level4 = 4
    }
    [SerializeField] private DensityLevel _densityLevel = DensityLevel.level1;
    [SerializeField] private Vector3[] _masterNodesArray;
    private List<Vector3> _childrenNodesList = new List<Vector3>();
    [HideInInspector] public List<GameObject> guideNodesList = new List<GameObject>();

    [Space] 
    [SerializeField] private Material _unActivatedMat;
    [SerializeField] private Material _activatedMat;
    [SerializeField] private GameObject _guideNodePrefab;


        // Start is called before the first frame update
    void Start()
    {
        FillChildrenList();
        ToggleGuideNodes(false);
    }

    public void ToggleGuideNodes(bool isOn)
    {
        if (isOn)
        {
            foreach (GameObject obj in guideNodesList)
            {
                obj.GetComponent<MeshRenderer>().material = _activatedMat;
            }
        }
        else
        {
            foreach (GameObject obj in guideNodesList)
            {
                obj.GetComponent<MeshRenderer>().material = _unActivatedMat;
            }
        }
        
    }

    private void FillChildrenList()
    {
        for (int i = 0; i < _masterNodesArray.Length; i++)
        {
            if (i + 1 < _masterNodesArray.Length)
            {
                Vector3 startPos = _masterNodesArray[i];
                Vector3 endPos = _masterNodesArray[i + 1];

                Vector3 dir = (endPos - startPos).normalized;
                float dist = Vector3.Distance(startPos, endPos);

                Vector3 tempVector = new Vector3();
                //float tempDist = 0;
                int j = 1;

                var ratio = 1f / (float)_densityLevel;
                
                //print($"{(int)dist/ratio}");
                for(int t = 0; t < (int)(dist/ratio); t++)
                {
                    tempVector = startPos + dir * j * ratio;
                    _childrenNodesList.Add(tempVector);
                    guideNodesList.Add(Instantiate(_guideNodePrefab, tempVector, Quaternion.identity)); 
                    
                    //tempDist += 0.25f;
                    j++;
                }
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        
        foreach (Vector3 pos in _masterNodesArray)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pos, 0.5f);
        }

        foreach (Vector3 pos in _childrenNodesList)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pos, 0.1f);
        }

       
        
        
    }
}
