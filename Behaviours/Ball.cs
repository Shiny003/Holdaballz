using System;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

namespace Holdaballz.Behaviours
{
    public class Ball : MonoBehaviour
    {
        public GorillaVelocityEstimator velocityEstimator;
        public Rigidbody rigidbody;
        public Renderer renderer;
        
        public bool isLocal;
        public bool isGrabbed;
        
        public VRRig rig;
        
        private void FixedUpdate()
        {
            if (Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, transform.position) < 0.1f)
            {
                if (ControllerInputPoller.instance.rightGrab && !isGrabbed)
                {
                    Grab();
                }

                if (!ControllerInputPoller.instance.rightGrab && isGrabbed)
                {
                    Throw();
                }
            }
        }

        private void Update()
        {
            if (renderer.material.color != rig.playerColor) renderer.material.color = rig.playerColor;
            if (isLocal)
            {
                if (transform.position.y < -20f)
                {
                    transform.position = Camera.main.transform.position;
                }
            }
        }
        
        #region Grab
        public void Grab()
        {
            PunCallbacks.SendEvent(new HoldaballzEvent
            {
                Type = BallEventType.Grab
            });
            ProcessGrab();
        }

        public void ProcessGrab()
        {
            Debug.Log("Grabbed");
            var theRig = isLocal ? GorillaTagger.Instance.offlineVRRig : rig;
            Debug.Log($"{theRig.name}, {theRig.rightHandTransform.position}, setting data");
            isGrabbed = true;
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
            transform.position = theRig.rightHandTransform.position;
            transform.SetParent(theRig.rightHandTransform);
        }
        #endregion

        #region Throw
        public void Throw()
        {
            PunCallbacks.SendEvent(new HoldaballzEvent
            {
                Type = BallEventType.Throw,
                Data = velocityEstimator.linearVelocity
            });
            ProcessThrow(velocityEstimator.linearVelocity);
        }

        public void ProcessThrow(Vector3 vel)
        {
            Debug.Log("Thrown");
            isGrabbed = false;
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
            rigidbody.velocity = vel;
            transform.SetParent(null);
        }
        #endregion
    }
}