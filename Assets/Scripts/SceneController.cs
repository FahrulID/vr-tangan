using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Start is called before the first frame update
    public static void restartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            restartScene();
        }
    }
}
