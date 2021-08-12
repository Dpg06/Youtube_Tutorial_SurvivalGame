using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectSurvival
{

    public class Player : MonoBehaviour
    {
        #region System class.
        [System.Serializable]
        internal class InputValue
        {
            public float axe_H;
            public float axe_V;
            public float axe_Hr;
            public float axe_Vr;

            public bool run;
            public bool jump;
        }

        [System.Serializable]
        public class Stats
        {
            [Header("Speed Walk :")]
            public float walkSpeed = 3;
            public float runSpeed = 6;
            public float jumpPower = 6;
            public float ejectForce = 800;

            [Header("Smooth :")]
            public float smoothWalkSpeed = 3;
            public float smoothAirSpeed = 1;
            public float smoothStopSpeed = 1;

            [Header("Crosshair forces :")]
            public float crossForce_Walk = 10;
            public float crossForce_Run = 20;
            public float crossForce_Jump = 50;
        }

        [System.Serializable]
        public class Status
        {
            public bool canMove = true;
            public bool canRun = true;
            public bool canJump = true;

            public bool isWalking = false;
            public bool isRunning = false;
            public bool inInAir = false;
        }

        [System.Serializable]
        internal class Vitals
        {
            [Header("Health")]
            public float healthValue = 100;
            public float healthMax = 100;
            [Header("Thirsty")]
            public float thirstyValue = 100;
            public float thirstyMax = 100;
            public float thirstyDecrease = 1;
            [Header("Hunger")]
            public float hungerValue = 100;
            public float hungerMax = 100;
            public float hungerDecrease = 1;

            [Header("Stamina")]
            public float staminaValue = 100;
            public float staminaMax = 100;

            public float staminaDecrease = 1;   // Diminuer for timer.
            public float staminaIncrease = 1;   // Augmenter for timer.
            public float jumpReduce = 10;


            [Header("Parameters :")]
            public float reduceValue = 1; // For decrease or increase value.
        }
        #endregion
        
        #region Variables.
        [SerializeField] private InputValue inp = new InputValue();
        [SerializeField] public Stats stats = new Stats();
        [SerializeField] public Status status = new Status();
        [SerializeField] private Vitals vitals = new Vitals();

        private CharacterController cc = null;
        [HideInInspector] public FpsCamera fpsCamera = null;


        private Vector3 move = new Vector3();
        private Vector3 moveAir = new Vector3();
        private float staminaTimer = 0;
        private float thirstyTimer = 0;
        private float hungerTimer = 0;

        // Fall Distance.
        private float lastYposition = 0;
        private float maxDistanceFallDamage = 5;

        [Header("Debug :")]
        [SerializeField] private float currentWalkSpeed = 0;
        [SerializeField] private float fallDistance = 0;
        [SerializeField] private float currentGravity = 0;
        [SerializeField] private bool initialized = false;
        #endregion


        

        #region Unity Méthods.
        private void Start()
        {
            Init();
        }

        private void Update()
        {
            if (initialized)
            {
                fpsCamera.Update_Raycast();

                Update_Inputs_HUD();

                // Stamina.
                if (vitals.staminaValue < vitals.staminaMax || inp.run)
                    Stamina();
                ThirstyReduce();
                HungerReduce();


                if (status.canMove)
                {
                    Update_Inputs_Movements();
                    
                    Motor();
                    fpsCamera.CameraRotation(inp.axe_Vr);
                    fpsCamera.Update_TargetLook();
                    fpsCamera.Update_HeadBob();
                    Running();
                }
                else
                {
                    StopPlayer();
                }

                Jumping();
                FallDistance();
            }
        }

        private void FixedUpdate()
        {
            if (initialized)
            {

                    cc.Move(move * Time.fixedDeltaTime);
            }
        }
        #endregion

        #region Init.
        private void Init()
        {
            

            cc = GetComponent<CharacterController>();
            fpsCamera = GetComponentInChildren<FpsCamera>();
            fpsCamera.Init(this);

            if (Inventory.instance != null)
                Inventory.instance.Init(this);
            if (Quickslot.instance != null)
                Quickslot.instance.Init(this);


            HUD_Vitals.instance.UI_Stamina(vitals.staminaValue, vitals.staminaMax);

            initialized = true;
        }
        #endregion

        #region Update Inputs.
        private void Update_Inputs_Movements()
        {
            inp.axe_H = Input.GetAxis("Horizontal");
            inp.axe_V = Input.GetAxis("Vertical");
            inp.axe_Hr = Input.GetAxis("Mouse X") * GameManager.instance.options.MouseSensitivity;
            inp.axe_Vr = Input.GetAxis("Mouse Y") * GameManager.instance.options.MouseSensitivity;

            inp.run = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            inp.jump = Input.GetKeyDown(KeyCode.Space);
            
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.T))
                AddThirsty(10);
            if (Input.GetKeyDown(KeyCode.U))
                AddHunger(5);
#endif
        }

        private void Update_Inputs_HUD()
        {
            if (Input.GetKeyDown(GameManager.instance.inputs.key_Inventory))
                Inventory.instance.ShowHide_Inventory();
        }
        #endregion

        #region Motor Controller.
        private void Motor()
        {
            if (status.inInAir)
            {
                move = moveAir;
            }
            else
            {
                move = new Vector3(inp.axe_H, 0, inp.axe_V);
            }

            if (move.magnitude > 1)
                move.Normalize();

            transform.Rotate(0, inp.axe_Hr, 0);

            if(!status.inInAir)
                move = transform.rotation * move;


            // Crosshair.
            if (status.isWalking)
                Crosshair.instance.WeaponBarre_Force(stats.crossForce_Walk);
            if (status.isRunning)
                Crosshair.instance.WeaponBarre_Force(stats.crossForce_Run);

        }

        private void Running()
        {
            bool inpDir = (inp.axe_H != 0 || inp.axe_V != 0);

            status.isWalking = !status.isRunning && inpDir;

            if (inp.run && inpDir && !status.inInAir && status.canRun)
            {
                currentWalkSpeed = Mathf.Lerp(currentWalkSpeed, stats.runSpeed,
                    Time.deltaTime * stats.smoothWalkSpeed);
                status.isRunning = true;
            }
            else
            {
                status.isRunning = false;

                if (status.isWalking && !status.inInAir)
                {
                    currentWalkSpeed = Mathf.Lerp(currentWalkSpeed, stats.walkSpeed,
                        Time.deltaTime * stats.smoothWalkSpeed);
                    
                }
                else
                {
                    if (status.inInAir)
                    {
                        currentWalkSpeed = Mathf.Lerp(currentWalkSpeed, 0,
                        Time.deltaTime * stats.smoothAirSpeed);
                    }
                    else
                    {
                        currentWalkSpeed = Mathf.Lerp(currentWalkSpeed, 0,
                        Time.deltaTime * stats.smoothWalkSpeed);
                    }
                    
                }
            }

            move = currentWalkSpeed * move;
        }

        private void Jumping()
        {
            if(inp.jump && !status.inInAir && status.canMove && status.canJump)
            {
                currentGravity = stats.jumpPower;
                moveAir = move;

                StaminaReduce(vitals.jumpReduce);
            }
            else
            {
                if (cc.isGrounded)
                {
                    currentGravity = GameManager.instance.options.gravityEarth;
                    status.inInAir = false;
                }
                else
                {
                    status.inInAir = true;
                    currentGravity += GameManager.instance.options.gravityEarth
                        * Time.deltaTime;
                    Crosshair.instance.WeaponBarre_Force(stats.crossForce_Jump);
                }
            }

            move.y = currentGravity;
        }

        private void FallDistance()
        {
            if(lastYposition > transform.position.y)
                fallDistance += lastYposition - transform.position.y;
            
            if (cc.isGrounded)
            {
                if(fallDistance >= maxDistanceFallDamage)
                {
                    // On prend des dégats de chutes.
                    ReduceHealth(fallDistance);

                    fallDistance = 0;
                    lastYposition = 0;
                }

                if(fallDistance < maxDistanceFallDamage)
                {
                    fallDistance = 0;
                    lastYposition = 0;
                }
            }

            lastYposition = transform.position.y;
        }

        private void StopPlayer()
        {
            move.x = Mathf.Lerp(move.x, 0, Time.deltaTime * stats.smoothStopSpeed);
            move.z = Mathf.Lerp(move.z, 0, Time.deltaTime * stats.smoothStopSpeed);
        }

        public void SetController(bool active)
        {
            status.canMove = active;

        }
        #endregion

        #region Vitals.

        #region Health.
        public void AddHealth(float value)
        {
            vitals.healthValue += value;
            if (vitals.healthValue > vitals.healthMax)
                vitals.healthValue = vitals.healthMax;

            HUD.instance.SetScreenEffect("Healing");
            HUD_Vitals.instance.UI_Health(vitals.healthValue, vitals.healthMax);
        }

        public void ReduceHealth(float value)
        {
            vitals.healthValue -= value;
            if(vitals.healthValue <= 0)
            {
                vitals.healthValue = 0;

                // Le player n'a plus de vie.

            }


            HUD.instance.SetScreenEffect("BloodDamage");
            HUD_Vitals.instance.UI_Health(vitals.healthValue, vitals.healthMax);
        }
        #endregion

        #region Thirsty.
        private void ThirstyReduce()
        {
            if(Time.time > thirstyTimer)
            {
                vitals.thirstyValue -= vitals.reduceValue;
                if (vitals.thirstyValue <= 0)
                    vitals.thirstyValue = 0;

                HUD_Vitals.instance.UI_Thirsty(vitals.thirstyValue, vitals.thirstyMax);

                thirstyTimer = Time.time + vitals.thirstyDecrease;
            }
        }

        public void AddThirsty(float value)
        {
            vitals.thirstyValue += value;
            if (vitals.thirstyValue > vitals.thirstyMax)
                vitals.thirstyValue = vitals.thirstyMax;
            HUD_Vitals.instance.UI_Thirsty(vitals.thirstyValue, vitals.thirstyMax);
        }
        #endregion

        #region Hunger.
        private void HungerReduce()
        {
            if (Time.time > hungerTimer)
            {
                vitals.hungerValue -= vitals.reduceValue;
                if (vitals.hungerValue <= 0)
                    vitals.hungerValue = 0;

                HUD_Vitals.instance.UI_Hunger(vitals.hungerValue, vitals.hungerMax);

                hungerTimer = Time.time + vitals.hungerDecrease;
            }
        }

        public void AddHunger(float value)
        {
            vitals.hungerValue += value;
            if (vitals.hungerValue > vitals.hungerMax)
                vitals.hungerValue = vitals.hungerMax;
            HUD_Vitals.instance.UI_Hunger(vitals.hungerValue, vitals.hungerMax);
        }
        #endregion

        #region Stamina.
        private void Stamina()
        {
            // Decrease.
            if (status.isRunning)
            {
                if(Time.time > staminaTimer && vitals.staminaValue > 0)
                {
                    vitals.staminaValue -= vitals.reduceValue;
                    if (vitals.staminaValue <= 0)
                        vitals.staminaValue = 0;

                    staminaTimer = Time.time + vitals.staminaDecrease;
                }
            }
            else
            {
                if(Time.time > staminaTimer && vitals.staminaValue != vitals.staminaMax)
                {
                    vitals.staminaValue += vitals.reduceValue;
                    if (vitals.staminaValue > vitals.staminaMax)
                        vitals.staminaValue = vitals.staminaMax;

                    staminaTimer = Time.time + vitals.staminaIncrease;
                }
            }

            status.canRun = vitals.staminaValue > 0;
            status.canJump = vitals.staminaValue >= vitals.jumpReduce;

            HUD_Vitals.instance.UI_Stamina(vitals.staminaValue, vitals.staminaMax);
        }

        private void StaminaReduce(float value)
        {
            vitals.staminaValue -= value;
            if (vitals.staminaValue <= 0)
                vitals.staminaValue = 0;

            HUD_Vitals.instance.UI_Stamina(vitals.staminaValue, vitals.staminaMax);
        }
        #endregion
        #endregion

        #region Other.
        public bool GetDistance(Transform obj, float distMax)
        {
            float dist = Vector3.Distance(obj.position, fpsCamera.transform.position);
            return (dist <= distMax);
        }

        public void EjectObject(Item item)
        {
            if (item == null || item.quantity <= 0 || item.prf_Ground == null)
                return;

            GameObject g = Instantiate(item.prf_Ground, 
                fpsCamera.targetEject.position, fpsCamera.targetEject.rotation);

            if(g.GetComponent<Rigidbody>())
            {
                Rigidbody rb = g.GetComponent<Rigidbody>();
                rb.AddForce(fpsCamera.transform.forward
                * (stats.ejectForce / rb.mass));
            }

            if (g.GetComponent<Interact>())
                g.GetComponent<Interact>().SetItem(item);

            Quickslot.instance.Selection();
        }
        #endregion
    }
}