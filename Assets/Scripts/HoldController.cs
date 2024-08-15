using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldController : MonoBehaviour
{
    public SerialController serialController;
    public AnimController animController;

    public bool isHolding = false;
    public GameObject heldObject;
    public GameObject palmHand;

    public GameObject collidedObject;

    private bool isColliding = false;

    // Start is called before the first frame update

    public void Hold(GameObject obj)
    {
        isHolding = true;
        heldObject = obj;
    }

    public void Release()
    {
        isHolding = false;
        heldObject = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(isHolding)
        {
            if(animController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6)
            {
                heldObject.transform.position = palmHand.transform.position;
                heldObject.transform.rotation = palmHand.transform.rotation;
            }
        }

        // If collision detected and button pressed, hold the ball
        if (isColliding && !isHolding && serialController.armState == 0)
        {
            Hold(collidedObject);
            isHolding = true;
        } else if(isHolding && serialController.armState == 1)
        {
            Release();
            isHolding = false;
        }
    }

    void OnTriggerEnter(Collider col) {
        Debug.Log("Collided with " + col.gameObject.tag);
        if (col.gameObject.tag == "Ball" && !isHolding){
            collidedObject = col.gameObject;
            isColliding = true;
        }
    }
    
    void OnTriggerExit(Collider col){
        if (col.gameObject.tag == "Ball" && isHolding){
            collidedObject = null;
            isColliding = false;
        }
    }
}
