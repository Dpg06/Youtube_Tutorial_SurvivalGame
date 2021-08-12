using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{
    public class WeaponController : MonoBehaviour
    {
        

        #region References.
        [Header("References :")]
        [SerializeField] private Transform muzzle = null;
        [SerializeField] private Transform sightPosition = null;
        [SerializeField] private ParticleSystem particle_Flame = null;
        [SerializeField] private ParticleSystem particle_Cartridge = null;
        [SerializeField] private AudioClip clip_Fire = null;
        [SerializeField] private AudioClip clip_Reload = null;
        [SerializeField] private AudioClip clip_Empty = null;
        #endregion

        #region Internal variables.
        private Player player = null;
        private AudioSource audioS = null;

        private Weapon weapon = null;

        private Transform parentModels = null;
        private ArmAnimations armAnimations = null;


        private Vector3 parentModelOrigin = new Vector3();
        private GameObject prefab_Bullet = null;
        private float timer = 0;
        private bool initialized = false;

        // Status.
        private bool canFire
        {
            get
            {
                // External Controller.
                bool externalValue = (parentModels.GetComponentInChildren<ArmAnimations>())
                    ? parentModels.GetComponentInChildren<ArmAnimations>().animState : true;

                return !weapon.isRealoding && !GameManager.instance.CheckHUDactive && externalValue
                    && !player.status.isRunning;
            }

        }
        
        #endregion
        

        #region Unity methods.

        void Update()
        {
            if (initialized)
            {
                Update_Inputs();

                Recoil_Return();

                muzzle.LookAt(player.fpsCamera.targetLook);

                PlayerStates();
            }
        }
        #endregion

        #region Init.
        private void Init()
        {
            if (!GetComponent<AudioSource>())
                gameObject.AddComponent<AudioSource>();

            audioS = GetComponent<AudioSource>();
            audioS.playOnAwake = false;
            audioS.volume = .5f;



            parentModels = transform.Find("ParentModels");
            armAnimations = parentModels.GetComponentInChildren<ArmAnimations>();
            if (armAnimations != null)
                armAnimations.Init();


            parentModelOrigin = parentModels.localPosition;

            prefab_Bullet = Resources.Load<GameObject>("Prefabs/Mechanics/Bullet");
            
            player = Inventory.instance.player;

            Crosshair.instance.ChangeCrosshair(CrosshairType.Weapon);

            HUD_Weapon.instance.Refresh_Weapon(weapon);

            initialized = true;
        }
        #endregion

        #region Inputs.
        private void Update_Inputs()
        {
            // Fire.
            if (weapon.isAutomatic)
            {
                if (Input.GetKey(GameManager.instance.inputs.key_PrimaryAction))
                {
                    Shooting();
                }
            }
            else
            {
                if (Input.GetKeyDown(GameManager.instance.inputs.key_PrimaryAction))
                {
                    Shooting();
                }
            }

            // Zoom.
            Zooming((Input.GetKey(GameManager.instance.inputs.key_SecondaryAction) && canFire && !player.status.isRunning));
            if (Input.GetKeyDown(GameManager.instance.inputs.key_SecondaryAction) && canFire)
                Crosshair.instance.ChangeCrosshair(CrosshairType.None);
            if (Input.GetKeyUp(GameManager.instance.inputs.key_SecondaryAction))
                Crosshair.instance.ChangeCrosshair(CrosshairType.Weapon);

            // Reload.
            if (Input.GetKeyDown(GameManager.instance.inputs.key_Reload) 
                && weapon.ammo < weapon.ammoMax)
                StartCoroutine(Reloading());
        }
        #endregion

        #region Actions.
        private void Shooting()
        {
            if(Time.time > timer && canFire)
            {

                if (weapon.ammo <= 0 || weapon.attribute.quality == "Breaked")
                {
                    audioS.PlayOneShot(clip_Empty);
                }
                else
                {
                    //weapon.ammo--;

                    Inventory.instance.panel_Options.EventBtn(Quickslot.instance.GetSlotID(), "Break");

                    // Effects.
                    particle_Flame.Play();
                    particle_Cartridge.Play();
                    audioS.PlayOneShot(clip_Fire);

                    // HUD.
                    Crosshair.instance.WeaponBarre_Force(weapon.crosshairForce);
                    HUD_Weapon.instance.Refresh_Weapon(weapon);


                    Recoil_Shoot();

                    Instantiate(prefab_Bullet, muzzle.position, muzzle.rotation);
                }

                timer = Time.time + weapon.fireRate;
            }
        }

        private IEnumerator Reloading()
        {
            weapon.isRealoding = true;
            audioS.PlayOneShot(clip_Reload);

            yield return new WaitForSeconds(clip_Reload.length);

            weapon.ammo = weapon.ammoMax;
            HUD_Weapon.instance.Refresh_Weapon(weapon);

            weapon.isRealoding = false;
        }

        private void Zooming(bool _isZooming)
        {
            weapon.isZooming = _isZooming;
            player.fpsCamera.CameraFOV(_isZooming);

            if (_isZooming)
            {
                parentModels.localPosition = Vector3.Lerp(
                    parentModels.localPosition,
                    player.fpsCamera.targetZoom.localPosition - sightPosition.localPosition,
                    Time.deltaTime * 8);
            }
            else
            {
                if (parentModels.localPosition != parentModelOrigin)
                {
                    parentModels.localPosition = Vector3.Lerp(
                        parentModels.localPosition,
                        parentModelOrigin,
                        Time.deltaTime * 8);
                }
            }
        }
        #endregion

        #region Effects.
        private void Recoil_Shoot()
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                new Vector3(0,0, -weapon.recoilForce),
                Time.deltaTime * 8);
        }

        private void Recoil_Return()
        {
            if(transform.localPosition != Vector3.zero)
            {
                transform.localPosition = Vector3.Lerp(
                    transform.localPosition,
                    Vector3.zero,
                    Time.deltaTime * 8);
            }
        }
        #endregion

        #region Set Item.
        public void SetItem(Item item)
        {
            if (item.itemType != ItemType.Weapon)
                return;

            weapon = item as Weapon;

            Init();
        }
        #endregion

        #region Player.
        private void PlayerStates()
        {
            if (player == null)
                return;

            if(armAnimations != null)
            {
                armAnimations.Running(player.status.isRunning);
                
            }

            // Crosshair.
            if (!weapon.isZooming)
            {
                Crosshair.instance.ChangeCrosshair(
                    (player.status.isRunning) ? CrosshairType.None : CrosshairType.Weapon);
            }
        }
        #endregion
    }
}