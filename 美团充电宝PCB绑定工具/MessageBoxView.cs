using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 美团充电宝PCB绑定工具
{
    public partial class MessageBoxView : Form
    {
        public MessageBoxView()
        {
            InitializeComponent();
            
        }

        public void ShowTips(string tips)
        {
            Tips.Text = tips;
            Tips.ForeColor = Color.Red;
        }

        private void Yes_Btn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void No_Btn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }
    }
}
