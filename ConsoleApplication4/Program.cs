using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace ConsoleApplication4
{
    public static class p {
        public static string ip = "239.0.0.222";
        public static int port = 2222;

        public static int from = 1;
        public static int to = 10;
    }
    class Program
    {
        static void Main(string[] args)
        {

            xmlRead xmr = new xmlRead();
            xmr.read();

            UdpClient udpclient = new UdpClient();

            IPAddress multicastaddress = IPAddress.Parse(p.ip);
            udpclient.JoinMulticastGroup(multicastaddress);
            IPEndPoint remoteep = new IPEndPoint(multicastaddress, p.port);

            Byte[] buffer = null;

            Console.WriteLine("Для запуска нажмите ENTER");
            Console.ReadLine();

            Random rnd = new Random();
            Counter cnt = new Counter();

            while (true) {
                string str = new Rand().GetRandom(rnd);
                buffer = Encoding.Unicode.GetBytes(str +"#"+ cnt.CounterUp());
                udpclient.Send(buffer, buffer.Length, remoteep);
                Console.WriteLine(str);
            }
            //for (int i = 0; i <= 8000; i++)
            //{
            //    buffer = Encoding.Unicode.GetBytes(i.ToString());
            //    udpclient.Send(buffer, buffer.Length, remoteep);
            //    Console.WriteLine("Sent " + i);
            //}

            //Console.ReadLine();
        }

        public class Rand{
            public string GetRandom(Random rnd) {
                
                int i = rnd.Next(p.from, p.to);
                double d = rnd.NextDouble();
                d += i;
                return string.Format("{0:f5}", d);
            }
        }

        //Нужно сделать запись в фаил последнего значения для восстановления соединения
        public class Counter {
            public Counter(){
                this.counter = 0;
                this.razrad = 1;
                }
            ulong counter { get; set; }
            ulong razrad { get; set; }

            public string CounterUp() {
                if (this.counter == UInt64.MaxValue)
                {
                    this.counter = 0;
                    this.razrad++;
                }
                this.counter++;
                return this.counter+"#"+razrad;
            }
        }

    }

    #region конфиг
    /// <summary>
    /// Не делал нее какие проверки на соответсвие так как вот так :)
    /// </summary>
    class xmlRead
    {
        public void read()
        {

            string[] ss = Environment.CurrentDirectory.Split(new char[] { '\\' });
            string pach = "";

            for (int i = 0; i < ss.Length - 4; i++)
            {
                pach += ss[i] + "\\";
            }
            pach += "config.xml";


            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(pach);
            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;
            // обход всех узлов в корневом элементе

            string ip = xRoot.SelectSingleNode("udp").SelectSingleNode("ip").InnerText;
            string port = xRoot.SelectSingleNode("udp").SelectSingleNode("port").InnerText;

            string from = xRoot.SelectSingleNode("rand").SelectSingleNode("from").InnerText;
            string to = xRoot.SelectSingleNode("rand").SelectSingleNode("to").InnerText;


            p.ip = ip;
            p.port = Convert.ToInt32(port);

            p.from = Convert.ToInt32(from);
            p.to = Convert.ToInt32(to);

        }
    }

    #endregion

}
