using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack2Script : StateMachineBehaviour
{

  
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        

        Debug.Log("Attack 2 set to false!");
        animator.SetBool("AttackTwo", false);

        if (ThirdPersonMovementScript.numOfClicks >= 3)
        {
            animator.SetBool("AttackThree", true);
        }
        else ThirdPersonMovementScript.ResetNumOfClicks();


    }
}
