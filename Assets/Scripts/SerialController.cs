using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Threading;

public class SerialController : MonoBehaviour
{
    private AnimController animController;
    SerialPort serialPort; //Set the port (com4) and the baud rate (9600, is standard on most devices)
    public float armState = 0;

    // Start is called before the first frame update
    void Start()
    {
        // this.loadPort("COM4", 115200);
        animController = GetComponent<AnimController>();
    }

    public void loadPort(string port, int baudRate)
    {
        Debug.Log(port);
        try 
        {
            if(port == null)
                return;

            if(serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }

            serialPort = new SerialPort(port, baudRate);
            serialPort.ReadTimeout = 1000;
            serialPort.Open(); //Open the Serial Stream.

            Thread readThread = new Thread(new ThreadStart(ReadDataThread));
            readThread.Start();
        }
        catch (Exception e)
        {
            //  Block of code to handle errors
            Debug.Log("Error: " + e.Message);
        }
    }

    void ReadDataThread()
    {
        while (serialPort.IsOpen)
        {
            try
            {
                if (serialPort.BytesToRead > 0)
                {
                    string packet = serialPort.ReadLine();
                    Debug.Log(packet);
                    string[] packetSplit = packet.Split(',');

                    armState = float.Parse(packetSplit[0]);
                }
            }
            catch (Exception e)
            {
                // Handle timeout exception
                Debug.Log("Error: " + e.Message);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // mouse click change armstate
        if (Input.GetMouseButtonDown(0))
        {
            if (armState == 1)
            {
                armState = 0;
            } else {
                armState = 1;
            }
            
            if(armState == 1)
            {
                animController.openHand();
            } else {
                animController.closeHand();
            }
        }

        if (serialPort != null)
        {
            
            if(serialPort.IsOpen)
            {
                if(armState == 1)
                {
                    animController.openHand();
                } else {
                    animController.closeHand();
                }
            }
        }

    }

    void OnDestroy()
    {
        if(serialPort != null)
            // Close the serial port when the script is destroyed
            serialPort.Close();
    }

    public void setMVC()
    {
        if(serialPort != null)
        {
            if(serialPort.IsOpen)
            {
                serialPort.WriteLine("4,1");
            }
        }
    }

    public void setOffset(float offset)
    {
        if(serialPort != null)
        {
            if(serialPort.IsOpen)
            {
                serialPort.WriteLine("2," + offset * 1000);
            }
        }
    }
}
