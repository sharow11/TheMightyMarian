using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DuzeIlosciNaRazMapek
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            logTextBox.Text = "";
            int ik;
            int rk;
            float zw;
            try 
            {
                int.TryParse(textBox1.Text, out ik);
                int.TryParse(textBox2.Text, out rk);
                float.TryParse(textBox1.Text, out zw);
            }
            catch (Exception sebek)
            {
                MessageBox.Show("Twoje argumenty są inwalidą");
                return;
            }
            MapGenerator mapka = new MapGenerator(ik,rk,zw/100);
            if (mapka.doMagic())
            {
                pictureBox1.Image = mapka.getBitmap();
            }
            else
            {
                MessageBox.Show("Masz pecha seba");
                return;
            }
            logTextBox.Text = mapka.getLog();
        }

    }
}
