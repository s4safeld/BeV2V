using System;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using System.Linq;
using System.Collections;

namespace WebXR
{
    public class ControllerRight : MonoBehaviour
    {
        private FixedJoint attachJoint = null;
        private Rigidbody currentRigidBody = null;
        private List<Rigidbody> contactRigidBodies = new List<Rigidbody>();
        private LineRenderer lr;
        private Transform rig;
        private Animator anim;
        private bool tpEnabled;
        private bool pointingAtPlatform = false;
        private GameObject currPlat;
        public LayerMask layerMask;
        public GameObject cursor;
        private bool teleportCalled;
        private float handDistance;
        private float oldHandDistance = 0;
        private GameObject pointingAt;
        private ChatManager chatmanager;

        void Awake()
        {
            attachJoint = GetComponent<FixedJoint>();
        }

        void Start()
        {
            Debug.Log("Initialising Controller Interaction for: " + gameObject);
            anim = gameObject.GetComponent<Animator>();
            lr = gameObject.GetComponent<LineRenderer>();
            lr.enabled = false;

            rig = transform.parent;
            Debug.Log("Layermask: " + layerMask);

            chatmanager = FindObjectOfType<ChatManager>();

            Debug.Log("chatmanger has name: "+chatmanager.gameObject.name);

            pointingAt = rig.gameObject;
        }

        void Update()
        {
            if (GlobalInformation.vrReady)
            {

                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward-transform.up, out hit/*, layerMask*/))
                {
                    if (hit.collider.transform != cursor.transform)
                    {
                        handDistance = Vector3.Distance(cursor.transform.position, transform.position);
                        if (Math.Abs(handDistance - oldHandDistance) > 0.2) {
                            oldHandDistance = handDistance;
                            if (handDistance < 1)
                            {
                                //cursor.transform.localScale = cursor.transform.localScale - new Vector3(handDistance / 100, handDistance / 100, handDistance / 100);
                            }
                        }
                        pointingAt = hit.collider.gameObject;
                        cursor.transform.position = hit.point;
                    }
                }


                WebXRController controller = gameObject.GetComponent<WebXRController>();

                float normalizedTime = controller.GetButton("Trigger") ? 1 : controller.GetAxis("Grip");

                //On Trigger Pressed
                if (controller.GetButtonDown("Trigger") || controller.GetButtonDown("Grip"))
                {
                    Debug.Log(gameObject.name + ": Trigger Down");
                    try { Pickup(); } catch (Exception e) { Debug.Log("Error in Pickup detected: \n" + e); }         //If NUll, throw Exception
                    try { callFunction(); } catch (Exception e) { Debug.Log("Error in Callfunction detected: \n"+e); }   //If Null, throw Exception
                }

                //On Trigger Released
                if (controller.GetButtonUp("Trigger") || controller.GetButtonUp("Grip"))
                {
                    Debug.Log(gameObject.name + ": Trigger Up");
                    try { Drop(); } catch (Exception e) { Debug.Log(e); }
                }

                if (controller.GetButtonDown("1"))
                    Debug.Log("Button 1 pressed");

                if (controller.GetButtonDown("2"))
                    Debug.Log("Button 2 pressed");

                if (controller.GetButtonDown("3"))
                    Debug.Log("Button 3 pressed");

                if (controller.GetButtonDown("4"))
                    Debug.Log("Button 4 pressed");

                if (controller.GetButtonDown("5"))
                    Debug.Log("Button 5 pressed");

                if (controller.GetButtonDown("6"))
                    Debug.Log("Button 6 pressed");

                if (controller.GetButtonDown("7"))
                    Debug.Log("Button 7 pressed");

                if (controller.GetButtonDown("8"))
                    Debug.Log("Button 8 pressed");

                if (controller.GetButtonDown("9"))
                    Debug.Log("Button 9 pressed");


                // Use the controller button or axis position to manipulate the playback time for hand model.
                if (anim != null)
                    anim.Play("Take", -1, normalizedTime);
            }
            else
            {
            }
        }

        //-----Primary Functions--------\\

        public void Pickup()
        {
            int id = GetNearestRigidBody().GetComponent<PhotonView>().ViewID;
            transform.GetChild(0).gameObject.GetComponent<PhotonView>().RPC("pickup", RpcTarget.All, id);
        }

        public void Drop()
        {
            transform.GetChild(0).gameObject.GetComponent<PhotonView>().RPC("drop", RpcTarget.All);
        }

        public void callFunction()
        {
            try { currentRigidBody = GetNearestRigidBody(); Debug.Log("Function Call has been fired on: " + currentRigidBody.name); } catch (Exception e) { Debug.Log("no rigidbody detected;"); }

            if (currentRigidBody)
            {
                Debug.Log("current Rigidbody found");
                try
                {
                    currentRigidBody.GetComponent<PhotonView>().RPC("SpawnObject", RpcTarget.All);
                    Debug.Log("Spawn RPC has been called for: " + currentRigidBody.name);
                }
                catch (Exception e)
                {
                    Debug.Log(currentRigidBody.name + " does not seem to be a Spawner");
                }
                try
                {
                    currentRigidBody.GetComponent<LeaveRoom>().leaveRoom();
                    Debug.Log("Leave Room has been called for: " + currentRigidBody.name);
                }
                catch (Exception e)
                {
                    Debug.Log(currentRigidBody.name + " does not seem to be a leaver");
                }
            }
            else {
                Debug.Log("no current Rigidbody found, continuing with pointingAt object, which is: " + pointingAt.name);
                try { pointingAt.GetComponent<VRBtn>().Send(); } catch { Debug.LogWarning("could not call \"Send()\" for: " + pointingAt.name); }
            }
        }
        //-----------------------------\\

        //-----supporting Functions----\\

        void OnTriggerEnter(Collider other)
        {
            int i = 0;
            if (!other.gameObject.GetComponent<Rigidbody>())
            {
                return;
            }
            contactRigidBodies.Add(other.gameObject.GetComponent<Rigidbody>());
            foreach (Rigidbody crb in contactRigidBodies)
            {
                Debug.LogWarning("contactRigidbody[" + i + "] : " + crb.name);
                i++;
            }
        }

        void OnTriggerExit(Collider other)
        {
            int i = 0;
            if (!other.gameObject.GetComponent<Rigidbody>())
            {
                return;
            }
            contactRigidBodies.Remove(other.gameObject.GetComponent<Rigidbody>());
            foreach (Rigidbody crb in contactRigidBodies)
            {
                Debug.LogWarning("contactRigidbody[" + i + "] : " + crb.name);
                i++;
            }
            Debug.Log("Nearest Rigidbody: " + GetNearestRigidBody().name);
        }

        private Rigidbody GetNearestRigidBody()
        {
            Rigidbody nearestRigidBody = null;
            float minDistance = float.MaxValue;
            float distance = 0.0f;
            foreach (Rigidbody contactBody in contactRigidBodies)
            {
                distance = (contactBody.gameObject.transform.position - transform.position).sqrMagnitude;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestRigidBody = contactBody;
                }
            }
            return nearestRigidBody;
        }

        IEnumerator waitAndPickup()
        {
            yield return new WaitUntil(() => currentRigidBody.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer);
            currentRigidBody.MovePosition(transform.position);
            attachJoint.connectedBody = currentRigidBody;
        }

        //----------------------------\\
    }
}
