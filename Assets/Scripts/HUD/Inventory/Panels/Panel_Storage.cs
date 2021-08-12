using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{

    public class Panel_Storage : MonoBehaviour
    {
        [HideInInspector]
        public Transform gridSlots = null;

        private GameObject sceneObject = null;
        private Storage storage = null;

        public void Init()
        {
            gridSlots = transform.Find("_GridSlots");

            HidePanel();
        }


        public void ShowPanel(GameObject _sceneObj, Item _sceneItem)
        {
            if(_sceneObj == null || _sceneItem == null || _sceneItem.itemType != ItemType.Storage)
            {
                HidePanel();
                return;
            }

            gameObject.SetActive(true);
            Inventory.instance.ShowHide_Inventory();

            sceneObject = _sceneObj;
            storage = _sceneItem as Storage;


            StartCoroutine(DelayCreateSlots());
        }

        private IEnumerator DelayCreateSlots()
        {
            Inventory.instance.DestroyAllObjects(gridSlots);

            yield return new WaitForSeconds(.1f);

            // if the storage already has slots.
            if (storage.slotList.Count > 0)
            {
                Inventory.instance.CreateSlots(gridSlots, storage.slotList);
            }
            else
            {
                // Create new empty slots.
                Inventory.instance.CreateSlots(gridSlots, storage.capacity);

                // Add items loot in grid of slots.
                if (storage.useRandomLoot && storage.randomListCreated && storage.lootItems.Count > 0)
                {
                    for (int i = 0; i < storage.lootItems.Count; i++)
                    {
                        Inventory.instance.AddItem_Storage(storage.lootItems[i]);
                    }
                }
            }
        }

        public void HidePanel()
        {
            if(storage != null)
            {
                storage.slotList.Clear();
                storage.slotList = Inventory.instance.GetSlots(gridSlots);
                storage.lootItems.Clear();
            }

            if(sceneObject != null && sceneObject.GetComponent<Interact>())
            {
                sceneObject.GetComponent<Interact>().SetItem(storage);
                sceneObject.GetComponent<Interact>().Action_CloseBox();
            }

            sceneObject = null;
            storage = null;

            gameObject.SetActive(false);
        }
    }
}