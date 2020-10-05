using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net.Sockets;
using System.Net;

public class CubeScript : MonoBehaviour
{
    [SerializeField]
    GameObject cube;

    public UdpClient udp;

    //Transform transform;
    public Vector3 cubePosition = new Vector3(0, 0, 0);
    void Start()
    {
        //transform = GetComponent<Transform>();
    }
    void Update()
    {
        // only updating cube's position
        transform.position = cubePosition;
    }
}
