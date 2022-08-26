using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSystem : MonoBehaviour
{
    public Transform shootingPoint;
    public GameObject blockObject;
    public GameObject previewBlockObject;
    public GameObject fractured;
    public float breakForce;

    // Start is called before the first frame update
    void Start()
    {
        previewBlockObject = Instantiate(previewBlockObject, new Vector3(0,0,0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.tag == "Block") //returns the face of the object we hit
            {
                Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x + hitInfo.normal.x / 2),
                    Mathf.RoundToInt(hitInfo.point.y + hitInfo.normal.y / 2), Mathf.RoundToInt(hitInfo.point.z + hitInfo.normal.z / 2));
                previewBlockObject.gameObject.transform.position = spawnPosition;
                //print(spawnPosition);
            }
            else
            {
                Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x), Mathf.RoundToInt(hitInfo.point.y), Mathf.RoundToInt(hitInfo.point.z));
                previewBlockObject.gameObject.transform.position = spawnPosition;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            BuildBlock(blockObject);
        }
        if (Input.GetMouseButtonDown(1))
        {
            DestroyBlock(fractured);
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
                StartCoroutine(BuildBlockScale(block, spawnPosition));

            }
            else 
            { 
                Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x), Mathf.RoundToInt(hitInfo.point.y), Mathf.RoundToInt(hitInfo.point.z));
                StartCoroutine(BuildBlockScale(block, spawnPosition));
            }
        }
        
    }

    void DestroyBlock(GameObject destroyedBlock)
    {
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.tag == "Block") //returns the face of the object we hit
            {
                StartCoroutine(DestroyBlockScale(destroyedBlock, hitInfo));
                
            }
        }
    }

    IEnumerator DestroyBlockScale(GameObject destroyedBlock, RaycastHit hitInfo)
    {
        iTween.ScaleBy(hitInfo.transform.gameObject, new Vector3(.75f, .75f, .75f), 1.5f);
        //iTween.ScaleBy(hitInfo.transform.gameObject, iTween.Hash("amount", new Vector3(.5f, .5f, .5f), "speed", 1.5f));
        yield return new WaitForSeconds(1.5f);

        Destroy(hitInfo.transform.gameObject);
        GameObject tempDestroyedBlock = Instantiate(destroyedBlock, hitInfo.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);

        foreach (Transform child in tempDestroyedBlock.transform)
            iTween.ScaleBy(child.gameObject, new Vector3(0f, 0f, 0f), 2.5f);
        yield return new WaitForSeconds(2.5f);

        Destroy(tempDestroyedBlock);
    }

    IEnumerator BuildBlockScale(GameObject block, Vector3 spawnPosition)
    {
        GameObject temp = Instantiate(block, spawnPosition, Quaternion.identity);
        iTween.ScaleFrom(temp, new Vector3(.75f, .75f, .75f), 1.5f);
        yield return new WaitForSeconds(1.5f);
    }
}
