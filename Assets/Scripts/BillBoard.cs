using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    Camera mainCamera;
    Transform trans;
    void Start()
    {
        trans = this.transform;
        mainCamera = Camera.main;
    }
    void LateUpdate()
    {
        Vector3 newRotation = mainCamera.transform.eulerAngles;
        newRotation.x = 0;
        newRotation.z = 0;
        trans.eulerAngles = newRotation;
    }

}
