using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ProjectSurvival
{

    public class HUD : MonoBehaviour
    {
        public static HUD instance = null;


        [Header("Colors :")]
        public Gradient gradientLife = new Gradient();

        [Header("Scene Object Infos :")]
        [SerializeField] private GameObject sceneObjectInfo = null;
        [SerializeField] private Text sceneObjText = null;
        [SerializeField] private GameObject sceneObjectLife = null;
        [SerializeField] private Image sceneObjectLifeImage = null;

        [Header("Scene Object Action :")]
        [SerializeField] private GameObject actionPickup = null;
        [SerializeField] private GameObject actionEquip = null;
        [SerializeField] private GameObject actionInteract = null;
        [SerializeField] private GameObject actionMove = null;

        [Header("Screen Effects :")]
        [SerializeField] private Animation anim_BloodDamage = null;
        [SerializeField] private Animation anim_Healing = null;

        [Header("Visual Message :")]
        [SerializeField] private Transform gridMessage = null;
        [SerializeField] private GameObject prfMessage = null;


        private void Awake()
        {
            if (instance == null)
                instance = this;
        }

        private void Start()
        {
            SceneObjectInfo(null);

            SetupTextInputsAction(actionPickup,GameManager.instance.inputs.key_Interact1 + " - Pickup");
            SetupTextInputsAction(actionEquip, GameManager.instance.inputs.key_Interact2 + " - Equip");
            SetupTextInputsAction(actionInteract, GameManager.instance.inputs.key_Interact1 + " - Interact");
            SetupTextInputsAction(actionMove, GameManager.instance.inputs.key_MoveObject + " - Move");
        }

        public void SceneObjectInfo(Interact interact)
        {
            SceneObjectInputs(interact);

            if(interact == null || interact.refItem == null)
            {
                if (sceneObjectInfo.activeSelf)
                    sceneObjText.text = string.Empty;
                sceneObjectInfo.SetActive(false);

                if (sceneObjectLife.activeSelf)
                    sceneObjectLifeImage.fillAmount = 0;
                sceneObjectLife.SetActive(false);
            }
            else
            {
                if (interact.typeAction == Interact.TypeAction.Resource)
                {
                    Slot quickSlot = Quickslot.instance.GetSlotID();
                    if (quickSlot == null || quickSlot.currentItem == null ||
                        !interact.refItem.InteractWith(quickSlot.currentItem.itemName)
                        || !interact.canHitObject)
                    {
                        if (sceneObjectLife.activeSelf)
                            sceneObjectLifeImage.fillAmount = 0;
                        sceneObjectLife.SetActive(false);
                    }
                    else
                    {
                        sceneObjectLife.SetActive(true);
                        sceneObjectLifeImage.fillAmount = interact.refItem.attribute.GetPercent();
                    }
                }
                else
                {
                    if (sceneObjectLife.activeSelf)
                        sceneObjectLifeImage.fillAmount = 0;
                    sceneObjectLife.SetActive(false);


                    sceneObjectInfo.SetActive(true);


                    string quality = string.Empty;
                    if (interact.refItem.attribute.name == "Water")
                    {
                        quality = " (" + interact.refItem.attribute.GetPercent() * 100
                            + "% - " + interact.refItem.attribute.quality + ")";
                    }

                    sceneObjText.text = (interact.refItem.quantity > 1)
                        ? "x" + interact.refItem.quantity + " " + interact.refItem.itemName
                        : interact.refItem.itemName + quality;
                }
            }
        }

        private void SceneObjectInputs(Interact interact)
        {
            actionPickup.SetActive(interact != null && interact.typeAction == Interact.TypeAction.Pick);
            actionEquip.SetActive(interact != null && interact.typeAction == Interact.TypeAction.Pick);
            actionInteract.SetActive(interact != null && interact.typeAction == Interact.TypeAction.Interact);
            actionMove.SetActive(interact != null && interact.movable);
        }


        private void SetupTextInputsAction(GameObject action, string text)
        {
            action.transform.Find("Text Action").GetComponent<Text>().text = text;
        }


        public void SetScreenEffect(string name)
        {

            Animation animation = null;

            switch (name)
            {
                case "BloodDamage":

                    animation = anim_BloodDamage;
                    break;

                case "Healing":
                    animation = anim_Healing;
                    break;

                default:
                    break;
            }


            if(animation != null)
            {
                animation.Play();
                if (animation.GetComponent<AudioSource>()
                    && animation.GetComponent<AudioSource>().clip != null)
                {
                    animation.GetComponent<AudioSource>().PlayOneShot(
                        animation.GetComponent<AudioSource>().clip);
                }
            }
        }

        #region Message.
        public void SetMessage(bool add, Item item)
        {
            if (item == null)
                return;

            PrfMessage msg = Instantiate(prfMessage, gridMessage).GetComponent<PrfMessage>();
            if(msg != null)
            {
                msg.Refresh(add, item);
            }
        }

        public void SetMessage(string message, Color color)
        {
            if (message == string.Empty)
                return;

            PrfMessage msg = Instantiate(prfMessage, gridMessage).GetComponent<PrfMessage>();
            if (msg != null)
            {
                msg.Refresh(message, color);
            }
        }
        #endregion
    }
}