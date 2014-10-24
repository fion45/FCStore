using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace FionPushFilm.HtmlHelper
{
    public class HtmlAnalyser
    {
        private const string MAGNETRGX = "<td\\sclass=\"name\">(?<NAME>.+?)</td><td\\sclass=\"size\">(?<SIZE>.+?)</td><td\\sclass=\"date\">(?<DATE>.+?)</td>.+?<a\\shref=\"(?<DETAIL>[^\"]+?)\".*?><a\\shref=\"(?<MAGNET>[^\"]+?)\"";
        private const string PAGERGX = "<a\\shref=\"(?<PAGENUM>\\d+?)\">\\d+?</a>";
        public struct MagnetResult
        {
            public string Description;
            public string MargnetLink;
            public string Size;
            public string Date;
            public string SeedLink;
            public string DetailLink;
        }

        private string m_htmlstr;

        private Regex regex = new Regex(MAGNETRGX, RegexOptions.IgnoreCase);
        private Regex pageregex = new Regex(PAGERGX, RegexOptions.IgnoreCase);
        public HtmlAnalyser(string htmlStr)
        {
            m_htmlstr = htmlStr;
        }

        public MagnetResult[]  GetResult()
        {
            MatchCollection tmpMC = regex.Matches(m_htmlstr);
            MagnetResult[] result = new MagnetResult[tmpMC.Count];
            for(int i=0;i<tmpMC.Count;i++)
            {
                Match match = tmpMC[i];
                HtmlAnalyser.MagnetResult MR = new MagnetResult();
                MR.Description = match.Groups["NAME"].Value;
                MR.MargnetLink = match.Groups["MAGNET"].Value;
                MR.DetailLink = match.Groups["DETAIL"].Value;
                MR.Size = match.Groups["SIZE"].Value;
                MR.Date = match.Groups["DATE"].Value;
                result[i] = MR;
            }
            return result;
        }

        public int GetPageCount()
        {
            MatchCollection tmpMC = pageregex.Matches(m_htmlstr);
            if(tmpMC.Count > 0)
            {
                Match tmpMatch = tmpMC[tmpMC.Count - 1];
                return int.Parse(tmpMatch.Groups["PAGENUM"].Value);
            }
            else
            {
                return 0;
            }
        }
    }
}
