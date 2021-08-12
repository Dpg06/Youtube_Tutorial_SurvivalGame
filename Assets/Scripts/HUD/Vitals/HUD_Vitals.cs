using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectSurvival
{

    public class HUD_Vitals : MonoBehaviour
    {
        public static HUD_Vitals instance = null;


        [Header("Value :")]
        [SerializeField] private Image healthImage = null;
        [SerializeField] private Image thirstyImage = null;
        [SerializeField] private Image hungerImage = null;
        [SerializeField] private Image staminaImage = null;

        private void Awake()
        {
            if (instance == null)
                instance = this;
        }

        public void UI_Health(float value, float max)
        {
            healthImage.fillAmount = Inventory.instance.GetPercent(value, max);
        }
        public void UI_Thirsty(float value, float max)
        {
            thirstyImage.fillAmount = Inventory.instance.GetPercent(value, max);
        }
        public void UI_Hunger(float value, float max)
        {
            hungerImage.fillAmount = Inventory.instance.GetPercent(value, max);
        }
        public void UI_Stamina(float value, float max)
        {
            staminaImage.fillAmount = Inventory.instance.GetPercent(value, max);
        }

    }
}