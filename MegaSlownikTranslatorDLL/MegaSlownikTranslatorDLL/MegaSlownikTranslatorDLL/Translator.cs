using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

/*  
    Copyright (C) 2011  Łukasz Sypniewski
 
    This file is part of MEGAslownikClient.

    MEGAslownikClient is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    MEGAslownikClient is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MEGAslownikClient; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

namespace MegaSlownikTranslatorDLL
{
    /// <summary>  
    ///<para>Class which provides translating word or phrase by using static method "Translate"</para>        
    /// </summary>
    public class Translator
    {
        /// <summary>  
        ///<para>Exception which is thrown when the result of translating is null or incorrect</para>        
        /// </summary>      
        public class WrongResult : SystemException
        {
            public WrongResult()
                : base()
            {
            }
            public WrongResult(string message)
                : base(message)
            {
            }
        }

        /// <summary>  
        ///<para>Available target languages</para>        
        /// </summary> 
        public enum Languages
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
        ///<para>Translate the given sting</para>        
        /// </summary>  
        /// <param name="strSource">A Word or a phrase that should be translated</param> 
        /// <param name="language">Language which given word or a phrase will be translated from/to</param> 
        /// <param name="PolishSource">Wether a source is in Polish or not (if yes a word will be translated from Polish to a language given as a parameter 'language')</param>
        /// <returns>All translated words are put into a List of string.</returns>      
        public static string[] Translate(string strSource, Languages language, bool PolishSource)  //If we want to translate from Polish to the other language then PolishSource = true
        {
            Regex regex;
            string uri;
            switch (language) //Each language has different regular expression
            {
                #region RegularExpressions
                case Languages.en:
                    if (PolishSource == false)
                    {
                        regex = new Regex("asd");
                        uri = @"http://megaslownik.pl/slownik/angielsko_polski/";
                    }
                    else
                    {
                        regex = new Regex("asd");
                        uri = @"http://megaslownik.pl/slownik/polsko_angielski/";
                    }
                    break;

                 case Languages.es:
                     if (PolishSource == false)
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:polsko_hiszpanski)/.*?"">(.+?)</a>", RegexOptions.Compiled);                       
                         uri = @"http://megaslownik.pl/slownik/hiszpansko_polski/";
                     }
                     else
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:hiszpansko_polski)/.*?"">(.+?)</a>", RegexOptions.Compiled);
                         uri = @"http://megaslownik.pl/slownik/polsko_hiszpanski/";
                     }
                     break;

                 case Languages.de:
                     if (PolishSource == false)
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:polsko_niemiecki)/.*?"">(.+?)</a>", RegexOptions.Compiled);
                         uri = @"http://megaslownik.pl/slownik/niemiecko_polski/";
                     }
                     else
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:niemiecko_polski)/.*?"">(.+?)</a>", RegexOptions.Compiled);
                         uri = @"http://megaslownik.pl/slownik/polsko_niemiecki/";                     
                     }
                     break;

                 case Languages.ru:
                     if (PolishSource == false)
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:polsko_rosyjski)/.*?"">(.+?)</a>", RegexOptions.Compiled);
                         uri = @"http://megaslownik.pl/slownik/rosyjsko_polski/";
                     }
                     else
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:rosyjsko_polski)/.*?"">(.+?)</a>", RegexOptions.Compiled);                     
                         uri = @"http://megaslownik.pl/slownik/polsko_rosyjski/";
                     }
                     break;

                 case Languages.fr:
                     if (PolishSource == false)
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:polsko_francuski)/.*?"">(.+?)</a>", RegexOptions.Compiled);
                         uri = @"http://megaslownik.pl/slownik/francusko_polski/";
                     }
                     else
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:francusko_polski)/.*?"">(.+?)</a>", RegexOptions.Compiled);                        
                         uri = @"http://megaslownik.pl/slownik/polsko_francuski/";
                     }
                     break;

                 case Languages.it:
                     if (PolishSource == false)
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:polsko_wloski)/.*?"">(.+?)</a>", RegexOptions.Compiled);
                         uri = @"http://megaslownik.pl/slownik/wlosko_polski/";

                     }
                     else
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:wlosko_polski)/.*?"">(.+?)</a>", RegexOptions.Compiled);
                         uri = @"http://megaslownik.pl/slownik/polsko_wloski/";
                     }
                     break;

                 case Languages.se:
                     if (PolishSource == false)
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:polsko_szwedzki)/.*?"">(.+?)</a>", RegexOptions.Compiled);                    
                         uri = @"http://megaslownik.pl/slownik/szwedzko_polski/";
                     }
                     else
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:szwedzko_polski)/.*?"">(.+?)</a>", RegexOptions.Compiled);
                         uri = @"http://megaslownik.pl/slownik/polsko_szwedzki/";
                     }
                     break;

                 case Languages.no:
                     if (PolishSource == false)
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:polsko_norweski)/.*?"">(.+?)</a>", RegexOptions.Compiled);
                         uri = @"http://megaslownik.pl/slownik/norwesko_polski/";
                     }
                     else
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:norwesko_polski)/.*?"">(.+?)</a>", RegexOptions.Compiled);                        
                         uri = @"http://megaslownik.pl/slownik/polsko_norweski/";
                     }
                     break;

                 case Languages.dk:
                     if (PolishSource == false)
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:polsko_dunski)/.*?"">(.+?)</a>", RegexOptions.Compiled);
                         uri = @"http://megaslownik.pl/slownik/dunsko_polski/";
                     }
                     else
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:dunsko_polski)/.*?"">(.+?)</a>", RegexOptions.Compiled);                        
                         uri = @"http://megaslownik.pl/slownik/polsko_dunski/";
                     }
                     break;

                 case Languages.hr:
                     if (PolishSource == false)
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:polsko_chorwacki)/.*?"">(.+?)</a>", RegexOptions.Compiled);
                         uri = @"http://megaslownik.pl/slownik/chorwacko_polski/";
                     }
                     else
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:chorwacko_polski)/.*?"">(.+?)</a>", RegexOptions.Compiled);                        
                         uri = @"http://megaslownik.pl/slownik/polsko_chorwacki/";
                     }
                     break;

                 case Languages.ua:
                     if (PolishSource == false)
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:polsko_ukrainski)/.*?"">(.+?)</a>", RegexOptions.Compiled);
                         uri = @"http://megaslownik.pl/slownik/ukrainsko_polski/";
                     }
                     else
                     {
                         regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:ukrainsko_polski)/.*?"">(.+?)</a>", RegexOptions.Compiled);
                         uri = @"http://megaslownik.pl/slownik/polsko_ukrainski/";
                        
                     }
                     break;
                     

                default:
                    {
                        regex = new Regex(@"<div class=""definicja"">.*<a href=""/slownik/(?:polsko_angielski|phrasal_verbs)/.*?"">(.+?)</a>", RegexOptions.Compiled);
                        uri = @"http://megaslownik.pl/slownik/angielsko_polski/";
                    }
                    break;
                #endregion
            }

             MatchCollection match = regex.Matches(Translator.GetSource(strSource, uri)); //Search the site's source to find the elements that match a regular expression
             List<string> result = new List<string>();
             foreach (var s in match)
             {
                 result.Add(s.ToString());
             }
                return result.ToArray();
        }

             private static string GetSource(string word, string webAdress)  //Get the website's source
             {
                 HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(webAdress + word);
                 myRequest.Method = "GET";
                 WebResponse myResponse = myRequest.GetResponse();
                 StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                 string strResponse = sr.ReadToEnd();
                 sr.Close();
                 myResponse.Close();
                 return strResponse;
             }
        }
    }
