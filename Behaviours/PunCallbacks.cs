using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Newtonsoft.Json;
using Photon.Realtime;
using UnityEngine;

namespace Holdaballz.Behaviours
{
    public class PunCallbacks : MonoBehaviourPunCallbacks
    {
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            PhotonNetwork.NetworkingClient.EventReceived += RaiseEvent;
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable()
            {
                { "Holdaballz", true }
            });
            foreach (var player in NetworkSystem.Instance.PlayerListOthers)
            {
                if (player.GetPlayerRef().CustomProperties.ContainsKey("Holdaballz"))
                    BallManager.CreateBall(GorillaParent.instance.vrrigDict[player]);
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (newPlayer.CustomProperties.ContainsKey("Holdaballz"))
                BallManager.CreateBall(GorillaParent.instance.vrrigDict[NetworkSystem.Instance.GetPlayer(newPlayer)]);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            var ball = BallManager.BallFromPlayer(otherPlayer);
            Destroy(ball.gameObject);
            ball = null;
        }

        private void RaiseEvent(ExitGames.Client.Photon.EventData obj)
        {
            if (obj.Sender == PhotonNetwork.LocalPlayer.ActorNumber) return;
            if (obj.Code == 88)
            {
                Debug.Log(obj.CustomData);
                var data = JsonConvert.DeserializeObject<HoldaballzEvent>((string)obj.CustomData);
                switch (data.Type)
                {
                    case BallEventType.Throw:
                        Debug.Log("Throw");
                        Throw(obj.Sender, (Vector3)data.Data);
                        break;
                    case BallEventType.Grab:
                        Debug.Log("Grab");
                        Grab(obj.Sender);
                        break;
                    default:
                        Debug.Log(data.Type);
                        break;
                }
            }
        }

        private void Throw(int playerActor, Vector3 vel)
        {
            var player = NetworkSystem.Instance.GetPlayer(playerActor).GetPlayerRef();
            var ball = BallManager.BallFromPlayer(player);
            ball.GetComponent<Ball>().ProcessThrow(vel);
        }

        private void Grab(int playerActor)
        {
            Debug.Log("Grab " + playerActor);
            var player = NetworkSystem.Instance.GetPlayer(playerActor);
            Debug.Log("Grab " + JsonConvert.SerializeObject(player));
            var ball = BallManager.BallFromPlayer(player.GetPlayerRef());
            Debug.Log("Grab " + ball.name);
            ball.GetComponent<Ball>().ProcessGrab();
        }

        public static void SendEvent(HoldaballzEvent data)
        {
            PhotonNetwork.RaiseEvent(88, JsonConvert.SerializeObject(data), RaiseEventOptions.Default, SendOptions.SendReliable);
        }
    }
}