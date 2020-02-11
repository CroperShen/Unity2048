using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private void Start()
    {
        double ratio = Screen.height / Screen.width;
        GetComponent<Camera>().orthographicSize = ratio > 1.9 ? 12 : 10.5f;

    }

}
