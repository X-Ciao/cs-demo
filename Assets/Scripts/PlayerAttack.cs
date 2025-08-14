using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{

    public ParticleSystem bulletParticleSystem;

    private ParticleSystem.EmissionModule em;

    float attackTimer = 0f;

    public NetworkVariable<bool> attacking = new NetworkVariable<bool>(
    false,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner
    );


    void Start()
    {
        em = bulletParticleSystem.emission;
    }

    private float FiringRate = 10f;

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            attacking.Value = Input.GetMouseButton(0);//Êó±ê×ó¼üÊäÈë


            attackTimer += Time.deltaTime;

            if (attacking.Value && attackTimer > 1f / FiringRate)
            {
                attackTimer = 0;

                AttackServerRpc();//¼ì²âÅö×²ºó¿Û³ýÍæ¼ÒÑªÁ¿
            }

            
        }

        em.rateOverTime = attacking.Value ? FiringRate : 0f;
    }

    [ServerRpc]
    void AttackServerRpc()
    {
        Ray ray = new Ray(bulletParticleSystem.transform.position, bulletParticleSystem.transform.forward);

        float raycastLength = 100f;

        if(Physics.Raycast(ray,out RaycastHit hit, raycastLength))
        {
            var playerHitHealthScript = hit.collider.GetComponent<PlayerHealth>();

            if (playerHitHealthScript != null)
            {
                float reduceHealthBy = 10f;

                playerHitHealthScript.ReduceHealth(reduceHealthBy);
            }
        }


    }



}
