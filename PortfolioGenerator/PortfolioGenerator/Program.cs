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

        static void Main(string[] args)
        {
            string baseDir = @"C:\Users\tgrue\OneDrive\Projects\Troy Gruetzmacher\www";
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
                using (Image image = Image.Load(portfolioFiles[index-1]))
                {
                    int width = 1000;
                    int height = 0;
                    image.Mutate(x => x.Resize(width, height));

                    image.Save(baseDir + @"\Images\Portfolio\Thumbnails\portfolio-" +index+".jpg",encoder);
                }
                //generate larger image
                using (Image image = Image.Load(portfolioFiles[index-1]))
                {
                    int width = 1500;
                    int height = 0;

                    image.Mutate(x => x.Resize(width, height));
                    string name = "portfolio-" + index + ".jpg";
                    image.Save(baseDir + @"\Images\Portfolio\" + name,encoder);
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
                using (var sr = new StreamReader(baseDir + @"\index.template.html"))
                {
                    using (StreamWriter outputFile = new StreamWriter(baseDir + @"\index.html"))
                    {
                        string s = null;

                        while ((s = sr.ReadLine()) != null)
                        {
                            outputFile.WriteLine(s);
                            if(s == "<!-- Photo Grid -->")//start generation process
                            {
                                foreach(ImageProperties ip in images)
                                {  
                                    outputFile.WriteLine("<a href=\"Images\\Portfolio\\"+ ip.name +"\" itemprop=\"contentUrl\" data-size=\""+ip.width +"x"+ip.height+"\">");
                                    outputFile.WriteLine(" <img src=\"Images\\Portfolio\\Thumbnails\\" + ip.name + "\" itemprop=\"thumbnail\" alt=\"Image description\" style = \"width:100% \"/>");
                                    outputFile.WriteLine("</a>");
                                }
   
                            }
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
