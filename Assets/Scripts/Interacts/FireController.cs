using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{

    public class FireController : MonoBehaviour
    {
        private Interact interact;

        [SerializeField] 
        private ParticleSystem particleFlame;
        [SerializeField]
        private ParticleSystem particleSmoke;

        [SerializeField] private AudioClip clipFire;
        [HideInInspector] public bool activeFire = false;

        [HideInInspector]
        public float fireTimer = 0;


        [HideInInspector]
        public Item itemCook1;
        [HideInInspector]
        public Item itemCook2;
        [HideInInspector]
        public Item itemCook3;
        [HideInInspector]
        public Item itemStarter;
        [HideInInspector]
        public Item itemFuel;


        private void Update()
        {
            if (activeFire)
            {
                fireTimer -= Time.deltaTime;
                if (Inventory.instance.panel_Fire.gameObject.activeSelf)
                    Inventory.instance.panel_Fire.Update_UI();

                if (fireTimer <= 0)
                    ActiveFire(true);

                CookItem(itemCook1, 0);
                CookItem(itemCook2, 1);
                CookItem(itemCook3, 2);
            }
        }

        public void Init(Interact interaction)
        {
            interact = interaction;

            interact.audioS.clip = clipFire;
            interact.audioS.loop = true;

            ActiveFire(false);



        }


        public void ActiveFire(bool active)
        {
            activeFire = active;

            if (active)
            {
                bool tryGetFuel = GetFuel();
                if (!tryGetFuel)
                {
                    activeFire = false;
                    fireTimer = 0;
                }
            }
            else
            {
                fireTimer = 0;
            }

            if (Inventory.instance.panel_Fire && Inventory.instance.panel_Fire.gameObject.activeSelf)
                Inventory.instance.panel_Fire.RefreshUI();



            if (activeFire)
            {
                if(particleFlame)
                    particleFlame.Play();
                if(particleSmoke)
                    particleSmoke.Play();
                if (clipFire) interact.audioS.Play();
            }
            else
            {
                if (particleFlame)
                    particleFlame.Stop();
                if (particleSmoke)
                    particleSmoke.Stop();
                interact.audioS.Stop();
            }

        }


        private bool GetFuel()
        {
            if (itemFuel == null || itemFuel.attribute.GetAction(ActionType.FuelFire) == null)
                return false;

            fireTimer = itemFuel.attribute.GetAction(ActionType.FuelFire).value;
            itemFuel.quantity--;
            if (itemFuel.quantity <= 0)
                itemFuel = null;


            return true;
        }


        private void CookItem(Item item, int id)
        {
            if (item == null || item.attribute.name != "Cooking")
                return;

            item.attribute.value += Time.deltaTime;
            if(item.attribute.value >= item.attribute.max)
            {
                Item changeItem = GameManager.instance.resources.GetItemByName(
                    item.attribute.changeItemName);
                if (changeItem)
                {
                    if (id == 0)
                        itemCook1 = changeItem;
                    if (id == 1)
                        itemCook2 = changeItem;
                    if (id == 2)
                        itemCook3 = changeItem;

                    Inventory.instance.sourceAudio.PlayOneShot(
                        Inventory.instance.clip_EndCook);


                    if (Inventory.instance.panel_Fire && Inventory.instance.panel_Fire.gameObject.activeSelf)
                        Inventory.instance.panel_Fire.RefreshUI();
                }
            }
        }
    }
}