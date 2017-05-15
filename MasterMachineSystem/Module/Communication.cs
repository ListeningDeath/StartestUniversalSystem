using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MasterMachineSystem.Module
{
    public enum CommType
    {
        COMM_TYPE_SERIAL_PORT = 0,
        COMM_TYPE_ETHERNET = 1,
        COMM_TYPE_USB
    }
    public enum EthernetType
    {
        ETHERNET_TYPE_SERVER = 0,
        ETHERNET_TYPE_CLIENT = 1
    }
    public abstract class Comm  // 通用通信接口
    {
        public CommType MyType { set; get; }  // 通信类型
        protected int ReceiveDataLength = 0;  // 接收数据长度
        protected Thread ReadThread;  // 接收线程
        public volatile bool KeepReading;  // 控制接收线程
        public delegate void EventHandle(byte[] readBuffer, int offset, int length);
        public abstract event EventHandle DataReceived;
        abstract public bool Open();  // 连接端口
        abstract public bool IsOpen();  // 判断是否打开
        abstract public void Close();  // 关闭端口
        abstract public bool Send(byte[] send, int offSet, int count);  // 发送消息
        abstract protected void Read();  // 接收线程
        public bool StartReading(int FrameLength)  // 开始监听，启动接收线程
        {
            ReceiveDataLength = FrameLength;
            if (!KeepReading)
            {
                KeepReading = true;
                ReadThread = new Thread(new ThreadStart(Read));
                ReadThread.Start();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool StopReading()  // 结束监听，关闭接收线程
        {
            if (KeepReading)
            {
                KeepReading = false;
                ReadThread.Join();
                ReadThread = null;
                return true;
            }
            else
            {
                return false;
            }
        }

    }
    public class SerialPortComm : Comm  // 串口通信类
    {
        public string _PortName { set; get; }
        public int _BaudRate { set; get; }
        public string _Parity { set; get; }
        public ushort _DataBits { set; get; }
        public string _StopBits { set; get; }
        public override event EventHandle DataReceived;
        public SerialPort _SerialPort;
        public SerialPortComm(string PortName, int BaudRate, string Parity, ushort DataBits, string StopBits)  // 初始化
        {
            MyType = CommType.COMM_TYPE_SERIAL_PORT;   // 通信类型

            _SerialPort = new SerialPort();
            _SerialPort.PortName = _PortName = PortName;
            _SerialPort.BaudRate = _BaudRate = BaudRate;
            _SerialPort.DataBits = _DataBits = DataBits;
            _Parity = Parity;
            _StopBits = StopBits;
            switch (Parity)
            {
                case "NONE":
                    _SerialPort.Parity = System.IO.Ports.Parity.None;
                    break;
                case "ODD":
                    _SerialPort.Parity = System.IO.Ports.Parity.Odd;
                    break;
                case "EVEN":
                    _SerialPort.Parity = System.IO.Ports.Parity.Even;
                    break;
                case "MARK":
                    _SerialPort.Parity = System.IO.Ports.Parity.Mark;
                    break;
                case "SPACE":
                    _SerialPort.Parity = System.IO.Ports.Parity.Space;
                    break;
                default:
                    _SerialPort.Parity = System.IO.Ports.Parity.None;
                    break;
            }
            switch (StopBits)
            {
                case "1":
                    _SerialPort.StopBits = System.IO.Ports.StopBits.One;
                    break;
                case "1.5":
                    _SerialPort.StopBits = System.IO.Ports.StopBits.OnePointFive;
                    break;
                case "2":
                    _SerialPort.StopBits = System.IO.Ports.StopBits.Two;
                    break;
                default:
                    _SerialPort.StopBits = System.IO.Ports.StopBits.None;
                    break;
            }
            _SerialPort.ReadBufferSize = 512;
            _SerialPort.ReadTimeout = 100;
            _SerialPort.WriteTimeout = -1;

            ReadThread = null;
            KeepReading = false;
        }

        public override bool IsOpen()  // 获取串口状态
        {
            return _SerialPort.IsOpen;
        }

        protected override void Read()  // 接收线程
        {
            while (KeepReading)
            {
                if (_SerialPort.IsOpen)
                {
                    int count = _SerialPort.BytesToRead;
                    if (count >= ReceiveDataLength || true)
                    {
                        byte[] readBuffer = new byte[count];
                        try
                        {
                            _SerialPort.Read(readBuffer, 0, count);
                            if (DataReceived != null)
                            {
                                DataReceived(readBuffer, 0, count);
                                readBuffer = null;
                            }
                            Thread.Sleep(100);
                        }
                        catch (TimeoutException)
                        {
                        }
                    }
                }
            }
        }

        public override bool Open()  // 连接串口
        {
            if (_SerialPort.IsOpen)
                return true;
            try
            {
                _SerialPort.Open();
            }
            catch
            {
                return false;
            }

            if (!_SerialPort.IsOpen)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Close()  // 关闭串口连接
        {
            StopReading();
            DataReceived = null;
            _SerialPort.Close();
        }

        public override bool Send(byte[] send, int offSet, int count)
        {
            if (_SerialPort.IsOpen)
            {
                _SerialPort.Write(send, offSet, count);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class EthernetComm : Comm  // 以太网通信类
    {
        public EthernetType _EthernetType { set; get; }
        public string _IPAddress { set; get; }
        public int _Port { set; get; }
        private IPAddress _IP;
        private IPEndPoint _IPE;
        private Socket _Socket, _ServerSocket;
        private Socket CurrentSocket() => _EthernetType == EthernetType.ETHERNET_TYPE_SERVER ? _ServerSocket : _Socket;
        private Thread _WaitThread;
        bool _IsConnected;
        public override event EventHandle DataReceived;
        public EthernetComm(string host, int port, EthernetType type)  // 初始化
        {

            _IPAddress = host;
            _Port = port;
            _EthernetType = type;

            _IP = IPAddress.Parse(host);
            _IPE = new IPEndPoint(_IP, port);
            _IsConnected = false;
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        private void Wait()  // 服务端等待连接
        {
            _ServerSocket = _Socket.Accept();
            _IsConnected = true;
            return;
        }
        public override bool Open()  // 连接端口
        {
            if (_EthernetType == EthernetType.ETHERNET_TYPE_SERVER)  // 服务端被动连接
            {
                try
                {
                    _Socket.Bind(_IPE);
                }
                catch
                {
                    return false;
                }
                _Socket.Listen(0);
                _WaitThread = new Thread(new ThreadStart(Wait));
                _WaitThread.Start();
            }
            else if (_EthernetType == EthernetType.ETHERNET_TYPE_CLIENT)  // 客户端主动连接
            {
                try
                {
                    _Socket.Connect(_IPE);
                }
                catch
                {
                    return false;
                }
                _IsConnected = true;
            }
            return true;
        }
        public override bool IsOpen()  // 判断是否打开
        {
            return _IsConnected;
        }
        public override void Close()  // 关闭端口
        {
            CurrentSocket().Close();
            _Socket.Close();
            _IsConnected = false;
        }
        public override bool Send(byte[] send, int offSet, int count)  // 发送消息
        {
            CurrentSocket().Send(send, send.Length, 0);
            return true;
        }
        protected override void Read()  // 接收线程
        {
            while (KeepReading)
            {
                if (_IsConnected)
                {
                    byte[] readBuffer = new byte[4096];
                    try
                    {
                        int count = CurrentSocket().Receive(readBuffer);
                        if (DataReceived != null)
                        {
                            DataReceived(readBuffer, 0, count);
                            readBuffer = null;
                        }
                        Thread.Sleep(100);
                    }
                    catch (TimeoutException)
                    {
                    }

                }
            }
        }
    }
    public class USB : Comm  // USB通信类
    {
        public override event EventHandle DataReceived;
        public override bool Open()  // 连接端口
        {
            return true;
        }
        public override bool IsOpen()  // 判断是否打开
        {
            return true;
        }
        public override void Close()  // 关闭端口
        {

        }
        public override bool Send(byte[] send, int offSet, int count)  // 发送消息
        {
            return true;
        }
        protected override void Read()  // 接收线程
        {

        }
    }
}
