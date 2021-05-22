using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArcaliveForm
{
    public partial class AddDataForm : Form
    {
        public AddDataForm()
        {
            InitializeComponent();
        }

        public AddDataForm(DateTime orgStartTime, DateTime orgEndTime)
            : this()
        {
            dateTimePicker1.Value = dateTimePicker2.Value = orgStartTime;
            dateTimePicker3.Value = dateTimePicker4.Value = orgEndTime;
        }

        // 확인 버튼
        private void button1_Click(object sender, EventArgs e)
        {
            
            DialogResult = DialogResult.OK;
        }

        public List<DateTime> SendDateTime()
        {
            DateTime startTime = dateTimePicker1.Value.Date + dateTimePicker2.Value.TimeOfDay;
            DateTime endTime = dateTimePicker3.Value.Date + dateTimePicker4.Value.TimeOfDay;
            return new List<DateTime> {startTime, endTime};
        }
    }
}
