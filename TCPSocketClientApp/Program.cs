using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SynchronousSocketClient
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public static void StartClient()
    {
        // Data buffer for incoming data.  
        byte[] bytes = new byte[1024];

        // Connect to a remote device.  
        try
        {
            // Establish the remote endpoint for the socket.  
            // This example uses port 11000 on the local computer.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP  socket.  
            Socket sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                sender.Connect(remoteEP);

                Console.WriteLine("Socket connected to {0}",
                    sender.RemoteEndPoint.ToString());

                Person dummy = new Person
                {
                    FirstName = "Joulu",
                    LastName = "Pukki",
                    Timestamp = DateTime.Now
                };
                Person dummy2 = new Person
                {
                    FirstName = "Kesä",
                    LastName = "Pukki",
                    Timestamp = DateTime.Now
                };
                List<Person> JSONlist = new List<Person>();
                JSONlist.Add(dummy);
                JSONlist.Add(dummy2);
                string JSONmessage = JsonConvert.SerializeObject(JSONlist);

                // Encode the data string into a byte array.  
                //byte[] msg = Encoding.Default.GetBytes("This is a test<EOF>");
                byte[] msg = Encoding.Default.GetBytes(JSONmessage + "<EOF>");

                // Send the data through the socket.  
                int bytesSent = sender.Send(msg);

                // Receive the response from the remote device.  
                int bytesRec = sender.Receive(bytes);
                string bytesEncoded = Encoding.Default.GetString(bytes, 0, bytesRec);

                Console.WriteLine($"Echoed test: {bytesEncoded}");

                List<Person> myList = JsonConvert.DeserializeObject<List<Person>>(bytesEncoded);

                foreach (Person iter in myList)
                {
                    Console.WriteLine(iter.FirstName + " " + iter.LastName + " " + iter.Timestamp);
                }
                //Console.WriteLine("Echoed test = {0}", myList);

                // Release the socket.  
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static int Main(String[] args)
    {
        StartClient();
        Console.WriteLine("<waiting a key>");
        Console.ReadKey();
        return 0;
    }
}