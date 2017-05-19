using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DerpiScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFile;
            string[] input;
            List<string> output = new List<string>();

            Console.WriteLine("Enter name of text file with derpibooru links:");
            inputFile = Console.ReadLine();
            if(!inputFile.Substring(inputFile.Length - 4).Equals(".txt"))
            {
                inputFile += ".txt";
            }
            input = File.ReadAllLines(inputFile);

            using (WebClient client = new WebClient())
            {
                for (int i = 0; i < input.Length; i++)
                {
                    string temp = input[i]; // so we only pull the string once
                    if (temp.Substring(0, 5).Equals("http:")) //make sure all of them are https
                    {
                        temp = "https:" + temp.Substring(5);
                    }
                    if (temp.Substring(8, 10).Equals("derpibooru")) //after formatting, if they're a derpi link, add the json so we get info
                    {
                        DerpiImage t = JsonConvert.DeserializeObject<DerpiImage>(client.DownloadString(temp + ".json")); //download the JSON from the url provided by derpi API, then turn it into a DerpiImage object to store
                        output.Add("https://derpicdn.net/img/view/" + dateConvert(t.created_at) + t.id + "." + t.original_format);
                    }
                }
            }

            if(File.Exists("output.text"))
            {
                File.Delete("output.text"); //get rid of previous output if it was still there
            }
            File.CreateText("output.txt").Close();
            File.WriteAllLines("output.txt", output.ToArray<string>());
        }

        static string dateConvert(string input)
        {
            int month = Int32.Parse(input.Substring(5, 2)); //removes leading zeros so url works properly
            int date = Int32.Parse(input.Substring(8, 2)); //removes leading zeros so url works properly
            return input.Substring(0, 4) + "/" + month.ToString() + "/" + date + "/";
        }
    }
}