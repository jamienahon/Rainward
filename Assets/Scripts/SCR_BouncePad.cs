using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RainWard.Managers;

public class SCR_BouncePad : MonoBehaviour
{
    private SCR_AudioManager audManager;

    [SerializeField]
    private float bounceValue;
    [SerializeField]
    private float bounceValueIdle;

    private void Start()
    {
        audManager = GameObject.Find("AudioManager").GetComponent<SCR_AudioManager>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            BouncePlayer(other);
        }
    }
    public void BouncePlayer(Collider other)
    {

        if (other.GetComponent<SCR_CC4>().HasSlammed)
        {
            //Play ASCEND Anim:
            other.GetComponent<SCR_CC4>().anim.Play("ANIM_DoubleJump");
            other.GetComponent<SCR_CC4>().IsAscending = true;

            //Initiate BigBounce:
            other.GetComponent<SCR_CC4>().BouncePlayer = bounceValue;
            other.GetComponent<SCR_CC4>().HasSlammed = false;
            other.GetComponent<SCR_CC4>().jumps = 0;

            other.GetComponent<AudioSource>().PlayOneShot(audManager.misc[4]);
            GetComponentInParent<Animator>().Play("ANIM_BouncePadBounce");

            //GetComponentInParent<Animator>().SetBool("hasSlammed", false);
        }
        else
        {
            //Idle SmallBounce:
            other.GetComponent<SCR_CC4>().anim.Play("ANIM_Jump");
            other.GetComponent<SCR_CC4>().BouncePlayer = bounceValueIdle;
            other.GetComponent<SCR_CC4>().jumps = 1;

            other.GetComponent<AudioSource>().PlayOneShot(audManager.misc[3]);
            //GetComponent<AudioSource>().PlayOneShot(audManager.misc[3], audManager.sFXLevel);
        }
    }
}
