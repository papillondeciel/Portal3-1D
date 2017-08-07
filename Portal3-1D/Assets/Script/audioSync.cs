using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
//Klasa służąca do synchronizacji dźwięku miedzy graczami

[RequireComponent(typeof(AudioSource))]
public class audioSync : NetworkBehaviour {
    private AudioSource source;

    public AudioClip[] clips;
	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
	}

    public void playSound(int id) //funkcja odpowiadająca za 'odpalenie' danego klipu dźwięku
    {
        if (id >= 0 && id < clips.Length)
        {
            CmdSendServerSoundID(id);
        }
    }
    //funkcje konkretnie służące do synchronizacji
    [Command]
    void CmdSendServerSoundID(int id)
    {
        RpcSendSoundIDToClients(id);
    }

    [ClientRpc]
    void RpcSendSoundIDToClients(int id)
    {
        source.PlayOneShot(clips[id]);
    }
}
