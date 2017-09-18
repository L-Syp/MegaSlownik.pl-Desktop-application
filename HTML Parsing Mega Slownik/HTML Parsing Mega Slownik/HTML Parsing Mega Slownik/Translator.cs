using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

namespace HTML_Parsing_Mega_Slownik
{
    static class Translator
    {          
        /// <summary>  
        ///<para>Exception which is thrown when the result of translation is null or incorrect</para>        
        /// </summary>      
        public class NullWord : SystemException
        {
            public NullWord()
                : base()
            {
            }
            public NullWord(string message)
                : base(message)
            {
            }
        }

        /// <summary>  
        ///<para>Available languages</para>        
        /// </summary> 
        public enum Language
        {
            /// <summary>  
            ///<para>English</para>        
            /// </summary> 
            en,
            /// <summary>  
            ///<para>Spanish</para>        
            /// </summary> 
            /// f
            es,
            /// <summary>  
            ///<para>Polish</para>        
            /// </summary> 
            pl,
            /// <summary>  
            ///<para>German</para>        
            /// </summary> 
            de,
            /// <summary>  
            ///<para>Russian</para>        
            /// </summary> 
            ru,
            /// <summary>  
            ///<para>French</para>        
            /// </summary> 
            fr,
            /// <summary>  
            ///<para>Italian</para>        
            /// </summary> 
            it,
            /// <summary>  
            ///<para>Swedish</para>        
            /// </summary> 
            se,
            /// <summary>  
            ///<para>Norwegian</para>        
            /// </summary> 
            no,
            /// <summary>  
            ///<para>Danish</para>        
            /// </summary> 
            dk,
            /// <summary>  
            ///<para>Croatian</para>        
            /// </summary> 
            hr,
            /// <summary>  
            ///<para>Ukrainian</para>        
            /// </summary> 
            ua
        } 

