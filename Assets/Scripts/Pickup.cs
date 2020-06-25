using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Pickup : MonoBehaviourPunCallbacks
{
    private List<Rigidbody> contactRigidBodies = new List<Rigidbody>();
    private FixedJoint attachJoint;
    private Rigidbody currentRigidBody;

    private void Start()
    {
        attachJoint = GetComponent<FixedJoint>();
    }

    [PunRPC]
    public void pickup(int id)
    {
        currentRigidBody = PhotonView.Find(id).gameObject.GetComponent<Rigidbody>();

        if (!currentRigidBody)
        {
            Debug.Log("Pickup Call has been fired unsuccesfully");
            return;
        }
        else
        {
            Debug.Log("Pickup Call has been fired succesfully for: " + currentRigidBody.name);
        }

        currentRigidBody.MovePosition(transform.position);
        attachJoint.connectedBody = currentRigidBody;
    }

    [PunRPC]
    public void drop()
    {
        Debug.Log("Drop Call has been fired on: " + currentRigidBody.name);

        if (!currentRigidBody)
            return;

        attachJoint.connectedBody = null;
        currentRigidBody = null;
    }


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
}
