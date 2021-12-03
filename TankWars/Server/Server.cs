using System;
using System.IO;
using System.Xml;

namespace Server
{
    class Server
    {
        private int UniverseSize;
        private int MSPerFrame;
        private int FramesPerShot;
        private int RespawnRate;
        static void Main(string[] args)
        {
            Server s = new Server();
            s.Run();
            Console.Read();
        }
        public Server()
        {

        }

        public void Run()
        {
            ReadXml();

        }
        private void ReadXml()
        {
            string toBeRead;
            try
            {
                using (XmlReader reader = XmlReader.Create("settings.xml"))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    switch (reader.Name)
                                    {

                                        case "UniverseSize":
                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Text)
                                            {
                                                toBeRead = reader.Value;
                                                if (!int.TryParse(toBeRead, out UniverseSize))
                                                {
                                                    throw new IOException("Could not parse universe size");
                                                }
                                            }
                                            break;
                                        case "MSPerFrame":
                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Text)
                                            {
                                                toBeRead = reader.Value;
                                                if (!int.TryParse(toBeRead, out MSPerFrame))
                                                {
                                                    throw new IOException("Could not parse MSPerFrame");
                                                }
                                            }
                                            break;
                                        case "FramesPerShot":
                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Text)
                                            {
                                                toBeRead = reader.Value;
                                                if (!int.TryParse(toBeRead, out FramesPerShot))
                                                {
                                                    throw new IOException("Could not parse FramesPerShot size");
                                                }
                                            }
                                            break;
                                        case "RespawnRate":
                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Text)
                                            {
                                                toBeRead = reader.Value;
                                                if (!int.TryParse(toBeRead, out RespawnRate))
                                                {
                                                    throw new IOException("Could not parse RespawnRate size");
                                                }
                                            }
                                            break;

                                    }
                                    break;
                                case XmlNodeType.EndElement:
                                    if (reader.Name == "cell")

                                        return;

                                    break;


                            }


                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}

/* case "name":
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.Text)
                                    name = reader.Value;
                                break;
                            case "contents":
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.Text)
                                    contents = reader.Value;
                                break; */