using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class TestPlayerController : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public bool isPicked = false;

    public TextMeshPro statusText;
    void Update()
    {
        if (!PV.IsMine && PhotonNetwork.IsConnected)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.transform.position = new Vector3(-1, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.transform.position = new Vector3(1, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.transform.position = new Vector3(0, 1, 0);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.transform.position = new Vector3(0, -1, 0);
        }
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
                statusText.text = $"{PV.ViewID} : Chaser";
            }
            else if (componentName == "EvaderController")
            {
                gameObject.AddComponent<EvaderController>();
                statusText.text = $"{PV.ViewID} : Evader";
            }

            gameObject.GetComponent<MeshRenderer>().material.color = (componentName == "ChaserController") ? Color.black : Color.green;
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
