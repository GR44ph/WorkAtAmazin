using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class conveyorBelt : MonoBehaviour
{
    Rigidbody rBody;
    List<GameObject> collidedObjects = new List<GameObject>();
    void Start() {
        rBody = GetComponent<Rigidbody>();
    }
    void FixedUpdate() {
        Vector3 pos = rBody.position;
        rBody.position += Vector3.right * 1 * Time.fixedDeltaTime;
        rBody.MovePosition(pos);
    }
}
