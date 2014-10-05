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
            int ik,rk,zw,cc;

            try 
            {
                int.TryParse(textBox1.Text, out ik);
                int.TryParse(textBox2.Text, out rk);
                int.TryParse(textBox3.Text, out zw);
                int.TryParse(textBox4.Text, out cc);
            }
            catch (Exception sebek)
            {
                MessageBox.Show("Twoje argumenty są inwalidą");
                return;
            }
            //MapGenerator mapka = new MapGenerator(ik,rk,zw/100);
            MapGenerator mapka = new MapGenerator(750, 750, ik, rk, zw, cc);
            if (mapka.Generate())
            {
                pictureBox1.Image = mapka.getBitmap();
            }
            else
            {
                MessageBox.Show("Masz pecha seba");
                return;
            }
            Random rand = new Random();
            for (int i = 0; i < 1000; i++)
            {
                //reuse this if you are generating many
                double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
                double u2 = rand.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                             Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
                double randNormal = (375 + 200 * randStdNormal);
               // double randNormal = mean + stdDev * randStdNormal;
                //Console.WriteLine(randNormal.ToString());
                logTextBox.Text += randNormal.ToString() +"\n";
            }

            //logTextBox.Text = mapka.getLog();
        }

    }
}
