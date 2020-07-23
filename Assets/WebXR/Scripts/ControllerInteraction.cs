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
        private Transform rig;
        private Animator anim;
        public GameObject cursor;
        private bool tpEnabled;
        private bool pointingAtPlatform = false;
        private GameObject currPlat;
        public LayerMask layerMask;
        private bool teleportCalled;
        
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
            Debug.Log("Layermask: "+ layerMask);
        }

        void Update()
        {
            if (GlobalInformation.vrReady)
            {
                WebXRController controller = gameObject.GetComponent<WebXRController>();

                float normalizedTime = controller.GetButton("Trigger") ? 1 : controller.GetAxis("Grip");

                //Move Cursor
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit/*, layerMask*/))
                {
                    if (hit.collider.transform != cursor.transform && gameObject.GetComponent<WebXRController>().hand == WebXRControllerHand.LEFT)
                    {
                        cursor.transform.SetPositionAndRotation(hit.point, Quaternion.identity);
                        if (hit.collider.tag == "tpPlatform")
                        {
                            Debug.Log("Pointing at Platform");
                            pointingAtPlatform = true;
                            currPlat = hit.collider.gameObject;
                            currPlat.GetComponent<tpPlatform>().select();
                        }
                        else
                        {
                            if (pointingAtPlatform)
                            {
                                pointingAtPlatform = false;
                                try { currPlat.GetComponent<tpPlatform>().unselect(); } catch (Exception) { }
                            }
                        }
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
                        try { /*teleport();*/ teleportCalled = true; } catch (Exception e) { Debug.LogWarning(e); }
                    }
                    else
                    {
                        try { Drop(); } catch (Exception e) { Debug.Log(e); }
                    }
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
                if(cursor)
                    Destroy(cursor);
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
            Transform cam = FindObjectsOfType<GameObject>().First(obj => obj.name == "CameraR").transform;
            //float dist = Vector3.Distance(new Vector3(cam.position.x, 0, cam.position.z), new Vector3(rig.position.x, 0, rig.position.z));
            Vector3 dir = new Vector3(rig.position.x, 0, rig.position.z) - new Vector3(cam.position.x, 0, cam.position.z);
            if (pointingAtPlatform)
            {
                rig.position = new Vector3(cursor.transform.position.x, rig.position.y - cursor.transform.position.y, cursor.transform.position.z);
                rig.Translate(new Vector3(dir.x,0,dir.z));
            }
            else
            {
                Debug.Log("teleport(): currPlat = " + currPlat + "\nwhich must be equal to 'null'");
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
