/*
 * GameWatch - Server Browser for online games
 * Copyright (C) 2003 Rodrigo Reyes <reyes@charabia.net>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA
 * 02111-1307, USA.
 *
 */
using System;
using System.IO;

namespace Apollo.Core.Services.Directory.IP2Country.Net
{
    public class IPToCountry : IIPToCountry
    {
        private BitVectorTrie m_trie = new BitVectorTrie();
        public static int NetworkCodeCount = 0;

        public void Load(string filename)
        {
            if (File.Exists(filename))
            {
                StreamReader nccin = new StreamReader(filename);
                Load(nccin);
            }
        }

        public void Load(StreamReader nccin)
        {
            try
            {
                string line;
                char[] seps = new char[] { '|' };
                while ((line = nccin.ReadLine()) != null)
                {
                    string[] data = line.Split(seps);

                    // Make the following assumption:
                    // if 2nd entry is 2 chars, it's a country code.
                    // if there is not dot in 4th entry, it's an IP (not an ASN)
                    if ((data.Length > 2) && (data[1].Length == 2) && (data[3].IndexOf('.') >= 0))
                    {
                        //				Console.WriteLine("{0} -> {1}", data[3], data[1]);
                        AddIp(data[3], data[1]);
                        NetworkCodeCount++;
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("exc: {0}", exc);
            }
            finally
            {
                nccin.Close();
            }

        }

        public string ConvertIPToCountry(string ip)
        {
            BitVector key = IpToBitVector(ip);
            return (string)m_trie.GetBest(key);
        }

        private void AddIp(string ip, string country)
        {
            BitVector key = IpToBitVector(ip);
            m_trie.Add(key, String.Intern(country.ToUpper()));
        }

        private BitVector IpToBitVector(string ip)
        {
            string[] elements;
            if (ip == "::1")
            {
                //IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                //ip=localIPs[localIPs.Length - 1].ToString();
                //elements = ip.Split('.');
                string testIp = "49.204.0.37";
                elements = testIp.Split('.');
            }
            else
                elements = ip.Split('.');

            BitVector bv = new BitVector();
            foreach (string e in elements)
            {
                if (e != "")
                {
                    int i = Int32.Parse(e);
                    bv.AddData(i, 8);
                }
            }
            return bv;
        }

        static public void Test()
        {
            IPToCountry itc = new IPToCountry();
            itc.Load(@"resources/ripencc.latest");
            itc.Load(@"resources/arin.20010316");
            itc.Load(@"resources/apnic-2001-05-01");
            itc.Load(@"resources/lacnic.20030101");

            //	    itc.Test();
            Console.WriteLine("217.224.0.0 = {0}", itc.ConvertIPToCountry("217.224.0.0"));
            Console.WriteLine("217.224.133.25 = {0}", itc.ConvertIPToCountry("217.224.133.25"));

            Console.WriteLine("217.208.23.45 = {0} (SE)", itc.ConvertIPToCountry("217.208.23.45"));
            Console.WriteLine("195.96.96.111 = {0} (NL)", itc.ConvertIPToCountry("195.96.96.111"));
            Console.WriteLine("81.16.128.23 = {0} (RU)", itc.ConvertIPToCountry("81.16.128.23"));
            Console.WriteLine("62.4.160.87 = {0} (BE)", itc.ConvertIPToCountry("62.4.160.87"));
            Console.WriteLine("81.50.140.32 = {0} (ICI)", itc.ConvertIPToCountry("81.50.140.32"));
            Console.WriteLine("81.70.12.45 = {0} (NL))", itc.ConvertIPToCountry("81.70.12.45"));
            Console.WriteLine("81.88.95.45 = {0} (KZ))", itc.ConvertIPToCountry("81.88.95.45"));

            for (int i = 0; i < 255; i++)
            {
                string ip = "217.170." + i + ".234";
                Console.WriteLine("{0} = {1}", ip, itc.ConvertIPToCountry(ip));
            }
        }

    }
}