       /// <summary>  
       ///<para>Prepare translator for doing translation by setting operation parameters</para>            
       /// <param name="req">Request object including parameters needed to perform a translation</param> 
       /// <returns>List of Output objects containing translations, examples, category/part of speech, similar words and pronuncuation of the word</returns> 
       /// </summary>
        public static List<Output> Translate(Request req)
        {
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            string uri = null;
            StringBuilder sb = new StringBuilder();
            List<Output> WordList = new List<Output>();            

            #region SettingUri
            if (req.InputLanguage == Language.pl)
            {
                switch(req.TargetLanguage)   
                {
                    case Language.de:
                    uri = @"http://megaslownik.pl/slownik/polsko_niemiecki/";
                    break;
                    case Language.dk:
                    uri = @"http://megaslownik.pl/slownik/polsko_dunski/";
                    break;
                    case Language.en:
                    uri = @"http://megaslownik.pl/slownik/polsko_angielski/";
                    break;
                    case Language.es:
                    uri = @"http://megaslownik.pl/slownik/polsko_hiszpanski";
                    break;
                    case Language.fr:
                    uri = @"http://megaslownik.pl/slownik/polsko_francuski";
                    break;
                    case Language.hr:
                    uri = @"http://megaslownik.pl/slownik/polsko_chorwacki/";
                    break;
                    case Language.it:
                    uri = @"http://megaslownik.pl/slownik/polsko_wloski/";
                    break;
                    case Language.no:
                    uri = @"http://megaslownik.pl/slownik/polsko_norweski/";
                    break;
                    case Language.ru:
                    uri = @"http://megaslownik.pl/slownik/polsko_rosyjski/";
                    break;
                    case Language.se:
                    uri = @"http://megaslownik.pl/slownik/polsko_szwedzki/";
                    break;
                    case Language.ua:
                    uri = @"http://megaslownik.pl/slownik/polsko_ukrainski/";
                    break;
                    default:
                    uri = @"http://megaslownik.pl/slownik/polsko_angielski/";
                    break;
                }
            }

            if (req.InputLanguage != Language.pl)
            {
                switch (req.InputLanguage)
                {
                    case Language.de:
                        uri = @"http://megaslownik.pl/slownik/niemiecko_polski/";
                        break;
                    case Language.dk:
                        uri = @"http://megaslownik.pl/slownik/dunsko_polski/";
                        break;
                    case Language.en:
                        uri = @"http://megaslownik.pl/slownik/angielsko_polski/";
                        break;
                    case Language.es:
                        uri = @"http://megaslownik.pl/slownik/hiszpansko_polski";
                        break;
                    case Language.fr:
                        uri = @"http://megaslownik.pl/slownik/francusko_polski";
                        break;
                    case Language.hr:
                        uri = @"http://megaslownik.pl/slownik/chorwacko_polski/";
                        break;
                    case Language.it:
                        uri = @"http://megaslownik.pl/slownik/wlosko_polski/";
                        break;
                    case Language.no:
                        uri = @"http://megaslownik.pl/slownik/norwesko_polski/";
                        break;
                    case Language.ru:
                        uri = @"http://megaslownik.pl/slownik/rosyjsko_polski/";
                        break;
                    case Language.se:
                        uri = @"http://megaslownik.pl/slownik/szwedzko_polski/";
                        break;
                    case Language.ua:
                        uri = @"http://megaslownik.pl/slownik/ukrainsko_polski/";
                        break;
                    default:
                        uri = @"http://megaslownik.pl/slownik/angielsko_polski/";
                        break;
                }
            }
            #endregion 
            
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(uri + req.Word);            
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            htmlDoc.Load(sr);
            sr.Close();
            //htmlDoc.Load(@"D:\\html.html");
            

            // ParseErrors is an ArrayList containing any errors from the Load statement
            if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
            {
                
                foreach (HtmlAgilityPack.HtmlParseError error in htmlDoc.ParseErrors)
                {
                    sb.Append(error.ToString());
                }               
            }
            else
            {
                HtmlAgilityPack.HtmlNodeCollection bodyNodes = htmlDoc.DocumentNode.SelectNodes("//div[@id='word2']"); // //div[@id='word2']/div[@class='definicja']"
               System.Text.RegularExpressions.Regex RegExFirst = new System.Text.RegularExpressions.Regex(@"<a href(.*?)</span>",System.Text.RegularExpressions.RegexOptions.Singleline);
               System.Text.RegularExpressions.Regex RegExWords = new System.Text.RegularExpressions.Regex(@""">(.*?)</a>", System.Text.RegularExpressions.RegexOptions.Singleline);
               System.Text.RegularExpressions.Regex RegExCategories = new System.Text.RegularExpressions.Regex(@"<span class=""podkategoria"">(.*?)\)", System.Text.RegularExpressions.RegexOptions.Singleline);

#region Test       
            // The first processing of the whole HTML file
              foreach (var Node in bodyNodes)
                   sb.Append(Node.OuterHtml);                
              string tempStr = sb.ToString();
              string First = Regex.Replace(tempStr, @"\t|\n|\r", "");
              

            // The second processing of the file which results in lines containing translations and categories
              MatchCollection Matches = RegExFirst.Matches(First);
              List<string> WholeWords = new List<string>(Matches.Count);

              // Assigning the lines of interest to the WordList. They will be processed in order to retrieve translated
              // words as well as additional data i.e. a category or synonyms.
              Output tempOutput = new Output();
              for (int i = 0; i < WholeWords.Capacity; i++)
              {
                  tempStr = null;                  
                  tempStr += Matches[i].Groups[1].Value +"; ";       
                  tempOutput.Meaning = tempStr.TrimEnd(';');
                  WordList.Add(tempOutput);              
              }     
           
                // Retrievieng meaning from HTML fortmatted string.

              for (int i = 0; i < WordList.Count; i++)
              {
                  tempStr = null;
                  tempOutput.Meaning = null;
                  Matches = RegExWords.Matches(WordList[i].Meaning);
                  foreach (Match SingleMatch in Matches)
                  {
                      tempStr += SingleMatch.Groups[1].Value + "; ";
                  }
                  tempStr = tempStr.TrimEnd(' ');
                  tempStr = tempStr.TrimEnd(';');
                //  tempOutput.Meaning = tempStr.TrimEnd(' ');
                 // tempOutput.Meaning = tempStr.TrimEnd(';');
                  tempOutput.Meaning = tempStr;
                  WordList[i] = tempOutput;
              }



               int z = 0;
        
                
             
#endregion



             /* if (bodyNodes != null)
                 {                     
                     Output o = new Output();
                     for (int i = 0; i < bodyNodes.Count; i++ )
                     {
                         o.Meaning = bodyNodes[i].OuterHtml;
                         WordList.Add(o);
                     }                        
                    
                 }*/
            }
            return WordList;
        }

    }
}
