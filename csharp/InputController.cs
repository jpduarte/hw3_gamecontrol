//using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class InputController
{
	public float CurrentValue;
	public string stringtoprint;
    public bool connection_status =false;//this is a flag to shut down the client in case we change of scene
    
    public void Begin(string ipAddress, int port)
	{
        
        // Give the network stuff its own special thread
        var thread = new Thread(() =>
			{
				// We'll use `LowPassFilter` to filter out some incorrect readings coming from the sensor
				//var filter = new LowPassFilter(0.95f);

				// This class makes it super easy to do network stuff
				var client = new TcpClient();

				// Change this to your devices real address
				client.Connect(ipAddress, port);
				var stream = new StreamReader(client.GetStream());
                
                if (client.Connected)
                {
                    connection_status = true;
                }

                // We'll read values and buffer them up in here
                //connection_status = 'listenning';
                var buffer = new List<byte>();
				while (client.Connected)
				{
					// Read the next byte
					var read = stream.Read();

					// We split readings with a carriage return, so check for it 
					if (read == 13)
					{
						// Once we have a reading, convert our buffer to a string, since the values are coming as strings
						var str = Encoding.ASCII.GetString(buffer.ToArray());
						stringtoprint = str;

						// Clear the buffer ready for another reading
						buffer.Clear();
					}
					else
						// If this wasn't the end of a reading, then just add this new byte to our buffer
						buffer.Add((byte)read);
                    //this is a flag to shut down the client in case we change of scene, flag is controlled in PlayerController.cs
                    if (!connection_status)
                    {
                        client.Close();
                    }

                }
			});

		thread.Start();
	}
}
