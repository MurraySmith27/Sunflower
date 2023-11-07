using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollisionHandler : MonoBehaviour
{
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Crow")) {
            collision.gameObject.GetComponent<CrowControl>().OnHit();
        }
    }

    //HACK:
    void Update() {
        transform.position = transform.parent.position;
    }
}
