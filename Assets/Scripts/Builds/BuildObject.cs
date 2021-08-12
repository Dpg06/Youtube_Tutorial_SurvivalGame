using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{

    public class BuildObject : MonoBehaviour
    {

        private List<Collider> contacts = new List<Collider>();
        private Item buildItem;

        //private bool isSnapped;
        //public bool IsSnapped { set { isSnapped = value; } }


        [SerializeField]
        private float offsetPositionY = 0;
        public float OffsetPositionY { get { return offsetPositionY; } }

        private bool isBuildable = false;
        public bool IsBuildable
        {
            get { return isBuildable; }
        }

        private bool snapped;
        public bool Snapped { set { snapped = value; } }

        private MeshRenderer[] meshRenderers = null;

        private Material greenMat;
        private Material redMat;

        private void Awake()
        {
            Collider collider = GetComponent<Collider>();
            if(collider == null)
            {
                Debug.LogWarning("Please add a collider on this object !");
                return;
            }

            collider.isTrigger = true;

            greenMat = Resources.Load<Material>("Materials/M_Build_Green");
            redMat = Resources.Load<Material>("Materials/M_Build_Red");


            meshRenderers = GetComponentsInChildren<MeshRenderer>();
            GameManager.SetLayer(gameObject, 12);
        }

        private void Start()
        {
            ChangeColor();
        }

        private void Update()
        {
            CheckBuildable();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 14)
                return;
            if (other.gameObject.layer == 15)
                return;

            contacts.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == 14)
                return;
            if (other.gameObject.layer == 15)
                return;

            contacts.Remove(other);
        }


        private void CheckBuildable()
        {
            if(contacts.Count == 0)
            {
                if (buildItem != null && buildItem.build.SnapRequired)
                {
                    isBuildable = snapped;
                }
                else
                {
                    isBuildable = true;
                }
            }
            else
            {
                isBuildable = false;
            }


            ChangeColor();
        }


        private void ChangeColor()
        {
            if(meshRenderers.Length > 0)
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    meshRenderers[i].material = (isBuildable) ? greenMat : redMat;
                }
            }
            else
            {
                Debug.Log("No mesh renderer on this object ?!");
            }
        }

        public void Setup(Item item)
        {
            if (item == null)
                return;

            buildItem = item;
        }
    }
}