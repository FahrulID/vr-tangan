using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Threading;

public class WitMotionSerialController : MonoBehaviour
{
    private AnimController animController;
    private Transform player;
    SerialPort serialPort; //Set the port (com4) and the baud rate (9600, is standard on most devices)
    int bytesRead;

    double[] a = new double[4], w = new double[4], h = new double[4], Angle = new double[4], Port = new double[4];
    double Temperature, Pressure, Altitude,  GroundVelocity, GPSYaw, GPSHeight;
    long Longitude, Latitude;
    private double[] LastTime = new double[10];
    short sRightPack = 0;
    short [] ChipTime = new short[7];
    private DateTime TimeStart = DateTime.Now;

    byte[] RxBuffer = new byte[1000];
    int usRxLength = 0;

    void Start()
    {
        // this.loadPort("COM6", 115200);
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    public void loadPort(string port, int baudRate) // 115200
    {
        Debug.Log(port);
        // Set up the serial port with the correct settings for the WT901
        try 
        {
            if(port == null)
                return;
                 
            if(serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }

            serialPort = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);
            animController = GetComponent<AnimController>();
            serialPort.ReadTimeout = 1000;
            serialPort.Open(); //Open the Serial Stream.

            // serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            Thread readThread = new Thread(new ThreadStart(ReadDataThread));
            readThread.Start();
        }
        catch (Exception e)
        {
            //  Block of code to handle errors
            Debug.Log("Error: " + e.Message);
        }
    }

    delegate void UpdateData(byte[] byteData);
    void ReadDataThread()
    {
        while (serialPort.IsOpen)
        {
            try
            {
                byte[] byteTemp = new byte[1000];
                UInt16 usLength=0;
                usLength = (UInt16)serialPort.Read(RxBuffer, usRxLength, 700);
                usRxLength += usLength;

                while (usRxLength >= 11)
                {
                    UpdateData updateDelegate = DecodeData;
                    RxBuffer.CopyTo(byteTemp, 0);
                    if (!((byteTemp[0] == 0x55) & ((byteTemp[1] & 0x50)==0x50)))
                    {
                        for (int i = 1; i < usRxLength; i++) RxBuffer[i - 1] = RxBuffer[i];
                        usRxLength--;
                        continue;
                    }
                    if (((byteTemp[0]+byteTemp[1]+byteTemp[2]+byteTemp[3]+byteTemp[4]+byteTemp[5]+byteTemp[6]+byteTemp[7]+byteTemp[8]+byteTemp[9])&0xff)==byteTemp[10])
                        updateDelegate(byteTemp);
                    for (int i = 11; i < usRxLength; i++) RxBuffer[i - 11] = RxBuffer[i];
                    usRxLength -= 11;
                }
            }
            catch (Exception e)
            {
                // Handle timeout exception
                Debug.Log("Error: " + e.Message);
            }
        }
    }

