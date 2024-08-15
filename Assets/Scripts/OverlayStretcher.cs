using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
 
public class OverlayStretcher : MonoBehaviour {
    // Initialize
    private int screenWidth;
    private int screenHeight;
    private Vector2 screenSize;

    // Update is called once per frame
    private void LateUpdate() {
        // Update screen width and height
        transform.localScale = new Vector3(1, 1, 1);

        // float width = sr.sprite.bounds.size.x;
        // float height = sr.sprite.bounds.size.y;

        // width and height of image from Image component
        float width = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        float height = GetComponent<SpriteRenderer>().sprite.bounds.size.y;

        transform.localScale = new Vector2(Screen.width / width, Screen.height / height);
    }
}