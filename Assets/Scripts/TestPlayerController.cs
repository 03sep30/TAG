using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class TestPlayerController : MonoBehaviourPunCallbacks
{
    public bool isPicked = false;

    public float moveSpeed = 5.0f;

    public TextMeshPro statusText;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
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
                PlayerPrefs.SetString("PlayerRole", "Chaser");
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                AddComponentRPCToAll("EvaderController");
                PlayerPrefs.SetString("PlayerRole", "Evader");
            }
        }
        if (PhotonNetwork.IsMasterClient)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                PhotonNetwork.LoadLevel("TestMapPlan");
            }
        }
    }

    [PunRPC]
    void AddComponentRPC(string componentName)
    {
        if (!isPicked)
        {
            statusText.text = $"{photonView.ViewID}{componentName} try";

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
        if (photonView.IsMine)
        {
            photonView.RPC("AddComponentRPC", RpcTarget.AllBuffered, componentName);
        }
    }
}
