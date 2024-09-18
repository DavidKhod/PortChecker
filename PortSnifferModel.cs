using System.Net;
using System.Net.Sockets;

namespace PortSniffer
{
    public class PortSnifferModel
    {
        private string[] _addressArray;
        private int[] _ports;
        private OutPutModel _outputModel = new OutPutModel();
        private static readonly object _lock = new object();
        private const AddressFamily IPV4 = AddressFamily.InterNetwork;
        private const SocketType STREAM = SocketType.Stream;
        private const ProtocolType TCP = ProtocolType.Tcp;
        private int[] deafult_ports = new int[]
        {
            20,   // FTP Data Transfer
            21,   // FTP Command Control
            22,   // SSH
            23,   // Telnet
            25,   // SMTP
            53,   // DNS
            67,   // DHCP (Client to Server)
            68,   // DHCP (Server to Client)
            80,   // HTTP
            110,  // POP3
            123,  // NTP
            143,  // IMAP
            161,  // SNMP
            194,  // IRC
            443,  // HTTPS
            465,  // SMTPS
            587,  // SMTP (Alternate)
            993,  // IMAPS
            995,  // POP3S
            1433, // Microsoft SQL Server
            1521, // Oracle Database
            3306, // MySQL
            3389, // RDP
            5432, // PostgreSQL
            5900, // VNC
            8080  // HTTP Alternate
        };

        public PortSnifferModel(string[] addressArr, int[] ports)
        {
            this._addressArray = addressArr;
            this._ports = ports;
        }

        public PortSnifferModel(string[] addressArr)
        {
            this._addressArray = addressArr;
            this._ports = deafult_ports;
        }

        public ref OutPutModel OutputModel { get { return ref _outputModel; } }

        public void ChangePorts(int[] ports)
        {
            this._ports = ports;
        }

        public async Task CheckAll()
        {
            foreach (string address in _addressArray)
            {
                _outputModel.OutPut($"For address: {address}");
                await CheckPortsAsync(address);
                Console.WriteLine();
            }
        }

        private void TryConnction(IPEndPoint iPEndPoint, int port)
        {
            Socket checker = new Socket(IPV4, STREAM, TCP);
            try
            {
                //checker.ReceiveTimeout = 1000;
                checker.Connect(iPEndPoint);
                lock (_lock)
                {
                    _outputModel.OutPut($"Connected to port: {port}", ConsoleColor.Green);
                }
                checker.Close();
            }
            catch
            {
                lock (_lock)
                {
                    _outputModel.OutPut($"Failed to connect to port: {port}", ConsoleColor.Red);
                }
                checker.Close();
            }
        }

        private async Task CheckPortsAsync(string address)
        {
            //Socket checker = new Socket(_IPV4, _STREAM, _TCP);
            Queue<Thread> threads = new Queue<Thread>();
            foreach (int port in _ports)
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(address), port);
                Thread thread = new Thread(() => TryConnction(endPoint, port));
                thread.Start();
                threads.Enqueue(thread);
            }
            while (threads.Count != 0)
            {
                if (threads.Peek().ThreadState == ThreadState.Stopped)
                {
                    threads.Dequeue().Join();
                }
                else
                {
                    threads.Enqueue(threads.Dequeue());
                    Thread.Sleep(10);
                }
            }
        }


    }
}
