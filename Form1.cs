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
using System.IO;

namespace workbase
{
    public partial class Form1 : Form
    {
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
        }

        private void добавитьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2(this);
            f2.ShowDialog();
            textBox1_Value();
        }

        private void удалитьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (int ind in listView1.SelectedIndices)
                files.RemoveAt(ind);
            show_files();
            textBox1_Value();
        }

        private void редактироватьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int sel = -1;
            foreach (int ind in listView1.SelectedIndices)
                sel = ind;
            if (sel != -1)
            {
                Form2 f2 = new Form2(this, files[sel].name, Convert.ToString(files[sel].value1), Convert.ToString(files[sel].value2));
                f2.ShowDialog();
            }
            textBox1_Value();
        }

        private void textBox1_Value()
        {
            int sum = 0;
            foreach (File ind in files)
            {
                sum += ind.sum;
                textBox1.Text = Convert.ToString(sum);
            }
        }

        private bool save_Files()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "txt files (*.txt)|*.txt";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string path = saveFileDialog.FileName;
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
                    return true;
                }
                else return false;
            }
        }

        private void сохранитьДанныеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool p = save_Files();
            if (p == true)
            {
                MessageBox.Show("Введите путь сохранения XML файла (вторичный файл)", "Сообщение");
                write_xml();
            }
            else return;
        }

        public string openFile()
        {
            string pathToFile = null;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "txt files (*.txt)|*.txt";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pathToFile = openFileDialog.FileName;
                }
            }
            return pathToFile;
        }
        public void write_xml()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "xml files (*.xml)|*.xml";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    XmlWriter w = XmlWriter.Create(saveFileDialog.FileName);
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
                }
                else return;
            }
        }
        public void read_xml()
        {
            XmlDocument r = new XmlDocument();
            string n = "";
            int v1 = 0;
            int v2 = 0;
            int s = 0;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "xml files (*.xml)|*.xml";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    r.Load(openFileDialog.FileName);
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
                                    v1 = Convert.ToInt32(ch.InnerText);
                                    break;
                                case "count_of_finished":
                                    v2 = Convert.ToInt32(ch.InnerText);
                                    break;
                                case "sum_of_work":
                                    s = Convert.ToInt32(ch.InnerText);
                                    break;
                            }
                        }
                        files.Add(new File(n, v1, v2, s));
                    }
                    show_files();
                }
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
            read_xml();
        }
    }
    public class File
    {
        public string name;
        public int value1;
        public int value2;
        public int sum;
        public File(string n, int v1, int v2, int s)
        {
            name = n;
            value1 = v1;
            value2 = v2;
            sum = s;
        }

        public List<string> get_columns(bool n, bool v1,bool v2, bool s)
        {
            var el1=new List<string>();
            if (n) el1.Add(name);
            if (v1) el1.Add(Convert.ToString(value1));
            if (v2) el1.Add(Convert.ToString(value2));
            if (s) el1.Add(Convert.ToString(sum));
            return el1;
        }
    }
}
