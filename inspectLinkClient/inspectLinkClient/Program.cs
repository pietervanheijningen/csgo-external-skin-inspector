using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text.RegularExpressions;

public class clnt
{
    
    public static void Main()
    {
        string[] args = Environment.GetCommandLineArgs();
        
        if (args.Length > 1)
        {
            string argumentinput = args[1];
            Match checkforinspectlink = Regex.Match(argumentinput, "csgo_econ_action_preview");

            if (checkforinspectlink.Success)
            {
                try
                {
                    argumentinput = argumentinput.Replace("steam://rungame/730/76561202255233023/+csgo_econ_action_preview%20", "");
                    TcpClient tcpclnt = new TcpClient();
                    Console.WriteLine(argumentinput);
                    //tcpclnt.Connect("192.168.178.35", 1337);
                    var result = tcpclnt.BeginConnect("192.168.178.35", 1337, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3.1));

                    if (!success)
                    {
                        throw new Exception("Took too long to connect to the server/it does not exist.");
                    }
                    Stream stm = tcpclnt.GetStream();

                    ASCIIEncoding asen = new ASCIIEncoding();

                    byte[] ba = asen.GetBytes(argumentinput);
                    stm.Write(ba, 0, ba.Length);

                    byte[] bb = new byte[80];
                    int k = stm.Read(bb, 0, 10);

                    //tcpclnt.Close();
                    tcpclnt.EndConnect(result);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error:\n\n" + e);
                    return;
                    throw;
                }
            }
            else
            {
                Process.Start("C:/Program Files (x86)/Steam/Steam.exe", argumentinput);
            }
        }
    }
}