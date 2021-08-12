using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectSurvival
{

    public class PrfMessage : MonoBehaviour
    {

        private Text txtQuantity = null;
        private Text txtName = null;
        private Text txtMessage = null;
        private Image imageIcon = null;

        private void Awake()
        {
            txtQuantity = transform.Find("Text Quantity").GetComponent<Text>();
            txtName = transform.Find("Text Name").GetComponent<Text>();
            txtMessage = transform.Find("Text Message").GetComponent<Text>();
            imageIcon = transform.Find("Image Icon").GetComponent<Image>();
        }

        private void Start()
        {
            Destroy(gameObject, 4);
        }


        public void Refresh(bool add, Item item)
        {
            if (item == null)
                Destroy(gameObject);
            else
            {
                string _qt = (add) ? "+" + item.quantity : "-" + item.quantity;

                txtQuantity.text = _qt + " (" + Inventory.instance.ReturnItemsQuantity(item.itemName) + ")";
                txtQuantity.color = (add) ? Color.green : Color.red;

                txtName.text = item.itemName;

                imageIcon.enabled = true;
                imageIcon.sprite = item.itemIcon;
                imageIcon.preserveAspect = true;

                txtMessage.text = string.Empty;
            }
        }

        public void Refresh(string message, Color color)
        {
            if (message == string.Empty)
                Destroy(gameObject);
            else
            {
                txtQuantity.text = string.Empty;
                txtQuantity.color = Color.white;

                txtName.text = string.Empty;

                imageIcon.sprite = null;
                imageIcon.enabled = false;

                txtMessage.text = message;
                txtMessage.color = color;
            }
        }
    }
}