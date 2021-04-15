using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public float speed = 5f;
    public Transform cameraTf;

    void Update() {
        Vector3 pos = cameraTf.position;
        Vector2 npos = new Vector2(pos.x, pos.y);
        if (Input.GetKey(KeyCode.DownArrow )) npos.y -= speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.UpArrow   )) npos.y += speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow)) npos.x += speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow )) npos.x -= speed * Time.deltaTime;
        GetComponent<Rigidbody2D>().MovePosition(npos);
        cameraTf.position = new Vector3(transform.position.x, transform.position.y, cameraTf.position.z);
        //transform.position = new Vector3(pos.x, pos.y, 1);
    }
}
