using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject player;
    private Vector3 playerOffset;
    void Start() {
        this.playerOffset = transform.position - player.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = this.playerOffset + new Vector3(this.player.transform.position.x, 0, this.player.transform.position.z);
    }
}
