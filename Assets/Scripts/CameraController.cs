using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviourPun
{
    public GameObject target;
    private Camera cam;

    public float offsetX = 0f;
    public float offsetY = 0f;
    public float offsetZ = 0f;

    public float angleX = 0f;
    public float angleY = 0f;
    public float angleZ = 0f;

    public float cameraSpeed = 10f;
    Vector3 targetPos;

    void Start()
    {
        cam = GetComponentInChildren<Camera>();

        if (!photonView.IsMine)
        {
            if (cam != null)
            {
                cam.gameObject.SetActive(false);
            }
        }
        else
        {
            if (cam != null)
            {
                cam.gameObject.SetActive(true);
            }
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
            return;

        targetPos = new Vector3(
            target.transform.position.x + offsetX,
            target.transform.position.y + offsetY,
            target.transform.position.z + offsetZ);

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * cameraSpeed);
        transform.rotation = Quaternion.Euler(angleX, angleY, angleZ);  
    }
}
