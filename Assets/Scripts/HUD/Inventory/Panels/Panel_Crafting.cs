using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ProjectSurvival
{

    public class Panel_Crafting : MonoBehaviour
    {


        private Transform gridCrafts = null;
        private Transform gridQueue = null;

        public void Init()
        {
            gridCrafts = transform.Find("_GridCrafts/Viewport/Content");
            gridQueue = transform.Find("_GridQueue/Viewport/Content");

            // Create all crafting slots.
            Item[] itemsDataBase = Resources.LoadAll<Item>("Scriptables/Items");
            if(itemsDataBase.Length > 0)
            {
                for (int i = 0; i < itemsDataBase.Length; i++)
                {
                    if(itemsDataBase[i].crafting.isCraftable == true)
                    {
                        Slot slot = Inventory.instance.CreateSlot(gridCrafts);
                        slot.useDragItem = false;
                        slot.slotType = SlotType.Craft;
                        Item item = Instantiate(itemsDataBase[i]);
                        item.quantity = itemsDataBase[i].crafting.resultQuantity;
                        slot.ChangeItem(item);
                    }
                }
            }
            
        }        

        public void CreateNewCraft(Item item)
        {
            if(item == null)
            {
                return;
            }

            Slot s = Inventory.instance.CreateSlot(gridQueue);
            s.useDragItem = false;
            s.slotType = SlotType.CraftTime;
            s.ChangeItem(item);

            // Delete items to inventory.
            s.requireItemsCraft = item.crafting.requireItems;

            List<Slot> allSlots = Inventory.instance.GetAllSlots();
            if (allSlots.Count <= 0)
                return;

            for (int i = 0; i < s.requireItemsCraft.Length; i++)
            {
                for (int j = 0; j < s.requireItemsCraft[i].requireQuantity; j++)
                {
                    Slot slotFound = allSlots.FirstOrDefault(
                        p => p.currentItem != null
                        && p.currentItem.itemName == s.requireItemsCraft[i].requireName);

                    if (slotFound != null)
                        slotFound.DeleteItem();
                }
            }

            Inventory.instance.panel_Description.RefreshDescriptions();
        }

        public bool CheckFirstSlotQueue(Slot slot)
        {
            return (gridQueue.childCount > 0 && gridQueue.GetChild(0).GetComponent<Slot>() == slot);
        }
    }
}