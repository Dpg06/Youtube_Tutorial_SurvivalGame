using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{

    public class ArmAnimations : MonoBehaviour
    {
        private Animator animator = null;
        public bool animState = false;

        public bool canRetryAction = true;

        #region Init.
        public void Init()
        {
            animator = GetComponent<Animator>();
        }
        #endregion

        #region States.
        public void Running(bool run)
        {
            animator.SetBool("isRunning", run);
        }
        #endregion

        #region Actions.
        public void StartAnimTrigger(string animName)
        {
            canRetryAction = false;
            animator.SetTrigger(animName);
        }
        #endregion

        #region Events by Animations.
        public void Event_ActiveController()
        {
            animState = true;
        }

        public void Event_StopAnim()
        {
            canRetryAction = true;
        }

        public void Event_Shoot()
        {
            RaycastHit hit = Inventory.instance.player.fpsCamera.RaycastHit();
            
            // Impact.
            if (hit.collider && hit.collider.sharedMaterial
                && (hit.collider.GetComponent<Interact>() 
                && hit.collider.GetComponent<Interact>().canHitObject))
            {
                GameManager.instance.resources.CreateImpact(hit, false);
            }


            // Hit Object.
            if(hit.collider && hit.collider.GetComponent<Interact>() && hit.collider.GetComponent<Interact>().canHitObject)
            {
                if (hit.collider.GetComponent<ResourceObject>())
                    hit.collider.GetComponent<ResourceObject>().HitObject(
                        Quickslot.instance.GetSlotID());
            }
            
        }
        #endregion

    }
}