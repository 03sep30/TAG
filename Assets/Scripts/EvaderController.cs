using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvaderController : MonoBehaviourPun
{
    public float maxHP = 1;
    private float currentHP;
    public float warningRange = 50f;

    private Image hpBar;
    private Transform textPos;
    private GameObject warningObj;
    private MeshRenderer warningRenderer;

    void Start()
    {
        var player = GetComponent<TestPlayerController>();

        player.moveSpeed = 7.5f;

        currentHP = maxHP;
        hpBar = transform.Find("EvaderPos/Canvas/HPFront").GetComponent<Image>();
        textPos = transform.Find("EvaderPos");
        textPos.gameObject.SetActive(true);

        player.statusText.text = $"{photonView.ViewID} : Evader";
        gameObject.GetComponent<MeshRenderer>().material.color = Color.green;

        if (textPos != null)
        {
            warningObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            warningObj.name = "warningObj";
            warningObj.transform.SetParent(textPos, false);
            warningObj.transform.localPosition = new Vector3(0f, -2f, -0.5f);
            warningObj.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            warningRenderer = warningObj.GetComponent<MeshRenderer>();
        }
    }

    void Update()
    {
        UpdateWarningObj();
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
            StartCoroutine(ChangeChaser());
        }
        else
        {
            Debug.LogError("TestPlayerController not found on this object");
        }
    }

    void UpdateWarningObj()
    {
        float nearestDistance = float.MaxValue;

        ChaserController[] chasers = FindObjectsOfType<ChaserController>();
        foreach (var chaser in chasers)
        {
            float distance = Vector3.Distance(transform.position, chaser.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
            }
        }

        if (chasers.Length > 0)
        {
            if (nearestDistance <= 10f)
            {
                warningRenderer.material.color = Color.red;
            }
            else if (nearestDistance <= 30f)
            {
                warningRenderer.material.color = Color.yellow;
            }
            else if (nearestDistance <= 50f)
            {
                warningRenderer.material.color = Color.blue;
            }
            else
            {
                warningRenderer.material.color = Color.green;
            }
        }
    }

    IEnumerator ChangeChaser()
    {
        yield return new WaitForSeconds(3f);

        gameObject.AddComponent<ChaserController>();
        textPos.gameObject.SetActive(false);
        Destroy(this);
    }
}
