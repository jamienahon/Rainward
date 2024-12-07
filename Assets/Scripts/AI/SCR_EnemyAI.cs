using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Apple;
using UnityEditor;
using System.Globalization;
using TMPro;
using static UnityEngine.Rendering.DebugUI;
using RainWard.Managers;

public class SCR_EnemyAI : MonoBehaviour
{
    [Header("Objects:")]
    public NavMeshAgent agent;
    public GameObject player;
    public SCR_EnemyAIController enemyAIController;
    private SCR_AudioManager audManager;
    Animator animator;

    [Header("Distances:")]
    public float agroRange;
    public float stopDistance;
    [HideInInspector] public float defaultStopDistance;
    public float unStopDistance;

    [Header("Patrolling:")]
    public float patrolDelay;
    float nextPatrol;
    public Vector3[] patrolPointPositions;
    int currentPatrolPointIndex = 0;

    [HideInInspector] public int behaviour;

    [Header("Attacking:")]
    public float[] attackFrequencyRange = new float[2];
    float nextAttack;
    public float attackRange;
    public bool isAttacking;
    public float delayAfterAttack;
    float resumeBehaviour;
    bool isAgentPaused = false;
    [HideInInspector] public int attackType = 0;
    public bool isFacing;

    public SphereCollider[] weapons;
    public ParticleSystem[] particles;

    int defaultBehaviour;
    float defaultAgentSpeed;

    private void Awake()
    {
        defaultStopDistance = stopDistance;
        defaultAgentSpeed = agent.speed;

        GameObject.Find("Hand_3_L").GetComponent<SphereCollider>().enabled = false;
        GameObject.Find("Hand_4_R").GetComponent<SphereCollider>().enabled = false;

        animator = GetComponent<Animator>();
        enemyAIController = GetComponentInParent<SCR_EnemyAIController>();
        audManager = GameObject.Find("AudioManager").GetComponent<SCR_AudioManager>();
    }

    void Update()
    {
        if(gameObject.tag == "EnemyType2")
            CheckForDazed();

        if (!isAgentPaused && agent.enabled != false)
        {
            if (Time.time >= nextAttack)
            {
                if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
                {
                    defaultBehaviour = behaviour;
                    behaviour = 5;
                    nextAttack = Time.time + UnityEngine.Random.Range(attackFrequencyRange[0], attackFrequencyRange[1]);
                    isAgentPaused = true;
                    resumeBehaviour = Time.time + delayAfterAttack;
                }
            }

            switch (behaviour)
            {
                case 0:
                    //sets destination to a set distance in front of the player's look direction
                    Vector3 frontVector = Quaternion.AngleAxis(0, player.transform.up) * -transform.forward * defaultStopDistance;
                    MoveAgentToPosition(frontVector);
                    break;
                case 1:
                    //sets destination to a set distance to the left of the player's look direction
                    Vector3 leftVector = Quaternion.AngleAxis(-90, player.transform.up) * player.transform.forward * defaultStopDistance;
                    MoveAgentToPosition(leftVector);
                    break;
                case 2:
                    //sets destination to a set distance to the right of the player's look direction
                    Vector3 rightVector = Quaternion.AngleAxis(90, player.transform.up) * player.transform.forward * defaultStopDistance;
                    MoveAgentToPosition(rightVector);
                    break;
                case 3:
                    StandStill();
                    break;
                case 4:
                    Patrol();
                    break;
                case 5:
                    Attack();
                    break;
            }
        }
        else
        {
            PauseEnemyAfterAttack();
        }
    }

    void PauseEnemyAfterAttack()
    {
        SetAnimationBools(false, false, false, false, false, false);
        agent.speed = 0;
        agent.nextPosition = transform.position;
        if (Time.time >= resumeBehaviour)
        {
            isAgentPaused = false;
            agent.speed = defaultAgentSpeed;
        }
    }

    void MoveAgentToPosition(Vector3 vector)
    {
        if (isEnemyFacingPlayer())
        {
            agent.speed = defaultAgentSpeed;
            agent.updatePosition = true;
        }
        else if(isPlayerBeneathEnemy())
        {
            behaviour = 3;
        }
        else
        {
            agent.updatePosition = false;
            agent.nextPosition = transform.position;
        }

        SetAnimationBools(true, false, false, false, false, false);
        Vector3 destination = player.transform.position + vector;
        destination.y = transform.position.y;
        agent.SetDestination(destination);
    }

    void StandStill()
    {
        agent.updatePosition = false;
        agent.updateRotation = true;

        if (isEnemyFacingPlayer())
        {
            animator.SetBool("isMoving", false);
        }
        else if(isPlayerBeneathEnemy())
        {
            animator.SetBool("isMoving", false);
            agent.updateRotation = false;
        }
        else
        {
            animator.SetBool("isMoving", true);
        }

        agent.SetDestination(player.transform.position);
        agent.nextPosition = transform.position;
    }

