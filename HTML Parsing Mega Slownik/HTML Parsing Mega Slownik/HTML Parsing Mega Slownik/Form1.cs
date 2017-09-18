using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace HTML_Parsing_Mega_Slownik
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Request r = new Request(textBox1.Text, Translator.Language.en, Translator.Language.pl, false, false);
            List<Output> output = new List<Output>();
            output = Translator.Translate(r);           
            StringBuilder sb = new StringBuilder();
            foreach (var result in output)
                sb.AppendLine(result.Meaning);           
            if (sb.Length != 0)
            {
                Clipboard.SetText(sb.ToString());
                MessageBox.Show(sb.ToString());
            }
          
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
 }
