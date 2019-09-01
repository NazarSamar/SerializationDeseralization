using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Serialization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    var All = CreateFileSystemXmlTree(fbd.SelectedPath);

                    XmlSerializer formatter = new XmlSerializer(typeof(XElement));
                    using (FileStream fs = new FileStream("serialize.xml", FileMode.Create))
                    {
                        formatter.Serialize(fs, All);
                    }
                    MessageBox.Show("Your file is in Serialization/bin/Debug/serialize.xml", "Path");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(XElement));
                    using (FileStream fs = new FileStream("serialize.xml", FileMode.Open))
                    {
                        var items = formatter.Deserialize(fs) as XElement;

                        using (StreamWriter sw = new StreamWriter("deserialize.txt"))
                        {
                            foreach (var item in items.Descendants())
                            {
                                foreach (var i in item.Elements())
                                {
                                    if (i.Element("Folder") != null)
                                        sw.WriteLine($"{i.Attribute("Name")} {i.Attribute("FullName")} {i.Attribute("CreationTime")} {i.Attribute("Root")} {i.Attribute("Attributes")} {i.Attribute("Extension")} {i.Attribute("Parent")} {i.Attribute("LastWriteTime")} {i.Attribute("LastAccessTime")}");
                                    else
                                        sw.WriteLine($"{i.Attribute("Name")} {i.Attribute("Attributes")} {i.Attribute("CreationTime")} {i.Attribute("Directory")} {i.Attribute("DirectoryName")} {i.Attribute("Extension")} {i.Attribute("LastAccessTime")} {i.Attribute("LastWriteTime")} {i.Attribute("Length")}");
                                }
                                
                            }
                        }
                        string destination = fbd.SelectedPath;
                        System.IO.File.Move(@"deserialize.txt", destination+@"\deserialize.txt");
                        MessageBox.Show($"Your file is in {destination}", "Path");
                    }
                    
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        static XElement CreateFileSystemXmlTree(string source)
        {
            DirectoryInfo di = new DirectoryInfo(source);
            return new XElement("Folder",
                new XAttribute("Name", di.Name),
                new XAttribute("FullName", di.FullName),
                new XAttribute("CreationTime", di.CreationTime),
                new XAttribute("Root", di.Root),
                new XAttribute("Attributes", di.Attributes),
                new XAttribute("Extension", di.Extension),
                new XAttribute("Parent", di.Parent),
                new XAttribute("LastWriteTime", di.LastWriteTime),
                new XAttribute("LastAccessTime", di.LastAccessTime),
                from d in Directory.GetDirectories(source)
                select CreateFileSystemXmlTree(d),
                from fi in di.GetFiles()
                select new XElement("File",
                    new XAttribute("Name", fi.Name),
                    new XAttribute("Attributes", fi.Attributes),
                    new XAttribute("CreationTime", fi.CreationTime),
                    new XAttribute("Directory", fi.Directory),
                    new XAttribute("DirectoryName", fi.DirectoryName),
                    new XAttribute("Extension", fi.Extension),
                    new XAttribute("LastAccessTime", fi.LastAccessTime),
                    new XAttribute("LastWriteTime", fi.LastWriteTime),
                    new XAttribute("Length", fi.Length)));
        }
    }
}
