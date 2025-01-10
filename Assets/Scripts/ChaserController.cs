using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class ChaserController : MonoBehaviourPun
{
    public float attackDamage = 0.5f;
    public float attackRange = 1f;
    public float swingTime = 2.5f;
    public float stunTime = 3f;
    public bool isAttacking = false;

    private Transform textPos;
    private TextMeshPro swingTimeText;

    void Start()
    {
        textPos = GetComponent<TestPlayerController>().textPos.transform;

        if (textPos != null)
        {
            swingTimeText = new GameObject("SwingTimeText").AddComponent<TextMeshPro>();
            swingTimeText.transform.SetParent(textPos, false);
            swingTimeText.transform.localPosition = Vector3.zero;
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.A) && !isAttacking)
        {
            StartCoroutine(PerformAttack());
            Debug.Log("Attack!");
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        float remainingTime = swingTime;
        bool hitDetected = false;   //공격 성공 여부

        while (remainingTime > 0)
        {
            swingTimeText.text = remainingTime.ToString("F1");
            remainingTime -= Time.deltaTime;

            if (!hitDetected)
            {
                Collider hit = CheckForHit();
                if (hit != null)
                {
                    OnHitDetected(hit);
                    hitDetected = true;
                }
            }

            yield return null;
        }

        isAttacking = false;
    }

    Collider CheckForHit()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out PhotonView targetPV) && !targetPV.IsMine)
                return hit;
        }
        return null;
    }

    void OnHitDetected(Collider hit)
    {
        if (hit.TryGetComponent(out PhotonView targetPV) && !targetPV.IsMine)
        {
            targetPV.RPC("TakeDamage", RpcTarget.Others, attackDamage);

            StartCoroutine(StopForSeconds(stunTime));
        }
    }

    IEnumerator StopForSeconds(float duration)
    {
        var playerController = GetComponent<TestPlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        yield return new WaitForSeconds(duration);

        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
