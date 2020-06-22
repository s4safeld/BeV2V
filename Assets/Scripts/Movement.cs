using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class Movement : MonoBehaviour
{

    private CharacterController myCC;

    private Movement pm;

    Transform camTransform;

    public float movementSpeed = GlobalInformation.movementSpeed;
    public float rotationSpeed = GlobalInformation.rotationSpeed;
    private bool initialised = false;

    public GameObject crosshair;

    private void Start()
    {
        camTransform = FindObjectsOfType<GameObject>().First(obj => obj.name == "Cameras").transform;
        pm = this;
        myCC = GetComponent<CharacterController>();
        crosshair.SetActive(false);
    }
    
    public void Initialise()
    {
        Debug.Log("Initialise Function call received");
        initialised = true;
        if ((transform.position.y < 0.5f && transform.position.y > -0.5f))
        {
            transform.SetPositionAndRotation(new Vector3(transform.position.x, GlobalInformation.height ,transform.position.z), transform.rotation);
        }
        //PV = FindObjectsOfType<PhotonView>().First();
        //Debug.Log(PV.ToString());
        //PV = GetComponent<PhotonView>();
        //myCC = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (initialised && !GlobalInformation.vrReady)
        {
            //Debug.Log("Movement vr ready: "+GlobalInformation.vrReady + "\nNon Vr Movement Script Enabled...");
            BasicMovement();
            BasicRotation();
            BasicSelection();
        }
    }

    void BasicMovement()
    {

        if (Input.GetKey(KeyCode.W))
        {
            myCC.Move(transform.forward * Time.deltaTime * movementSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            myCC.Move(-transform.right * Time.deltaTime * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            myCC.Move(-transform.forward * Time.deltaTime * movementSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            myCC.Move(transform.right * Time.deltaTime * movementSpeed);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            myCC.Move(transform.up * Time.deltaTime * movementSpeed);
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            myCC.Move(-transform.up * Time.deltaTime * movementSpeed);
        }
    }

    void BasicRotation()
    {
        if (camTransform.localRotation.x < 0.7 && camTransform.localRotation.x > -0.7)
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * rotationSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * rotationSpeed;
                transform.Rotate(new Vector3(0, mouseX, 0));
                camTransform.Rotate(new Vector3(Mathf.Clamp(-mouseY, -0.7f, 0.7f), 0, 0));

            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                camTransform.Rotate(new Vector3(Mathf.Clamp(-1, -0.7f, 0.7f), 0, 0));
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(new Vector3(0, Mathf.Clamp(-1, -10000, 10000), 0));
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                camTransform.Rotate(new Vector3(Mathf.Clamp(1, -0.7f, 0.7f), 0, 0));
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(new Vector3(0, Mathf.Clamp(1, -10000, 10000), 0));
            }

        }
        else
        {
            //Debug.Log("Camera rotation x = " + camTransform.rotation.x);
            if (camTransform.localRotation.x > 0.7)
            {
                //camTransform.Rotate(new Vector3(0.1f,0,0));
                camTransform.SetPositionAndRotation(camTransform.position, new Quaternion(0.69f, camTransform.rotation.y, camTransform.rotation.z, camTransform.rotation.w));
            }
            else
            {
                //camTransform.Rotate(new Vector3(-0.1f, 0, 0));
                camTransform.SetPositionAndRotation(camTransform.position, new Quaternion(-0.69f, camTransform.rotation.y, camTransform.rotation.z, camTransform.rotation.w));
            }

        }


    }

    void BasicSelection() {
        crosshair.SetActive(true);
        RaycastHit hit;

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Physics.Raycast(crosshair.transform.position, crosshair.transform.rotation * Vector3.forward, out hit))
            {
                if (hit.collider.tag == "Spawner")
                {
                    hit.collider.GetComponent<PhotonView>().RPC("SpawnObject", RpcTarget.All);
                }
                if (hit.collider.tag == "spawnable")
                {
                    Rigidbody currentRigidBody = hit.collider.GetComponent<Rigidbody>();
                    Debug.Log("Requesting Ownership for: " + currentRigidBody.name + " for Teleporting");
                    currentRigidBody.GetComponent<PhotonView>().RequestOwnership();
                    currentRigidBody.MovePosition(crosshair.transform.position);
                }
                if (hit.collider.tag == "Leaver")
                {
                    hit.collider.GetComponent<LeaveRoom>().leaveRoom();
                    Debug.Log("Leave Room has been called for: " + hit.collider.name);
                }
                Debug.Log("hit.point = " + hit.point + "\nhit.collider.name = " + hit.collider.name);
            }
            StartCoroutine("DisableScript");
        }
    }

    IEnumerator DisableScript()
    {
        this.enabled = false;

        yield return new WaitForSeconds(0.1f);

        this.enabled = true;
    }
}
