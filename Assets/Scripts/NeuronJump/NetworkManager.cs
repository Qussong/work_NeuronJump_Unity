using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace GH
{
    /// <summary>
    /// 
    /// 장치관리를 확인하여 이미 사용중인 포트를 UnusedPorts.txt 파일에 작성한다.
    /// ex) COM1/COM2/COM3
    /// 작성한 포트를 제외한 다른 포트료 부터 데이터를 받아온다.
    /// 먄약 아무 포트번호를 입력하지 않았다면 제일 처음 발견된 포트와 연결된다.
    ///  
    /// </summary>

    public class NetworkManager : SingletonTemplate<NetworkManager> //where T : MonoBehaviour
    {

        [Tooltip("전달 받을 데이터의 Headr 값")]
        [SerializeField] private Byte receiveHeader;

        [Tooltip("전달 받는 데이터의 길이를 적어주세요.(Byte 단위, Header 포함)")]
        [SerializeField] private int reciveDataLength;

        [Tooltip("전달 할 데이터의 Headr 값을 입력해주세요.")]
        [SerializeField] private Byte sendHeader;

        [Tooltip("전달 할 데이터의 길이를 적어주세요. (Byte 단위, Header 포함)")]
        [SerializeField] private int sendDataLength;

        [Tooltip("통신 속도")]
        [SerializeField] private int communicationSpeed;

        private SerialPort serialPort;
        private List<string> portNames;
        private string[] hasPortNames;
        private string targetPortName;
        private byte[] reciveDataArray;
        private byte[] saveDataArray;
        private byte[] sendDataArray;

        public DPacketDelegate sendData;
        public DPacketDelegate reciveData;


        protected override void Awake()
        {
            base.Awake();

            if (FindSerialPortName())
            {
                InitSerialPort();
            }

            receiveHeader = (byte)0XFA; // = 250
            sendHeader = (byte)0XFA;
            communicationSpeed = 9600;

            reciveDataArray = new byte[reciveDataLength];
            saveDataArray = new byte[reciveDataLength];
            sendDataArray = new byte[sendDataLength];
        }

        async void Start()
        {
            await ListeningSerialPort();
        }

        bool FindSerialPortName()
        {
            string path = Path.Combine(Application.persistentDataPath, "UnusedPorts.txt");

            /*if (File.Exists(path))
            {
                string Content = File.ReadAllText(path);

                if (!String.IsNullOrEmpty(Content))
                {
                    portNames = new List<string>();
                    SettingPortName(Content);
                }
            }*/

            if(false == File.Exists(path))
            {
                Debug.LogWarning("포트 설정 파일이 존재하지 않아 새로 생성합니다.");
                File.WriteAllText(path, "");    // 빈 파일 생성
            }

            string Content = File.ReadAllText(path);
            // 읽어온 파일에 포트에 대한 설정이 입력되어있는 경우
            if (false == String.IsNullOrEmpty(Content))
            {
                portNames = new List<string>();
                SettingPortName(Content);
            }

            hasPortNames = SerialPort.GetPortNames();

            if (hasPortNames.Length > 0)
            {
                string selectedPort = "";

                foreach (var port in hasPortNames)
                {
                    Debug.Log("Port Name : " + port);
                    bool IsFind = false;

                    for (int i = 0; i < portNames.Count; ++i)
                    {
                        if (portNames[i] != port)
                        {
                            selectedPort = port;
                            IsFind = true;
                            break;
                        }
                    }

                    if (IsFind)
                    {
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(selectedPort))
                {
                    targetPortName = selectedPort;
                    return true;
                }
                else
                {
                    Debug.Log("새로 연결된 Serial Port를 찾지 못했습니다. ");
                    return false;
                }
            }
            else
            {
                Debug.LogWarning("컴퓨터에 연결된 SerialPort가 단하나도 없습니다.");
                return false;
            }
        }

        void InitSerialPort()
        {
            try
            {
                serialPort = new SerialPort(targetPortName, communicationSpeed)
                {
                    DataBits = 8, //한번에 전송하는 비트 수(Default : 8)
                    Parity = Parity.None, //패리티 검출 방식을 사용하지 않음
                    StopBits = StopBits.One, //정지비트(비트의 끝을 알림), 정지비트는 1비트 사용
                    ReadTimeout = 500, //읽기시간, 500ms 동안 응답이 없다면 예외발생
                    WriteTimeout = 500 //쓰기시간, 500ms 동안 응답이 없다면 예외발생
                };

                serialPort.Open();
                Debug.Log("Serial Port가 정상적으로 열렸습니다.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Serial Port Open Error : {e.Message}");
            }
        }

        async Task ListeningSerialPort()
        {
            while (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    if (serialPort.BytesToRead >= reciveDataArray.Length)
                    {
                        await Task.Run(() => serialPort.Read(reciveDataArray, 0, reciveDataArray.Length));
                        ReceiveData(reciveDataArray);
                    }
                    else
                    {
                        await Task.Delay(50); // 데이터를 기다리는 동안 CPU 사용량 최소화
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error reading from serial port: {e.Message}");
                    break; // 오류 발생 시 루프 종료
                }
            }
        }

        void ReceiveData(byte[] data)
        {
            if (data == null || data.Length < reciveDataLength)
            {
                Debug.LogError("수신된 데이터 오류(데이터가 짧습니다.)");
                return;
            }

            Array.Copy(data, saveDataArray, data.Length);

            if (saveDataArray[0] == receiveHeader)
            {
                //데이터가 정상적으로 수신됨.
                if (saveDataArray[1] == 0x41) 
                {
                    // 동작 로직 작성
                    Debug.Log("Get Serial Data");
                }

                if (reciveData != null)
                {
                    reciveData(saveDataArray);
                }
            }
        }

        //데이터를 발신해야 하는 경우 사용.
        public void SendData()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                if (sendData != null)
                {
                    //sendDataArray[0] = sendHeader;
                    //sendData(sendDataArray);
                }

                //보낼 데이터를 설정하는 코드를 추가하면 된다.
                try
                {
                    serialPort.Write(sendDataArray, 0, sendDataArray.Length);
                    Debug.Log($"Sent {sendDataArray.Length} bytes: {BitConverter.ToString(sendDataArray)}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to send data: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning("Serial port is not open or not connected.");
            }
        }

        void SettingPortName(string ReadFile)
        {
            char[] charArray = ReadFile.ToCharArray();
            string Name = "";

            for (int i = 0; i < charArray.Length; ++i)
            {
                if (charArray[i] == '/')
                {
                    portNames.Add(Name);
                    Name = "";
                }
                else
                {
                    Name += charArray[i];
                }
            }

            if (!string.IsNullOrEmpty(Name))
            {
                portNames.Add(Name);
            }

        }

    }
}
