using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSystem : MonoBehaviour
{
    /*
    fracturing mesh: https://www.youtube.com/watch?v=NGixJd79mcE
    block placement: https://www.youtube.com/watch?v=J4y_orHzcXQ
    hold down button: https://stackoverflow.com/questions/62361746/how-to-take-button-hold-as-an-input-in-unity-c-sharp
    changing color: https://forum.unity.com/threads/how-to-change-color-via-script-on-hdrp.732869/

    */

    public Transform shootingPoint;
    public GameObject blockObject;
    public GameObject previewBlockObject;
    public GameObject fracturedBlockObject;

    public Material transparentMat;
    public Material edgeMat;

    public float fadeOutTime;
    public float holdToDestroyTime;

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
            /*if (hitInfo.transform.tag == "Block" && hitInfo.transform.tag != "UI") //returns the face of the object we hit
            {
                Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x + hitInfo.normal.x / 2),
                    Mathf.RoundToInt(hitInfo.point.y + hitInfo.normal.y / 2), Mathf.RoundToInt(hitInfo.point.z + hitInfo.normal.z / 2));
                previewBlockObject.gameObject.transform.position = spawnPosition;
                //print(spawnPosition);
            }
            else if (hitInfo.transform.tag != "UI")
            {
                Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x), Mathf.RoundToInt(hitInfo.point.y), Mathf.RoundToInt(hitInfo.point.z));
                previewBlockObject.gameObject.transform.position = spawnPosition;
            }*/
            Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x), Mathf.RoundToInt(hitInfo.point.y), Mathf.RoundToInt(hitInfo.point.z));
            if (hitInfo.transform.tag != "UI")
            {
                previewBlockObject.GetComponent<Renderer>().material = transparentMat;
                previewBlockObject.gameObject.transform.position = spawnPosition;
                if (hitInfo.transform.tag == "Block") //switch the previewBlock mat if space is occupied
                {
                    previewBlockObject.GetComponent<Renderer>().material = edgeMat;
                }
            }
        }
        if (Input.GetMouseButtonDown(0)) //left mouse button
        {
            BuildBlock(blockObject);
        }
        if (Input.GetMouseButtonDown(1) && hitInfo.transform.tag == "Block") //HOLD right mouse button
        {
            StartCoroutine(ClickHold(hitInfo));
        }
        if (Input.GetMouseButtonUp(1)) //if released right mouse button
        {
            StopCoroutine(ClickHold(hitInfo));
            iTween.ScaleTo(hitInfo.transform.gameObject, new Vector3(1f, 1f, 1f), 1.5f);
        }
        if (Input.GetMouseButtonDown(2)) //press middle mouse button
        {
            print(hitInfo);
            //ChangeCubeColor();
        }
    }

    // creates a block on a grid system
    void BuildBlock(GameObject block)
    {
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo)) //if we hit something
        {
            if (hitInfo.transform.tag == "Block" && hitInfo.transform.tag != "UI") //if we hit another block, build on it!
            {
                Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x + hitInfo.normal.x/2), 
                    Mathf.RoundToInt(hitInfo.point.y + hitInfo.normal.y/2), Mathf.RoundToInt(hitInfo.point.z + hitInfo.normal.z/2)); //pretty sure this calc snaps to a grid
                StartCoroutine(BuildBlockScale(block, spawnPosition));

            }
            else if (hitInfo.transform.tag != "UI") //if we didn't hit another block
            { 
                Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x), Mathf.RoundToInt(hitInfo.point.y), Mathf.RoundToInt(hitInfo.point.z));
                StartCoroutine(BuildBlockScale(block, spawnPosition));
            }
        }
        
    }

    // destroys the specified block
    // (should) only get called when ClickHold() successfully finishes executing
    void DestroyBlock(GameObject destroyedBlock)
    {
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.tag == "Block" && hitInfo.transform.tag != "UI") //returns the face of the object we hit
            {
                StartCoroutine(DestroyBlockScale(destroyedBlock, hitInfo));
                
            }
        }
    }

    // scales the block down and destroys it
    IEnumerator DestroyBlockScale(GameObject destroyedBlock, RaycastHit hitInfo)
    {
        //destroys the initial cube and instantiates the fractured one in its place
        Color tempColor = hitInfo.transform.GetComponent<Renderer>().material.color;
        Destroy(hitInfo.transform.gameObject);
        GameObject tempDestroyedBlock = Instantiate(destroyedBlock, hitInfo.transform.position, Quaternion.identity);
        foreach (Transform child in tempDestroyedBlock.transform) //makes sure the children are the same color as the og block
            child.GetComponent<Renderer>().material.SetColor("_BaseColor", tempColor);
        yield return new WaitForSeconds(1f);

        //once fractured, reduces the size of each one to zero over fadeOutTime seconds
        foreach (Transform child in tempDestroyedBlock.transform)
            iTween.ScaleBy(child.gameObject, new Vector3(0f, 0f, 0f), fadeOutTime);
        yield return new WaitForSeconds(fadeOutTime);

        //deletes the tiny tiny tiny fragments
        Destroy(tempDestroyedBlock);
    }

    // scales the block in when building
    IEnumerator BuildBlockScale(GameObject block, Vector3 spawnPosition)
    {
        GameObject temp = Instantiate(block, spawnPosition, Quaternion.identity);
        iTween.ScaleFrom(temp, new Vector3(.75f, .75f, .75f), 1.5f);
        yield return new WaitForSeconds(1.5f);
    }

    // waits for holdToDestroyTime seconds to destroy the specified block
    IEnumerator ClickHold(RaycastHit hitInfo)
    {
        iTween.ScaleBy(hitInfo.transform.gameObject, new Vector3(.75f, .75f, .75f), holdToDestroyTime);
        yield return new WaitForSeconds(holdToDestroyTime);

        DestroyBlock(fracturedBlockObject);
    }

    // scales the block after changing the color
    IEnumerator ColorScale(RaycastHit hitInfo)
    {
        iTween.ScaleTo(hitInfo.transform.gameObject, new Vector3(.75f, .75f, .75f), 1.5f);
        yield return new WaitForSeconds(1.5f);
        iTween.ScaleTo(hitInfo.transform.gameObject, new Vector3(1, 1, 1), 1.5f);
        yield return new WaitForSeconds(1.5f);
    }

    // changes the cube color to an arbitrary value lmao
    public void ChangeCubeColor()
    {
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo)) //if we hit something
        {
            if (hitInfo.transform.tag == "Block" && hitInfo.transform.tag != "UI") //if we hit another block, build on it!
            {
                hitInfo.transform.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.cyan);
                StartCoroutine(ColorScale(hitInfo));
                print("changed color");
            }
        }
        
    }
}
