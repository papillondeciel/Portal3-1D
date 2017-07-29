using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class audioSync : NetworkBehaviour {
    private AudioSource source;

    public AudioClip[] clips;
	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
	}
	
	public void playSound(int id)
    {
        if(id >= 0 && id < clips.Length)
        {
            CmdSendServerSoundID(id);
        }
    }

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
