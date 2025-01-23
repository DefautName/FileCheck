using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace FileCheck
{
    /// <summary>
    /// Логика взаимодействия для AdressEditor.xaml
    /// Выбор текстового файла с информацие о файлах(их названии, версии и расположении)
    /// </summary>
    public partial class AdressEditor : Window
    {
        public string NewAdress { get; set; }
        public AdressEditor(string ad)
        {
            InitializeComponent();
            NewAdress = ad;
            AdText.Text = NewAdress;
        }
        public AdressEditor()
        {
            InitializeComponent();
        }
        public AdressEditor(string ad, Window parent)
        {
            InitializeComponent();
            NewAdress = ad;
            AdText.Text = NewAdress;
            Left = parent.Left-(Width-parent.Width)/2;
            Top = parent.Top+parent.Height/3;
           
        }

        /*
         * Кнопка принятия изменений и закрытие окна изменния файла адресов 
         */
        private void Button_Change(object sender, RoutedEventArgs e)
        {
            try
            {
                NewAdress = Path.GetFullPath(AdText.Text);
                this.Close();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessageBox.Show(ex.Message, "Предупреждение");
            }
        }

        /*
         * Открытие окна обозревателя файлов
         */
        private void Button_View(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
            if (fileDialog.FileName.Length > 0)
            {
                NewAdress = fileDialog.FileName;
                AdText.Text = NewAdress;
            }

        }

    }
}
