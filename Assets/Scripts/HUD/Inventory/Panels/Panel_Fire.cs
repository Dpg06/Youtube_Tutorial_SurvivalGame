using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectSurvival
{

    public class Panel_Fire : MonoBehaviour
    {
        [SerializeField] private GameObject flameIcon = null;

        private Interact interact = null;
        private FireController campfire = null;

        [Header("Fire :")]
        [SerializeField]
        private Text titleFireName;
        [SerializeField]
        private GameObject buttonStartFire;
        [SerializeField]
        private GameObject buttonStopFire;
        [SerializeField]
        private Text txtFireTime;

        [Header("Slots :")]
        [SerializeField]
        private Text titleSlotName;
        [SerializeField]
        private Slot slotCook1;
        [SerializeField]
        private Slot slotCook2;
        [SerializeField]
        private Slot slotCook3;
        [SerializeField]
        private Slot slotStarter;
        [SerializeField]
        private Slot slotFuel;

        public void Init()
        {
            Hide();
        }



        public void Show(Interact interaction)
        {
            if(interaction == null)
            {
                Hide();
                return;
            }

            interact = interaction;
            campfire = interact.GetComponent<FireController>();
            if (campfire == null)
            {
                Hide();
                return;
            }
            
            gameObject.SetActive(true);
            SetTitleText();

            RefreshUI();

            Inventory.instance.ShowHide_Inventory();
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            interact = null;
            campfire = null;


            RefreshUI();
            

        }


        public void SetSlot(Slot slot)
        {
            if (campfire == null)
                return;


            if(slot && slot.currentItem && slot.currentItem.attribute.name == "Cooking"
                && slot.currentItem.attribute.GetAction(ActionType.Cook) != null)
            {
                slot.currentItem.attribute.value = 0;
                slot.currentItem.attribute.max = slot.currentItem.attribute.GetAction(ActionType.Cook).value;
            }

            if(slot.GetInstanceID() == slotCook1.GetInstanceID())
            {
                campfire.itemCook1 = slotCook1.currentItem;
                PlaySoundCook(slotCook1.currentItem);
            }

            if (slot.GetInstanceID() == slotCook2.GetInstanceID())
            {
                campfire.itemCook2 = slotCook2.currentItem;
                PlaySoundCook(slotCook2.currentItem);
            }
            if (slot.GetInstanceID() == slotCook3.GetInstanceID())
            {
                campfire.itemCook3 = slotCook3.currentItem;
                PlaySoundCook(slotCook3.currentItem);
            }

            if (slot.GetInstanceID() == slotStarter.GetInstanceID())
            {
                campfire.itemStarter = slotStarter.currentItem;
            }


            if (slot.GetInstanceID() == slotFuel.GetInstanceID())
            {
                campfire.itemFuel = slotFuel.currentItem;
            }
            

            SetButton_StartFire();
            SetButton_StopFire();
        }



        public void RefreshUI()
        {
            flameIcon.SetActive(campfire && campfire.activeFire);

            slotCook1.ChangeItem(campfire ? campfire.itemCook1 : null);
            slotCook2.ChangeItem(campfire ? campfire.itemCook2 : null);
            slotCook3.ChangeItem(campfire ? campfire.itemCook3 : null);
            slotStarter.ChangeItem(campfire ? campfire.itemStarter : null);
            slotFuel.ChangeItem(campfire ? campfire.itemFuel : null);

            SetButton_StartFire();
            SetButton_StopFire();

            Update_UI();
        }

        public void Update_UI()
        {
            if (campfire == null)
                return;


            txtFireTime.text = "Fire time " + Mathf.RoundToInt(campfire.fireTimer) + "s";
        }

        private void SetTitleText()
        {
            string fireName = string.Empty;
            string slotName = string.Empty;

            if(interact.refItem.itemType == ItemType.Campfire)
            {
                fireName = "CAMPFIRE";
                slotName = "GRILL STAND";
            }
            else if(interact.refItem.itemType == ItemType.Furnace)
            {
                fireName = "FURNACE";
                slotName = "ORE MELT";
            }


            titleFireName.text = fireName;
            titleSlotName.text = slotName;
        }


        private void SetButton_StartFire()
        {
            buttonStartFire.SetActive(campfire && !campfire.activeFire
                && slotStarter.currentItem && slotFuel.currentItem);
        }

        private void SetButton_StopFire()
        {
            buttonStopFire.SetActive(campfire && campfire.activeFire);
        }

        public void EventButton_StartFire()
        {
            if (buttonStartFire.activeSelf && campfire)
                campfire.ActiveFire(true);
        }

        public void EventButton_StopFire()
        {
            if (buttonStopFire.activeSelf && campfire)
                campfire.ActiveFire(false);
        }


        private void PlaySoundCook(Item item)
        {
            if (item == null || item.attribute.name != "Cooking" || campfire == null || !campfire.activeFire)
                return;

            Inventory.instance.sourceAudio.PlayOneShot(Inventory.instance.clip_StartCook);
        }

    }

}