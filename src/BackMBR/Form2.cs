using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackMBR
{
    public partial class Form2 : Form
    {
        public string title;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Generating a setting the random title
            const string chars = "qwertyuiopasdfghjklzxcvbnm"; // The chars for random title
            StringBuilder stringBuilder = new StringBuilder(12);
            Random random = new Random();

            for (int i = 0; i < 12; i++)
            {
                title = stringBuilder.Append(chars[random.Next(chars.Length)]).ToString(); // The random title
            }

            this.Text = title; // Setting title to form

            stringBuilder.Clear(); // Cleaning symbols
        }
    }
}
