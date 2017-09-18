using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/*
    html2struct parses HTML code into a simple tree-like structure of objects and provides a little tool-set for extracting data from it.
    Source: http://www.codeproject.com/Articles/752625/html2struct-Class-Library
    
    If you find this solution useful and would like to support me you could donate to my paypal account. 
    www.paypal.com/cgi-bin/webscr?cmd=_donations&business=thorssig%40hotmail%2ecom&lc=US&item_name=thorssig&no_note=0&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donate_LG%2egif%3aNonHostedGuest
    This would be greatly appreciated since I'm unemployed and completely broke 8-
  
    I am currently unemployed and looking for a job. I am sorry to say that I have been unemployed for 6 years now after running into the danish crown basically and I haven't got a single job since then and I desperately need an income.
    If anyone like to hire me for a job they can approach me at freelancer.com (https://www.freelancer.com/u/thorssig.html) or contact me directly.
    
    This solution is distributed under The Code Project Open License (CPOL) (http://www.codeproject.com/info/cpol10.aspx)
    Copyright (c) 2014, Thorsteinn Sigurdsson (thorssig@hotmail.com)
    All rights reserved.
 */

namespace html2struct
{
    /// <summary>
    /// Version: 5
    /// html2struct parses HTML code into a simple tree-like structure of objects and provides a little tool-set for extracting data from it.
    /// </summary>
    public class htmlStruct
    {
        #region Member variables
        private List<htmlTag> _allTags;
        private List<htmlTag> _innerTags;
        #endregion

        #region Constructors / Destructors
        public htmlStruct()
        {
            _innerTags = new List<htmlTag>();
            _allTags = new List<htmlTag>();
        }
        public htmlStruct(string strHTML)
        {
            _innerTags = new List<htmlTag>();
            _allTags = new List<htmlTag>();
            this.Parse(strHTML);
        }
        #endregion

        #region Attribute - AllTags
        /// <summary>
        /// Contains a list of all tags found to be contained within parsed HTML code.
        /// </summary>
        public List<htmlTag> AllTags
        {
            get
            {
                if (_allTags == null) _allTags = new List<htmlTag>();
                return _allTags;
            }
        }
        #endregion

        #region Attribute - InnerTags
        /// <summary>
        /// Contains a list of top-level tags found to be contained within parsed HTML code.
        /// </summary>
        public List<htmlTag> InnerTags
        {
            get
            {
                if (_innerTags == null) _innerTags = new List<htmlTag>();
                return _innerTags;
            }
        }
        #endregion

