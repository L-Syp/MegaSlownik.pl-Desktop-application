using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTML_Parsing_Mega_Slownik
{
    class Request
    {
        
        private string _word;
        /// <summary>  
        ///<para>Word to be translated</para>        
        /// </summary>  
        public string Word
        {
            get { return _word; }
            private set
            {
                if (value.Trim().Length == 0)
                {
                    Translator.NullWord NullWordEx = new Translator.NullWord("Brak słowa to przetłumaczenia! Wprowadź tekst.");
                    throw NullWordEx;
                }
                else
                    _word = value;
            }

        }
        private Translator.Language _inputLanguage;
        /// <summary>  
        ///<para>Language of an input word</para>        
        /// </summary>   
        public Translator.Language InputLanguage { get { return _inputLanguage; } private set { _inputLanguage = value; } }

        private Translator.Language _targetLanguage;
        /// <summary>  
        ///<para>Language of an output word</para>        
        /// </summary>   
        public Translator.Language TargetLanguage { get { return _targetLanguage; } private set { _targetLanguage = value; } }

        private bool _isCompressed;
        /// <summary>  
        ///<para>Determines wheter xml file containing translation results will be compressed</para>        
        /// </summary>   
        public bool IsCompressed { get { return _isCompressed; } private set { _isCompressed = value; } }

        private bool _split;
        /// <summary>  
        ///<para>Indicates if words of similar meaning/context should be divided into separate strings</para>        
        /// </summary>   
        public bool Split { get { return _split; } private set { _split = value; } }


        /// <summary>  
        /// <para>Prepare translator for doing translation by setting operation parameters</para>            
        /// <param name="Word">Word to be translated</param> 
        /// <param name="InputLanguage">Language of an input word</param>
        /// <param name="TargetLanguage">Language of an output word</param>
        /// <param name="IsCompressed">Determine wheter xml file containing translation results will be compressed</param>  
        /// <param name="Split">Indicates if words of similar meaning/context should be divided into separate strings</param>  
        /// </summary>   
       public Request(string Word, Translator.Language InputLanguage, Translator.Language TargetLanguage, bool IsCompressed, bool Split)
        {
           this.Word = Word;
           this.InputLanguage = InputLanguage;
           this.TargetLanguage = TargetLanguage;
           this.IsCompressed = IsCompressed;
           this.Split = Split;
        }       
    }
}
