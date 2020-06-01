using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbMessageInactiveState : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var orbMessageUI = animator.GetComponent<OrbMessageUI>();

        animator.GetComponent<Text>().text = "";
        if (orbMessageUI.messageQueue.Count > 0)
            animator.SetTrigger("Appear");
        
        // no more messages left, check for win condition
        else
        {
            var gm = orbMessageUI.gameManager.GetComponent<GameManager>();
            var orbCountUI = gm.orbCountUI.GetComponent<OrbCountUI>();

            if (orbCountUI.hasWon)
            {
                gm.winLevel.Invoke();
            }
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        (string message, float duration) = animator.GetComponent<OrbMessageUI>().messageQueue.Dequeue();
        animator.GetComponent<Text>().text = message;

        animator.GetComponent<OrbMessageUI>().FadeAfter(duration);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
