using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{

    public class StartItems : MonoBehaviour
    {
        [SerializeField] List<RequireItem> items = new List<RequireItem>();


        public void Initialize()
        {
            if (items.Count <= 0)
                return;


            for (int i = 0; i < items.Count; i++)
            {
                Item item = GameManager.instance.resources.GetItemByName(items[i].requireName);
                if(item != null)
                {
                    if (items[i].requireQuantity > 0)
                    {
                        item.quantity = items[i].requireQuantity;

                        Inventory.instance.AddItem_Quickslots(item);

                    }
                }
            }
        }

    }
}