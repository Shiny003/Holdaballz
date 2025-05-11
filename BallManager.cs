using System.Collections.Generic;
using Holdaballz.Behaviours;
using Photon.Realtime;
using UnityEngine;

namespace Holdaballz
{
    public class BallManager
    {
        public static GameObject BallObj;
        public static Ball MyBall;
        public static List<Ball> Balls = new List<Ball>();
        public static Dictionary<string, Ball> BallDict = new Dictionary<string, Ball>();
        
        public static void CreateBall(VRRig forRig)
        {
            Debug.Log($"Creating ball for {forRig.Creator.NickName ?? "null"}");
            BallObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            BallObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            if (Camera.main != null) BallObj.transform.position = Camera.main.transform.position;
            BallObj.name = $"{forRig.Creator.NickName} Ball";
            BallObj.layer = 8;

            var ball = BallObj.AddComponent<Ball>();
            ball.velocityEstimator = BallObj.AddComponent<GorillaVelocityEstimator>();
            ball.rigidbody = BallObj.AddComponent<Rigidbody>();
            ball.isLocal = forRig.isOfflineVRRig;
            ball.rig = forRig;
            MyBall = ball;
            
            var renderer = BallObj.GetComponent<Renderer>();
            ball.renderer = renderer;
         
            renderer.material.shader = Shader.Find("GorillaTag/UberShader");
            
            Balls.Add(ball);
            BallDict.Add(forRig.Creator.GetPlayerRef().UserId, ball);
        }

        public static Ball BallFromPlayer(Player player)
        {
            Ball result = null;
            if (BallDict.TryGetValue(player.UserId, out var res))
            {
                Debug.Log($"Found {player.NickName}'s ball {res.gameObject.name ?? "no name"}");
                result = res;
            }
            else
                Debug.Log($"Could not find {player} in dict");

            return result;
        }
    }
}
