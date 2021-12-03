using System;
using System.IO;
using System.Xml;
using GameModel;
using TankWars;

namespace TankWars
{
    public class Server
    {
        private int UniverseSize = -1;
        private int MSPerFrame = -1;
        private int FramesPerShot = -1;
        private int RespawnRate = -1;

        private int wallCount = 0;

        public World world = new World();

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
            try
            {
                ReadSettingsXml();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }



        }

        private void ReadSettingsXml()
        {
            using (XmlReader reader = XmlReader.Create(@"..\..\..\..\Resources\settings.xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "UniverseSize":
                                ParseSettingsXmlVal(reader, ref UniverseSize);
                                break;
                            case "MSPerFrame":
                                ParseSettingsXmlVal(reader, ref MSPerFrame);
                                break;
                            case "FramesPerShot":
                                ParseSettingsXmlVal(reader, ref FramesPerShot);
                                break;
                            case "RespawnRate":
                                ParseSettingsXmlVal(reader, ref RespawnRate);
                                break;
                            case "Wall":
                                ParseWallXml(reader);
                                break;
                        }
                    }

                }
            }
        }

        private void ParseWallXml(XmlReader reader)
        {
            // at this point, the reader has encountered a <Wall> tag
            string x1 = null;
            string y1 = null;
            string x2 = null;
            string y2 = null;

            Vector2D p1 = null;
            Vector2D p2 = null;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "p1":
                                while (reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Element)
                                    {
                                        switch (reader.Name)
                                        {
                                            case "x":
                                                reader.Read();
                                                if (reader.NodeType == XmlNodeType.Text)
                                                    x1 = reader.Value;
                                                break;
                                            case "y":
                                                reader.Read();
                                                if (reader.NodeType == XmlNodeType.Text)
                                                    y1 = reader.Value;
                                                break;
                                        }
                                    }
                                    else if (reader.NodeType == XmlNodeType.EndElement)
                                    {
                                        if (reader.Name == "p1")
                                            break;
                                    }
                                }
                                break;
                            case "p2":
                                while (reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Element)
                                    {
                                        switch (reader.Name)
                                        {
                                            case "x":
                                                reader.Read();
                                                if (reader.NodeType == XmlNodeType.Text)
                                                    x2 = reader.Value;
                                                break;
                                            case "y":
                                                reader.Read();
                                                if (reader.NodeType == XmlNodeType.Text)
                                                    y2 = reader.Value;
                                                break;
                                        }
                                    }
                                    else if (reader.NodeType == XmlNodeType.EndElement)
                                    {
                                        if (reader.Name == "p2")
                                            break;
                                    }
                                }
                                break;

                        }

                        if (double.TryParse(x1, out double x1Dub) && double.TryParse(y1, out double y1Dub))
                        {
                            p1 = new Vector2D(x1Dub, y1Dub);
                        }

                        if (double.TryParse(x2, out double x2Dub) && double.TryParse(y2, out double y2Dub))
                        {
                            p2 = new Vector2D(x2Dub, y2Dub);
                        }

                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Name == "Wall")
                        {
                            if (p1 is null || p2 is null)
                            {
                                throw new IOException("Could not parse Wall");
                            } else
                            {
                                world.Walls.Add(wallCount, new Wall(wallCount, p1, p2));
                                wallCount++;
                            }
                            return;
                        }
                        break;
                }


            }
        }

        private void ParseSettingsXmlVal(XmlReader reader, ref int settingsVal)
        {
            reader.Read();
            if (reader.NodeType == XmlNodeType.Text)
            {
                string toBeRead = reader.Value;
                if (!int.TryParse(toBeRead, out settingsVal))
                {
                    throw new IOException("Could not parse" + reader.Name);
                }
            }
        }
    }


}