using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        // load player
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.isGameRunning)
        {
            // move
            // float moveHorizontal = Input.GetAxis("Horizontal");
            // float moveVertical = Input.GetAxis("Vertical");
            // player.Translate(new Vector3(moveVertical, 0, moveHorizontal) * Time.deltaTime * 5, Space.World);

            // move with movement speed
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            player.Translate(new Vector3(moveVertical, 0, moveHorizontal) * Time.deltaTime * SettingController.movementSpeed, Space.World);

            // // rotate
            // float rotateHorizontal = Input.GetAxis("RotateHorizontal");
            // float rotateVertical = Input.GetAxis("RotateVertical");
            // transform.Rotate(new Vector3(rotateVertical, rotateHorizontal, 0) * Time.deltaTime * 50, Space.World);
        }    
    }
}
