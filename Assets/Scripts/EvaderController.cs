using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvaderController : MonoBehaviourPun
{
    public float maxHP = 1;
    private float currentHP;
    private Image hpBar;

    void Start()
    {
        currentHP = maxHP;
        hpBar = transform.Find("Canvas/HPFront").GetComponent<Image>();
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (!photonView.IsMine) return;

        currentHP -= damage;
        photonView.RPC("UpdateHPBar", RpcTarget.AllBuffered, currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    [PunRPC]
    public void UpdateHPBar(float health)
    {
        currentHP = health;
        hpBar.fillAmount = health / maxHP;
    }

    void Die()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("OnPlayerDeath", RpcTarget.AllBuffered);
        }
        
    }

    [PunRPC]
    public void OnPlayerDeath()
    {
        var testController = GetComponent<TestPlayerController>();
        if (testController != null)
        {
            testController.statusText.text = "Die";
            gameObject.GetComponent<MeshRenderer>().material.color = Color.gray;
        }
        else
        {
            Debug.LogError("TestPlayerController not found on this object");
        }
    }
}