    bool isEnemyFacingPlayer()
    {
        return Vector3.Dot(transform.forward, (player.transform.position - transform.position).normalized) > 0.7f;
    }

    bool isPlayerBehindEnemy()
    {
        return Vector3.Dot(-transform.forward, (player.transform.position - transform.position).normalized) > 0.7f;
    }

    bool isPlayerBeneathEnemy()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= 3;
    }

    void SetAnimationBools(bool isMoving, bool isAttacking, bool isAttacking1, bool isAttacking2, bool isPowerAttacking1, bool isPowerAttacking2)
    {
        animator.SetBool("isMoving", isMoving);

        if (gameObject.tag == "EnemyType1")
        {
            animator.SetBool("isAttacking", isAttacking);
        }
        else if (gameObject.tag == "EnemyType2")
        {
            animator.SetBool("isAttacking1", isAttacking1);
            animator.SetBool("isAttacking2", isAttacking2);
            animator.SetBool("PowerAttack1", isPowerAttacking1);
            animator.SetBool("PowerAttack2", isPowerAttacking2);
        }
    }

    void Patrol()
    {
        agent.updatePosition = true;
        agent.updateRotation = true;
        Vector3 pointPosition = patrolPointPositions[currentPatrolPointIndex];
        pointPosition.y = transform.position.y;

        //if the agent has reached the patrol point position
        if (transform.position == pointPosition)
        {
            animator.SetBool("isMoving", false);
            //if the currentPatrolPointIndex is the last in the array then reset the index to 0,
            //if not then add 1 to the index to access the next patrol point position
            if (currentPatrolPointIndex + 1 > patrolPointPositions.Length - 1)
                currentPatrolPointIndex = 0;
            else
                currentPatrolPointIndex++;

            //sets how long the agent will pause once it has reached each patrol point position
            nextPatrol = Time.time + patrolDelay;
        }
        //only moves the agent if it has paused for the set amount of time
        if (Time.time > nextPatrol)
        {
            animator.SetBool("isMoving", true);
            agent.SetDestination(pointPosition);
        }
    }

    public void Attack()
    {
        isAttacking = true;
        if (gameObject.tag == "EnemyType1")
        {
            attackType = 0;
            animator.SetBool("isAttacking", true);

            GetComponent<AudioSource>().PlayOneShot(audManager.bogwarts[0]);
        }
        else if (gameObject.tag == "EnemyType2")
        {
            if (isPlayerBehindEnemy())
            {
                attackType = 3;
            }
            else if (isPlayerBeneathEnemy())
                attackType = 4;
            else
                attackType = UnityEngine.Random.Range(0, 3);

            switch (attackType)
            {
                case 0:
                    SetAnimationBools(animator.GetBool("isMoving"), false, true, false, false, false);
                    break;
                case 1:
                    SetAnimationBools(animator.GetBool("isMoving"), false, false, true, false, false);
                    break;
                case 2:
                    SetAnimationBools(animator.GetBool("isMoving"), false, false, false, false, true);
                    break;
                case 3:
                    SetAnimationBools(animator.GetBool("isMoving"), false, false, false, true, false);
                    break;
                case 4:
                    animator.Play("ButtSmash");
                    break;
            }
        }
        behaviour = defaultBehaviour;
        isAttacking = false;
    }

    //void isEnemyAttacking()
    //{
    //    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Bogwart_Attack")
    //    || animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1")
    //    || animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2")
    //    || animator.GetCurrentAnimatorStateInfo(0).IsName("Power Attack 1")
    //    || animator.GetCurrentAnimatorStateInfo(0).IsName("Power Attack 2"))
    //    {
    //        agent.updateRotation = false;
    //        isAttacking = true;
    //    }
    //    else
    //    {
    //        agent.updateRotation = true;
    //        isAttacking = false;
    //    }
    //}

    public void DazeEnemy()
    {
        GetComponent<AudioSource>().PlayOneShot(audManager.machinery[1]);
        animator.Play("Dazed");
        agent.enabled = false;
    }

    public void ExitDazed()
    {
        animator.SetBool("isDazed", false);
        agent.enabled = true;
    }

    public void ToggleHitbox()
    {

        if(gameObject.tag == "EnemyType1")
        {
            if (!weapons[0].enabled)
            {
                weapons[0].enabled = true;
            }
            else
            {
                weapons[0].enabled = false;
            }
        }
        else if (gameObject.tag == "EnemyType2")
        {
            if (attackType == 0 || attackType == 1)
            {
                if (!weapons[0].enabled || !weapons[1].enabled)
                {
                    weapons[0].enabled = true;
                    weapons[1].enabled = true;
                }
                else
                {
                    weapons[0].enabled = false;
                    weapons[1].enabled = false;
                }
            }
            else if(attackType == 2 || attackType == 3)
            {
                if (!weapons[2].enabled)
                    weapons[2].enabled = true;
                else
                    weapons[2].enabled = false;
            }
        }
    }

    public void ToggleAttacking()
    {
        isAttacking = !isAttacking;
    }

    //void UpdateLookDirection()
    //{
    //    //updates agents direction to face the player
    //    Quaternion lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
    //    transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, agroRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }


    //JAMIE CODE: Player Collision
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<SCR_CC4>().IsDashing)
        {
            if (gameObject.tag == "EnemyType1")
            {
                Death();
            }
            else if (gameObject.tag == "EnemyType2")
            {
                other.GetComponent<SCR_CC4>().DamagePlayer(-other.GetComponent<SCR_CC4>().DashVelocity);

                GetComponent<AudioSource>().PlayOneShot(audManager.misc[9]);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(gameObject.tag == "EnemyType2")
        {
            if (other.tag == "Player" && weapons[2].enabled == true)
            {
                Debug.Log("DAMAGE");
                if (gameObject.tag == "EnemyType2")
                {
                    DamagePlayer(3);
                    weapons[2].enabled = false;
                }
            }
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    SCR_CC4 cc = other.GetComponent<SCR_CC4>();
    //    if (other.CompareTag("Player") && isAttacking)
    //    {

    //        if(animator.GetCurrentAnimatorStateInfo(0).IsName("ButtSmash") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
    //        {
    //            other.gameObject.GetComponent<SCR_CC4>().anim.Play("ANIM_Thorn_FallDamage");
    //            Debug.Log("anim played");
    //            isAttacking = false;
    //        }
    //        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Power Attack 1") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
    //        {
    //            cc.OnDeath();
    //        }
    //    }
    //}

    public void ButtSmash()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 2f);
        foreach(Collider hit in hits)
        {
            if (hit == GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>())
            {
                hit.GetComponentInChildren<Animator>().Play("ANIM_Thorn_FallDamage");
                DamagePlayer(2);
                break;
            }
        }
        isAttacking = false;
    }

    void DamagePlayer(int damage)
    {
        SCR_GameManager manager = GameObject.Find("GameManager").GetComponent<SCR_GameManager>();
        if (manager.currentHP > damage)
            manager.SetHP(manager.currentHP - damage);
        else
            GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_CC4>().OnDeath();
    }

    void CheckForDazed()
    {
        BoxCollider hitbox = GetComponentInChildren<BoxCollider>();
        if (player.GetComponent<SCR_CC4>().HasSlammed)
            hitbox.enabled = true;
        else
            hitbox.enabled = false;
    }

    //JAMIE CODE: Death Testing & Corpse Remove
    public void Death()
    {
        enemyAIController.enemies.Remove(gameObject);
        if (enemyAIController.enemiesInRange.Contains(gameObject))
            enemyAIController.enemiesInRange.Remove(gameObject);

        animator.Play("Bogwart_Death");
        this.enabled = false;
        agent.enabled = false;

        GetComponent<AudioSource>().PlayOneShot(audManager.bogwarts[1]);

        //persistantData.deletedCollectiblesNames.Add(gameObject.name);

        Invoke("CorpseRemove", 3);
    }

    public void CorpseRemove()
    {
        gameObject.SetActive(false);
    }

    public void WeaponSwing()
    {
        if(gameObject.tag == "EnemyType1")
        {
            GetComponent<AudioSource>().PlayOneShot(audManager.misc[1]);
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(audManager.misc[8]);
        }
    }

    public void MechMoveSFX()
    {
        GetComponent<AudioSource>().PlayOneShot(audManager.misc[UnityEngine.Random.Range(5, 6)]);
    }

    public void MechPowerAttack1SFX()
    {
        GetComponent<AudioSource>().PlayOneShot(audManager.misc[7]);
    }

    public void MechSlamSFX()
    {
        ParticleSystem slamFX = Instantiate(particles[0], transform.position, particles[0].transform.rotation) as ParticleSystem;
        GetComponent<AudioSource>().PlayOneShot(audManager.misc[10]);
    }

    public void DazedStarFX()
    {
        if (!particles[1].gameObject.activeInHierarchy)
        {
            particles[1].gameObject.SetActive(true);
        }
        else
        {
            particles[1].gameObject.SetActive(false);
        }
    }

    //void MaintainDistanceToPlayer()
    //{
    //    //disables automatic rotation of the agent so it can be set to follow the player's direction manually
    //    agent.updateRotation = false;
    //    UpdateLookDirection();

    //    //sets agent's destination directly behind the agent to maintain the minimum distance to the player
    //    Vector3 destination = player.transform.position + -transform.forward * (defaultStopDistance - 0.5f);
    //    agent.SetDestination(destination);
    //}
}