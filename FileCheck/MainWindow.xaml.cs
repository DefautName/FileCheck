using System.Windows;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Win32;
using System.Diagnostics;
using System;
using System.Text;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;

namespace FileCheck
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// Окно выбора файлов, копирования и открытия для просмотра
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private double disIcon = 0.4;
        private string path;
        private string oldPath;
        StreamReader reader;
        List<FileClass> Files { get; set; }
        List<string> result;
        ObservableCollection<FileClass> Items { get; set; }
        IEnumerable<FileClass> Sort;
        int first;

        /*
         * Запуск программы
         * Получение сохраненных данных положения и размера окна, адреса файла адресов 
         * Получение данных из фала адресов и вывод их в окно
         */
        public MainWindow()
        {
     
            //Properties.Settings.Default.Reset();//Сброс до стандартных настроек(Положение окна;ширина и высота окна; адрес строки подключения)

            Width = Properties.Settings.Default.Width;
            Height = Properties.Settings.Default.Height;
            Left = Properties.Settings.Default.Xins;
            Top = Properties.Settings.Default.Yins;
            path =Properties.Settings.Default.Path;//Path.GetFullPath()
            first = 0;
            ReadFileAdress();
            InitializeComponent();
            foreach (FileClass item in Items)
                List.Items.Add(item);
            BOpen.IsEnabled = false;
            BCopy.IsEnabled = false;
            BCopy.Opacity = disIcon;
            BOpen.Opacity = disIcon;
        }

        /*
         * Кнопка копирования выделенных файлов
         */
        void BCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                result = new List<string>();
                Sort = Items.Where(x => x.IsCheked == true);
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = Sort.First(x => x.IsCheked == true).Name;
                
                dlg.Filter = "Excel files(*.xlsx)|*.xlsx|All files (*.*)|*.*|Binary Excel (*.xlsb)|*.xlsb";
                //dlg.InitialDirectory = @"%HOMEPATH%\Desktop";
                
                dlg.ShowDialog();
                string savestr = Path.GetDirectoryName(dlg.FileName);
                DirectoryInfo info = new DirectoryInfo(savestr);

                foreach (FileClass fileClass in Sort)
                    File.Copy(fileClass.Aderess, $"{savestr}/{fileClass.Name}{fileClass.Extension}", true);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //MessageBox.Show(ex.Message, "Предупреждение");
            }
        }

        /*
         * Кнопка открытия отмеченного эксель файла
         */
        void BOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                result = new List<string>();
                Sort = Items.Where(x => x.IsCheked == true);
                foreach (FileClass fileClass in Sort)
                {
                   //Process.Start(fileClass.Aderess.TrimEnd('\r'));
                    Excel.Application exApp = new Excel.Application();
                    exApp.Visible = true;
                    exApp.Workbooks.Open(fileClass.Aderess.TrimEnd('\r'), 0, true);
                }

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessageBox.Show(ex.Message, "Предупреждение");
            }
        }

        /*
         * Считывание названия, версии и адреса файлов из файлов адресов и вывод их в список
         */
        void ReadFileAdress()
        {
           Files = new List<FileClass>();
            try
            {
                reader = new StreamReader(path, Encoding.GetEncoding("windows-1251"));
                string adressText = reader.ReadToEnd();
                adressText=adressText.TrimEnd('\n','\r');
                
                string[] namesAdresses = adressText.Split('\n');
                foreach (string file in namesAdresses)
                {
                    string[] fileClass = file.Split('|');
                    string[] fullName = fileClass[2].Split('.');
                    Files.Add(new FileClass(fileClass[0], fileClass[1], fileClass[2],$".{fullName[1].TrimEnd('\r')}"));
                }
                Items = new ObservableCollection<FileClass>(Files);
                if (first != 0)
                {
                    List.Items.Clear();
                    foreach (FileClass item in Items)
                    {
                        List.Items.Add(item);
                        
                    }

                }
                first= 1;
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //string test =ex.Source;
                //MessageBox.Show(ex.Message,"Предупреждение");//"mscorlib"

                    MessageBox.Show("Отсутвует файл адресов, введите путь к существующему файлу","Предупреждение");
                AdressEditor winEd = new AdressEditor(oldPath,this);
                    winEd.ShowDialog();
                    path = winEd.NewAdress;
                    Properties.Settings.Default.Path = path;
                    Properties.Settings.Default.Save();
                    ReadFileAdress();
                
            }
        }

        /*
         * Кнопка открытия окна для изменения файла адресов 
         */
        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            oldPath = path;
            AdressEditor winEd = new AdressEditor(path,this);
            winEd.ShowDialog();
            path = winEd.NewAdress;
            Properties.Settings.Default.Path = path;
            Properties.Settings.Default.Save();
            ReadFileAdress();
        }

        /*
         * Ответка элемента ListViewItem не только при нажатии на CheckBox, но и при нажатии на сам элемент 
         */
        private void ListViewItem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            object listItem = (sender as ListViewItem).Content;
            if ((listItem as FileClass).IsCheked == false)
                (listItem as FileClass).IsCheked = true;
            else
                (listItem as FileClass).IsCheked = false;
        }

        /*
         * Запоминание размеров и положение окна при закрытии программы
         */
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Xins = (sender as System.Windows.Window).Left;
            Properties.Settings.Default.Yins = (sender as System.Windows.Window).Top;
            Properties.Settings.Default.Width = (sender as System.Windows.Window).Width;
            Properties.Settings.Default.Height = (sender as System.Windows.Window).Height;
            Properties.Settings.Default.Save();
        }


        /*
         * Отслеживание доступности кнопопок при постановке отметки на CheckBox
         * Если отмечен только один элемент - доступны обе кнопки
         * Если отмечено больше - доступна только кнопка копирования
         */
        private void Cheked_Checked(object sender, RoutedEventArgs e)
        {
            Sort = Items.Where(x => x.IsCheked == true);
            if (Sort.Count<FileClass>() > 1)
            {
                BOpen.IsEnabled = false;
                BCopy.IsEnabled = true;
                BCopy.Opacity = 1;
                BOpen.Opacity = disIcon;
            }
            if(Sort.Count<FileClass>() == 1)
            {
                BOpen.IsEnabled = true;
                BCopy.IsEnabled = true;
                BCopy.Opacity = 1;
                BOpen.Opacity = 1;

            }
        }

       /*
       * Отслеживание доступности кнопопок при снятия отметки с CheckBox
       * Если отмечено 0 элементов - недоступны обе кнопки
       * Если отмечен 1 элемент - доступны обе кнопки
       */
        private void Cheked_Unchecked(object sender, RoutedEventArgs e)
        {
            Sort = Items.Where(x => x.IsCheked == true);

            switch(Sort.Count<FileClass>())
            {
                case 0:
                    BOpen.IsEnabled = false;
                    BCopy.IsEnabled = false;
                    BCopy.Opacity = disIcon;
                    BOpen.Opacity = disIcon;
                    break;
                case 1:
                    BOpen.IsEnabled = true;
                    BOpen.Opacity = 1;
                    break;
            }
        }

    }
}
