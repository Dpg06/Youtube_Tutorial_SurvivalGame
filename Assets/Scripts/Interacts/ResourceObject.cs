using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{

    public class ResourceObject : MonoBehaviour
    {

        private Interact interact = null;


        [SerializeField] private GameObject model = null;
        [SerializeField] private float timeToRespawn = 20;
        [SerializeField] private string takeItemName = string.Empty;
        [SerializeField] private string oreItemName = string.Empty;

        public void Initialize(Interact _interact)
        {
            interact = _interact;
        }

        public void HitObject(Slot slotUse)
        {
            if (!interact.canHitObject || slotUse == null || slotUse.currentItem == null || interact.refItem == null
                || !interact.refItem.InteractWith(slotUse.currentItem.itemName))
                return;

            

            // Ore item.
            if (oreItemName != string.Empty)
            {
                Item oreItem = GameManager.instance.resources.GetItemByName(oreItemName);
                if (oreItem)
                {
                    float range = Random.Range(0, 100);
                    if(range <= oreItem.rarirtyLoot)
                    {
                        Inventory.instance.AddItem_Backpack(oreItem);
                    }
                    else
                    {
                        // Take item.
                        Inventory.instance.AddItem_Backpack(
                            GameManager.instance.resources.GetItemByName(takeItemName));

                    }
                }
            }
            else
            {
                // Take item.
                Inventory.instance.AddItem_Backpack(
                    GameManager.instance.resources.GetItemByName(takeItemName));

            }

            // Break object.
            if (slotUse.currentItem.attribute.name == "Break")
            {
                Inventory.instance.panel_Options.EventBtn(slotUse, "Break");
            }

            if(interact.refItem.attribute.name == "Life")
            {
                interact.refItem.attribute.DecreaseValue();

                HUD.instance.SceneObjectInfo(interact);

                if(interact.refItem.attribute.value <= 0)
                {
                    // Destroy object.
                    StartCoroutine(TimeToDestroy());
                }
            }

        }

        private IEnumerator TimeToDestroy()
        {
            interact.canHitObject = false;

            model.SetActive(false);
            if (gameObject.GetComponent<Collider>())
                gameObject.GetComponent<Collider>().isTrigger = true;

            GameManager.instance.resources.DeleteImpact(gameObject);


            HUD.instance.SceneObjectInfo(null);

            yield return new WaitForSeconds(timeToRespawn);

            model.SetActive(true);
            if (gameObject.GetComponent<Collider>())
                gameObject.GetComponent<Collider>().isTrigger = false;


            interact.refItem.attribute.Reset();

            interact.canHitObject = true;
        }
    }
}
