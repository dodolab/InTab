using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using InteractiveTable.Core.Data.TableObjects.FunctionObjects;
using Emgu.CV;
using InteractiveTable.Accessories;
using Emgu.CV.Structure;
using System.IO.Compression;
using System.IO;
using System.Windows;

namespace InteractiveTable.Core.ClientServer
{
    //lowerCamelCase for compatibility with JAXB
    [Serializable()]
    [XmlRoot("rockList")]
    public class RockList
    {
        public RockList() { }

        public RockList(double[] scale, int[] initSize, int[] positionX,
            int[] positionY, int[] angle, String[] type, byte[] intensity, int tableWidth, int tableHeight)
        {
            this.scale = scale;
            this.initSize = initSize;
            this.positionX = positionX;
            this.positionY = positionY;
            this.angle = angle;
            this.type = type;
            this.intensity = intensity;
            this.tableHeight = tableHeight;
            this.tableWidth = tableWidth;
            this.hasimage = false;
        }

        public RockList(byte[] red, byte[] green, byte[] blue, int width, int positionX, int positionY)
        {
            this.width = width;
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.positionx = positionX;
            this.positiony = positionY;
            this.hasimage = true;
        }

        [XmlElement("tableWidth")]
        public int tableWidth { get; set; }
        [XmlElement("tableHeight")]
        public int tableHeight { get; set; }
        [XmlElement("scale")]
        public double[] scale { get; set; }
        [XmlElement("initSize")]
        public int[] initSize { get; set; }
        [XmlElement("positionX")]
        public int[] positionX { get; set; }
        [XmlElement("positionY")]
        public int[] positionY { get; set; }
        [XmlElement("angle")]
        public int[] angle { get; set; }
        [XmlElement("type")]
        public String[] type { get; set; }
        [XmlElement("intensity")]
        public byte[] intensity { get; set; }


        [XmlElement("red")]
        public byte[] red { get; set; }
        [XmlElement("green")]
        public byte[] green { get; set; }
        [XmlElement("blue")]
        public byte[] blue { get; set; }
        [XmlElement("width")]
        public int width { get; set; }
        [XmlElement("positionx")]
        public int positionx { get; set; }
        [XmlElement("positiony")]
        public int positiony { get; set; }
        [XmlElement("hasimage")]
        public Boolean hasimage { get; set; }

        public static RockList createRockImage(Image<Bgr, byte> image, Point leftDown, Point rightUp, double rotation)
        {
            image = image.Rotate(rotation, new Bgr(0, 0, 0));
           // image._Flip(Emgu.CV.CvEnum.FLIP.HORIZONTAL);
            int width = (int)Math.Abs(rightUp.X - leftDown.X);
            int height = (int)Math.Abs(rightUp.Y - leftDown.Y);

            byte[] red = new byte[width * height];
            byte[] green = new byte[width * height];
            byte[] blue = new byte[width * height];

            int pixelCounter = 0;

           
                for (int j = (int)rightUp.Y; j < leftDown.Y; j++)
                {
                    for (int i = (int)leftDown.X; i < rightUp.X; i++)
                    {
                    Bgr pixel = new Bgr();
                    if (i < 0 || i > image.Width || j < 0 || j > image.Height) pixel = new Bgr(0, 0, 255);
                    else pixel = image[j,i];
                        
                    red[pixelCounter] = (byte)(pixel.Red - 127);
                    green[pixelCounter] = (byte)(pixel.Green - 127);
                    blue[pixelCounter] = (byte)(pixel.Blue - 127);
                    if (red[pixelCounter] == 128) red[pixelCounter] = 127;
                    if (green[pixelCounter] == 128) green[pixelCounter] = 127;
                    if (blue[pixelCounter] == 128) blue[pixelCounter] = 127;
                    pixelCounter++;
                }
            }

           /* red = compressData(red);
            blue = compressData(blue);
            green = compressData(green);*/
            return new RockList(red, green, blue, width, (int)leftDown.X, (int)rightUp.Y);
        }

        public static RockList createRockList(List<A_Rock> rocks, int tableWidth, int tableHeight)
        {
            Boolean isEmpty = rocks.Count == 0;
            if (!isEmpty)
            {
                double[] scales = new double[rocks.Count];
                int[] initSizes = new int[rocks.Count];
                int[] positionsX = new int[rocks.Count];
                int[] positionsY = new int[rocks.Count];
                int[] angles = new int[rocks.Count];
                String[] types = new String[rocks.Count];
                byte[] intensity = new byte[rocks.Count];

                for (int i = 0; i < rocks.Count; i++)
                {
                    A_Rock rck = rocks.ElementAt(i);
                    types[i] = rck.Name;

                    scales[i] = rck.Scale;
                    initSizes[i] = 5; // toto se musi predelat :)
                    positionsX[i] = (int)rck.Position.X;
                    positionsY[i] = (int)rck.Position.Y;
                    angles[i] = (int)(180 / Math.PI * rck.Angle);
                    intensity[i] = (byte)rck.Intensity;
                }


                return new RockList(scales, initSizes, positionsX, positionsY, angles, types, intensity, tableWidth, tableHeight);
            }
            else return new RockList();
        }


        private static byte[] compressData(byte[] input)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gzip.Write(input, 0, input.Length);
                }
                return memory.ToArray();
            }
        }

    }
}
