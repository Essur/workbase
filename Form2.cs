using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workbase
{
    public partial class Form2 : Form
    {
        public int file_index;
        public Form1 f1;
        public Form2(Form1 f2, string n = "", string v1 = "", string v2 = "", int cn =-1)
        {
            InitializeComponent();
            textBox1.Text = n;
            textBox2.Text = v1;
            textBox3.Text = v2;
            file_index = cn;
            f1 = f2;
        }
        public Form2()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int a, b;
            if (textBox2.Text == string.Empty) a = 0;
            else a = Convert.ToInt32(textBox2.Text);
            if (textBox3.Text == string.Empty) b = 0;
            else b = Convert.ToInt32(textBox3.Text);
            int m =a * b;
            if (file_index == -1)
            {
                f1.files.Add(new File(textBox1.Text, a, b, m));
                f1.show_files();
            }
            else
            {
                File f = new File(textBox1.Text, a, b, m);
                f1.files[file_index] = f;
                f1.show_files();
            }
            Close();
        }
    }
}
