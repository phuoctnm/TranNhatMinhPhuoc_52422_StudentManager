using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentManagerApplication
{
    public partial class ChartForm : Form
    {
        public ChartForm()
        {
            InitializeComponent();
        }

        // Hàm để form chính truyền số liệu 
        public void SetData(int pass, int fail)
        {
            // xoá dữ liệu mẫu cũ
            chart1.Series[0].Points.Clear();
            // Thêm dữ liệu mới ĐẬU
            chart1.Series[0].Points.AddXY("Đậu", pass);
            chart1.Series[0].Points[0].Color = System.Drawing.Color.Green;
            // RỚT
            chart1.Series[0].Points.AddXY("Rớt", fail);
            chart1.Series[0].Points[0].Color = System.Drawing.Color.Red;
            // Đặt tiêu đề
            chart1.Titles.Clear();
            chart1.Titles.Add("Biểu đồ Tỷ lệ Kết quả Học tập");
        }
    }
}
