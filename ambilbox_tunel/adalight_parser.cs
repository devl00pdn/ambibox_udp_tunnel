
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
using System.Text;
using System.Collections.Generic;


namespace ambibox_tunnel_dev
{

    public struct led_strip_state
    {
        public UInt16 magic;
        public Int16 count_of_leds;
        public byte[] leds_arr;
    }


    class Adalight_parser
    {
        public Adalight_parser()
        {
            state = parser_state.wait_header;
            raw_data_iterrator = 0;
        }
        ~Adalight_parser()
        {

        }


        // return true if soution exist
        public bool parsing_msg(string serial_data, ref led_strip_state solution)
        {
            if (serial_data.Length <= ada_magic_header.Length)
            {
                return false;
            }

            raw_data = Encoding.ASCII.GetBytes(serial_data);

            bool is_solution_ready = false;

            if(state == parser_state.wait_header)
            {
                header_handler();
            }
            if(state == parser_state.getting_data)
            {
                is_solution_ready = data_handler();
            }

            if (is_solution_ready)
            {
                solution = led_strip_data;
                reset();
            }

            return is_solution_ready;
        }


        public static byte[] serialize(led_strip_state packet)
        {
            List<byte> serializer = new List<byte>();
            byte[] magic_raw = BitConverter.GetBytes(packet.magic);
            byte[] count_of_leds_raw = BitConverter.GetBytes(packet.count_of_leds);
            foreach(byte b in magic_raw)
            {
                serializer.Add(b);
            }
            foreach(byte b in count_of_leds_raw)
            {
                serializer.Add(b);
            }


            foreach (byte b in packet.leds_arr)
            {
                serializer.Add(b);
            }
            return serializer.ToArray();
        }

        private enum parser_state
        {
            wait_header,
            getting_data
        }

        private void reset()
        {
            state = parser_state.wait_header;
            raw_data_iterrator = 0;
            leds_count_remining = 0;
            leds_raw_data.Clear();
        }


        private bool header_handler()
        {
            for (int i = 0; i < raw_data.Length - ada_magic_header.Length; ++i)
            {
                byte[] tmp_arr = { raw_data[i], raw_data[i + 1], raw_data[i + 2] };
                string tmp_str = Encoding.ASCII.GetString(tmp_arr);
                if (tmp_str == "Ada")
                {
                    // magic has been catched
                    // move iterator to the next byte afret magic
                    raw_data_iterrator = i + ada_magic_header.Length;
                    break;
                }
            }
            if (raw_data.Length - raw_data_iterrator < 3)
            {
                //
                reset();
                return false;
            }

            //get count of leds and checksum
            Int16 hi = raw_data[raw_data_iterrator++];
            Int16 lo = raw_data[raw_data_iterrator++];
            Int16 chk = raw_data[raw_data_iterrator++];
            if (chk != (hi ^ lo ^ 0x55))
            {
                // invalid checksum
                reset();
                return false;
            }

            leds_count_remining = Convert.ToInt16((256 * hi) + lo + 1);

            // hardcode value borders
            if(leds_count_remining <= 0 || leds_count_remining > 100)
            {
                reset();
                return false;
            }

            state = parser_state.getting_data;

            return true;
        }

        private bool data_handler()
        {
            bool got_all_bytes = false;
            int bytes_remining = leds_count_remining * 3; //1 led contains 3 bytes (RGB)

            do
            {
                if (bytes_remining <= 0)
                {
                    got_all_bytes = true;
                    break;
                }

                if (raw_data.Length - 1 - raw_data_iterrator >= 0)
                { 
                    leds_raw_data.Add(raw_data[raw_data_iterrator++]);
                    bytes_remining--;
                }
                else
                {
                    break;
                }

            } while (true);

            if (got_all_bytes)
            {
                led_strip_data.magic = 0xADAF; // magic byte
                led_strip_data.count_of_leds = leds_count_remining;
                led_strip_data.leds_arr = leds_raw_data.ToArray();
            }
            return got_all_bytes;
        }

        private byte[] raw_data;
        private List<byte> leds_raw_data = new List<byte>();
        private parser_state state;
        private int raw_data_iterrator;
        private Int16 leds_count_remining;
        private string ada_magic_header = "Ada";
        private led_strip_state led_strip_data;
    }
}
