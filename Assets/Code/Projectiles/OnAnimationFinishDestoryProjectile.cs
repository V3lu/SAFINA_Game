using Assets.Code.Interfaces;
using UnityEngine;

public class OnAnimationFinishDestoryProjectile : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.gameObject.TryGetComponent<IProjectile>(out var prefab))
        {
            ObjectPoolManager.ReturnObjectToPool(animator.gameObject, ObjectPoolManager.PoolType.Projectiles);
        }

        if (animator.gameObject.TryGetComponent<IVFX>(out var vfx))
        {
            ObjectPoolManager.ReturnObjectToPool(animator.gameObject, ObjectPoolManager.PoolType.VFXs);
        }
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
