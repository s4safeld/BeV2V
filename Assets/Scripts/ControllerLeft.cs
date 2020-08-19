using System;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using System.Linq;
using System.Collections;

namespace WebXR
{
    public class ControllerLeft : MonoBehaviour
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
            Debug.Log("Layermask: " + layerMask);
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
                    if (hit.collider.transform != cursor.transform)
                    {
                        cursor.transform.SetPositionAndRotation(hit.point, Quaternion.identity);
                        if (hit.collider.tag == "tpPlatform")
                        {
                            pointingAtPlatform = true;
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

                    lr.enabled = true;
                }

                //On Trigger Released
                if (controller.GetButtonUp("Trigger") || controller.GetButtonUp("Grip"))
                {
                   Debug.Log(gameObject.name + ": Trigger Up");
                   lr.enabled = false;
                   try { teleport();} catch (Exception e) { Debug.LogWarning(e); }
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
                if (cursor)
                    Destroy(cursor);
            }
        }

        //-----Primary Functions--------\\

        public void teleport()
        {
            Debug.Log("Teleport Call has been fired");
            Transform cam = GlobalInformation.XRSet.transform.Find("Cameras").Find("CameraR");
            //float dist = Vector3.Distance(new Vector3(cam.position.x, 0, cam.position.z), new Vector3(rig.position.x, 0, rig.position.z));
            Vector3 dir = new Vector3(rig.position.x, 0, rig.position.z) - new Vector3(cam.position.x, 0, cam.position.z);
            if (pointingAtPlatform)
            {
                rig.position = new Vector3(cursor.transform.position.x, rig.position.y, cursor.transform.position.z);
                rig.Translate(new Vector3(dir.x, 0, dir.z));
            }
            else
            {
                Debug.Log("teleport(): currPlat = " + currPlat + "\nwhich must be equal to 'null'");
            }
        }
        //-----------------------------\\

        //-----supporting Functions----\\
        //----------------------------\\
    }
}
