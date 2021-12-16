using Microsoft.Win32;
using QRCodeGenerator.Models;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;


namespace QRCodeGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private QRCodeHandler _qrCodeHandler;

        public QRCodeHandler QRHandler
        {
            get { return _qrCodeHandler; }
            set 
            { 
                _qrCodeHandler = value;
                NotifyPropertyChanged("QRHandler");
            }
        }



        //Notify property changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            QRHandler = new QRCodeHandler();
        }

        private void Save_Path_Btn_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn)
            {
                string btnName = btn.Name;

                if (string.IsNullOrEmpty(btnName))
                    return;

                switch (btnName)
                {
                    case "LOGO":
                        {

                            OpenFileDialog openFileDialog = new OpenFileDialog()
                            {
                                Title = "Logo file"
                            };

                            if (openFileDialog.ShowDialog() == true)
                            {
                                if (!string.IsNullOrEmpty(openFileDialog.FileName))
                                {
                                    QRHandler.LogoImg = openFileDialog.FileName;
                                }
                            }
                            break;
                        }
                    case "SAVE":
                        {
                            SaveFileDialog saveFileDialog = new SaveFileDialog()
                            {
                                Title = "Save directory"
                            };

                            if (saveFileDialog.ShowDialog() == true)
                            {
                                if (!string.IsNullOrEmpty(saveFileDialog.FileName))
                                {
                                    FileInfo fileInfo = new FileInfo(saveFileDialog.FileName);

                                    QRHandler.SavePath = fileInfo.DirectoryName;
                                }
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        private async void GenerateQRCode_Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await QRHandler.GenerateQRCode();
            }
            catch (MyException ex)
            {
                MessageBox.Show(ex.Msg, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
