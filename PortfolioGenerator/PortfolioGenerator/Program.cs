using System;
using System.Collections.Generic;
using System.IO;

namespace PortfolioGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseDir = @"C:\Users\tgrue\OneDrive\Projects\Troy Gruetzmacher\www";
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
                                List<string> column1 = new List<string>();
                                List<string> column2 = new List<string>();
                                string[] fileEntries = Directory.GetFiles(baseDir + @"\Images\Portfolio");

                                Dictionary<int, string> fileDict = new Dictionary<int, string>();
                                foreach(string fullFile in fileEntries)
                                {
                                    string[] fileArray = fullFile.Split('\\');
                                    string file = fileArray[fileArray.Length - 1];
                                    string[] fileNameArray = file.Split('-');
                                    int number = int.Parse(fileNameArray[fileNameArray.Length - 1].Replace(".jpg",""));
                                    fileDict.Add(number, file);
                                }


                                for (int key = 1; key <= fileDict.Keys.Count; ++key)
                                {
                                    if ((key % 2) == 1)
                                        column1.Add(fileDict[key]);
                                    else
                                        column2.Add(fileDict[key]);
                                }
                                //column 1
                                outputFile.WriteLine("<div class=\"w3-half\">");
                                foreach(string file in column1)
                                {
                                   
                                    outputFile.WriteLine("<a target = \"_blank\" href = \"Images/Portfolio/"+file+"\" >");
                                    outputFile.WriteLine("<img src = \"Images/Portfolio/" + file+ "\" loading = \"lazy\" style = \"width:100% \" >");
                                    outputFile.WriteLine("</a>");
             
                                }
                                outputFile.WriteLine("</div>");
                                //column 2
                                outputFile.WriteLine("<div class=\"w3-half\">");
                                foreach (string file in column2)
                                {

                                    outputFile.WriteLine("<a target = \"_blank\" href = \"Images/Portfolio/" + file + "\" >");
                                    outputFile.WriteLine("<img src = \"Images/Portfolio/" + file + "\" loading = \"lazy\" style = \"width:100% \" >");
                                    outputFile.WriteLine("</a>");

                                }
                                outputFile.WriteLine("</div>");
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
