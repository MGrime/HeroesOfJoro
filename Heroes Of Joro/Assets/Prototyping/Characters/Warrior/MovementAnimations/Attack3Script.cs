using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack3Script : StateMachineBehaviour
{


    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
        animator.SetBool("AttackThree", false);
        ThirdPersonMovementScript.ResetNumOfClicks();
    }
}
