using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ProjectSurvival
{
    public enum SlotType { None, Quick , Craft , Require, CraftTime}

    public class Slot : MonoBehaviour
    {
        [HideInInspector] public Item currentItem = null;
        public ItemType itemAccepted = ItemType.All;
        public ActionType actionType = ActionType.None;

        public string slotName = string.Empty;

        public SlotType slotType = SlotType.None;
        public bool useDragItem = true;

        private Image im_Enter = null;
        private Image im_Select = null;
        private Image im_Icon = null;
        private Image fill_Craft = null;
        [HideInInspector] public Text txt_Quantity = null;
        private Text txt_ID = null;
        private Text txt_X = null;  // For cancel.

        // Barres.
        private GameObject barreHorizontal = null;
        private Image fillHorizontal = null;
        private GameObject barreVertical = null;
        private Image fillVertical = null;

        // Crafting.
        [HideInInspector] public RequireItem[] requireItemsCraft = null;

        #region Unity méthods.
        private void Awake()
        {
            im_Enter = transform.Find("Enter").GetComponent<Image>();
            im_Select = transform.Find("Select").GetComponent<Image>();
            im_Icon = transform.Find("Icon").GetComponent<Image>();
            fill_Craft = transform.Find("Fill_Craft").GetComponent<Image>();
            txt_Quantity = transform.Find("Quantity").GetComponent<Text>();
            txt_ID = transform.Find("ID").GetComponent<Text>();
            txt_X = transform.Find("X").GetComponent<Text>();
            
            im_Enter.enabled = false;
            im_Select.enabled = false;
            fill_Craft.fillAmount = 0;
            txt_X.enabled = false;

            barreHorizontal = transform.Find("Barre Horizontal").gameObject;
            fillHorizontal = barreHorizontal.transform.Find("FillValue").GetComponent<Image>();
            barreVertical = transform.Find("Barre Vertical").gameObject;
            fillVertical = barreVertical.transform.Find("FillValue").GetComponent<Image>();
        }

        private void Update()
        {
            if(slotType == SlotType.Quick)
            {
                Update_Quickslot_Actions();
            }
            if(slotType == SlotType.CraftTime)
            {
                Update_Crafting();
            }
        }
        #endregion

        #region Items.
        public void ChangeItem(Item item)
        {
            currentItem = item;
            Refresh();

            if (Inventory.instance.panel_Fire.gameObject.activeSelf)
                Inventory.instance.panel_Fire.SetSlot(this);
        }

        /// <summary>
        /// Delete one item.
        /// </summary>
        public void DeleteItem()
        {
            if (currentItem == null || currentItem.quantity <= 0)
                return;

            currentItem.quantity--;
            if (currentItem.quantity <= 0)
            {
                ChangeItem(null);
                return;
            }

            Refresh();
        }
        public void DeleteItem(int quantity)
        {
            if (currentItem == null || currentItem.quantity <= 0 || quantity <= 0)
                return;

            currentItem.quantity -= quantity;
            if (currentItem.quantity <= 0)
            {
                ChangeItem(null);
                return;
            }

            Refresh();
        }
        #endregion

        #region Refresh.
        public void Refresh()
        {
            if (currentItem != null && currentItem.quantity <= 0)
                currentItem = null;

            Refresh_Icon();
            Refresh_Quantity();
            Refresh_Barres();
        }

        private void Refresh_Icon()
        {
            if(currentItem == null)
            {
                im_Icon.sprite = null;
                im_Icon.color = new Color(1, 1, 1, 0);
                return;
            }

            im_Icon.sprite = currentItem.itemIcon;
            im_Icon.type = Image.Type.Simple;
            im_Icon.preserveAspect = true;
            im_Icon.color = Color.white;
        }

        private void Refresh_Quantity()
        {
            if(currentItem == null)
            {
                txt_Quantity.text = string.Empty;
                return;
            }
            
            if(slotType == SlotType.Craft || slotType == SlotType.CraftTime)
            {
                txt_Quantity.text = (currentItem.crafting.resultQuantity > 1)
                ? "x" + currentItem.crafting.resultQuantity.ToString() : string.Empty;

                return;
            }

            if (slotType == SlotType.Require)
            {
                string _QuantityRequired = currentItem.quantity.ToString();
                int _QuantityInventory = Inventory.instance.ReturnItemsQuantity(currentItem.itemName);

                txt_Quantity.text = "x" + _QuantityRequired + "\n (" + _QuantityInventory + ")";
                txt_Quantity.color = (_QuantityInventory >= currentItem.quantity) ? Color.green : Color.red;

                return;
            }

            txt_Quantity.text = (currentItem.quantity > 1) 
                ? currentItem.quantity.ToString() : string.Empty;
        }

        private void Refresh_Barres()
        {
            barreHorizontal.SetActive(currentItem != null && currentItem.attribute.ActiveBarreHorizontal());
            barreVertical.SetActive(currentItem != null && currentItem.attribute.ActiveBarreVertical());

            fillHorizontal.fillAmount = 0;
            fillVertical.fillAmount = 0;

            if (barreHorizontal.activeSelf)
            {
                fillHorizontal.fillAmount = currentItem.attribute.GetPercent();
                fillHorizontal.color = HUD.instance.gradientLife.Evaluate(currentItem.attribute.GetPercent());
            }
            
            if (barreVertical.activeSelf)
                fillVertical.fillAmount = currentItem.attribute.GetPercent();

        }
        #endregion

        #region UI.
        public void Set_SelectImage(bool active)
        {
            im_Select.enabled = active;
        }

        public void Set_Interactable(bool active)
        {
            GetComponent<Image>().raycastTarget = active;
        }

        public void SetID(string _id)
        {
            txt_ID.text = _id;
        }
        #endregion

        #region Mouse Events.
        public void MouseEvent_Enter()
        {
            im_Enter.enabled = true;

            if (slotType == SlotType.CraftTime)
                txt_X.enabled = true;
            
            
            if (useDragItem && Inventory.instance.dragEnable)
                Inventory.instance.endSlot = this;
        }

        public void MouseEvent_Exit()
        {
            im_Enter.enabled = false;

            if (slotType == SlotType.CraftTime)
                txt_X.enabled = false;

            if (useDragItem)
                Inventory.instance.endSlot = null;
        }

        public void MouseEvent_Select(BaseEventData data)
        {
            if (currentItem == null)
                return;

            PointerEventData pointer = (PointerEventData)data;

            // Left clic.
            if(pointer.button == PointerEventData.InputButton.Left)
            {
                if (slotType == SlotType.CraftTime)
                {
                    CancelCrafting();
                }
                else
                {
                    Inventory.instance.panel_Description.ShowDescription(this);
                }
            }

            // Right clic.
            if (pointer.button == PointerEventData.InputButton.Right)
            {
                if (slotType == SlotType.Craft || slotType == SlotType.CraftTime
                    || slotType == SlotType.Require)
                    return;

                Inventory.instance.panel_Options.Show_Options(this);
            }
        }

        public void MouseEvent_Down()
        {
            if (Inventory.instance.dragEnable)
                return;

            if (Input.GetKey(KeyCode.Mouse0) && currentItem != null)
            {

                if(useDragItem)
                    Inventory.instance.StartDrag(this);
            }
        }

        public void MouseEvent_Up()
        {
            if(useDragItem)
                Inventory.instance.StopDrag();
        }
        #endregion

        #region Actions.
        private void Update_Quickslot_Actions()
        {
            if(currentItem != null)
            {
                if (Input.GetKeyDown(GameManager.instance.inputs.key_Eject))
                {
                    Inventory.instance.panel_Options.EventBtn(Quickslot.instance.GetSlotID(), "Drop");
                    Quickslot.instance.Selection();
                }
            }
        }
        
        private void Update_Crafting()
        {
            if (Inventory.instance.panel_Crafting.CheckFirstSlotQueue(this))
            {
                //print("Update crafting.");

                currentItem.crafting.timer += Time.deltaTime;
                fill_Craft.fillAmount = Inventory.instance.GetPercent(
                    currentItem.crafting.timer, currentItem.crafting.timeToCraft);

                if(currentItem.crafting.timer > currentItem.crafting.timeToCraft)
                {
                    // Craft finish.
                    //print("Craft finish !");

                    Inventory.instance.AddItem_Backpack(currentItem);
                    Inventory.instance.panel_Description.RefreshDescriptions();

                    Destroy(gameObject);
                }
            }
            
        }

        private void CancelCrafting()
        {
            if (requireItemsCraft.Length <= 0)
                return;

            // Add all items requied in inventory.
            for (int i = 0; i < requireItemsCraft.Length; i++)
            {
                Item item = GameManager.instance.resources.GetItemByName(requireItemsCraft[i].requireName);
                if (item != null)
                {
                    item.quantity = requireItemsCraft[i].requireQuantity;
                    Inventory.instance.AddItem_Backpack(item);
                }
            }

            Inventory.instance.panel_Description.RefreshDescriptions();


            Destroy(gameObject);
        }
        #endregion
    }
}