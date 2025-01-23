using FileCheck.Abstract;

namespace FileCheck
{
    /// <summary>
    /// Класс отвечающий за храние данных о файле(название, версия, расположение)
    /// </summary>
    public class FileClass : Notifier
    {
        private string name;
        private string adress;
        private bool isCheked;
        private string version;
        private string extension;

        public FileClass(string n, string ver, string ad, string ex)
        {
            name = n;
            version = ver;
            adress = ad;
            isCheked = false;
            extension = ex;
        }
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(IsCheked));
            }
        }
        public string Version
        {
            get => version;
            set
            {
                version = value;
                OnPropertyChanged(nameof(IsCheked));
            }
        }

        public string Aderess
        {
            get => adress;
        }
        public bool IsCheked
        {
            get => isCheked;
            set
            {
                isCheked = value;
                OnPropertyChanged(nameof(IsCheked));
            }
        }
       public string Extension
        {
            get => extension;
        }
    }
}
