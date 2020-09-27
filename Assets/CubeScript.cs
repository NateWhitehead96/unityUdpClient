using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    [SerializeField]
    GameObject cube;

    void Spawn()
    {
        Instantiate(cube, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
