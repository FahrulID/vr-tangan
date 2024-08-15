using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static bool isGameRunning = false;
    public static SerialController SerialController;
    public static WitMotionSerialController WitMotionSerialController;

    public static GameObject menu;

    public static void PauseGame ()
    {
        Time.timeScale = 0;
        isGameRunning = false;
    }

    public static void ResumeGame ()
    {
        Time.timeScale = 1;
        isGameRunning = true;
    }

    public static void StartGame()
    {
        SerialController.loadPort(SettingController.emgPort, 115200);
        WitMotionSerialController.loadPort(SettingController.imuPort, 115200);
        ResumeGame();
        menu.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        // pause
        PauseGame();

        // load SerialController and WitMotionSerialController
        SerialController = GetComponent<SerialController>();
        WitMotionSerialController = GetComponent<WitMotionSerialController>();

        // load menu
        menu = GameObject.Find("Menu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
