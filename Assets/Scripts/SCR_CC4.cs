using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using UnityEngine.Windows;
using RainWard.Entities;
using RainWard.Managers;
using UnityEngine.SceneManagement;
using RainWard.UI;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine.VFX;

public class SCR_CC4 : MonoBehaviour
{
    //Jamie Code: Made INPUT Public to enable/disable from GameUI Script:
    [System.NonSerialized] public Controls input;

    //SCRIPTS:
    private SCR_Hazards hazards;
    private SCR_GameManager gameManager;
    private SCR_SceneManagerRW sceneManager;
    private SCR_PersistantData persistantData;
    private GameUI ui;
    private SCR_AudioManager audManager;

    [System.NonSerialized] public SCR_Checkpoint cp;
    [System.NonSerialized] public Animator anim;

    //COMPONENTS:
    private CharacterController cc;
    private AudioSource aud;

    [Header("Navigation Settings")]
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float dashMultiplier;
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private float jumpHeight;
    [SerializeField]
    public float jumps;
    [SerializeField]
    private float jumpMultiplier;
    [SerializeField]
    private float attackDistance;
    [SerializeField]
    private float attackRadius = 1;
    [SerializeField]
    private float aerialGravity;
    [SerializeField]
    private float groundedGravity;
    private float currentGravity;
    [SerializeField]
    private float dashCooldown;
    private float currentDashCooldown;
    [SerializeField]
    private float setDashDuration;
    private float dashDuration;
    [SerializeField]
    private float setSlamCooldown;
    private float slamCooldown;
    [SerializeField]
    private bool multiDash;

    [Header("Player VFX:")]
    [Space]
    [SerializeField]
    private ParticleSystem[] vFX;

    [Header("Slope Handling:")]
    [SerializeField]
    private float slideFriction;
    [SerializeField]
    private float inputDisableTime;
    private float inputDisabledTimer;
    private bool inputDisabled;

    [Header("Knockback Testing:")]
    [Space]
    [SerializeField]
    private float kBForce;
    [SerializeField]
    private float kBForceY;
    [SerializeField]
    private float dazedDuration;


    [Header("Drowning Testing:")]
    [Space]
    public float drownSpeed;

    public  Vector3 playerVelocity;
    private Vector3 dashVelocity;
    private Vector3 lastPosition;
    private Vector3 currentPosition;
    private Vector3 hitNormal;
    private Vector3 kbDirection;

    [Header("States")]
    [Space]
    private bool isRunning;
    private bool isJumping;
    private bool isDoubleJumping;
    private bool isFalling;
    private bool isDashing;
    public bool isGrounded;
    public float groundedTesting;
    private bool hasDashed;

    private bool hasSlammed;
    private bool isDazed;
    private bool isAscending;
    [System.NonSerialized] public bool isDrowning;
    [System.NonSerialized] public bool isDead;

    private bool isHit;
    private bool canSlam = true;
    private bool canDash;

    private bool collisionWithNonGround;

    [System.NonSerialized]
    public Vector3 respawnPos;
    private Vector3 levelPosition;

    public bool IsDashing { get { return isDashing; } }
    public bool HasSlammed { get { return hasSlammed; } set { hasSlammed = value; } }
    public bool IsDazed { get { return isDazed; } }
    public bool IsAscending { get { return isAscending; } set { isAscending = value; } }
    public bool IsDead { get { return isDead; } }
    public float BouncePlayer { get { return playerVelocity.y; } set { playerVelocity.y = value; } }
    public Vector3 DashVelocity { get { return dashVelocity; } }
    public Vector3 LevelPosition { get { return levelPosition; } }

    private Vector3 spawnpoint;
    private Vector3 playerFacing;

    private bool prevScene;
    private bool nextScene;

