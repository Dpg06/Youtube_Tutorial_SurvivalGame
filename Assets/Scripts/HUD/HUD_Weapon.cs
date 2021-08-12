using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectSurvival
{
    public class HUD_Weapon : MonoBehaviour
    {
        public static HUD_Weapon instance = null;

        private Text weaponTxt = null;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            weaponTxt = transform.Find("Ammos").GetComponent<Text>();
        }

        public void Refresh_Weapon(Weapon weapon)
        {
            if(weapon == null)
            {
                weaponTxt.text = string.Empty;
                return;
            }

            weaponTxt.text = weapon.ammo + " / " + weapon.ammoMax;
        }
    }
}