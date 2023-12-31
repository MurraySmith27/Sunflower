using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Transform player;
    private Vector3 playerOffset;
    private bool cameraStarted = false;

    private Vector3 initialPosition;
    void Awake() {
        this.initialPosition = transform.position;
    }
    public void StartCamera() {
        this.player = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        this.playerOffset = transform.position - player.position;
        this.cameraStarted = true;
        transform.position = this.initialPosition;
    }
    // Update is called once per frame
    void Update()
    {
        if (this.cameraStarted) {
            transform.position = this.playerOffset + new Vector3(this.player.position.x, 0, this.player.position.z);
        }
    }
}