    private void Awake()
    {
        //GetComponents:
        anim = GetComponentInChildren<Animator>();
        aud = GetComponent<AudioSource>();
        cc = GetComponent<CharacterController>();


        input = new Controls();
        input.Player.Enable();
        input.Player.Jump.performed += OnJump;
        input.Player.Attack.performed += OnSlam;
        input.Player.Dash.performed += OnDash;

        //GetScripts:
        gameManager = GameObject.Find("GameManager").GetComponent<SCR_GameManager>();
        ui = GameObject.Find("GameUI").GetComponent<GameUI>();
        audManager = GameObject.Find("AudioManager").GetComponent<SCR_AudioManager>();
    }

    void Start()
    {
        spawnpoint = transform.position;
        playerFacing = transform.forward;

        respawnPos = transform.position;
        levelPosition = transform.position;

        inputDisabledTimer = inputDisableTime;
        currentDashCooldown = dashCooldown;
        dashDuration = setDashDuration;
        slamCooldown = setSlamCooldown;
    }
    void Update()
    {
        lastPosition = cc.transform.position;
        OnMove();

        if (!isDashing)
        {
            HandleGravity();
        }

        if (!isGrounded)
        {
            playerVelocity.x += (1f - hitNormal.y) * hitNormal.x * (1f - slideFriction);
            playerVelocity.z += (1f - hitNormal.y) * hitNormal.z * (1f - slideFriction);
        }

        if (isDashing)
        {
            playerVelocity.y = 0;
            cc.Move(dashVelocity * Time.deltaTime);
        }
        else if (isHit)
        {
            anim.Play("ANIM_Knockback");
            playerVelocity.y = kBForceY;
            cc.Move(new Vector3(kBForce * kbDirection.x, kBForceY, kBForce * kbDirection.z) * Time.deltaTime);
            playerVelocity.x = 0;
            playerVelocity.z = 0;
        }
        else
        {
            if (hasSlammed)
            {
                playerVelocity.x = 0;
                playerVelocity.z = 0;
            }

            cc.Move(playerVelocity * Time.deltaTime);
        }

        currentPosition = cc.transform.position;

        if (currentPosition.y < lastPosition.y)
        {
            isFalling = true;
            isAscending = false;
        }
        else
        {
            isFalling = false;
        }

        if (currentPosition.y == lastPosition.y && !isGrounded && input.Player.Walk.ReadValue<Vector2>() != Vector2.zero && !isDashing)
        {
            input.Disable();
            inputDisabled = true;
        }

        //if (isDashing && Physics.Raycast(cc.transform.position, Vector3.forward, 1f) )
        //{
        //    isDashing = false;
        //    cc.Move(-cc.transform.forward * Time.deltaTime);
        //}

        if (inputDisabled == true)
        {
            inputDisabledTimer -= Time.deltaTime;
            if (inputDisabledTimer <= 0)
            {
                input.Enable();
                inputDisabled = false;
                inputDisabledTimer = inputDisableTime;
            }
        }

        if (hasDashed)
        {
            dashDuration -= Time.deltaTime;
            if (dashDuration <= 0)
            {
                currentDashCooldown -= Time.deltaTime;
                isDashing = false;
                if (currentDashCooldown <= 0)
                {
                    hasDashed = false;
                    currentDashCooldown = dashCooldown;
                    dashDuration = setDashDuration;
                    if (multiDash)
                    {
                        canDash = true;
                    }
                }
            }
        }

        if (hasSlammed && isGrounded)
        {
            SlamAttack();
        }

        if (!canSlam)
        {
            slamCooldown -= Time.deltaTime;
            if (slamCooldown <= 0)
            {
                canSlam = true;
                slamCooldown = setSlamCooldown;
                hasSlammed = false;
            }
        }

        if (isHit && hasDashed)
        {
            Invoke(nameof(EndDazed), dashCooldown / 2);
        }

        if (input.Player.Walk.ReadValue<Vector2>() == Vector2.zero)
        {
            playerVelocity.x = 0;
            playerVelocity.z = 0;
        }

        HandleAnimationStates();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        //Single Jump:
        if (jumps > 0)
        {
            isJumping = true;
            isFalling = false;
            //JAMIE CODE: ANIM_JUMP override (seeing as Bool_isJumping can't override isFalling -.-) -- works FINE (for now)
            anim.Play("ANIM_Jump");

            playerVelocity.y = jumpHeight;

            //Double Jump:
            if (jumps == 1)
            {
                isFalling = false;
                isJumping = false;
                isDoubleJumping = true;
                playerVelocity.y = jumpHeight * jumpMultiplier;
            }

            jumps -= 1;
        }
    }

