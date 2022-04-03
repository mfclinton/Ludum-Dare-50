using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// https://stackoverflow.com/questions/64544973/unity-intellisense-doesnt-worksolution1-miscellaneous-files-issue
public class CameraOrthoFix : MonoBehaviour
{
    bool MaintainWidth = true;
    Vector3 CameraPos;
    float DefaultWidth;
    float DefaultHeight;

    // Start is called before the first frame update
    void Start()         
    {

        CameraPos = Camera.main.transform.position;

        DefaultWidth = Camera.main.orthographicSize * 1920/1080 ; // the resolution the game was designed for
        DefaultHeight = Camera.main.orthographicSize;
        if (MaintainWidth){
            Camera.main.orthographicSize = DefaultWidth / Camera.main.aspect;
        }
        Camera.main.transform.position = new Vector3(CameraPos.x, -1 * (DefaultHeight - Camera.main.orthographicSize), CameraPos.z);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
