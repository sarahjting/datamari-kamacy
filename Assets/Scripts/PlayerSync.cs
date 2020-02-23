using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSync : MonoBehaviourPun
{
    public MonoBehaviour[] localScripts;
    public GameObject[] localObjects;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            for (int i = 0; i < localScripts.Length; i++)
            {
                localScripts[i].enabled = false;
            }
            for (int i = 0; i < localObjects.Length; i++)
            {
                localObjects[i].SetActive(false);
            }
        }

    }
}
