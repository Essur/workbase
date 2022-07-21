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
            double pricePerPiece;
            int numberOfMade;
            if (textBox2.Text == string.Empty) pricePerPiece = 0;
            else pricePerPiece = Convert.ToDouble(textBox2.Text);
            if (textBox3.Text == string.Empty) numberOfMade = 0;
            else numberOfMade = Convert.ToInt32(textBox3.Text);
            double multiplication = pricePerPiece * numberOfMade;
            if (file_index == -1)
            {
                f1.files.Add(new File(textBox1.Text, pricePerPiece, numberOfMade, multiplication));
                f1.show_files();
            }
            else
            {
                f1.files.RemoveAt(file_index);
                f1.files.Insert(file_index, new File(textBox1.Text, pricePerPiece, numberOfMade, multiplication));
                f1.show_files();
            }
            Close();
        }
    }
}
