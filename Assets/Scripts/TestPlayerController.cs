using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class TestPlayerController : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public bool isPicked = false;

    public float moveSpeed = 5.0f;
    public float turnSpeed = 2.0f;

    public TextMeshPro statusText;
    void Update()
    {
        if (!PV.IsMine && PhotonNetwork.IsConnected)
            return;

        float moveX = Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        transform.Translate(move, Space.World);
         
        if (!isPicked)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                AddComponentRPCToAll("ChaserController");
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                AddComponentRPCToAll("EvaderController");
            }
        }
    }

    [PunRPC]
    void AddComponentRPC(string componentName)
    {
        if (!isPicked)
        {
            statusText.text = $"{PV.ViewID}{componentName} try";

            if (componentName == "ChaserController")
            {
                gameObject.AddComponent<ChaserController>();
            }
            else if (componentName == "EvaderController")
            {
                gameObject.AddComponent<EvaderController>();
            }

            isPicked = true;
        }
    }

    void AddComponentRPCToAll(string componentName)
    {
        if (PV.IsMine)
        {
            PV.RPC("AddComponentRPC", RpcTarget.AllBuffered, componentName);
        }
    }
}
