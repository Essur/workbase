using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace workbase
{
    public partial class Form1 : Form
    {
        private string pathToFile;
        private string pathToTxt;
        public List<File> files = new List<File>();

        public bool n = true;
        public bool v1 = true;
        public bool v2 = true;
        public bool s = true;

        private void recreate_columns()
        {
            listView1.Columns.Add("Название макета");
            listView1.Columns.Add("Стоимость за штуку");
            listView1.Columns.Add("Кол-во сделаных экземпляров");
            listView1.Columns.Add("Стоимость сделаной работы за этот макет");
            show_files();
        }
        public void show_files()
        {
            listView1.Items.Clear();
            ListViewItem li;
            foreach (File pr in files)
            {
                li = listView1.Items.Add(pr.name);
                li.SubItems.AddRange(pr.get_columns(false, v1, v2, s).ToArray());
                li.Tag = pr;
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            textBox1_Value();
        }
        public Form1()
        {
            InitializeComponent();
            recreate_columns();
            this.KeyPreview = true;
        }

        private void добавитьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2(this);
            f2.ShowDialog();
            textBox1_Value();
        }

        private void удалитьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                MessageBox.Show("Не выбраны елементы для удаления!", "Сообщение");
            else
            {
                foreach (int ind in listView1.SelectedIndices)
                    files.RemoveAt(ind);
                show_files();
                textBox1_Value();
            }
        }

        private void редактироватьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int sel = -1;
            foreach (int ind in listView1.SelectedIndices)
                sel = ind;
            if (sel != -1)
            {
                Form2 f2 = new Form2(this, files[sel].name, Convert.ToString(files[sel].value1), Convert.ToString(files[sel].value2), sel);
                f2.ShowDialog();
            }
            textBox1_Value();
        }

        private void textBox1_Value()
        {
            double sum = 0;
            foreach (File ind in files)
            {
                sum += ind.sum;
                textBox1.Text = Convert.ToString(sum);
            }
        }

        private string save_Files()
        {
            if (pathToFile == null)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "txt files (*.txt)|*.txt";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string path = saveFileDialog.FileName;
                        pathToFile = path.Trim('.', 't', 'x', 't');
                        pathToTxt = path;
                        using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
                        {
                            foreach (File ind in files)
                            {
                                byte[] array = Encoding.Default.GetBytes(ind.name + " -" + ind.value1 + " грн" + " -" + ind.value2 + " шт." + " -" + ind.sum + " грн" + "\n");
                                fstream.Write(array, 0, array.Length);
                            }
                            byte[] array1 = Encoding.Default.GetBytes("\nВсего: " + textBox1.Text + " грн");
                            fstream.Write(array1, 0, array1.Length);
                        }
                        return path;
                    }
                    else return "cancel";
                }
            }
            else
            {
                using (FileStream fstream = new FileStream(pathToTxt, FileMode.OpenOrCreate))
                {
                    foreach (File ind in files)
                    {
                        byte[] array = Encoding.Default.GetBytes(ind.name + " -" + ind.value1 + " грн" + " -" + ind.value2 + " шт." + " -" + ind.sum + " грн" + "\n");
                        fstream.Write(array, 0, array.Length);
                    }
                    byte[] array1 = Encoding.Default.GetBytes("\nВсего: " + textBox1.Text + " грн");
                    fstream.Write(array1, 0, array1.Length);
                }
            }
            return null;

        }

        public void write_xml(string path)
        {
            if (path == "cancel")
            {
                
            }
            else
            {
                if (pathToFile == null)
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "xml files (*.xml)|*.xml";
                        saveFileDialog.FilterIndex = 2;
                        saveFileDialog.RestoreDirectory = true;

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            XmlWriter w = XmlWriter.Create(path + ".xml");
                            w.WriteStartDocument();
                            w.WriteStartElement("layouts");
                            foreach (File p in files)
                            {
                                w.WriteStartElement("layout");
                                w.WriteElementString("name", p.name);
                                w.WriteElementString("count_of_one_object", Convert.ToString(p.value1));
                                w.WriteElementString("count_of_finished", Convert.ToString(p.value2));
                                w.WriteElementString("sum_of_work", Convert.ToString(p.sum));
                                w.WriteEndElement();
                            }
                            w.WriteEndElement();
                            w.WriteEndDocument();
                            w.Flush();
                            w.Close();
                        }
                    }
                }
                else
                {
                    pathToFile.Trim('.', 'x', 'm', 'l');
                    XmlWriter w = XmlWriter.Create(pathToFile + ".xml");
                    w.WriteStartDocument();
                    w.WriteStartElement("layouts");
                    foreach (File p in files)
                    {
                        w.WriteStartElement("layout");
                        w.WriteElementString("name", p.name);
                        w.WriteElementString("count_of_one_object", Convert.ToString(p.value1));
                        w.WriteElementString("count_of_finished", Convert.ToString(p.value2));
                        w.WriteElementString("sum_of_work", Convert.ToString(p.sum));
                        w.WriteEndElement();
                    }
                    w.WriteEndElement();
                    w.WriteEndDocument();
                    w.Flush();
                    w.Close();
                }
            }
        }

        private void сохранитьДанныеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            write_xml(save_Files());
        }

        public string openFile()
        {
            string pathToFile = null;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "xml files (*.xml)|*.xml";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pathToFile = openFileDialog.FileName;
                }
            }
            return pathToFile;
        }

        public void read_xml()
        {
            XmlDocument r = new XmlDocument();
            string n = "";
            double v1 = 0;
            int v2 = 0;
            double s = 0;
            if(pathToFile == null)
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "xml files (*.xml)|*.xml";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        r.Load(openFileDialog.FileName);
                        pathToFile = openFileDialog.FileName.Trim('.', 'x', 'm', 'l');
                        pathToTxt = openFileDialog.FileName.Trim('.', 'x', 'm', 'l');
                        pathToTxt = pathToTxt + ".txt";
                        foreach (XmlNode node in r.GetElementsByTagName("layout"))
                        {
                            foreach (XmlNode ch in node.ChildNodes)
                            {
                                switch (ch.Name)
                                {
                                    case "name":
                                        n = ch.InnerText;
                                        break;
                                    case "count_of_one_object":
                                        v1 = Convert.ToDouble(ch.InnerText);
                                        break;
                                    case "count_of_finished":
                                        v2 = Convert.ToInt32(ch.InnerText);
                                        break;
                                    case "sum_of_work":
                                        s = Convert.ToDouble(ch.InnerText);
                                        break;
                                }
                            }
                            files.Add(new File(n, v1, v2, s));
                        }
                        show_files();
                    }
                }
            }
            else
            {
                r.Load(pathToFile);
                pathToFile = pathToFile.Trim('.', 'x', 'm', 'l');
                pathToTxt = pathToFile.Trim('.', 'x', 'm', 'l');
                pathToTxt = pathToTxt + ".txt";
                foreach (XmlNode node in r.GetElementsByTagName("layout"))
                {
                    foreach (XmlNode ch in node.ChildNodes)
                    {
                        switch (ch.Name)
                        {
                            case "name":
                                n = ch.InnerText;
                                break;
                            case "count_of_one_object":
                                v1 = Convert.ToDouble(ch.InnerText);
                                break;
                            case "count_of_finished":
                                v2 = Convert.ToInt32(ch.InnerText);
                                break;
                            case "sum_of_work":
                                s = Convert.ToDouble(ch.InnerText);
                                break;
                        }
                    }
                    files.Add(new File(n, v1, v2, s));
                }
                show_files();
            }
            
        }

        private void закрытьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
              "Закрыть программу?",
              "Внимание!",
              MessageBoxButtons.YesNo,
              MessageBoxIcon.Information,
              MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                save_Files();
                Close();
            }
        }

        private void загрузитьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            files.Clear();
            read_xml();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.S)
            {
                write_xml(save_Files());
            }
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            string [] filesDrag = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in filesDrag)
            {
                listView1.Items.Clear();
                files.Clear();
                pathToFile = file;
                read_xml();
            }
        }

        private void закрытьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            files.Clear();
            pathToFile = null;
            pathToTxt = null;
            recreate_columns();
            textBox1.Clear();
        }
    }
    public class File
    {
        public string name;
        public double value1;
        public int value2;
        public double sum;
        public File(string n, double v1, int v2, double s)
        {
            name = n;
            value1 = v1;
            value2 = v2;
            sum = s;
        }

        public List<string> get_columns(bool n, bool v1,bool v2,  bool s)
        {
            var el1 = new List<string>();
            if (n) el1.Add(name);
            if (v1) el1.Add(Convert.ToString(value1));
            if (v2) el1.Add(Convert.ToString(value2));
            if (s) el1.Add(Convert.ToString(sum));
            return el1;
        }
    }  
}
