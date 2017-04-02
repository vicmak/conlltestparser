using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DocumentParser
{
    class Program
    {
        static void Main(string[] args)
        {
            XDocument file = XDocument.Load("official-2014.0.sgml");
            var documents = file.Descendants("DOC");
            var articles = (from d in documents
                           select new Article() {
                               Title = d.Descendants("TITLE").FirstOrDefault()?.Value,
                               Paragraphs = d.Descendants("P").Select(x => x.Value).ToList(),
                               Mistakes = (from mistakeElement in d.Descendants("MISTAKE")
                                           select new Mistake()
                                           {
                                               Type = mistakeElement.Descendants("TYPE").SingleOrDefault().Value,
                                               ParagraphIndex = int.Parse(mistakeElement.Attribute("start_par").Value),
                                               Correction = mistakeElement.Descendants("CORRECTION").SingleOrDefault().Value,
                                               StartOffset = int.Parse(mistakeElement.Attribute("start_off").Value),
                                               EndOffset = int.Parse(mistakeElement.Attribute("end_off").Value)
                                           }).Where(y => y.Type == "Wci").ToList()
                           }).Where(z => z.Mistakes.Any()).ToList();

            foreach (var article in articles)
            {
                foreach (var mistake in article.Mistakes)
                {
                    Console.WriteLine(article.Paragraphs[mistake.ParagraphIndex].Insert(mistake.StartOffset+1,"*").Insert(mistake.EndOffset+1,@"/" + mistake.Correction));
                    //File.AppendAllText()
                }
            }
        }
    }

    public class Article
    {
        public string Title { get; set; }
        public List<String> Paragraphs { get; set; } = new List<string>();
        public List<Mistake> Mistakes { get; set; } = new List<Mistake>();
    }

    public class Mistake
    {
        public String Type { get; set; }
        public int ParagraphIndex { get; set; }
        public int StartOffset { get; set; }
        public int EndOffset { get; set; }
        public String Correction { get; set; }

        public override string ToString()
        {
            return $"Type = {Type}, Correction = {Correction}";
        }
    }
}
