using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{

    public class ToolController : MonoBehaviour
    {
        private Item tool = null;
        private ArmAnimations armAnimations = null;
        private bool initialized = false;


        [Header("Primary Action :")]
        [SerializeField] private string primaryAnimName = string.Empty;
        [SerializeField] private bool primaryHoldKey = true;

        [Header("Secondary Action :")]
        [SerializeField] private string secondaryAnimName = string.Empty;
        [SerializeField] private bool secondaryHoldKey = true;

        void Update()
        {
            if (initialized)
            {
                Update_Inputs();
            }
        }

        private void Update_Inputs()
        {
            if (armAnimations.canRetryAction && !GameManager.instance.CheckHUDactive)
            {

                if (primaryAnimName != string.Empty)
                {
                    if (primaryHoldKey)
                    {
                        if (Input.GetKey(GameManager.instance.inputs.key_PrimaryAction))
                            armAnimations.StartAnimTrigger(primaryAnimName);
                    }
                    else
                    {
                        if (Input.GetKeyDown(GameManager.instance.inputs.key_PrimaryAction))
                            armAnimations.StartAnimTrigger(primaryAnimName);
                    }
                }
                if (secondaryAnimName != string.Empty)
                {
                    if (secondaryHoldKey)
                    {
                        if (Input.GetKey(GameManager.instance.inputs.key_SecondaryAction))
                            armAnimations.StartAnimTrigger(secondaryAnimName);
                    }
                    else
                    {
                        if (Input.GetKeyDown(GameManager.instance.inputs.key_SecondaryAction))
                            armAnimations.StartAnimTrigger(secondaryAnimName);
                    }
                }
            }
        }
        

        #region Set Item.
        public void SetItem(Item item)
        {
            if (item.itemType != ItemType.Tool)
                return;

            tool = item;

            armAnimations = GetComponentInChildren<ArmAnimations>();
            if (armAnimations == null)
                return;
            armAnimations.Init();

            initialized = true;
        }
        #endregion
    }
}