
/* --------------------------------------------------------------------
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU Lesser General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
* GNU Lesser General Public License for more details.
*
* You should have received a copy of the GNU Lesser General Public License
* along with this program.If not, see<http://www.gnu.org/licenses/>.
* --------------------------------------------------------------------
*/

using System;
using System.Net;
using System.Timers;
using System.IO.Ports;
using System.Net.Sockets;

namespace ambibox_tunel_dev
{
    class Adalight_tunel
    {
        private SerialPort serial;
        private bool is_started = false;
        private bool is_data_incomming = false;
        private UdpClient client = new UdpClient();
        private const int PORT_NUMBER = 40000;
        private IPEndPoint ip = new IPEndPoint(IPAddress.Parse("255.255.255.255"), PORT_NUMBER);
        private Adalight_parser parser = new Adalight_parser();
        private static System.Timers.Timer wdt_timer;
        
        public Adalight_tunel()
        {

        }

        ~Adalight_tunel()
        {
            if (serial.IsOpen)
            {
                serial.Close();
            }
            client.Close();
        }

        public void start()
        {
            if (is_started)
            {
                return;
            }

            // create serial port
            serial = new SerialPort("COM5",
                                    115200,
                                    Parity.None,
                                    8,
                                    StopBits.One);
            serial.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            serial.Open();

            if (!serial.IsOpen)
            {
                return;
            }
            // start wdt timer for adalight
            setup_timer();
            is_started = true;
        }
        public void stop()
        {
            if (!is_started)
            {
                return;
            }
            serial.Close();
            wdt_timer.Stop();
        }
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Show all the incoming data in the port's buffer
            string recv_data = serial.ReadExisting();
            adalight_parser(recv_data);
        }

        private bool adalight_parser(string msg)
        {
            bool parsin_result = false;
            var led_strip_data = new led_strip_state();
            parsin_result = parser.parsing_msg(msg, ref led_strip_data);
            if (parsin_result)
            {
                is_data_incomming = true;
                byte[] udp_package = Adalight_parser.serialize(led_strip_data);
                client.Send(udp_package, udp_package.Length, ip);

                //Console.WriteLine("Ada msg parsed, msg_len:{1}, led count: {0}", led_strip_data.count_of_leds, msg.Length);
                //for (int led_num = 0; led_num < led_strip_data.count_of_leds; led_num++)
                //{
                //    int firs_collor_of_led = (led_num * 3);
                //   Console.WriteLine("led{0} rgb: {1},{2},{3}", led_num, led_strip_data.leds_arr[firs_collor_of_led], led_strip_data.leds_arr[firs_collor_of_led + 1], led_strip_data.leds_arr[firs_collor_of_led + 2]);
                //}
            }
            return true;
        }

        private void setup_timer()
        {
            // Create a timer with a two second interval.
            wdt_timer = new System.Timers.Timer(1000);
            // Hook up the Elapsed event for the timer. 
            wdt_timer.Elapsed += OnTimedEvent;
            wdt_timer.AutoReset = true;
            wdt_timer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (!is_started)
            {
                return;
            }
            if (!is_data_incomming)
            {
                // if incoming data is stuck. Send signal to adalight 
                serial.Write("Ada\n");
            }
            is_data_incomming = false;
        }

    }
}
