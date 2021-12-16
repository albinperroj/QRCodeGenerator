using QRCoder;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using SWM = System.Windows.Media;
using QRCodeGen = QRCoder.QRCodeGenerator;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;
using MigraDoc.Rendering;
using MigraDoc.DocumentObjectModel;
using Color = System.Drawing.Color;
using Image = MigraDoc.DocumentObjectModel.Shapes.Image;
using System.IO;
using System;

namespace QRCodeGenerator.Models
{
    public class QRCodeHandler:INotifyPropertyChanged
    {
        private string _url = string.Empty;
        public string QRCodeUrl
        {
            get { return _url; }
            set 
            { 
                _url = value;
                NotifyPropertyChanged("QRCodeUrl");
            }
        }

        private string _logoImg = string.Empty;
        public string LogoImg
        {
            get { return _logoImg; }
            set 
            {
                _logoImg = value;
                NotifyPropertyChanged("LogoImg");
            }
        }

        private string _savePath = string.Empty;
        public string SavePath
        {
            get { return _savePath; }
            set 
            { 
                _savePath = value;
                NotifyPropertyChanged("SavePath");
            }
        }

        private List<MyColor> _colorList;
        public List<MyColor> ColorList
        {
            get { return _colorList; }
            set 
            {
                _colorList = value;
                NotifyPropertyChanged("ColorList");
            }
        }

        private MyColor _qrCodeColor;
        public MyColor QRCodeColor
        {
            get { return _qrCodeColor; }
            set 
            { 
                _qrCodeColor = value;
                NotifyPropertyChanged("QRCodeColor");
            }
        }

        private bool _saveQRToPdf = false;
        public bool SaveQRToPdf
        {
            get { return _saveQRToPdf; }
            set 
            {
                _saveQRToPdf = value;
                NotifyPropertyChanged("SaveQRToPdf");
            }
        }

        private string _savedImagePath = string.Empty;

        public string SavedImagePath
        {
            get { return _savedImagePath; }
            set 
            { 
                _savedImagePath = value;
                NotifyPropertyChanged("SavedImagePath");
            }
        }


        //Notify property changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public QRCodeHandler()
        {

            ColorList = GetAllColors();

            if (ColorList?.Count > 0)
                QRCodeColor = ColorList.Find(p => p.ColorName.Equals("Black"));
        }


        public async Task GenerateQRCode()
        {
             await Task.Run(() =>
             {
                 try
                 {
                     if (string.IsNullOrEmpty(QRCodeUrl))
                         throw new MyException() { Msg = "Url or content is empty!" };

                     if (string.IsNullOrEmpty(SavePath))
                         throw new MyException() { Msg = "Save path is empty!" };


                     QRCodeGen qRCodeGenerator = new QRCodeGen();

                     QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(QRCodeUrl, QRCodeGen.ECCLevel.Q);

                     QRCode qRCode = new QRCode(qRCodeData);

                     Bitmap bitmap = null;

                     if (string.IsNullOrEmpty(LogoImg))
                     {
                         bitmap = qRCode.GetGraphic(20, Color.FromName(QRCodeColor.ColorName), Color.White, true);
                     }
                     else
                     {
                         bitmap = qRCode.GetGraphic(20, Color.FromName(QRCodeColor.ColorName), Color.White, (Bitmap)Bitmap.FromFile(LogoImg), 25, 1, true);
                     }

                     string path = SavePath;

                     if (File.Exists(path))
                     {
                         MessageBox.Show("File with this name exists!", "File exists", MessageBoxButton.OK, MessageBoxImage.Warning);
                         return;
                     }

                     string imgPath = path + "\\" + Path.GetRandomFileName() + ".png";

                     bitmap.Save(imgPath);

                     SavedImagePath = imgPath;

                     if (SaveQRToPdf)
                     {
                         PdfDocumentRenderer pdfDocumentRenderer = new PdfDocumentRenderer();

                         Document document = new Document();

                         Section section = document.AddSection();

                         section.PageSetup = new PageSetup()
                         {
                             LeftMargin = 50,
                             TopMargin = 50,
                             RightMargin = 50,
                             BottomMargin = 50
                         };

                         Image img = section.AddImage(imgPath);
                         img.Height = 150;
                         img.Width = 150;

                         pdfDocumentRenderer.Document = document;

                         pdfDocumentRenderer.RenderDocument();

                         string pdfPath = path + "\\" + Path.GetRandomFileName() + ".pdf";
                         pdfDocumentRenderer.Save(pdfPath);
                     }

                 }
                 catch (FileNotFoundException fnfEx)
                 {
                     throw new MyException() { Msg = "File not found, check file path!" };
                 }
                 catch (Exception ex)
                 {
                     throw new MyException() { Msg ="Error, check your input data!"};
                 }
             });
        }



        private List<MyColor> GetAllColors()
        {
            List<MyColor> allMyColors = new List<MyColor>();

            foreach(PropertyInfo property in typeof(SWM.Colors).GetProperties())
            {
                if(property.PropertyType == typeof(SWM.Color))
                {
                    MyColor myColor = new MyColor()
                    {
                        ColorName = (string)property.Name,
                        Color = new SolidColorBrush((SWM.Color)property.GetValue(null))
                    };
                    allMyColors.Add(myColor);
                }
            }

            return allMyColors;
        }
    }
}
