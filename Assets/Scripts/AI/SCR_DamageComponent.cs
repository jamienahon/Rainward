using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RainWard.Managers;

namespace RainWard.Entities
{
    public class SCR_DamageComponent : MonoBehaviour
    {
        [Header("Script References:")]
        [Space]
        public SCR_GameManager gameManager;
        public SCR_AudioManager audioManager;
        public SCR_CC4 cC;
        public SCR_EnemyAI enemyAI;
        public Transform damageSource;
        public enum DamageTypes
        {
            Player = 1,
            Blade = 2,
            Blunt = 3,
            Saw = 4
        }

        /// <summary>
        /// Damage dealt by GameObject
        /// </summary>
        [field: SerializeField] public float Damage { get; set; }

        [field: SerializeField] public DamageTypes DamageType { get; private set; }

        [field: SerializeField] public bool IsPiercing { get; private set; }

        //JAMIE CODE: Collision w/ Player
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                enemyAI.isAttacking = true;
                //Weapon Collision & DamageSource isAttacking Check:
                if (gameObject.tag == "Weapon" && !cC.IsDazed && enemyAI.isAttacking)
                {
                    enemyAI.isAttacking = false;

                    if (enemyAI.attackType == 0 || enemyAI.attackType == 1)
                        gameManager.SetHP(gameManager.currentHP - 1);
                    else if (enemyAI.attackType == 2)
                        gameManager.SetHP(gameManager.currentHP - 2);
                    else if (enemyAI.attackType == 3)
                        gameManager.SetHP(gameManager.currentHP - 3);

                    if(DamageType == DamageTypes.Blade)
                    {
                        GetComponentInParent<AudioSource>().PlayOneShot(audioManager.misc[2]);
                    }
                    else if (DamageType == DamageTypes.Blunt)
                    {
                        GetComponentInParent<AudioSource>().PlayOneShot(audioManager.misc[11]);
                    }

                    if (gameManager.currentHP > 0)
                    {
                        Vector3 toPlayer = other.transform.position - transform.position;
                        other.GetComponent<SCR_CC4>().DamagePlayer(toPlayer);
                        toPlayer.y = 0;
                        toPlayer.Normalize();

                        cC.OnDamaged();
                    }
                    else
                    {
                        if (!cC.IsDead)
                        {
                            gameManager.currentHP = 0;
                            cC.OnDeath();
                        }
                    }
                }
            }
        }
    }
}
