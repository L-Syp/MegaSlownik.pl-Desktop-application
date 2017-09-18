using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Reflection;
using System.ComponentModel;

/*  
    Copyright (C) 2011 Łukasz Sypniewski
 
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

namespace MegaSlownikWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();

        static int[] RightArrow = new int[3] { 10, -3, 8 }; //Coordinates for drawing the right and the left arrow
        static int[] LeftArrow = new int[3] { -10, 3, 8 };
        public Hashtable HashRadioBtn = new Hashtable(); ///Hashtable which I store the name of each RadioButton in. Each name is assigned to the corresponding language abbreviation        
        

        public void DrawArrowOnButton(Button btn, int[] Coordinates)
        {
            var canvas = new Canvas();
            btn.Content = canvas;
            canvas.Children.Add(new Line
            {
                X1 = -10,
                Y1 = 0,
                X2 = 10,
                Y2 = 0,
                Stroke = Brushes.Black,
                StrokeThickness = 2.0
            });


            canvas.Children.Add(new Line
            {
                X1 = Coordinates[0],
                Y1 = 0,
                X2 = Coordinates[1],
                Y2 = -Coordinates[2],
                Stroke = Brushes.Black,
                StrokeThickness = 2.0
            });

            canvas.Children.Add(new Line
            {
                X1 = Coordinates[0],
                Y1 = 0,
                X2 = Coordinates[1],
                Y2 = Coordinates[2],
                Stroke = Brushes.Black,
                StrokeThickness = 2.0
            });

        }

        public void HashControlsSetNames() //Assign Radiobutton's name to each abbreviation
        {
            HashRadioBtn.Add("EN", RadioBtnEN);
            HashRadioBtn.Add("ES", RadioBtnES);
            HashRadioBtn.Add("DE", RadioBtnDE);
            HashRadioBtn.Add("FR", RadioBtnFR);
            HashRadioBtn.Add("IT", RadioBtnIT);
            HashRadioBtn.Add("RU", RadioBtnRU);
            HashRadioBtn.Add("NO", RadioBtnNO);
            HashRadioBtn.Add("DK", RadioBtnDK);
            HashRadioBtn.Add("SE", RadioBtnSE);
            HashRadioBtn.Add("HR", RadioBtnHR);
            HashRadioBtn.Add("UA", RadioBtnUA);
        }

        public void SetUIImages()
        {           
           Assembly _assembly = Assembly.GetExecutingAssembly();
           Stream imgStream = _assembly.GetManifestResourceStream("MegaSlownikWPF.Resources.icon.ico");
           this.Icon = BitmapFrame.Create(imgStream);   
        
           #region DrawingFlags                     
           string[] abbrev = { "EN", "ES","DE","FR","IT","RU","DK","NO","SE","CRO","UA","PL" };
           BitmapImage[] mapImg = new BitmapImage[12];  

           #region SetFlagsImagesHashTable
           Hashtable HashFlagsImgs = new Hashtable(); 
           HashFlagsImgs.Add("EN", ImgEN);             
           HashFlagsImgs.Add("PL", ImgPL);
           HashFlagsImgs.Add("ES", ImgES);
           HashFlagsImgs.Add("DE", ImgDE);
           HashFlagsImgs.Add("FR", ImgFR);
           HashFlagsImgs.Add("IT", ImgIT);
           HashFlagsImgs.Add("RU", ImgRU);
           HashFlagsImgs.Add("DK", ImgDK);
           HashFlagsImgs.Add("NO", ImgNO);
           HashFlagsImgs.Add("SE", ImgSE);
           HashFlagsImgs.Add("CRO", ImgCRO);
           HashFlagsImgs.Add("UA", ImgUA);
           #endregion
           
           int i = 0;
           foreach (string s in abbrev)
           {
               mapImg[i] = new BitmapImage();
               mapImg[i].BeginInit();
               imgStream = _assembly.GetManifestResourceStream("MegaSlownikWPF.Resources." + s + ".png");               
               mapImg[i].StreamSource = imgStream;
               mapImg[i].DecodePixelHeight = 25;  //For saving memory, it's enough for one facet (Height lub Width)
               mapImg[i].EndInit();
               (HashFlagsImgs[s] as Image).Source = mapImg[i];
               i++;
           }           
           #endregion
        }

        public MainWindow()
        {
            InitializeComponent();
            DrawArrowOnButton(btnArrow, RightArrow);
            HashControlsSetNames();
            SetUIImages();
     
        }

        private void btnArrow_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag.ToString() == "1")
            {
                DrawArrowOnButton((sender as Button), LeftArrow);
                (sender as Button).Tag = "0"; //Tag = 0 for translating from Polish to another language
            }
            else
            {
                DrawArrowOnButton((sender as Button), RightArrow);
                (sender as Button).Tag = "1"; //Tag = 1 for translating from another language to Polish
            }
        }

        private void btnTranslate_Click(object sender, RoutedEventArgs e)
        {            
                if (txtBoxInput.Text.Length != 0)
                {
                    txtBoxResult.Clear();
                    try
                    {
                        bool Polish;
                        if (btnArrow.Tag.ToString() == "1")
                            Polish = false;
                        else
                            Polish = true;
                        
                        string language = btnTranslate.Tag.ToString(); //(Translator.Languages)Enum.Parse(typeof(Translator.Languages), language.ToLower(), false) - converting from string to Enum
                        string[] Translated = MegaSlownikTranslatorDLL.Translator.Translate(txtBoxInput.Text.Trim(), (MegaSlownikTranslatorDLL.Translator.Languages)Enum.Parse(typeof(MegaSlownikTranslatorDLL.Translator.Languages), language.ToLower(), false), Polish);
                        foreach (string s in Translated)
                        {
                            txtBoxResult.Text = txtBoxResult.Text + s + ";  ";
                        }
                    }
                    catch (SystemException ex)
                    {
                        MessageBox.Show(ex.Message, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }                       
        }

        private void ButtonUK_Click(object sender, RoutedEventArgs e)
        {            
            (HashRadioBtn[(sender as Control).Name.Remove(0, 6)] as RadioButton).IsChecked = true;      
        }

        private void RadioBtnEN_Checked(object sender, RoutedEventArgs e)
        {                  
            btnTranslate.Tag = (sender as Control).Name.Remove(0, 8);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D1:                    
                        RadioBtnEN.IsChecked = true;
                        break;
                case Key.D2:
                        RadioBtnES.IsChecked = true;
                        break;
                case Key.D3:
                        RadioBtnDE.IsChecked = true;
                        break;
                case Key.D4:
                        RadioBtnFR.IsChecked = true;
                        break;
                case Key.D5:
                        RadioBtnIT.IsChecked = true;
                        break;
                case Key.D6:
                        RadioBtnRU.IsChecked = true;
                        break;
                case Key.D7:
                        RadioBtnDK.IsChecked = true;
                        break;
                case Key.D8:
                        RadioBtnNO.IsChecked = true;
                        break;
                case Key.D9:
                        RadioBtnSE.IsChecked = true;
                        break;
                case Key.D0:
                        RadioBtnHR.IsChecked = true;
                        break;
                case Key.OemMinus:
                        RadioBtnUA.IsChecked = true;
                        break;  
            }
        }

        private void txtBoxInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((txtBoxInput.Text.Trim().Length == 0) && (txtBoxInput.Text.Length >= 0))
                btnTranslate.IsEnabled = false;
            else
                btnTranslate.IsEnabled = true;
           
        }

                                                                                         
    }
}
