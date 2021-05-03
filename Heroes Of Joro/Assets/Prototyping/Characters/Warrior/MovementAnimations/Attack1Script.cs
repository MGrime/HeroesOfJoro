using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack1Script : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("AttackOne", false);
        Debug.Log("Attack 1 set to false!");
        Debug.Log(ThirdPersonMovementScript.NumOfClicks);

        if (ThirdPersonMovementScript.NumOfClicks >= 2)
        {
            animator.SetBool("AttackTwo", true);
        }
        else ThirdPersonMovementScript.ResetNumOfClicks();

    }
}
