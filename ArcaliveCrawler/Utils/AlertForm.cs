using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArcaliveCrawler.Utils
{
    public partial class AlertForm : Form
    {
        public AlertForm()
        {
            InitializeComponent();
        }

        public enum AlertFormState
        {
            Start, Wait, Close
        }

        public AlertFormState state;

        private void AlertForm_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AlertForm_Click(sender, e);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            AlertForm_Click(sender, e);
        }
    }
}
