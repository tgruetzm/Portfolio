using System;
using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace PortfolioGenerator
{
    class Program
    {
        struct ImageProperties{
            public int index;
            public int width;
            public int height;
            public string name;
        }

        static string wwwBaseDir = @"C:\Users\tgrue\OneDrive\Projects\Troy Gruetzmacher\www";
        static string baseDir = @"C:\Users\tgrue\OneDrive\Projects\Troy Gruetzmacher";

        //arg 1 generate html
        //arg 2 generate photogrid
        //no arg generate all
        static void Main(string[] args)
        {
            if(args.Length > 0 && args[0] == "1" || args.Length == 0)
                GenerateHtmlPages();
            if (args.Length > 0 && args[0] == "2" || args.Length == 0)
                GeneratePhotoGrid();
        }


        static void GenerateHtmlPages()
        {
            string[] mainPages = Directory.GetFiles(baseDir + "\\WebTemplates");
            string[] controlFiles = Directory.GetFiles(baseDir + "\\WebTemplates\\controls");
            Dictionary<string, string> controls = new Dictionary<string, string>();

            //read all control content into dictionary
            foreach (string control in controlFiles)
            {
                using (var sr = new StreamReader(control))
                {
                    string[] c1 = control.Split("\\");
                    string name = c1[c1.Length - 1].Replace(".html", "");
                    string content = sr.ReadToEnd();
                    controls.Add("<!--" + name+ "-->", content);
                }
            }
            //write control content to pages and output to www dir
            foreach (string page in mainPages)
            {
                using (var sr = new StreamReader(page))
                {
                    string[] c1 = page.Split("\\");
                    string name = c1[c1.Length - 1].Replace(".html","");
                    using (StreamWriter outputFile = new StreamWriter(wwwBaseDir + @"\"+name+".html"))
                    {
                        string s = null;
                        while ((s = sr.ReadLine()) != null)
                        {
                            outputFile.WriteLine(s);
                            if (controls.ContainsKey(s.Trim()))//replace content 
                            {
                                 controls.TryGetValue(s.Trim(), out string content);
                                outputFile.Write(content);
                            }
                        }
                    }
                }
            }
        }

        static void GeneratePhotoGrid()
        {
            string portfolioDir = @"C:\Users\tgrue\OneDrive\Pictures\Portfolio";

            List<ImageProperties> images = new List<ImageProperties>();
            //get portfolio images
            string[] portfolioFiles = Directory.GetFiles(portfolioDir);
            int index = 1;
            foreach (string iName in portfolioFiles)
            {
                JpegEncoder encoder = new JpegEncoder();
                encoder.Quality = 65;
                //generate thumbnail
                using (Image image = Image.Load(portfolioFiles[index - 1]))
                {
                    int width = 700;
                    int height = 0;
                    image.Mutate(x => x.Resize(width, height));

                    image.Save(wwwBaseDir + @"\Images\Portfolio\Thumbnails\portfolio-" + index + ".jpg", encoder);
                }
                //generate larger image
                using (Image image = Image.Load(portfolioFiles[index - 1]))
                {
                    int width = 1500;
                    int height = 0;

                    image.Mutate(x => x.Resize(width, height));
                    string name = "portfolio-" + index + ".jpg";
                    image.Save(wwwBaseDir + @"\Images\Portfolio\" + name, encoder);
                    ImageProperties ip = new ImageProperties();
                    ip.width = width;
                    ip.height = image.Height;
                    ip.index = index;
                    ip.name = name;
                    images.Add(ip);
                }
                index++;
            }

            try

            {
                string content;

                using (var sr = new StreamReader(wwwBaseDir + @"\index.html"))
                {
                    content = sr.ReadToEnd();
                }
                using (StreamWriter outputFile = new StreamWriter(wwwBaseDir + @"\index.html"))
                {
                    string[] contentArr = content.Split("\r\n");

                    foreach(string s in contentArr)
                    {
                        outputFile.WriteLine(s);
                        if (s == "<!-- Photo Grid -->")//start generation process
                        {
                            int count = 0;
                            int half = images.Count / 2;
                            outputFile.WriteLine("<div class=\"w3-half\">");
                            foreach (ImageProperties ip in images)
                            {
                                if(count == half)
                                {
                                    outputFile.WriteLine("</div>");
                                    outputFile.WriteLine("<div class=\"w3-half\">");
                                }
                                outputFile.WriteLine("<a href=\"Images\\Portfolio\\" + ip.name + "\" itemprop=\"contentUrl\" data-size=\"" + ip.width + "x" + ip.height + "\">");
                                outputFile.WriteLine(" <img src=\"Images\\Portfolio\\Thumbnails\\" + ip.name + "\" itemprop=\"thumbnail\" alt=\"Image description\" style = \"width:100% \"/>");
                                outputFile.WriteLine("</a>");
                                ++count;
                            }
                            outputFile.WriteLine("</div>");

                        }
                    }
                }

            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        
    }

    }

}
