using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawnable : MonoBehaviour
{
    public bool attached = true;
    public PhotonView attachedView;
    void Start()
    {
        attachedView = null;
    }
}
