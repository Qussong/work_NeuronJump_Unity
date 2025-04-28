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
    /// ��ġ������ Ȯ���Ͽ� �̹� ������� ��Ʈ�� UnusedPorts.txt ���Ͽ� �ۼ��Ѵ�.
    /// ex) COM1/COM2/COM3
    /// �ۼ��� ��Ʈ�� ������ �ٸ� ��Ʈ�� ���� �����͸� �޾ƿ´�.
    /// �þ� �ƹ� ��Ʈ��ȣ�� �Է����� �ʾҴٸ� ���� ó�� �߰ߵ� ��Ʈ�� ����ȴ�.
    ///  
    /// </summary>

    public class NetworkManager : SingletonTemplate<NetworkManager> //where T : MonoBehaviour
    {

        [Tooltip("���� ���� �������� Headr ��")]
        [SerializeField] private Byte receiveHeader;

        [Tooltip("���� �޴� �������� ���̸� �����ּ���.(Byte ����, Header ����)")]
        [SerializeField] private int reciveDataLength;

        [Tooltip("���� �� �������� Headr ���� �Է����ּ���.")]
        [SerializeField] private Byte sendHeader;

        [Tooltip("���� �� �������� ���̸� �����ּ���. (Byte ����, Header ����)")]
        [SerializeField] private int sendDataLength;

        [Tooltip("��� �ӵ�")]
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
                Debug.LogWarning("��Ʈ ���� ������ �������� �ʾ� ���� �����մϴ�.");
                File.WriteAllText(path, "");    // �� ���� ����
            }

            string Content = File.ReadAllText(path);
            // �о�� ���Ͽ� ��Ʈ�� ���� ������ �ԷµǾ��ִ� ���
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
                    Debug.Log("���� ����� Serial Port�� ã�� ���߽��ϴ�. ");
                    return false;
                }
            }
            else
            {
                Debug.LogWarning("��ǻ�Ϳ� ����� SerialPort�� ���ϳ��� �����ϴ�.");
                return false;
            }
        }

        void InitSerialPort()
        {
            try
            {
                serialPort = new SerialPort(targetPortName, communicationSpeed)
                {
                    DataBits = 8, //�ѹ��� �����ϴ� ��Ʈ ��(Default : 8)
                    Parity = Parity.None, //�и�Ƽ ���� ����� ������� ����
                    StopBits = StopBits.One, //������Ʈ(��Ʈ�� ���� �˸�), ������Ʈ�� 1��Ʈ ���
                    ReadTimeout = 500, //�б�ð�, 500ms ���� ������ ���ٸ� ���ܹ߻�
                    WriteTimeout = 500 //����ð�, 500ms ���� ������ ���ٸ� ���ܹ߻�
                };

                serialPort.Open();
                Debug.Log("Serial Port�� ���������� ���Ƚ��ϴ�.");
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
                        await Task.Delay(50); // �����͸� ��ٸ��� ���� CPU ��뷮 �ּ�ȭ
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error reading from serial port: {e.Message}");
                    break; // ���� �߻� �� ���� ����
                }
            }
        }

        void ReceiveData(byte[] data)
        {
            if (data == null || data.Length < reciveDataLength)
            {
                Debug.LogError("���ŵ� ������ ����(�����Ͱ� ª���ϴ�.)");
                return;
            }

            Array.Copy(data, saveDataArray, data.Length);

            if (saveDataArray[0] == receiveHeader)
            {
                //�����Ͱ� ���������� ���ŵ�.
                if (saveDataArray[1] == 0x41) 
                {
                    // ���� ���� �ۼ�
                    Debug.Log("Get Serial Data");
                }

                if (reciveData != null)
                {
                    reciveData(saveDataArray);
                }
            }
        }

        //�����͸� �߽��ؾ� �ϴ� ��� ���.
        public void SendData()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                if (sendData != null)
                {
                    //sendDataArray[0] = sendHeader;
                    //sendData(sendDataArray);
                }

                //���� �����͸� �����ϴ� �ڵ带 �߰��ϸ� �ȴ�.
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
