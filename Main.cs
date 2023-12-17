using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using GorillaLocomotion;
using BepInEx;
using UnityEngine;
using Utilla;
using System.IO;
using System.Diagnostics;
using BepInEx.Logging;

namespace Holdaballz
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("LateUpdate", MethodType.Normal)]
    public class Main
    {
        public static GameObject BallObj;
        public static void CreateBall()
        {
            //Too lazy to organize
            BallObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            BallObj.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
            BallObj.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            BallObj.AddComponent<Rigidbody>();
            BallObj.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            BallObj.layer = 8;
            BallObj.name = "Ball";
            BallObj.transform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
            BallObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
        public static void DestroyBall()
        {
            if (BallObj != null)
            {
                GameObject.Destroy(BallObj);
                BallObj = null;
            }
        }
        public static void ResetBall()
        {
            DestroyBall();
            CreateBall();
        }
    }
}
