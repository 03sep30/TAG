using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChaserController : MonoBehaviourPun
{
    public int attackDamage = 1;
    public float attackRange = 1f;
    public PhotonView PV;
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!PV.IsMine) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            Attack();
        }
    }

    void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        foreach (var hit in hits)
        {
            PhotonView targetPV = hit.gameObject.GetComponent<PhotonView>();
            if (targetPV != null && !targetPV.IsMine)
            {
                targetPV.RPC("TakeDamage", RpcTarget.Others, attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
