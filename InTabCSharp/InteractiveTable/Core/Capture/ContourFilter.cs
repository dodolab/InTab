using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading.Tasks;
using InteractiveTable.Core.Data.Capture;
using System.Drawing;

namespace InteractiveTable.Core.Graphics
{

   /// <summary>
   /// Trida pro zpracovani zachytavaneho obrazku ->
   /// zpracuje obrazek, nalezne kontury a ulozi je do seznamu nalezenych kontur
   /// </summary>
   public class ContourFilter
    {
            //nastaveni
            public bool noiseFilter = false; // prachovy filtr
            public int cannyThreshold = 50; // maximalni oblast kontury
            public bool blur = true; // rozmazavani
            public int adaptiveThresholdBlockSize = 4; // prahova hodnota oblasti kontrolovanych bloku
            public double adaptiveThresholdParameter = 1.2d; // parametr prachoveho filtru
            public bool filterContoursBySize = true; // filtrovat kontury podle velikosti
            public bool onlyFindContours = false; // hledat pouze kontury
            public int minContourLength = 15; // nejnizsi delka kontury
            public int minContourArea = 10; // minimalni oblast kontury
            public double minFormFactor = 0.5; // minimalni faktor pro filtrovani kontur  

            public List<Contour<Point>> contours; // kontury
            public Templates templates = new Templates(); // sablony
            public Templates samples = new Templates(); // vzorky
            public List<FoundTemplateDesc> foundTemplates = new List<FoundTemplateDesc>(); // nalezene sablony
            public TemplateFinder finder = new TemplateFinder(); // hledac sablon
            public Image<Gray, byte> binarizedFrame; // binarizovany obrazek


           /// <summary>
           /// Zpracuje obrazek v RGB barvach
           /// </summary>
           /// <param name="frame"></param>
            public void ProcessImage(Image<Bgr, byte> frame)
            {
                ProcessImage(frame.Convert<Gray, Byte>());
            }

            /// <summary>
            /// Zpracuje obrazek ve stupnich sedi
            /// </summary>
            /// <param name="grayFrame"></param>
            public void ProcessImage(Image<Gray, byte> grayFrame)
            {
                //vyhlazeni -> zmenseni + zpetne zvetseni binarni interpolaci
                Image<Gray, byte> smoothedGrayFrame = grayFrame.PyrDown();
                smoothedGrayFrame = smoothedGrayFrame.PyrUp();

                Image<Gray, byte> cannyFrame = null;

                // filtrovani sumu
                if (noiseFilter) cannyFrame = smoothedGrayFrame.Canny(new Gray(cannyThreshold), new Gray(cannyThreshold));
                

               // rozmazavani
                if (blur) grayFrame = smoothedGrayFrame;

                // binarizace
                CvInvoke.cvAdaptiveThreshold(grayFrame, grayFrame, 255, Emgu.CV.CvEnum.ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_MEAN_C, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY, adaptiveThresholdBlockSize + adaptiveThresholdBlockSize % 2 + 1, adaptiveThresholdParameter);

                // inverze
                grayFrame._Not();

                // logicky OR s cannyFrame vyhladi prebytecny sum
                if (cannyFrame != null) grayFrame._Or(cannyFrame);

                this.binarizedFrame = grayFrame;

                //dilatuje obrazek pro filtrovani
                if (cannyFrame != null) cannyFrame = cannyFrame.Dilate(3);

                // ZDE se naleznou vsechny kontury
                var sourceContours = grayFrame.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST);
                // ZDE se kontury profiltruji
                contours = FilterContours(sourceContours, cannyFrame, grayFrame.Width, grayFrame.Height);
                // ZDE se hledaji jiz zname sablony
                lock (foundTemplates)
                    foundTemplates.Clear();
                samples.Clear();
           
                // synchronizacni pristup do sablon, vsechny nalezene kontury zpracovavat paralelne
                lock (templates)
                    Parallel.ForEach<Contour<Point>>(contours, (contour) =>
                    {
                        // vytvori z kazde kontury sablonu a nalezne jeji odpovidajici vzor
                        var arr = contour.ToArray();
                        Template sample = new Template(arr, contour.Area, samples.templateSize);
                        lock (samples)
                            samples.Add(sample);

                        if (!onlyFindContours)
                        {
                            // nalezne sablonu podle vzoru sample
                            FoundTemplateDesc desc = finder.FindTemplate(templates, sample);

                            if (desc != null)
                                lock (foundTemplates)
                                    foundTemplates.Add(desc);
                        }
                    }
                    );
                // filtruj podle pruniku
                FilterByIntersection(ref foundTemplates);
            }

            /// <summary>
            /// Filtruje nalezene kontury z obrazku cannyFrame podle velikosti, podle filtru atd.
            /// </summary>
            /// <param name="contours"></param>
            /// <param name="cannyFrame"></param>
            /// <param name="frameWidth"></param>
            /// <param name="frameHeight"></param>
            /// <returns></returns>
            private List<Contour<Point>> FilterContours(Contour<Point> contours, Image<Gray, byte> cannyFrame, int frameWidth, int frameHeight)
            {
                int maxArea = frameWidth * frameHeight / 5;
                var c = contours;
                List<Contour<Point>> result = new List<Contour<Point>>();
                while (c != null)
                {
                    if (filterContoursBySize)
                        if (c.Total < minContourLength ||
                            c.Area < minContourArea || c.Area > maxArea ||
                            c.Area / c.Total <= minFormFactor)
                            goto next;

                    if (noiseFilter)
                    {
                        Point p1 = c[0];
                        Point p2 = c[(c.Total / 2) % c.Total];
                        if (cannyFrame[p1].Intensity <= double.Epsilon && cannyFrame[p2].Intensity <= double.Epsilon)
                            goto next;
                    }
                    result.Add(c);

                next:
                    c = c.HNext;
                }

                return result;
            }

            /// <summary>
            /// Filtruje nalezene kontury podle pruniku
            /// </summary>
            /// <param name="templates"></param>
            private static void FilterByIntersection(ref List<FoundTemplateDesc> templates)
            {
                //seradi sablony podle oblasti
                templates.Sort(new Comparison<FoundTemplateDesc>((t1, t2) => -t1.sample.contour.SourceBoundingRectArea().CompareTo(t2.sample.contour.SourceBoundingRectArea())));

                //vylouci sablony uvnitr jinych sablon
                HashSet<int> toDel = new HashSet<int>();
                for (int i = 0; i < templates.Count; i++)
                { 
                    if (toDel.Contains(i))
                        continue;
                    Rectangle bigRect = templates[i].sample.contour.SourceBoundingRect;
                    int bigArea = templates[i].sample.contour.SourceBoundingRectArea();
                    bigRect.Inflate(4, 4);
                    for (int j = i + 1; j < templates.Count; j++)
                    {
                        if (bigRect.Contains(templates[j].sample.contour.SourceBoundingRect))
                        {
                            double a = templates[j].sample.contour.SourceBoundingRectArea();
                            if (a / bigArea > 0.9d)
                            {
                             
                                if (templates[i].rate > templates[j].rate)
                                    toDel.Add(j);
                                else
                                    toDel.Add(i);
                            }
                            else//smaze sablonu
                                toDel.Add(j);
                        }
                    }
                }
                List<FoundTemplateDesc> newTemplates = new List<FoundTemplateDesc>();
                for (int i = 0; i < templates.Count; i++)
                    if (!toDel.Contains(i))
                        newTemplates.Add(templates[i]);
                templates = newTemplates;
            }
    }
}
