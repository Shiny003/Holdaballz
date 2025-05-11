using System;
using BepInEx;
using Holdaballz.Behaviours;
using UnityEngine;

namespace Holdaballz
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static PunCallbacks PunCallbacks;
        public static bool Null;
        private void Start()
        {
            Debug.Log("Plugin loaded");
            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
        }

        private void OnGameInitialized()
        {
            Null = true;
            PunCallbacks = new GameObject("HoldaballzPunCallbacks").AddComponent<PunCallbacks>();
        }

        public float LastUpdate;
        public void Update()
        {
            if (Null && !BallManager.MyBall && (Time.time > LastUpdate + 0.1f))
            {
                BallManager.CreateBall(GorillaTagger.Instance.offlineVRRig);
                LastUpdate = Time.time;
            }
        }
    }
}
