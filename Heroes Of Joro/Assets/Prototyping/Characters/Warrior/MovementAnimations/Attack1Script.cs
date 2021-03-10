using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack1Script : StateMachineBehaviour
{
  
   
   
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("AttackOne", false);
        Debug.Log("Attack 1 set to false!");
        Debug.Log(ThirdPersonMovementScript.numOfClicks);

        if (ThirdPersonMovementScript.numOfClicks >= 2)
        {
            animator.SetBool("AttackTwo", true);
        }
        else ThirdPersonMovementScript.ResetNumOfClicks();

    }
}
