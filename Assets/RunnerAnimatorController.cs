using UnityEngine;
using System.Collections;

public class RunnerAnimatorController: MonoBehaviour
{

    private Animator anim;
    private Vector3 prevPosition;
    void Start()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
        prevPosition = transform.position;
    }

    void LateUpdate()
    {
        if (prevPosition != transform.position)
        {
            anim.SetInteger("Run", 1);
        }
        else
            anim.SetInteger("Run", 0);

        prevPosition = transform.position;
    }
}

