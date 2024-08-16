// using UnityEngine;
// using System.Collections.Generic;
// using System;
// using System.Linq;
// using System.IO.Ports;
// using TMPro;
// using System.Text;
// using Unity.VisualScripting;
// using System.Collections;

// public class SerialManager : MonoBehaviour
// {
//     public EyesHandler eyes;
//     public InteractionHandler interaction;


//     public TMP_Dropdown PortsDropdown;

//     public TMP_Text ConnectionText;

//     private List<string> ports;
//     private SerialPort serial;
//     void Start()
//     {
//         RefreshPortsDropdown();
//     }

//     void FixedUpdate()
//     {
//         // Make sure we have a serial port and that the connection is open
//         if (serial != null && serial.IsOpen)
//         {
//             ReadSerial();
//         }
//         else
//         {
//             ConnectionText.text = "Not connected";
//         }
//     }

//     string dump = "";
//     void ReadSerial()
//     {
//         serial.DiscardInBuffer();
//         int tries = 0;
//         const int triesLimit = 10000;
//         bool done = false;
//         bool triedReading = false;
//         while (!done && tries < triesLimit)
//         {
//             if (serial.BytesToRead > 0)
//             {
//                 done = serial.ReadChar() == '$';
//                 triedReading = true;
//             }
//             ++tries;
//         }
//         if (tries >= triesLimit)
//         {
//             Debug.LogError("Sensor error. Tried reading: " + triedReading.ToString());
//             return;
//         }
//         string tempDump = serial.ReadLine();
//         if (tempDump != "")
//         {
//             if (tempDump.Contains("#"))
//             {
//                 tempDump = tempDump.Split("#")[0];

//                 if (tempDump == dump) return;
//                 dump = tempDump;

//                 string[] splittedDump = dump.Split(';');

//                 if (splittedDump.Length == 5)
//                 {
//                     ExecuteData(splittedDump);
//                 }
//             }
//         }
//     }

//     // void ReadSerial()
//     // {

//     // }

//     public void ExecuteData(string[] data)
//     {
//         int rotation = int.Parse(data[0]);
//         int zoom = int.Parse(data[1]);
//         bool interact1Pressed = int.Parse(data[2]) == 1 ? true : false;
//         bool interact2Pressed = int.Parse(data[3]) == 1 ? true : false;

//         DebugConsole.LogValues(rotation, interact1Pressed, interact2Pressed, zoom);


//         StartCoroutine(LerpEyes(rotation, zoom));

//         if (interact1Pressed)
//         {
//             interaction.Interact();
//         }

//     }

//     public IEnumerator LerpEyes(int rotation, int zoom)
//     {
//         int steps = 100;

//         for (int i = 0; i < steps; i++)
//         {
//             float t = i / (float)steps;

//             eyes.rotation = Mathf.Lerp(eyes.rotation, rotation, t);
//             eyes.zoom = (int)Mathf.Lerp(eyes.zoom, zoom, t);

//             yield return new WaitForSeconds(0.001f);
//         }
//     }


//     public void RefreshPortsDropdown()
//     {
//         // Remove all the previous options
//         PortsDropdown.ClearOptions();

//         // Get port names
//         string[] portNames = SerialPort.GetPortNames();
//         ports = portNames.ToList();

//         // Add the port names to our options
//         PortsDropdown.AddOptions(ports);
//     }

//     public void ConnectToPort()
//     {
//         if (ports.Count == 0)
//         {
//             ConnectionText.text = "Not Connected";
//             return;
//         }

//         // Get the port we want to connect to from the dropdown
//         string port = ports[PortsDropdown.value];

//         try
//         {
//             // Attempt to create our serial port using 9600 as our baud rate which matches the baud rate we set in the Arduino Sketch we created earlier.
//             serial = new SerialPort(port, 115200)
//             {
//                 Encoding = Encoding.UTF8,
//                 WriteTimeout = 300,
//                 ReadTimeout = 500000,
//                 DtrEnable = true,
//                 RtsEnable = true,
//                 NewLine = "\r\n"
//             };

//             // serial.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);

//             // Open up our serial connection
//             serial.Open();

//             ConnectionText.text = "Connected to " + port;
//         }
//         catch (Exception e)
//         {
//             Debug.LogError(e.Message);
//             ConnectionText.text = "Nicht verbunden";
//         }
//     }

//     private void port_DataReceived(object sender,
//                                  SerialDataReceivedEventArgs e)
//     {
//         // Show all the incoming data in the port's buffer in the output window
//         Debug.Log("data : " + serial.ReadExisting());
//     }

//     public void Disconnect()
//     {
//         if (serial != null)
//         {
//             // Close the connection if it is open
//             if (serial.IsOpen)
//             {
//                 serial.Close();
//             }

//             // Release any resources being used
//             serial.Dispose();
//             serial = null;

//             ConnectionText.text = "Not connected";
//         }
//     }

//     // Make sure to close our connection if the script is destroyed
//     void OnDestroy()
//     {
//         Disconnect();
//     }
// }