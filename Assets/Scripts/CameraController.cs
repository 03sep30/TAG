using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviourPun
{
    public GameObject target;
    private Camera cam;

    public float distance = 3f;
    public float offsetX = 0f;
    public float offsetY = 0f;
    public float offsetZ = 0f;

    public float xRotation = 0f;
    public float yRotation = 0f;

    public float cameraSpeed = 10f;
    public float rotationSpeed = 5f;
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

        yRotation += Input.GetAxis("Mouse X") * rotationSpeed;
        xRotation -= Input.GetAxis("Mouse Y") * rotationSpeed;
        xRotation = Mathf.Clamp(xRotation, 15f, 25f);

        target.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

        Vector3 targetPosition = target.transform.position - target.transform.forward * distance + Vector3.up * offsetY;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraSpeed);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