    private void DecodeData(byte[] byteTemp)
    {
        double[] Data = new double[4];
        double TimeElapse = (DateTime.Now - TimeStart).TotalMilliseconds / 1000;

        Data[0] = BitConverter.ToInt16(byteTemp, 2);
        Data[1] = BitConverter.ToInt16(byteTemp, 4);
        Data[2] = BitConverter.ToInt16(byteTemp, 6);
        Data[3] = BitConverter.ToInt16(byteTemp, 8);
        sRightPack++;

        switch (byteTemp[1])
        {
            case 0x50:
                ChipTime[0] = (short)(2000 + byteTemp[2]);
                ChipTime[1] = byteTemp[3];
                ChipTime[2] = byteTemp[4];
                ChipTime[3] = byteTemp[5];
                ChipTime[4] = byteTemp[6];
                ChipTime[5] = byteTemp[7];
                ChipTime[6] = BitConverter.ToInt16(byteTemp, 8);
                break;

            case 0x51:
                Temperature = Data[3] / 100.0;
                Data[0] = Data[0] / 32768.0 * 16;
                Data[1] = Data[1] / 32768.0 * 16;
                Data[2] = Data[2] / 32768.0 * 16;
                a[0] = Data[0];
                a[1] = Data[1];
                a[2] = Data[2];
                a[3] = Data[3];

                if ((TimeElapse - LastTime[1]) < 0.1) return;
                LastTime[1] = TimeElapse;
                break;

            case 0x52:
                Temperature = Data[3] / 100.0;
                Data[0] = Data[0] / 32768.0 * 2000;
                Data[1] = Data[1] / 32768.0 * 2000;
                Data[2] = Data[2] / 32768.0 * 2000;
                w[0] = Data[0];
                w[1] = Data[1];
                w[2] = Data[2];
                w[3] = Data[3];

                if ((TimeElapse - LastTime[2]) < 0.1) return;
                LastTime[2] = TimeElapse;
                break;

            case 0x53:
                Temperature = Data[3] / 100.0;
                Data[0] = Data[0] / 32768.0 * 180;
                Data[1] = Data[1] / 32768.0 * 180;
                Data[2] = Data[2] / 32768.0 * 180;
                Angle[0] = Data[0];
                Angle[1] = Data[1];
                Angle[2] = Data[2];
                Angle[3] = Data[3];

                if ((TimeElapse - LastTime[3]) < 0.1) return;
                LastTime[3] = TimeElapse;
                break;

            case 0x54:
                //Data[3] = Data[3] / 32768 * double.Parse(textBox9.Text) + double.Parse(textBox8.Text);
                Temperature = Data[3] / 100.0;
                h[0] = Data[0];
                h[1] = Data[1];
                h[2] = Data[2];
                h[3] = Data[3];
                if ((TimeElapse - LastTime[4]) < 0.1) return;
                LastTime[4] = TimeElapse;
                break;
            case 0x55:
                Port[0] = Data[0];
                Port[1] = Data[1];
                Port[2] = Data[2];
                Port[3] = Data[3];
        
                break;

            case 0x56:
                Pressure = BitConverter.ToInt32(byteTemp, 2);
                Altitude = (double)BitConverter.ToInt32(byteTemp, 6) / 100.0;

                break;

            case 0x57:
                Longitude = BitConverter.ToInt32(byteTemp, 2);
                Latitude  = BitConverter.ToInt32(byteTemp, 6);

                break;

            case 0x58:
                GPSHeight = (double)BitConverter.ToInt16(byteTemp, 2) / 10.0;
                GPSYaw = (double)BitConverter.ToInt16(byteTemp, 4) / 10.0;
                GroundVelocity = BitConverter.ToInt16(byteTemp, 6)/1e3;

                break;
            default:
                Debug.Log("Error");
                break;
        }   
    }      

    public float rollSensitivity = 1000f; // Sensitivity of roll control
    public float pitchSensitivity = 5000f; // Sensitivity of pitch control
    public float yawSensitivity = 1000f; // Sensitivity of yaw control
    public float rotationSpeed = 100.0f;
    
    // initialized hand 
    public GameObject hand;

    void Update()
    {
        // Debug log a, w , h , angle
        // Debug.Log("a: " + a[0] + " " + a[1] + " " + a[2] + " " + a[3]);
        // Debug.Log("w: " + w[0] + " " + w[1] + " " + w[2] + " " + w[3]);
        // Debug.Log("h: " + h[0] + " " + h[1] + " " + h[2] + " " + h[3]);
        // Debug.Log("Angle: " + Angle[0] + " " + Angle[1] + " " + Angle[2] + " " + Angle[3]);
        // Debug.Log("Port[3]: " + Port[3]);
        // // Debug Log buffer
        // Debug.Log("Buffer: " + BitConverter.ToString(buffer));

        // Map roll, pitch, and yaw values to object's orientation
        if(serialPort != null)
        {
            Quaternion rotation = Quaternion.Euler((float)-Angle[0] * pitchSensitivity, (float)-Angle[2] * yawSensitivity, 0);

            // Update object's orientation
            hand.transform.rotation = Quaternion.Lerp(hand.transform.rotation, rotation, Time.deltaTime * rotationSpeed);

            // Rotate transform player on y axis
            // player.transform.rotation = Quaternion.Lerp(hand.transform.rotation, rotationYAxis, Time.deltaTime * rotationSpeed);
        }
    }

    
    void OnDestroy()
    {
        if(serialPort != null)
            // Close the serial port when the script is destroyed
            serialPort.Close();
    }
}