        #region Parse html
        /// <summary>
        /// Parses the given HTML code.
        /// If Tag is given it will match tags with that name.
        /// If attribute is given it will match tags having that attribute.
        /// If value is given it will match tags with any attribute having that value.
        /// If both attribute and value is given it will match attributes with that value.
        /// </summary>
        public void Parse(string strHTML)
        {
            #region Init/Clear local variables
            htmlTag lastTag = null;
            Int32 currLineNr = 1;
            htmlTag currParent = null;
            this.AllTags.Clear();
            this.InnerTags.Clear();
            #endregion

            do
            {
                Match mCurrHtml;
                // Process comments
                if ((mCurrHtml = Regex.Match(strHTML, @"^\s*(<!--((?!-->).)*-->|<![^<>]+>|<!\[[^\s/<>]+\[((?!\]\]>).)*\]\]>)\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline)).Success)
                {
                    htmlTag t = new htmlTag("<COMMENT>", mCurrHtml.Groups[0].Value, currLineNr, currParent, lastTag);

                    if (currParent == null) this.InnerTags.Add(t); else t.ParentTag.InnerTags.Add(t);
                    if (lastTag != null) lastTag.NextTag = t; lastTag = t;
                    this.AllTags.Add(t);

                    currLineNr += mCurrHtml.Groups[0].Value.Split('\n').Length - 1;
                    strHTML = Regex.Replace(strHTML, @"^\s*(<!--((?!-->).)*-->|<![^<>]+>|<!\[[^\s/<>]+\[((?!\]\]>).)*\]\]>)\s*", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                }
                // Process scripts
                else if ((mCurrHtml = Regex.Match(strHTML, @"^\s*(<script(?<a>\s+[^>]*)?>(?<s>((?!</script>).)*)</script>)\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline)).Success)
                {
                    htmlTag t = new htmlTag("<SCRIPT>", mCurrHtml.Groups[0].Value, currLineNr, currParent, lastTag);

                    if (currParent == null) this.InnerTags.Add(t); else t.ParentTag.InnerTags.Add(t);
                    if (lastTag != null) lastTag.NextTag = t; lastTag = t;
                    this.AllTags.Add(t);

                    // Process tag attributes
                    MatchCollection mAttributeCollection = Regex.Matches(mCurrHtml.Groups["a"].Value, "\\s+(?<a>[^\\n/>\"'’=]+)\\s*=\\s*([\"'])(?<v>[^\"']*)[\"']|\\s+(?<a>[^\\n/>\"'’=]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    foreach (Match mAttribute in mAttributeCollection)
                    {
                        if (t.Attributes.ContainsKey(mAttribute.Groups["a"].Value.Trim())) t.Attributes[mAttribute.Groups["a"].Value.Trim()] = mAttribute.Groups["v"].Value.Trim();
                        else t.Attributes.Add(mAttribute.Groups["a"].Value.Trim(), mAttribute.Groups["v"].Value.Trim());
                    }

                    currLineNr += mCurrHtml.Groups[0].Value.Split('\n').Length - 1;
                    strHTML = Regex.Replace(strHTML, @"^\s*(<script(?<a>\s+[^>]*)?>(?<s>((?!</script>).)*)</script>)\s*", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                }
                // Process tags
                else if ((mCurrHtml = Regex.Match(strHTML, @"^\s*<(?<t>[^\s/<>]+)(?<a>\s+[^>]*?)?\s*(?<c>/)?>\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline)).Success)
                {
                    htmlTag t = new htmlTag("<" + mCurrHtml.Groups["t"].Value + ">", mCurrHtml.Groups[0].Value, currLineNr, currParent, lastTag);

                    if (currParent == null) this.InnerTags.Add(t); else t.ParentTag.InnerTags.Add(t);
                    if (lastTag != null) lastTag.NextTag = t; lastTag = t;
                    this.AllTags.Add(t);

                    // Process tag attributes
                    MatchCollection mAttributeCollection = Regex.Matches(mCurrHtml.Groups["a"].Value, "\\s+(?<a>[^\\n/>\"'’=]+)\\s*=\\s*([\"'])(?<v>[^\"']*)[\"']|\\s+(?<a>[^\\n/>\"'’=]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    foreach (Match mAttribute in mAttributeCollection)
                    {
                        if (t.Attributes.ContainsKey(mAttribute.Groups["a"].Value.Trim())) t.Attributes[mAttribute.Groups["a"].Value.Trim()] = mAttribute.Groups["v"].Value.Trim();
                        else t.Attributes.Add(mAttribute.Groups["a"].Value.Trim(), mAttribute.Groups["v"].Value.Trim());
                    }

                    // Treat tag as parent if it does not have /> ending
                    if (mCurrHtml.Groups["c"].Value == "") currParent = t;

                    currLineNr += mCurrHtml.Groups[0].Value.Split('\n').Length - 1;
                    strHTML = Regex.Replace(strHTML, @"^\s*<(?<t>[^\s/<>]+)(?<a>\s+[^>]*?)?\s*(?<c>/)?>\s*", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                }
                // Process closing tags
                else if ((mCurrHtml = Regex.Match(strHTML, @"^\s*</(?<t>[^\s/<>]+)( /)?>\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline)).Success)
                {
                    if (currParent != null) currParent = currParent.ParentTag;

                    currLineNr += mCurrHtml.Groups[0].Value.Split('\n').Length - 1;
                    strHTML = Regex.Replace(strHTML, @"^\s*</(?<t>[^\s/<>]+)( /)?>\s*", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                }
                // Process text in between tags
                else if ((mCurrHtml = Regex.Match(strHTML, @"^\s*([^<]+)(?=<)", RegexOptions.IgnoreCase | RegexOptions.Singleline)).Success)
                {
                    htmlTag t = new htmlTag("<TEXT>", mCurrHtml.Groups[0].Value, currLineNr, currParent, lastTag);

                    if (currParent == null) this.InnerTags.Add(t); else t.ParentTag.InnerTags.Add(t);
                    if (lastTag != null) lastTag.NextTag = t; lastTag = t;
                    this.AllTags.Add(t);

                    currLineNr += mCurrHtml.Groups[0].Value.Split('\n').Length - 1;
                    strHTML = Regex.Replace(strHTML, @"^\s*([^<]+)(?=<)", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                }
                else
                {
                    // Remove unrecognized stuff and just keep on going. May of course cause missing elements so if you find it skipping something important please let me know.
                    strHTML = Regex.Replace(strHTML, "^.+?(?=<)", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                }

                // Generate an error if number of html tags exceed 10.000.000
                if (this.AllTags.Count > 10000000) throw new Exception("Number of tags in document have exceeded 10.000.000 entries! Surely something must be wrong!");

            } while (!Regex.IsMatch(strHTML, @"^\s*$", RegexOptions.Singleline));

        }
        #endregion

        #region FirstTag() - Returns the first tag from AllTags that matches all given search expressions
        /// <summary>
        /// Returns the first tag from AllTags that matches all given search expressions.
        /// If Tag is given it will return tags with names that match.
        /// If attribute is given it return tags with attributes that match.
        /// If value is given it will return tags with any attribute having values that match.
        /// If both attribute and value is given it will return tags with any attributes that match having a value that match.
        /// All parameters take a case-insensitive regular expression as search string.
        /// </summary>
        public htmlTag FirstTag(string strTag, string strAttribute, string strValue)
        {
            foreach (htmlTag t in this.AllTags)
            {
                bool bFound = true;
                // Check if current Tag matches criteria
                if (!string.IsNullOrEmpty(strTag) && !Regex.IsMatch(t.Tag, strTag, RegexOptions.IgnoreCase)) bFound = false;
                if (!string.IsNullOrEmpty(strAttribute) && !string.IsNullOrEmpty(strValue))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, strAttribute, RegexOptions.IgnoreCase) && Regex.IsMatch(t.Attributes[s], strValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }
                else if (!string.IsNullOrEmpty(strAttribute))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, strAttribute, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }
                else if (!string.IsNullOrEmpty(strValue))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Values) if (Regex.IsMatch(s, strValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }

                // Return current Tag if it matches criteria
                if (bFound) return t;
            }

            //return null if nothing is found
            return null;
        }
        #endregion

        #region FirstHtml() - Returns the first tag from AllTags that matches HTML expression
        /// <summary>
        /// Returns the first tag from AllTags that matches HTML expression.
        /// Takes a case-insensitive regular expression as search string.
        /// </summary>
        public htmlTag FirstHtml(string strHTML)
        {
            foreach (htmlTag t in this.AllTags)
            {
                if (Regex.IsMatch(t.Html, strHTML, RegexOptions.IgnoreCase)) return t;
            }

            //return null if nothing is found
            return null;
        }
        #endregion

        #region Search() - Returns list of tags from AllTags that match all given search expressions
        /// <summary>
        /// Returns list of tags from AllTags that match all given search expressions.
        /// If Tag is given it will return tags with names that match.
        /// If attribute is given it return tags with attributes that match.
        /// If value is given it will return tags with any attribute having values that match.
        /// If both attribute and value is given it will return tags with any attributes that match having a value that match.
        /// All parameters take a case-insensitive regular expression as search string.
        /// </summary>
        public List<htmlTag> Search(string strTag, string strAttribute, string strValue)
        {
            List<htmlTag> l = new List<htmlTag>();
            foreach (htmlTag t in this.AllTags)
            {
                // Check if current Tag matches criteria
                if (!string.IsNullOrEmpty(strTag) && !Regex.IsMatch(t.Tag, strTag, RegexOptions.IgnoreCase)) continue;
                if (!string.IsNullOrEmpty(strAttribute) && !string.IsNullOrEmpty(strValue))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, strAttribute, RegexOptions.IgnoreCase) && Regex.IsMatch(t.Attributes[s], strValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) continue;
                }
                else if (!string.IsNullOrEmpty(strAttribute))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, strAttribute, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) continue;
                }
                else if (!string.IsNullOrEmpty(strValue))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Values) if (Regex.IsMatch(s, strValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) continue;
                }

                // Add current Tag to list if it matches criteria
                l.Add(t);
            }
            return l;
        }
        #endregion
    }
    
    public class htmlTag
    {
        #region Member variables
        private string _tag;
        private string _html;
        private Int32 _lineNr;
        private Dictionary<string, string> _attributes;
        private htmlTag _parentTag;
        private htmlTag _previousTag;
        private htmlTag _nextTag;
        private List<htmlTag> _innerTags;
        #endregion

        #region Constructors / Destructors
        public htmlTag()
        {
            _tag = "";
            _html = "";
            _lineNr = 0;
            _attributes = new System.Collections.Generic.Dictionary<string, string>();
            _parentTag = null;
            _previousTag = null;
            _nextTag = null;
            _innerTags = new List<htmlTag>();
        }
        public htmlTag(string strTag, string strHTML, int iLineNr, htmlTag pParentTag, htmlTag pPreviousTag)
        {
            _tag = strTag.Trim().ToUpper();
            _html = strHTML.Trim();
            _lineNr = iLineNr;
            _attributes = new System.Collections.Generic.Dictionary<string, string>();
            _parentTag = pParentTag;
            _previousTag = pPreviousTag;
            _nextTag = null;
            _innerTags = new List<htmlTag>();
        }
        #endregion

        #region Attribute - Tag
        /// <summary>
        /// Contains the name/type of this tag.
        /// </summary>
        public string Tag
        {
            get { return _tag; }
            set 
            {
                _tag = value.Trim().ToUpper(); 
            }
        }
        #endregion

        #region Attribute - Html
        /// <summary>
        /// Contains the HTML code from which this tag was generated.
        /// </summary>
        public string Html
        {
            get { return _html; }
            set { _html = value.Trim(); }
        }
        #endregion

        #region Attribute - LineNr
        /// <summary>
        /// Contains the line number in HTML code from which this tag was generated.
        /// </summary>
        public Int32 LineNr
        { 
            get { return _lineNr; }
            set { _lineNr = value; }
        }
        #endregion

        #region Attribute - Attributes
        /// <summary>
        /// Contains the attributes found to be defined with this tag.
        /// </summary>
        public Dictionary<string, string> Attributes
        {
            get
            {
                if (_attributes == null) _attributes = new System.Collections.Generic.Dictionary<string, string>();
                return _attributes;
            }
        }
        #endregion

        #region Attribute - ParentTag
        /// <summary>
        /// Contains the parent tag found to encapsulate this tag.
        /// </summary>
        public htmlTag ParentTag
        {
            get { return _parentTag; }
            set { _parentTag = value; }
        }
        #endregion

        #region Attribute - PreviousTag
        /// <summary>
        /// Contains the previous tag found before this tag.

        /// </summary>
        public htmlTag PreviousTag
        {
            get { return _previousTag; }
            set { _previousTag = value; }
        }
        #endregion

        #region Attribute - NextTag
        /// <summary>
        /// Contains the next tag found after this tag.
        /// </summary>
        public htmlTag NextTag
        {
            get { return _nextTag; }
            set { _nextTag = value; }
        }
        #endregion

        #region Attribute - InnerTags
        /// <summary>
        /// Contains a list of inner tags (children) found to be contained within this tag (parent).
        /// </summary>
        public List<htmlTag> InnerTags
        {
            get
            {
                if (_innerTags == null) _innerTags = new List<htmlTag>();
                return _innerTags;
            }
        }
        #endregion

        #region FirstTag() - Returns the first tag from InnerTags that matches all given search expressions
        /// <summary>
        /// Returns the first tag from InnerTags that matches all given search expressions.
        /// If Tag is given it will return tags with names that match.
        /// If attribute is given it return tags with attributes that match.
        /// If value is given it will return tags with any attribute having values that match.
        /// If both attribute and value is given it will return tags with any attributes that match having a value that match.
        /// All parameters take a case-insensitive regular expression as search string.
        /// </summary>
        public htmlTag FirstTag(string strTag, string strAttribute, string strValue)
        {
            foreach (htmlTag t in this.InnerTags)
            {
                bool bFound = true;
                // Check if current Tag matches criteria
                if (!string.IsNullOrEmpty(strTag) && !Regex.IsMatch(t.Tag, strTag, RegexOptions.IgnoreCase)) bFound = false;
                if (!string.IsNullOrEmpty(strAttribute) && !string.IsNullOrEmpty(strValue)) 
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, strAttribute, RegexOptions.IgnoreCase) && Regex.IsMatch(t.Attributes[s], strValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }
                else if (!string.IsNullOrEmpty(strAttribute))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, strAttribute, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }
                else if (!string.IsNullOrEmpty(strValue))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Values) if (Regex.IsMatch(s, strValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }

                // Return current Tag if it matches criteria
                if (bFound) return t;
                
                // Search InnerTags
                htmlTag t2 = t.FirstTag(strTag, strAttribute, strValue);
                if (t2 != null) return t2;
            }

            //return null if nothing is found
            return null;
        }
        #endregion

        #region FirstHtml() - Returns the first tag from InnerTags that matches HTML expression
        /// <summary>
        /// Returns the first tag from InnerTags that matches HTML expression.
        /// Takes a case-insensitive regular expression as search string.
        /// </summary>
        public htmlTag FirstHtml(string strHTML)
        {
            foreach (htmlTag t in this.InnerTags)
            {
                if (Regex.IsMatch(t.Html, strHTML, RegexOptions.IgnoreCase)) return t;
                htmlTag t2 = t.FirstHtml(strHTML);
                if (t2 != null) return t2;
            }

            //return null if nothing is found
            return null;
        }
        #endregion

        #region Search() - Returns list of tags from InnerTags that match all given search expressions
        /// <summary>
        /// Returns list of tags from InnerTags that match all given search expressions.
        /// If Tag is given it will return tags with names that match.
        /// If attribute is given it return tags with attributes that match.
        /// If value is given it will return tags with any attribute having values that match.
        /// If both attribute and value is given it will return tags with any attributes that match having a value that match.
        /// All parameters take a case-insensitive regular expression as search string.
        /// </summary>
        public List<htmlTag> Search(string strTag, string strAttribute, string strValue)
        {
            List<htmlTag> l = new List<htmlTag>();
            foreach (htmlTag t in this.InnerTags)
            {
                // Check if current Tag matches criteria
                bool bFound = true;
                if (!string.IsNullOrEmpty(strTag) && !Regex.IsMatch(t.Tag, strTag, RegexOptions.IgnoreCase)) bFound = false;
                if (!string.IsNullOrEmpty(strAttribute) && !string.IsNullOrEmpty(strValue))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, strAttribute, RegexOptions.IgnoreCase) && Regex.IsMatch(t.Attributes[s], strValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }
                else if (!string.IsNullOrEmpty(strAttribute))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, strAttribute, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }
                else if (!string.IsNullOrEmpty(strValue))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Values) if (Regex.IsMatch(s, strValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }

                // Add current Tag to list if it matches criteria
                if (bFound) l.Add(t);

                // Add InnerTags to list that match criteria, causes depth-first search
                l.AddRange(t.Search(strTag, strAttribute, strValue));
            }
            return l;
        }
        #endregion

        #region ToText()
        /// <summary>
        /// Goes thru all the InnerTags of this tag and returns all text. Tags like BR and P are treated as newlines.
        /// </summary>
        public string ToText()
        {
            string strText = "";
            if (this.Tag == "<TEXT>") strText = this.Html;
            else if (this.Tag == "<BR>" && this.InnerTags.Count == 0) strText = "\n";
            foreach (htmlTag t in this.InnerTags)
            {
                strText += t.ToText();
            }
            if (this.Tag == "<P>" && this.InnerTags.Count > 0) strText += "\n\n";
            else if (this.Tag == "<BR>" && this.InnerTags.Count > 0) strText += "\n"; 
            return strText;
        }
        #endregion

        #region ToString() override
        public override string ToString()
        {
            return string.Format("{3}: {0} - a({1}), it({2})", this.Tag, this.Attributes.Count, this.InnerTags.Count, this.LineNr);
        }
        #endregion
    }
}
