using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{
    public enum CrosshairType { None, Pointer, Weapon }
    public class Crosshair : MonoBehaviour
    {

        public static Crosshair instance = null;
        

        [Header("Pointer :")]
        [SerializeField] private GameObject pointer = null;

        [Header("Weapon :")]
        [SerializeField] private GameObject weapon = null;
        [SerializeField] private RectTransform barreLeft = null;
        [SerializeField] private RectTransform barreRight = null;
        [SerializeField] private RectTransform barreUp = null;
        [SerializeField] private RectTransform barreDown = null;
        [SerializeField] private Vector2 barreSize = new Vector2(14, 3);
        [SerializeField] private float barrePosition = 14;
        [SerializeField] private float barreSpeed = 8;

        private void Awake()
        {
            if (instance == null)
                instance = this;
        }

        private void Update()
        {
            if (weapon.activeInHierarchy)
            {
                WeaponBarre_Origin();
            }
        }

        private void OnValidate()
        {
            if (weapon.activeInHierarchy)
            {
                barreLeft.sizeDelta = barreSize;
                barreRight.sizeDelta = barreSize;
                barreUp.sizeDelta = barreSize;
                barreDown.sizeDelta = barreSize;
                WeaponBarre_Origin();
            }
        }


        private void WeaponBarre_Origin()
        {
            barreLeft.localPosition = Vector3.Lerp(
                barreLeft.localPosition, new Vector3(-barrePosition, 0, 0),
                Time.deltaTime * barreSpeed);

            barreRight.localPosition = Vector3.Lerp(
                barreRight.localPosition, new Vector3(barrePosition, 0, 0),
                Time.deltaTime * barreSpeed);

            barreUp.localPosition = Vector3.Lerp(
                barreUp.localPosition, new Vector3(0, barrePosition, 0),
                Time.deltaTime * barreSpeed);

            barreDown.localPosition = Vector3.Lerp(
                barreDown.localPosition, new Vector3(0, -barrePosition, 0),
                Time.deltaTime * barreSpeed);
        }

        public void WeaponBarre_Force(float force)
        {
            barreLeft.localPosition = Vector3.Lerp(
                barreLeft.localPosition, new Vector3(-barrePosition * force, 0, 0),
                Time.deltaTime * barreSpeed);

            barreRight.localPosition = Vector3.Lerp(
                barreRight.localPosition, new Vector3(barrePosition * force, 0, 0),
                Time.deltaTime * barreSpeed);

            barreUp.localPosition = Vector3.Lerp(
                barreUp.localPosition, new Vector3(0, barrePosition * force, 0),
                Time.deltaTime * barreSpeed);

            barreDown.localPosition = Vector3.Lerp(
                barreDown.localPosition, new Vector3(0, -barrePosition * force, 0),
                Time.deltaTime * barreSpeed);
        }


        public void ChangeCrosshair(CrosshairType type)
        {
            pointer.SetActive(type == CrosshairType.Pointer && type != CrosshairType.None);
            weapon.SetActive(type == CrosshairType.Weapon && type != CrosshairType.None);
        }
    }
}