using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{

    public class Panel_Options : MonoBehaviour
    {

        private Transform gridButtons = null;
        private Slot slotSelected = null;

        [Header("Buttons :")]
        [SerializeField] private GameObject btn_DropAll = null;
        [SerializeField] private GameObject btn_Split = null;
        [SerializeField] private GameObject btn_Eat = null;
        [SerializeField] private GameObject btn_Drink = null;
        [SerializeField] private GameObject btn_Use = null;

        public void Init()
        {
            gridButtons = transform.Find("GridButtons");

            Hide_Options();
        }

        #region Show Hide.
        public void Show_Options(Slot slot)
        {
            if (slot == null || slot.currentItem == null)
                return;

            slotSelected = slot;
            slotSelected.Set_SelectImage(true);

            gameObject.SetActive(true);

            // Buttons.
            btn_DropAll.SetActive(slotSelected.currentItem.quantity > 1);
            btn_Split.SetActive(slotSelected.currentItem.quantity > 1);
            btn_Eat.SetActive(slotSelected.currentItem.attribute.GetAction(ActionType.Eat) != null);
            btn_Drink.SetActive(slotSelected.currentItem.attribute.GetAction(ActionType.Drink) != null
                && slotSelected.currentItem.attribute.value > 0);
            btn_Use.SetActive(slotSelected.currentItem.attribute.GetAction(ActionType.Use) != null);

            gridButtons.position = slot.transform.position;
        }

        public void Hide_Options()
        {
            if (slotSelected != null && slotSelected.slotType != SlotType.Quick)
                slotSelected.Set_SelectImage(false);

            slotSelected = null;

            if (Inventory.instance.panel_Description != null)
                Inventory.instance.panel_Description.RefreshDescriptions();

            gameObject.SetActive(false);
        }
        #endregion

        #region Actions.
        public void EventBtn(Slot slot, string nameOption)
        {
            if (slot == null || slot.currentItem == null || slot.currentItem.quantity <= 0)
                return;

            slotSelected = slot;
            EventBtn(nameOption);
        }
        #endregion

        #region Events Buttons.
        public void EventBtn(string nameOption)
        {
            if (nameOption == string.Empty || slotSelected == null
                || slotSelected.currentItem == null)
            {
                Hide_Options();
                return;
            }


            switch (nameOption)
            {
                case "Drop":
                    Item itemDrop = Instantiate(slotSelected.currentItem);
                    itemDrop.quantity = 1;
                    Inventory.instance.player.EjectObject(itemDrop);

                    slotSelected.DeleteItem();

                    HUD.instance.SetMessage(false, itemDrop);
                    break;

                case "DropAll":

                    Item itemDropped = Instantiate(slotSelected.currentItem);

                    int qtItems = slotSelected.currentItem.quantity;
                    if (slotSelected.currentItem.stackOnGround)
                    {
                        Item itemStack = Instantiate(slotSelected.currentItem);
                        Inventory.instance.player.EjectObject(itemStack);
                        slotSelected.ChangeItem(null);
                    }
                    else
                    {
                        for (int i = 0; i < qtItems; i++)
                        {
                            Item itemDropAll = Instantiate(slotSelected.currentItem);
                            itemDropAll.quantity = 1;
                            Inventory.instance.player.EjectObject(itemDropAll);

                            slotSelected.DeleteItem();
                        }

                    }

                    HUD.instance.SetMessage(false, itemDropped);
                    break;

                case "Split":
                    // Rest.
                    int rest = Mathf.RoundToInt(slotSelected.currentItem.quantity / 2);
                    //print("Rest : " + rest);

                    // Items to empty slot.
                    Item item4 = Instantiate(slotSelected.currentItem);
                    item4.quantity = rest;
                    Inventory.instance.AddItemsEmptySlot(item4);

                    slotSelected.DeleteItem(rest);
                    break;
                case "Eat":
                    ActionItem(ActionType.Eat);
                    break;

                case "Drink":

                    if(slotSelected.currentItem.attribute.value > 0)
                    {
                        slotSelected.currentItem.attribute.DecreaseValue();

                        ActionItem(ActionType.Drink);
                    }

                    break;

                case "Break":

                    if (slotSelected.currentItem.attribute.value > 0)
                    {
                        slotSelected.currentItem.attribute.DecreaseValue();

                        ActionItem(ActionType.Break);
                    }
                    break;

                case "Use":
                    ActionItem(ActionType.Use);
                    break;


                default:
                    break;
            }

            

            Hide_Options();
        }
        
        private void ActionItem(ActionType typeAction)
        {
            if (typeAction == ActionType.None || slotSelected == null || slotSelected.currentItem == null 
                || slotSelected.currentItem.attribute.actions.Length > 0)
            {
                for (int i = 0; i < slotSelected.currentItem.attribute.actions.Length; i++)
                {
                    Action action = slotSelected.currentItem.attribute.actions[i];
                    if (action != null && action.type == typeAction)
                    {
                        if (action.fonction == "AddHealth")
                        {
                            Inventory.instance.player.AddHealth(action.value);
                        }

                        if (action.fonction == "AddHunger")
                        {
                            Inventory.instance.player.AddHunger(action.value);
                        }
                        if (action.fonction == "AddThirsty")
                        {
                            if (slotSelected.currentItem.attribute.name == "Water")
                            {
                                Inventory.instance.player.AddThirsty(
                                    slotSelected.currentItem.attribute.quality == "Safe"
                                    ? action.value
                                    : action.value / 2);
                            }
                            else
                            {
                                Inventory.instance.player.AddThirsty(action.value);
                            }
                        }

                    }
                }

                
            }

            if (slotSelected.currentItem.attribute.deleteItemAfterAction)
                slotSelected.DeleteItem();
            


            slotSelected.Refresh();
        }
        #endregion

    }
}