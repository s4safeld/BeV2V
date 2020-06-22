using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Photon.Pun;

public class Spawn : MonoBehaviourPunCallbacks
{
    private TextMeshPro spawnText;
    private bool spawned;
    public GameObject ots;          //ots -> ObjectToSpawn
    private Transform otsTransform;
    private Vector3 oldPos;
    private Vector3 spawnPos;
    private Quaternion oldRot;
    private Renderer rend;
    public Material spawnedMat;
    public Material notSpawnedMat;
    private PhotonView otsPV;

    public void Start()
    {
        otsPV = ots.GetComponent<PhotonView>();

        spawnText = gameObject.GetComponentInChildren<TextMeshPro>();
        spawnText.SetText(ots.name);

        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = notSpawnedMat;

        otsTransform = ots.transform;
        oldPos = otsTransform.position;
        oldRot = otsTransform.rotation;

        spawnPos = new Vector3(0, 1.25f, 0);

        spawned = false;

        Debug.Log(ots.name + " linked to " + gameObject.name);
    }

    private void Update()
    {
        if (Vector3.Distance(otsTransform.position, spawnPos) < 1) {
            spawned = true;
            rend.sharedMaterial = spawnedMat;
        }
        else
        {
            spawned = false;
            rend.sharedMaterial = notSpawnedMat;
        }
    }

    [PunRPC]
    public void SpawnObject()
    {
        Debug.Log("Spawn for " + ots.name + " fired");
        Debug.Log(ots.name + " is " + otsTransform.position);
        if (spawned == false)
        {
            Debug.Log("Spawning");
            Debug.Log("Requesting Ownership for: " + ots.name + "for Spawning");
            otsPV.RequestOwnership();
            ots.GetComponent<Rigidbody>().velocity = Vector3.zero;
            otsTransform.position = spawnPos;
            otsTransform.localRotation = oldRot;

            spawned = true;
            rend.sharedMaterial = spawnedMat;

            Debug.Log(ots.name + " is now at " + otsTransform.position);
        }
        else
        {
            Debug.Log("Despawning");
            Debug.Log("Requesting Ownership for: " + ots.name + "for Despawning");
            otsPV.RequestOwnership();
            ots.GetComponent<Rigidbody>().velocity = Vector3.zero;
            otsTransform.position = oldPos;
            otsTransform.localRotation = oldRot;

            spawned = false;
            rend.sharedMaterial = notSpawnedMat;

            Debug.Log(ots.name + " is now at " + otsTransform.position);
        }
        StartCoroutine("DisableScript");
    }

    //This is just disabling the Script for a short time, so that the function wont be called multiple times in a second
    IEnumerator DisableScript()
    {
        this.enabled = false;

        yield return new WaitForSeconds(0.1f);

        this.enabled = true;
    }

}
