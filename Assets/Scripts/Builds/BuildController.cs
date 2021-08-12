using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{

    public class BuildController : MonoBehaviour
    {
        [SerializeField]
        private LayerMask ignorLayerMask;

        private Slot slotRef;
        private Player player;
        private bool initialized = false;

        private RaycastHit hit;
        private BuildObject currentBuildObject;

        private Vector3 snapPosition;
        private Vector3 snapRotation;
        private float offsetRotation;
        private bool snap;

        void Update()
        {
            if (initialized && !Inventory.instance.inventoryOpen)
            {
                BuildRay();
                Snapping();
                MoveBuildObject();
                GetInputs();
            }
        }

        private void OnDisable()
        {
            DestroyBuildObject();
        }

        private void GetInputs()
        {
            if(currentBuildObject != null)
            {
                if (slotRef.currentItem.build.CanBeRotate)
                {
                    if (Input.GetKeyDown(GameManager.instance.inputs.key_BuildRotateLeft))
                        RotateBuildObject(-1);
                    if (Input.GetKeyDown(GameManager.instance.inputs.key_BuildRotateRight))
                        RotateBuildObject(1);
                }

                if (Input.GetKeyDown(GameManager.instance.inputs.key_PrimaryAction))
                {
                    PlaceBuildObject();
                }


                
            }
        }


        private void BuildRay()
        {
            Physics.Raycast(player.fpsCamera.transform.position,
                player.fpsCamera.transform.forward, out hit, 5, ignorLayerMask);

            if (hit.collider)
            {
                //print($" hit : {hit.collider.name}");
                CreateBuildObject();
            }
            else
            {
                DestroyBuildObject();
            }
        }


        private void CreateBuildObject()
        {
            if(currentBuildObject == null)
            {
                currentBuildObject = Instantiate(slotRef.currentItem.build.BuildObject).GetComponent<BuildObject>();
                currentBuildObject.Setup(slotRef.currentItem);
            }
        }

        private void DestroyBuildObject()
        {
            if (currentBuildObject != null)
                Destroy(currentBuildObject.gameObject);
        }

        private void MoveBuildObject()
        {
            if (currentBuildObject != null)
            {
                if (snap)
                {
                    currentBuildObject.transform.position = snapPosition;
                    currentBuildObject.transform.rotation = Quaternion.Euler(snapRotation);
                }
                else
                {
                    currentBuildObject.transform.position =
                        new Vector3(
                        snapPosition.x,
                        snapPosition.y + currentBuildObject.OffsetPositionY,
                        snapPosition.z);
                    currentBuildObject.transform.rotation = Quaternion.Euler(Vector3.up * offsetRotation);
                }
            }
        }

        private void PlaceBuildObject()
        {
            if (currentBuildObject == null || !currentBuildObject.IsBuildable)
                return;


            Instantiate(slotRef.currentItem.prf_Ground, 
                currentBuildObject.transform.position,
                currentBuildObject.transform.rotation);


            slotRef.DeleteItem();
            Quickslot.instance.Selection();
        }


        private void RotateBuildObject(int direction)
        {
            if (direction == -1)
            {
                offsetRotation -= slotRef.currentItem.build.AngleRotation;
            }
            if (direction == 1)
            {
                offsetRotation += slotRef.currentItem.build.AngleRotation;
            }
        }



        private void Snapping()
        {

            if(hit.collider && hit.collider.gameObject.layer == 15)
            {
                SnapPoint sp = hit.collider.GetComponent<SnapPoint>();
                if(sp && sp.SnapType == slotRef.currentItem.build.SnapType)
                {
                    snap = true;
                    if (currentBuildObject) currentBuildObject.Snapped = true;
                    snapPosition = sp.transform.position;
                    snapRotation = sp.SnapRotation + sp.transform.rotation.eulerAngles + (Vector3.up * offsetRotation);
                }
                else
                {
                    snap = false;
                    if (currentBuildObject) currentBuildObject.Snapped = false;
                    snapPosition = hit.point;
                    snapRotation = Vector3.zero;
                }

            }
            else
            {
                snap = false;
                if (currentBuildObject) currentBuildObject.Snapped = false;
                snapPosition = hit.point;
                snapRotation = Vector3.zero;
            }
        }


        public void SetSlot(Slot slot)
        {
            if (slot == null || slot.currentItem == null)
                return;


            slotRef = slot;
            if(slotRef.currentItem.prf_Ground == null)
            {
                Debug.LogWarning($"The prefab ground of {slot.currentItem.itemName} is null !");
                return;
            }
            if (slotRef.currentItem.build.BuildObject == null)
            {
                Debug.LogWarning($"The prefab buildObject of {slot.currentItem.itemName} is null !");
                return;
            }


            player = Inventory.instance.player;
            if (player == null)
                return;


            initialized = true;
        }
    }
}