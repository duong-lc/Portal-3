using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBehavior : MonoBehaviour
{
    RaycastHit _hit;
    public GameObject outline;
    public GameObject edgeChecker;
    public GameObject viewport;
    [SerializeField] private GameObject _cam;
    private string portalTag = "Portal";

    private void Start() {
        outline.SetActive(false);
        viewport.SetActive(false);
    }
    void Update()
    {
        if(Input.GetKeyDown("l"))
        {
            print($"{gameObject.name} {CheckPerimeter()} {CheckNormalOverlap()}" );
        }
    }

    public void AttemptPlacingPortal()
    {
        if(CheckPerimeter() && CheckNormalOverlap())
        {
            //outline.GetComponentInChildren<ParticleSystem>().Stop();
            //assigning portal transform if edge checker valid
            transform.position = edgeChecker.transform.position;  
            transform.rotation = edgeChecker.transform.rotation;
            //reset edge checker position to in center of portal parent obj
            edgeChecker.transform.position = transform.position;
            edgeChecker.transform.rotation = transform.rotation;
            outline.SetActive(true);
            outline.GetComponentInChildren<ParticleSystem>().Simulate(0.1f, false, true, false);
            outline.GetComponentInChildren<ParticleSystem>().Play();
            viewport.SetActive(true);
        }else{
            
           _cam.GetComponent<CameraShake>().Shake(0.2f,0.1f);

        }
    }
    public bool CheckPerimeter()
    {
        bool canPlace = true;
        //var vectorArray =  edgeChecker.GetComponentsInChildren<Transform>();
        var posPerimList = new List<Vector3>()
        {
            new Vector3(-2, 0, 0),
            new Vector3(2, 0, 0),
            new Vector3(0, 3, 0),
            new Vector3(0, -3, 0),
            new Vector3(0, 0 , 0)
        };

        var dirPerimList = new List<Vector3>()
        {
            Vector3.right,
            -Vector3.right,
            Vector3.up,
            -Vector3.up
        };        
 

        //Checking out of bounds on current plane that's spawning on
        for (int i = 0; i < posPerimList.Count; ++i)
        {
            //turning world rot and pos to edgechecker local relative pos and rot
            var edgePos = edgeChecker.transform.TransformPoint(posPerimList[i]);
            
            //Check if the edge touch a portable surface
            var hitCol = Physics.OverlapSphere(edgePos, 0.1f, PlayerController.Instance.GetComponent<PortalPlacement>().portalableSurfaceLayer);

            if(hitCol.Length == 0 && i != posPerimList.Count-1)//has no surface on that point
            {
                RaycastHit hit;
                var edgeDir = edgeChecker.transform.TransformDirection(dirPerimList[i]);
                if(Physics.Raycast(edgePos, edgeDir, out hit, 2f, PlayerController.Instance.GetComponent<PortalPlacement>().portalableSurfaceLayer))
                {
                    print($"smt");
                    var offset = hit.point - edgePos;
                    edgeChecker.transform.Translate(offset, Space.World);
                    canPlace = true;
                }else
                    canPlace = false;
            }   
            
            var portalCol = Physics.OverlapSphere(edgePos, 0.2f);

            for(int j = 0; j < portalCol.Length; ++j)
            {
                if(portalCol[j].tag == portalTag)
                {
                    //print($"found different");
                    //portalCol[j].gameObject.transform.position = new Vector3(0,0,0);
                    portalCol[j].gameObject.GetComponent<PortalBehavior>().viewport.SetActive(false);
                    portalCol[j].gameObject.GetComponent<PortalBehavior>().outline.SetActive(false);
                }
            }
        }
       
        

        if(!canPlace)
            print($"can't place here");
        return canPlace;
    }
    public bool CheckNormalOverlap()    
    {
        bool canPlace = true;

        var normalDrawPoints = new List<Vector3>()
        {
            new Vector3 (-2, 3, -0.2f),//top left
            new Vector3 (2, 3, -0.2f),//top right
            new Vector3 (2, -3, -0.2f),//bottom right
            new Vector3 (-2,-3,-0.2f)//bottom left
            
        };

        var normalDrawDists = new List<float>()
        {
            4f, 6f, 4f, 6f
        };

        var normalDirPoint = new List<Vector3>()
        {
            Vector3.right,
            -Vector3.up,
            -Vector3.right,
            Vector3.up
        };

        for (int i = 0; i < normalDirPoint.Count; ++i)
        {
            Vector3 startPoint, dir;
            startPoint = edgeChecker.transform.TransformPoint(normalDrawPoints[i]);
            dir = edgeChecker.transform.TransformDirection(normalDirPoint[i]);
            
            RaycastHit hit;
            if(Physics.Raycast(startPoint, dir , out hit, normalDrawDists[i] , PlayerController.Instance.GetComponent<PortalPlacement>().portalableSurfaceLayer)){
                canPlace = false;
                print($"normal overlapped ! ");  
            }
        }
        return canPlace;
    }


    private void OnDrawGizmosSelected()
    {

    }
}
