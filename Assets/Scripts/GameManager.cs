using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    public Transform[] spawnPoints;

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("spawnPoints 배열이 비어 있습니다.");
            return;
        }

        AssignRaandomPositions();
    }

    void AssignRaandomPositions()
    {
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;

        List<int> availableSpawns = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            availableSpawns.Add(i);
        }

        foreach (Photon.Realtime.Player player in players)
        {
            if (availableSpawns.Count == 0) break;

            int randomIndex = Random.Range(0, availableSpawns.Count);
            int spawnIndex = availableSpawns[randomIndex];
            availableSpawns.RemoveAt(randomIndex);

            photonView.RPC("SetPlayerPosition", RpcTarget.AllBuffered, spawnPoints[spawnIndex].position, player.ActorNumber);
        }
    }

    [PunRPC]
    void SetPlayerPosition(Vector3 position, int actorNum)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNum)
        {
            PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
            foreach (PhotonView pv in photonViews)
            {
                if (pv.IsMine)
                {
                    pv.transform.position = position;
                    break;
                }
            }
        }
    }
}
