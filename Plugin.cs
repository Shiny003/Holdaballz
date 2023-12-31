using System;
using BepInEx;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UIElements;
using Utilla;
using Holdaballz.Tools;
using UnityEngine.Device;
using ComputerPlusPlus;
using Valve.VR.InteractionSystem;
using Oculus.Platform;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;
using GorillaLocomotion.Climbing;

namespace Holdaballz
{
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInDependency("com.kylethescientist.gorillatag.computerplusplus")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool Enabled;
        bool InRoom;
        string HandGrabbing;
        float DistanceThingy = 0.1f;
        public static GameObject Cube;

        void Start()
        {
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnEnable()
        {
            Enabled = true;

            if (InRoom)
            {
                Main.CreateBall();
            }

            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            Enabled = false;

            Main.DestroyBall();

            HarmonyPatches.RemoveHarmonyPatches();
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            
        }
        //renderer
        public bool ThrowPhys;
        void Update()
        {
            if (!Screen.StuckToHand)
            {
                if (Main.BallObj != null && Enabled && InRoom)
                {
                    // Too lazy to figure out OnTrigger crap

                    if (ControllerInputPoller.instance.rightGrab && GorillaLocomotion.Player.Instance.rightControllerTransform.position.Distance(Main.BallObj.transform.position) < DistanceThingy && HandGrabbing != "left")  // Right Hand
                    {
                        Main.BallObj.transform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
                        Main.BallObj.transform.rotation = GorillaLocomotion.Player.Instance.rightControllerTransform.rotation;
                        Main.BallObj.GetComponent<Rigidbody>().useGravity = false;
                        ThrowPhys = true;
                        DistanceThingy = 10f;
                        HandGrabbing = "right";
                    }
                    else if (ControllerInputPoller.instance.leftGrab && GorillaLocomotion.Player.Instance.leftControllerTransform.position.Distance(Main.BallObj.transform.position) < DistanceThingy && HandGrabbing != "right") // Left hand
                    {
                        Main.BallObj.transform.position = GorillaLocomotion.Player.Instance.leftControllerTransform.position;
                        Main.BallObj.transform.rotation = GorillaLocomotion.Player.Instance.leftControllerTransform.rotation;
                        Main.BallObj.GetComponent<Rigidbody>().useGravity = false;
                        ThrowPhys = true;
                        DistanceThingy = 10f;
                        HandGrabbing = "left";
                    }
                    else //None
                    {
                        if (ThrowPhys)
                        {
                            Main.BallObj.GetComponent<Rigidbody>().velocity = Main.BallObj.GetComponent<GorillaVelocityEstimator>().linearVelocity;
                            ThrowPhys = false;
                        }
                        Main.BallObj.GetComponent<Rigidbody>().useGravity = true;
                        DistanceThingy = 0.1f;
                        HandGrabbing = "none";
                    }

                    if (GorillaLocomotion.Player.Instance.bodyCollider.transform.position.Distance(Main.BallObj.transform.position) > 10f) // Far from player detector
                    {
                        Main.ResetBall();
                    }
                }
            }
            else
            {
                Main.BallObj.transform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
                Main.BallObj.transform.rotation = GorillaLocomotion.Player.Instance.rightControllerTransform.rotation;
                Main.BallObj.GetComponent<Rigidbody>().useGravity = false;
            }

            if (Enabled == false)
            {
                Main.DestroyBall();
            }

            //Model changer
            //I put InRooms everywhere don't worry about it
            if (!Screen.Sphere && InRoom)
            {
                if (Cube == null)
                {
                    Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    Cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    Cube.transform.position = Main.BallObj.transform.position;
                    Cube.transform.rotation = Main.BallObj.transform.rotation;

                    Cube.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
                    Cube.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                    GameObject.Destroy(Cube.GetComponent<BoxCollider>());

                    Cube.layer = 8;
                }
                Cube.transform.position = Main.BallObj.transform.position;
            }
            else
            {
                DestroyCube();
            }

        }

        void DestroyCube()
        {
            if (Cube != null)
            {
                GameObject.Destroy(Cube);
                Cube = null;
            }
        }

        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            InRoom = true;

            if (Enabled)
            {
                Main.CreateBall();
            }
        }

        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            InRoom = false;
            Main.DestroyBall();
            DestroyCube();
        }
    }
    public class Screen : IScreen
    {
        public string Title => "Test";

        public static bool StuckToHand;
        public static bool Sphere = true;

        public string Description => "Press [Option 1] to use the sphere option and [Option 2] for the cube. Press 1 to make it stuck to your hand and 2 to not";

        public string ButtonPressed;

        // This is called every frame while the screen is active
        public string GetContent()
        {
            if (ButtonPressed == "1")
            {
                StuckToHand = true;
            }

            if (ButtonPressed == "2")
            {
                StuckToHand = false;
            }

            if (ButtonPressed == "option1")
            {
                Sphere = true;
            }

            if (ButtonPressed == "option2")
            {
                Sphere = false;
            }

            return "Sphere: " + Sphere.ToString() + "\r\nStuck in hand: " + StuckToHand.ToString();
        }

        // This is called whenever a key is pressed while the screen is active
        public void OnKeyPressed(GorillaKeyboardButton button)
        {
            ButtonPressed = button.characterString;
        }

        // This is called when the screen is registered
        public void Start() { }
    }
}
