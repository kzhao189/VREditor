using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSystem : MonoBehaviour
{
    public Transform shootingPoint;
    public GameObject blockObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            BuildBlock(blockObject);
        }
    }

    void BuildBlock(GameObject block)
    {

        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo)) //if we hit something
        {
            if (hitInfo.transform.tag == "Block") //returns the face of the object we hit
            {
                Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x + hitInfo.normal.x/2), 
                    Mathf.RoundToInt(hitInfo.point.y + hitInfo.normal.y/2), Mathf.RoundToInt(hitInfo.point.z + hitInfo.normal.z/2));
                Instantiate(block, spawnPosition, Quaternion.identity);
            }
            else 
            { 
                Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x), Mathf.RoundToInt(hitInfo.point.y), Mathf.RoundToInt(hitInfo.point.z));
                Instantiate(block, spawnPosition, Quaternion.identity);
                //print(hitInfo.transform.name);
            }
        }
        
    }
}
