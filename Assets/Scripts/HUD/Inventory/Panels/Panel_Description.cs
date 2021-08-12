using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectSurvival
{

    public class Panel_Description : MonoBehaviour
    {
        [Header("Item :")]
        [SerializeField] private Slot slotItem = null;
        [SerializeField] private Text txtItemName = null;
        [SerializeField] private Text txtItemDesc = null;

        [Header("Crafting :")]
        [SerializeField] private GameObject panelCraft = null;
        [SerializeField] private Transform gridRequireSlot = null;
        [SerializeField] private GameObject btnCrafting = null;

 
        private Slot lastSlot = null;

        public void ShowDescription(Slot slot)
        {

            if (slot == null || slot.currentItem == null)
            {
                HideDescription();
                return;
            }

            gameObject.SetActive(true);
            
            if(lastSlot != null && lastSlot == slot)
            {
                HideDescription();
                return;
            }

            lastSlot = slot;

            slotItem.slotType = slot.slotType;
            slotItem.ChangeItem(slot.currentItem);
            txtItemName.text = slot.currentItem.itemName;
            txtItemDesc.text = slot.currentItem.itemDesc;

            // Panels.
            StartCoroutine(Show_Panel_Crafting());
        }

        public void HideDescription()
        {
            if (gameObject.activeInHierarchy)
            {
                slotItem.ChangeItem(null);
                txtItemName.text = string.Empty;
                txtItemDesc.text = string.Empty;
            }

            lastSlot = null;

            btnCrafting.SetActive(false);

            gameObject.SetActive(false);
        }

        #region Panels.
        private IEnumerator Show_Panel_Crafting()
        {
            Inventory.instance.DestroyAllObjects(gridRequireSlot);

            yield return new WaitForSeconds(.1f);

            if (lastSlot != null && lastSlot.currentItem != null
                && lastSlot.slotType == SlotType.Craft && lastSlot.currentItem.crafting.isCraftable)
            {
                panelCraft.SetActive(true);
                

                // Slots require items.
                for (int i = 0; i < slotItem.currentItem.crafting.requireItems.Length; i++)
                {
                    // Create require slot.
                    Slot s = Inventory.instance.CreateSlot(gridRequireSlot);
                    s.txt_Quantity.fontSize = 16;
                    s.slotType = SlotType.Require;
                    
                    Item requireItem = GameManager.instance.resources.GetItemByName(
                        slotItem.currentItem.crafting.requireItems[i].requireName);
                    if(requireItem != null)
                    {
                        requireItem.quantity = 
                            slotItem.currentItem.crafting.requireItems[i].requireQuantity;

                        s.ChangeItem(requireItem);
                    }
                    
                }
            }
            else
            {
                panelCraft.SetActive(false);
            }


            RefreshDescriptions();
        }


        public void RefreshDescriptions()
        {

            // Refresh 'panel craft'.
            if (panelCraft.activeInHierarchy)
            {
                if(gridRequireSlot.childCount > 0)
                {
                    List<bool> hasRequireItems = new List<bool>();

                    for (int i = 0; i < gridRequireSlot.childCount; i++)
                    {
                        Slot s = gridRequireSlot.GetChild(i).GetComponent<Slot>();
                        s.Refresh();

                        hasRequireItems.Add(
                            Inventory.instance.ReturnItemsQuantity(s.currentItem.itemName)
                            >= s.currentItem.quantity);
                    }

                    btnCrafting.SetActive(hasRequireItems.TrueForAll(p => p == true));
                }
            }
        }
        #endregion


        #region Evenbt buttons.
        public void EventBtn_Crafting()
        {
            Inventory.instance.panel_Crafting.CreateNewCraft(Instantiate(lastSlot.currentItem));
        }
        #endregion
    }
}