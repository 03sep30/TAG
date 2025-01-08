using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaderController : MonoBehaviourPun
{
    public int maxHP = 2;
    private int currentHP;
    public PhotonView PV;

    void Start()
    {
        PV = photonView;
        currentHP = maxHP;
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (!PV.IsMine) return;
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        if (PV.IsMine)
        {
            PV.RPC("OnPlayerDeath", RpcTarget.AllBuffered);
        }
        
    }

    [PunRPC]
    public void OnPlayerDeath()
    {
        Debug.Log("OnPlayerDeath RPC called");
        var testController = GetComponent<TestPlayerController>();
        if (testController != null)
        {
            testController.statusText.text = "Die";
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            Debug.Log("Status text updated to 'Die'");
        }
        else
        {
            Debug.LogError("TestPlayerController not found on this object");
        }
    }
}