    void OnSlam(InputAction.CallbackContext context)
    {
        if (!isGrounded && canSlam)
        {
            hasSlammed = true;
            //cc.Move(new Vector3(0, attackDistance, 0));
            //JAMIE CODE: REPLACED ABOVE (CC.MOVE) w/ PLAYER.Y DOWN (Removes Movement Snapping)
            playerVelocity.y = attackDistance;

            aud.PlayOneShot(audManager.player[0]);
            //Overrides other ANIMS & plays SLAM Anim instantly
            anim.Play("EnterSlam_Thorn");
        }
    }

    private void SlamAttack()
    {
        if(canSlam)
        {
            canSlam = false;

            //Spawn SLAM Particles:
            ParticleSystem slamPuffClone = Instantiate(vFX[0], transform.position, vFX[0].transform.rotation) as ParticleSystem;

            Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius);
            foreach (Collider hit in hits)
            {
                if (hit.gameObject.CompareTag("SapTapper"))
                {
                    hit.GetComponentInParent<SCR_SapTapper>().TapperDestroy();
                    break;
                }
                //JAMIE CODE: Triggering Enemy Death on SLAM
                else if (hit.gameObject.CompareTag("EnemyType1"))
                {
                    hit.GetComponent<SCR_EnemyAI>().Death();
                    Debug.Log("Enemy Hit!");
                }
                else if (hit.gameObject.CompareTag("DazeHitbox") && hit.gameObject.layer == 3)
                {
                    Debug.Log("hit");
                    //playerVelocity.y = 0f;
                    playerVelocity.y = 30f;

                    anim.Play("ANIM_DoubleJump");
                    isAscending = true;

                    hit.gameObject.GetComponentInParent<SCR_EnemyAI>().DazeEnemy();

                    break;
                }
                else if (hit.gameObject.CompareTag("BouncePad"))
                {
                    break;
                }
                else if (hit.gameObject.CompareTag("EnemyType2"))
                {
                    aud.PlayOneShot(audManager.player[3]);
                    continue;
                }
                else if (!hit.gameObject.CompareTag("Sludge") || !hit.gameObject.CompareTag("Water"))
                {
                    Debug.Log("GroundHit");
                    //playerVelocity.y = 0f;
                    playerVelocity.y = 10f;
                    aud.PlayOneShot(audManager.player[3]);
                }

                Debug.Log(hit.transform);

            }

        }
    }

    void OnDash(InputAction.CallbackContext context)
    {
        Vector2 move = input.Player.Walk.ReadValue<Vector2>();
        if (move == Vector2.zero || !canDash) { return; }

        Vector3 dashMove = (move.y * (walkSpeed * dashMultiplier) * Camera.main.transform.forward) + (Camera.main.transform.right * move.x * (walkSpeed * dashMultiplier));
        float mag = dashMove.magnitude;
        dashMove.y = 0;
        dashMove = dashMove.normalized * mag;

        dashVelocity.x = dashMove.x;
        dashVelocity.z = dashMove.z;

        isDashing = true;
        hasDashed = true;
        canDash = false;

        //aud.PlayOneShot(audManager.player[0], audManager.sFXLevel);
    }

    void OnMove()
    {
        Vector2 move = input.Player.Walk.ReadValue<Vector2>();

        //If IDLE:
        if (move == Vector2.zero)
        {
            if (isGrounded)
            {
                isRunning = false;
                isFalling = false;
                isJumping = false;
                isDoubleJumping = false;
                isDashing = false;
            }

            return;
        }

        //If MOVING:
        else
        {
            if (isGrounded)
            {
                isRunning = true;
                isFalling = false;
                isJumping = false;
                isDoubleJumping = false;
            }
            else
            {
                isRunning = false;
            }

        }

        Vector3 m = (move.y * walkSpeed * Camera.main.transform.forward) + (Camera.main.transform.right * move.x * walkSpeed);
        float mag = m.magnitude;
        m.y = 0;
        m = m.normalized * mag;

        //Set Player Movement Speed:
        playerVelocity.x = m.x;
        playerVelocity.z = m.z;

        //Handle Rotation:
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(m), rotateSpeed * Time.deltaTime);

        //Set isGrounded:
        isGrounded = Vector3.Angle(Vector3.up, hitNormal) <= cc.slopeLimit;
    }

    void HandleAnimationStates()
    {
        if (isDoubleJumping && !isGrounded && !hasSlammed && !isDrowning && !isFalling)
        {
            anim.SetBool("isDoubleJumping", true);
            anim.Play("ANIM_DoubleJump");
        }
        else
        {
            anim.SetBool("isDoubleJumping", false);
        }

        if (isJumping && !isGrounded)
        {
            anim.SetBool("isJumping", true);
            anim.Play("ANIM_Jump");
        }
        else
        {
            anim.SetBool("isJumping", false);
        }

        if (isFalling && !isDrowning && !isGrounded && !isRunning && !isJumping && !isDashing && !isDazed)
        {
            anim.SetBool("isFalling", true);
        }
        else
        {
            anim.SetBool("isFalling", false);
        }

        if (isDashing && !isDoubleJumping)
        {
            anim.Play("ANIM_Dash");
        }
        else
        {
            anim.SetBool("isDashing", false);
        }

        if (isRunning && isGrounded)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        if (isGrounded && !isJumping && !isFalling && !isDoubleJumping && !isDrowning)
        {
            anim.SetBool("isGrounded", true);
        }
        else
        {
            anim.SetBool("isGrounded", false);
        }

        if (!isDrowning && !isDoubleJumping && !isJumping && !isFalling && !isDashing && !isRunning && isGrounded)
        {
            anim.SetBool("isIdle", true);
        }
        else
        {
            anim.SetBool("isIdle", false);
        }

        if (isDazed)
        {
            anim.Play("ANIM_Knockback");
        }

        if (isDrowning)
        {
            anim.Play("ANIM_Drown");
        }

        if (isDead && gameManager.currentHP > 0)
        {
            anim.Play("ANIM_Death");
        }

        if (isAscending)
        {
            anim.SetBool("isAscending", true);
        }
        else
        {
            anim.SetBool("isAscending", false);
        }

        if(hasSlammed)
        {
            anim.SetBool("hasSlammed", true);
        }
        else
        {
            anim.SetBool("hasSlammed", false);
        }

    }
    void HandleGravity()
    {
        if (SurfaceCheck())
        {
            //Gravity (Grounded)
            currentGravity = groundedGravity;
            jumps = 1;

            //Set ANIM Bools:
            isGrounded = true;
            isJumping = false;
            isDoubleJumping = false;
            isFalling = false;
            canDash = true;

            //JAMIE CODE: SLOWS GRAVITY WHEN DROWNING
            //Gravity (Drowning)
            if (isDrowning)
            {
                currentGravity = drownSpeed;
            }

        }
        else
        {
            //Gravity (Airborne)
            isGrounded = false;
            currentGravity = aerialGravity;

            //JAMIE CODE: SLOWS GRAVITY WHEN DROWNING
            //Gravity (Drowning)
            if (isDrowning)
            {
                currentGravity = drownSpeed;
            }

            //INCREASES SLAM DIRECTIONAL SPEED:
            if (hasSlammed)
            {
                currentGravity = aerialGravity - 40f;
            }
        }

        playerVelocity.y += currentGravity * Time.deltaTime;

        if (playerVelocity.y < currentGravity)
        {
            playerVelocity.y = currentGravity;
        }
    }
    private bool SurfaceCheck()
    {
        LayerMask mask = LayerMask.GetMask("Ground", "Environment", "Default", "CamCollision");
        RaycastHit surface;
        if (Physics.Raycast(cc.transform.position, -Vector3.up, out surface, groundedTesting, mask))
        {
            return true;
        }
        else
        {
            //LayerMask getGrounded = LayerMask.GetMask("NonPlatform");
            //if (Physics.Raycast(cc.transform.position, -Vector3.up, out surface, groundedTesting, getGrounded))
            //{
            //    if(isFalling)
            //    {
            //        playerVelocity.y = -20f;
            //    }
            //}

            return false;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormal = hit.normal;
        
        //if(hasSlammed)
        //{
        //    hasSlammed = false;
        //}
    }

    public void DamagePlayer(Vector3 hitdirection)
    {
        SetKnockback(hitdirection);
        isHit = true;
        isDashing = false;
        isJumping = false;
    }

    private void SetKnockback(Vector3 knockbackDirection)
    {
        kbDirection = knockbackDirection;
        kbDirection.y = 0;
        kbDirection.Normalize();
    }

    //JAMIE CODE: Damage & Knockback Testing
    public void OnDamaged()
    {
        if (!isDazed)
        {
            isDazed = true;

            //DISABLES PLAYER INPUT (temporary: most likely CC Script On/Off more efficient?)
            input.Disable();
            Invoke(nameof(EndDazed), dazedDuration);
        }
    }

    //Jamie CODE: Drown Testing
    public void OnDrown()
    {
        isDrowning = true;
        //ANIM OVerride:
        //anim.Play("ANIM_Drown");
        //Screen FadeOut:
        //ui.OnDeath();
        //DISABLES PLAYER INPUT (temporary: most likely CC Script On/Off more efficient?)
        //input.Player.Disable();

        //if (cp != null)
        //{
        //    cp.Invoke(nameof(cp.RespawnPlayer), 2);
        //}
        //else
        //{
        //    Invoke(nameof(PlayerRespawn), 2);
        //}
        //Invoke(nameof(gameManager.PlayerDrown), 3);
        
    }

    //JAMIE CODE: Ends the Knockback ('Dazed') Loss-of-Control 
    public void EndDazed()
    {
        isDazed = false;

        //RE-ENABLES PLAYER INPUT (temporary: most likely CC Script On/Off more efficient?)
        input.Enable();
        isHit = false;
    }

    public void OnDeath()
    {
        //Screen FadeOut:
        //ui.OnDeath();
        //DISABLES PLAYER INPUT (temporary: most likely CC Script On/Off more efficient?)
        //input.Player.Disable();

        //Set isDead Bool:
        isDead = true;
        //Disable Movement:
        playerVelocity = Vector3.zero;
        //ANIM Override:
        anim.Play("ANIM_Death");

        gameManager.SetHP(0);

        //FINDS CHECKPOINT:
        //if(cp != null)
        //{
        //    cp.Invoke(nameof(cp.RespawnPlayer), 3);
        //}
        ////RESPAWNS @ LEVEL START
        //else
        //{
        //    Invoke(nameof(PlayerRespawn), 3);
        //}
        //Invoke(nameof(gameManager.OnPlayerDeath), 3);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "WombatHoleEnd")
        {
            prevScene = false;
            nextScene = true;

            persistantData = other.GetComponent<SCR_PersistantData>();

            input.Player.Disable();
            anim.Play("ANIM_Burrow");

        }
        else if (other.tag == "WombatHoleStart")
        {
            prevScene = true;
            nextScene = false;

            persistantData = other.GetComponent<SCR_PersistantData>();

            input.Player.Disable();
            anim.Play("ANIM_Burrow");
        }

    }

    public void SceneTransition()
    {
        if(prevScene)
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(3))
            {
                ui.FadeToLevel(2);
            }
            else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(4))
            {
                ui.FadeToLevel(3);
            }
            else
            {
                return;
            }
        }
        else if (nextScene)
        {
            ui.DisplayTotalScore();
        }
    }

}

