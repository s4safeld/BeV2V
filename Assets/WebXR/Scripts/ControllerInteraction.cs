using System;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using System.Linq;
using System.Collections;

namespace WebXR
{
    public class ControllerInteraction : MonoBehaviour
    {
        private FixedJoint attachJoint = null;
        private Rigidbody currentRigidBody = null;
        private List<Rigidbody> contactRigidBodies = new List<Rigidbody>();
        private LineRenderer lr;
        private Transform xrRigTransform;
        private Animator anim;
        public GameObject cursor;
        private bool tpEnabled;

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
            
            xrRigTransform = FindObjectsOfType<GameObject>().First(obj => obj.name.Contains("WebXRCameraSet")).transform;
        }

        void Update()
        {
            if (GlobalInformation.vrReady)
            {
                WebXRController controller = gameObject.GetComponent<WebXRController>();

                float normalizedTime = controller.GetButton("Trigger") ? 1 : controller.GetAxis("Grip");

                //Move Cursor
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit))
                {
                    if (hit.collider.transform != cursor.transform && gameObject.GetComponent<WebXRController>().hand == WebXRControllerHand.LEFT)
                    {
                        cursor.transform.SetPositionAndRotation(hit.point, Quaternion.identity);
                    }
                }

                //On Trigger Pressed
                if (controller.GetButtonDown("Trigger") || controller.GetButtonDown("Grip"))
                {
                    Debug.Log(gameObject.name + ": Trigger Down");
                    
                    //If Left Controller then enable line Renderer
                    if(gameObject.GetComponent<WebXRController>().hand == WebXRControllerHand.LEFT)
                    {
                        lr.enabled = true;
                    }
                    //If Right Controller try to pickup or call Function
                    else
                    {
                        try { Pickup(); } catch (Exception e) { Debug.Log(e); }         //If NUll, throw Exception
                        try { callFunction(); } catch (Exception e) { Debug.Log(e); }   //If Null, throw Exception
                    }
                }

                //On Trigger Released
                if (controller.GetButtonUp("Trigger") || controller.GetButtonUp("Grip"))
                {
                    Debug.Log(gameObject.name + ": Trigger Up");
                    if (gameObject.GetComponent<WebXRController>().hand == WebXRControllerHand.LEFT)
                    {
                        lr.enabled = false;
                        try { teleport(); } catch (Exception e) { Debug.Log(e); }
                    }
                    else
                    {
                        try { Drop(); } catch (Exception e) { Debug.Log(e); }
                    }
                }

                // Use the controller button or axis position to manipulate the playback time for hand model.
                if (anim != null)
                    anim.Play("Take", -1, normalizedTime);
            }
        }

        //-----Primary Functions--------\\

        public void Pickup()
        {
            currentRigidBody = GetNearestRigidBody();

            if (!currentRigidBody)
            {
                Debug.Log("Pickup Call has been fired unsuccesfully");
                return;
            }
            else
            {
                Debug.Log("Pickup Call has been fired succesfully for: " + currentRigidBody.name);
            }
            
            Debug.Log("Requesting Ownership for: " + currentRigidBody.name);
            currentRigidBody.GetComponent<PhotonView>().RequestOwnership();
            StartCoroutine(waitAndPickup());   
        }

        public void Drop()
        {
            Debug.Log("Drop Call has been fired on: " + currentRigidBody.name);

            if (!currentRigidBody)
                return;

            attachJoint.connectedBody = null;
            currentRigidBody = null;
        }

        public void callFunction()
        {

            currentRigidBody = GetNearestRigidBody();
            Debug.Log("Function Call has been fired on: " + currentRigidBody.name);

            if (!currentRigidBody)
                return;

            if (currentRigidBody.tag == "Spawner")
            {
                currentRigidBody.GetComponent<PhotonView>().RPC("SpawnObject", RpcTarget.All);
                Debug.Log("Spawn RPC has been called for: " + currentRigidBody.name);
            }
            if (currentRigidBody.tag == "Leaver")
            {
                currentRigidBody.GetComponent<LeaveRoom>().leaveRoom();
                Debug.Log("Leave Room has been called for: " + currentRigidBody.name);
            }


        }

        public void teleport()
        {
            Debug.Log("Teleport Call has been fired");
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.rotation * Vector3.forward, out hit))
            {
                if (hit.collider.tag == "tpPlatform")
                {
                    Debug.Log("tpPlatform hit");
                    xrRigTransform.SetPositionAndRotation(new Vector3(hit.point.x, xrRigTransform.position.y, hit.point.z), xrRigTransform.rotation);
                }
                Debug.Log("hit.point = " + hit.point + "\nhit.name = " + hit.ToString() + "\nhit.collider.tag" + hit.collider.tag);
            }
        }

        //-----------------------------\\

        //-----supporting Functions----\\

        void OnTriggerEnter(Collider other)
        {
            int i = 0;
            if (!other.gameObject.GetComponent<Rigidbody>()) {
                return;
            }
            contactRigidBodies.Add(other.gameObject.GetComponent<Rigidbody>());
            foreach (Rigidbody crb in contactRigidBodies){
                Debug.LogWarning("contactRigidbody["+i+"] : "+crb.name);
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
            Debug.Log("Nearest Rigidbody: "+GetNearestRigidBody().name);
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
