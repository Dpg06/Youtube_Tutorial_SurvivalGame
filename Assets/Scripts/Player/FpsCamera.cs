using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{

    public class FpsCamera : MonoBehaviour
    {
        private Player player = null;
        private Camera cam = null;
        
        [Header("Properties :")]
        [Range(0, 90)] [SerializeField] private float limitRot_Up = 85;
        [Range(0, 90)] [SerializeField] private float limitRot_Down = 85;
        private float currentRotX = 0;

        [HideInInspector] public Transform targetEject = null;
        [HideInInspector] public Transform targetZoom = null;
        [HideInInspector] public Transform targetLook = null;
        [HideInInspector] public Transform armHolder = null;

         private float fov_Origin = 60;
        [SerializeField] private float fov_Reduction = 15;
        private bool isZooming = false;

        [System.Serializable]
        internal class HeadBob
        {
            public float intensity = 1;
            public float frequency = 1;
        }
        [Header("Head bob :")]
        [SerializeField] private HeadBob headBob_Idle = new HeadBob();
        [SerializeField] private HeadBob headBob_Walk = new HeadBob();
        [SerializeField] private HeadBob headBob_Run = new HeadBob();
        [SerializeField] private HeadBob headBob_Zoom = new HeadBob();
        private float sin = 0;



        // Interaction.
        private Interact currentInteraction = null;

        public void Init(Player _player)
        {
            player = _player;
            cam = GetComponentInChildren<Camera>();
            fov_Origin = cam.fieldOfView;

            targetEject = transform.Find("Targets/TargetEject");
            targetZoom = transform.Find("Targets/TargetZoom");
            targetLook = transform.Find("Targets/TargetLook");
            armHolder = transform.Find("ArmHolder");
        }

        public void CameraRotation(float inpRotV)
        {
            currentRotX -= inpRotV;
            currentRotX = Mathf.Clamp(currentRotX, -limitRot_Up, limitRot_Down);


            transform.localRotation = Quaternion.Euler(currentRotX, 0, 0);
        }

        public void CameraFOV(bool zoom)
        {
            isZooming = zoom;

            if (zoom)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov_Origin - fov_Reduction,
                    Time.deltaTime * 8);
                GameManager.instance.options.multiMouseSensi
                    = GameManager.instance.options.mouseSpeed_Zoom;
            }
            else
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov_Origin,
                    Time.deltaTime * 8);
                GameManager.instance.options.multiMouseSensi
                    = GameManager.instance.options.mouseSpeed_Default;
            }
        }
        
        public void Update_TargetLook()
        {
            RaycastHit hit;
            if(Physics.Raycast(cam.transform.position, cam.transform.forward * 1000, out hit))
            {
                targetLook.position = hit.point;
            }
            else
            {
                targetLook.position = cam.transform.forward * 1000;
            }
        }
        
        public void Update_Raycast()
        {
            if (CanInteract())
            {
                // Build ray.
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;
                Physics.Raycast(ray, out hit, 3);

                if(hit.collider && hit.collider.GetComponent<Interact>())
                {
                    //print("Ray");

                    if (currentInteraction == null || currentInteraction != hit.collider.GetComponent<Interact>())
                    {
                        if(currentInteraction != null)
                        {
                            currentInteraction.EventInteraction(false);
                        }

                        currentInteraction = hit.collider.GetComponent<Interact>();
                        currentInteraction.EventInteraction(true);
                    }
                }
                else
                {
                    if (currentInteraction != null)
                    {
                        currentInteraction.EventInteraction(false);
                        currentInteraction = null;
                    }
                }

                if (currentInteraction != null)
                    currentInteraction.Update_Inputs();
            }
            else
            {
                if (currentInteraction != null)
                {
                    currentInteraction.EventInteraction(false);
                    currentInteraction = null;
                }
            }
        }

        private bool CanInteract()
        {
            bool blockArm = (Quickslot.instance.CurrentArm != null && Quickslot.instance.CurrentArm.BlockInteract);
            
            return (Inventory.instance.inventoryOpen == false && !blockArm);
        }
        

        public RaycastHit RaycastHit()
        {
            Ray ray = new Ray(transform.position, transform.forward);

            RaycastHit hit;

            Physics.Raycast(ray, out hit, 3);

            return hit;
        }

        public void Update_HeadBob()
        {
            float f = 0;
            float i = 0;

            // Idle.
            if(!player.status.isWalking && !player.status.isRunning)
            {
                if (isZooming)
                {
                    f = headBob_Zoom.frequency;
                    i = headBob_Zoom.intensity;
                }
                else
                {
                    f = headBob_Idle.frequency;
                    i = headBob_Idle.intensity;
                }
            }
            // Walk.
            if (player.status.isWalking)
            {
                f = headBob_Walk.frequency;
                i = headBob_Walk.intensity;
            }
            // Run.
            if (player.status.isRunning)
            {
                f = headBob_Run.frequency;
                i = headBob_Run.intensity;
            }
            

            sin = Mathf.Lerp(sin, Mathf.Sin(Time.time * f) * i, Time.deltaTime * 8);

            cam.transform.localPosition = new Vector3(0, sin, 0);
        }
    }
}