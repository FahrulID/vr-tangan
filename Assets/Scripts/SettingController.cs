using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.IO.Ports;
using TMPro;
using UnityEngine.UI;

using Microsoft.Win32;

public class SettingController : MonoBehaviour
{
    public static string imuPort = "COM0";
    public static string emgPort = "COM0";

    public static string[] availablePorts;

    // TMPro dropdowns
    public static TMP_Dropdown[] dropdowns;
    public static TextMeshProUGUI[] selectedTexts;
    // array of lists
    private static List<string>[] options = new List<string>[] { new List<string>() { "Loading..." }, new List<string>() { "Loading..." } };

    public static float movementSpeed = 10.0f;

    public static void setEmgPort(string port)
    {
        imuPort = port;
    }

    public static void setWitPort(string port)
    {
        emgPort = port;
    }

    public static void readPorts()
    {
        availablePorts = SerialPort.GetPortNames();

        foreach (List<string> option in options)
        {
            option.Clear();
        }

        foreach (string port in availablePorts)
        {
            options[0].Add(port);
            options[1].Add(port);
        }

        options[0].Add("None");
        options[1].Add("None");

        imuPort = options[0][0];
        emgPort = options[1][0];
    }

    public static void getDropdowns()
    {
        // get dropdowns with object name
        dropdowns = new TMP_Dropdown[] {
            GameObject.Find("IMU Dropdown").GetComponent<TMP_Dropdown>(),
            GameObject.Find("EMG Dropdown").GetComponent<TMP_Dropdown>()
        };
    }

    public static void loadDropdowns()
    {
        // set dropdowns with foreach
        int index = 0;
        foreach (TMP_Dropdown dropdown in dropdowns)
        {
            // Create a list of TMP_Dropdown.OptionData to hold the dropdown options
            var dropdownOptions = new System.Collections.Generic.List<TMP_Dropdown.OptionData>();

            // Populate the dropdownOptions list with the strings from the options array
            // foreach (string option in options)
            // {
            //     dropdownOptions.Add(new TMP_Dropdown.OptionData(option));
            // }

            // loop through option list
            foreach (string option in options[index])
            {
                dropdownOptions.Add(new TMP_Dropdown.OptionData(option));
            }

            // Add the dropdown options to the TMP_Dropdown component
            dropdown.AddOptions(dropdownOptions);
            
            // dropdown.onValueChanged.AddListener(
            //     delegate { 
            //         onDropdownValueChanged(dropdown.value, index);
            //     }
            // );
            dropdown.onValueChanged.AddListener(
                delegate { 
                    // onDropdownValueChanged(dropdown.value,
                    // delegate onDropdownValueChanged with arguments dropdown.value and index of dropdown without using index
                    onDropdownValueChanged(dropdown.value, Array.IndexOf(dropdowns, dropdown));
                }
            );

            index++;
        }
    }

    private static void onDropdownValueChanged(int portIndex, int dropdownIndex)
    {
        // Debug.Log("Dropdown changed to: " + port);
        // Debug.Log("Dropdown index: " + dropdownIndex);
        // Debug.Log("Dropdown text: " + dropdowns[dropdownIndex].options[dropdowns[dropdownIndex].value].text);

        Debug.Log("Dropdown changed to: " + portIndex + " " + dropdownIndex);

        string port = options[dropdownIndex][portIndex];

        // set port
        if (dropdownIndex == 0)
        {
            imuPort = (port == "None") ? null : port;
        }
        else if (dropdownIndex == 1)
        {
            emgPort = (port == "None") ? null : port;
        }

        Debug.Log("IMU Port: " + imuPort);
        Debug.Log("EMG Port: " + emgPort);
    }

    void Start() {
        readPorts();
        getDropdowns();
        loadDropdowns();
    }
}
