using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

public class server
{
    public static void Main()
    {
        Console.SetWindowSize(70,62);
        //Console.SetWindowPosition(1,1);
        Task.Factory.StartNew(() => keepiteminfowindowup(true));
        Task.Factory.StartNew(() => keepWindowOpen());
        try
        {
            // Initializes/makes the listener
            TcpListener myList = new TcpListener(getipadress(), 1337);

            // start the listener at the specified port
            Console.WriteLine("Connect to the server via this ip: {0}", myList.LocalEndpoint);

            string inspectlink = "";
            
            while (true)
            {
                myList.Start();
                Console.WriteLine("Waiting for a connection.....");

                Socket s = myList.AcceptSocket();
                //Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

                
                byte[] b = new byte[150];
                
                int k = s.Receive(b);
                for (int i = 0; i < k; i++)
                {
                    inspectlink = inspectlink + Convert.ToChar(b[i]);
                }
                
                
                //inspectlink = Encoding.UTF8.GetString(b);
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(inspectlink);
                Console.ForegroundColor = ConsoleColor.Gray;
                writetolog(inspectlink);

                try
                {
                    Process.Start("steam://rungame/730/76561202255233023/+csgo_econ_action_preview%20" + inspectlink);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: {0}", e);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                ASCIIEncoding asen = new ASCIIEncoding();
                s.Send(asen.GetBytes("recieved, plz remove this message somehow"));
                //Console.WriteLine("\nSent Acknowledgement");
                Task.Factory.StartNew(() => keepiteminfowindowup(false));

                // close all the stuff
                inspectlink = "";
                s.Close();
                myList.Stop();
                
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error..... " + e);
        }
    }
    public static void keepiteminfowindowup(bool cursorwait)
    {
        
        while (true)
        {
            //Console.WriteLine("start of the function");
            
            if (cursorwait)
            {
                //Console.WriteLine("in if part of the function, sleeping now...");
                Win32.SetCursorPos(1520, 800);
                Thread.Sleep(60000);
                //Console.WriteLine("setting cursor pos and going back");
                Win32.SetCursorPos(1520, 900);
                Thread.Sleep(50);
            }
            else
            {
                //Console.WriteLine("in else part of the function");
                Win32.SetCursorPos(1520, 900);
                Thread.Sleep(2500);
                Win32.SetCursorPos(1520, 800);
                break;
            }
        }
        //Console.WriteLine("broke out of while loop in keepiteminfowindowup function");
        
    }
    public class Win32
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }
    }
    static IPAddress getipadress()
    {
        IPHostEntry host;
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip;
            }
        }
        return IPAddress.Parse("0.0.0.0");
    }
    static void writetolog(string input)
    {
        DateTime dateTime = DateTime.Now;
        StreamWriter file2 = new StreamWriter("C:/pcpie/inspectlinkserver/log.txt", true);
        file2.WriteLine(dateTime.ToString("dd-MM-yyyy\tHH:mm:ss\t") + input);
        file2.Close();
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
    static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

    static void keepWindowOpen()
    {

        string originalTitle = Console.Title;
        string uniqueTitle = Guid.NewGuid().ToString();
        Console.Title = uniqueTitle;
        Thread.Sleep(50);
        IntPtr handle = FindWindowByCaption(IntPtr.Zero, uniqueTitle);

        if (handle == IntPtr.Zero)
        {
            Console.WriteLine("Oops, cant find main window.");
            return;
        }
        Console.Title = originalTitle;

        while (true)
        {
            Thread.Sleep(1000);
            SetForegroundWindow(handle);
        }
    }
}   