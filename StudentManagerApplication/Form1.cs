using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentManagerApplication
{
    public partial class Form1 : Form
    {
        List<Student> studentList = new List<Student>();
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboGender.Items.Add("Nam");
            cboGender.Items.Add("Nữ");
            cboGender.Items.Add("Khác");

            // Chọn giá trị mặc định
            cboGender.SelectedIndex = 0; // chọn "Nam"

        }
        private void LoadDataToGrid()
        {
            dgvStudents.DataSource = null;
            dgvStudents.DataSource = studentList;
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Kiểm tra nhập đầy đủ thông tin
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtClass.Text) ||
                cboGender.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin sinh viên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Kiểm tra trùng ID
            if (studentList.Any(item => item.ID == txtID.Text))
            {
                MessageBox.Show("Mã SV này đã tồn tại! Vui lòng nhập mã khác.", "Lỗi trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 3. Kiểm tra Tên
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Chưa nhập tên!");
                return;
            }
            // Kiểm tra điểm hợp lệ
            double score;
            if (!double.TryParse(txtScore.Text, out score) || score < 0 || score > 10)
            {
                MessageBox.Show("Điểm phải là số từ 0 đến 10!");
                return;
            }
            // Tạo đối tượng Student và thêm vào danh sách
            Student s = new Student();
            s.ID = txtID.Text;
            s.FullName = txtName.Text;
            s.ClassName = txtClass.Text;
            s.Gender = cboGender.Text;
            s.Score = score;
            studentList.Add(s);
            LoadDataToGrid();
        }

        private void dgvStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Lấy dòng hiện tại được chọn
                DataGridViewRow row = dgvStudents.Rows[e.RowIndex];

                // Lấy dữ liệu từ các ô trong dòng đó
                // (Lưu ý: Tên "ID", "FullName"... phải khớp với tên thuộc tính trong class Student)
                txtID.Text = row.Cells["ID"].Value.ToString();
                txtID.Enabled = false;
                txtName.Text = row.Cells["FullName"].Value.ToString();
                txtClass.Text = row.Cells["ClassName"].Value.ToString();
                cboGender.Text = row.Cells["Gender"].Value.ToString();
                txtScore.Text = row.Cells["Score"].Value.ToString();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // 1. Lấy Mã SV từ ô txtID để biết cần sửa ai
            string idToUpdate = txtID.Text;

            // 2. Tìm sinh viên trong "kho" (studentList)
            // Dùng LINQ (Find) để tìm SV có ID trùng
            Student studentToUpdate = studentList.Find(sv => sv.ID == idToUpdate);

            // 3. Nếu tìm thấy
            if (studentToUpdate != null)
            {
                // 4. Cập nhật lại thông tin cho SV đó từ các ô textbox
                studentToUpdate.FullName = txtName.Text;
                studentToUpdate.ClassName = txtClass.Text;
                studentToUpdate.Gender = cboGender.Text;
                studentToUpdate.Score = double.Parse(txtScore.Text);
                // (Ta cũng có thể cập nhật cả ID nếu muốn)
                // studentToUpdate.ID = txtID.Text; 

                // 5. Gọi hàm LoadDataToGrid() để làm mới bảng
                LoadDataToGrid();

                MessageBox.Show("Cập nhật thông tin thành công!");
            }
            else
            {
                MessageBox.Show("Không tìm thấy sinh viên để cập nhật. Hãy chọn một sinh viên từ bảng.");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 1. Tạo hộp thoại Save File
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt";
            sfd.FileName = "DanhSachSinhVien.csv"; // Tên mặc định
            sfd.Title = "Lưu Danh Sách Sinh Viên";

            // 2. Nếu người dùng bấm OK
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filePath = sfd.FileName; // Lấy đường dẫn người dùng chọn

                    // Sử dụng StreamWriter với Encoding UTF8 để không lỗi font tiếng Việt
                    using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                    {
                        // Ghi tiêu đề cột (Header) - Tùy chọn
                        // writer.WriteLine("ID,FullName,ClassName,Gender");

                        // Duyệt danh sách và ghi từng dòng
                        foreach (Student s in studentList)
                        {
                            // Format: ID,Tên,Lớp,Giới tính
                            string line = $"{s.ID},{s.FullName},{s.ClassName},{s.Gender}";
                            writer.WriteLine(line);
                        }
                    }
                    MessageBox.Show("Lưu dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem có dòng nào đang được chọn không
            if (dgvStudents.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa!");
                return;
            }

            // 2. Lấy ID của dòng đang chọn
            string studentID = dgvStudents.CurrentRow.Cells["ID"].Value.ToString();

            // 3. Hỏi xác nhận cho chắc chắn
            DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa sinh viên có mã {studentID} không?",
                                                  "Xác nhận xóa",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // 4. Tìm và xóa trong List
                Student studentToRemove = studentList.Find(s => s.ID == studentID);
                if (studentToRemove != null)
                {
                    studentList.Remove(studentToRemove);

                    // 5. Cập nhật lại bảng
                    LoadDataToGrid();
                    MessageBox.Show("Đã xóa thành công!");

                    // Xóa trắng các ô nhập liệu
                    txtID.Text = ""; txtName.Text = ""; txtClass.Text = "";
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtName.Text.ToLower(); // Lấy từ khóa và chuyển về chữ thường

            if (string.IsNullOrWhiteSpace(keyword))
            {
                // Nếu không nhập gì thì hiện lại toàn bộ danh sách
                LoadDataToGrid();
                return;
            }

            // Lọc danh sách: Tìm những SV mà Tên có chứa từ khóa
            List<Student> searchResult = studentList.FindAll(s => s.FullName.ToLower().Contains(keyword));

            // Hiển thị kết quả tìm được lên bảng
            dgvStudents.DataSource = null;
            dgvStudents.DataSource = searchResult;

            MessageBox.Show($"Tìm thấy {searchResult.Count} sinh viên.");
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            // 1. Tạo hộp thoại Open File
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt";
            ofd.Title = "Mở File Dữ Liệu Sinh Viên";

            // 2. Nếu người dùng chọn file và bấm OK
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filePath = ofd.FileName;

                    // Xóa danh sách cũ trước khi nạp mới (tùy nhu cầu)
                    studentList.Clear();

                    // Đọc toàn bộ các dòng trong file
                    string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

                    foreach (string line in lines)
                    {
                        // Bỏ qua dòng trống
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        // Tách chuỗi bằng dấu phẩy
                        string[] parts = line.Split(',');

                        // Kiểm tra xem dòng có đủ 4 cột dữ liệu không (ID, Tên, Lớp, Giới tính)
                        if (parts.Length >= 4)
                        {
                            Student s = new Student();
                            s.ID = parts[0].Trim();
                            s.FullName = parts[1].Trim();
                            s.ClassName = parts[2].Trim();
                            s.Gender = parts[3].Trim();

                            // Thêm vào danh sách
                            studentList.Add(s);
                        }
                    }

                    // Cập nhật lại bảng hiển thị
                    LoadDataToGrid();
                    MessageBox.Show($"Đã tải {studentList.Count} sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi đọc file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem danh sách có dữ liệu không
            if (studentList.Count == 0)
            {
                MessageBox.Show("Danh sách sinh viên đang trống, không thể sắp xếp.");
                return;
            }

            // 2. Thực hiện sắp xếp danh sách (LINQ OrderBy)
            // Sắp xếp theo FullName (Tên đầy đủ), theo thứ tự Alphabet (tăng dần)
            // Nếu muốn sắp xếp theo Điểm (Score): studentList = studentList.OrderBy(s => s.Score).ToList();
            // Nếu muốn sắp xếp giảm dần: studentList = studentList.OrderByDescending(s => s.FullName).ToList();

            // LINQ OrderBy trả về IEnumerable<Student>, cần .ToList() để chuyển lại thành List<Student>
            studentList = studentList.OrderBy(s => s.FullName).ToList();

            // 3. Tải lại dữ liệu lên DataGridView để hiển thị kết quả
            LoadDataToGrid();

            MessageBox.Show("Đã sắp xếp danh sách sinh viên theo Tên (tăng dần).");
        }


        private void btnAverage_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra danh sách có sinh viên nào không
            if (studentList.Count == 0)
            {
                MessageBox.Show("Danh sách sinh viên trống, không thể tính điểm trung bình.");
                return;
            }

            try
            {
                // 2. Sử dụng LINQ để tính điểm trung bình (Average)
                // Lấy tất cả các giá trị Score và tính trung bình của chúng.
                double averageScore = studentList.Average(s => s.Score);

                // 3. Hiển thị kết quả cho người dùng
                MessageBox.Show($"Điểm trung bình của tất cả sinh viên là: {averageScore:F2}", // :F2 để làm tròn 2 chữ số thập phân
                                "Điểm Trung Bình",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có (ví dụ: lỗi chuyển đổi kiểu dữ liệu nếu Score không phải double/int)
                MessageBox.Show($"Đã xảy ra lỗi khi tính điểm trung bình: {ex.Message}", "Lỗi Tính Toán", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            // Xóa trắng nội dung của các ô TextBox
            txtID.Text = "";
            txtID.Enabled = true;
            txtName.Text = "";
            txtClass.Text = "";
            txtScore.Text = "";

            // Đặt lại ComboBox về giá trị mặc định (ví dụ: chọn mục đầu tiên là "Nam")
            if (cboGender.Items.Count > 0)
            {
                cboGender.SelectedIndex = 0;
            }

            // Đưa con trỏ chuột về ô nhập liệu đầu tiên (tùy chọn)
            txtID.Focus();

            MessageBox.Show("Đã xóa trắng các ô nhập liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnStatistic_Click(object sender, EventArgs e)
        {
            int total = studentList.Count;
            int passCount = 0; // Số sinh viên Đạt (>= 5)
            int failCount = 0; // Số sinh viên Rớt (< 5)

            // Duyệt danh sách để đếm
            foreach (Student s in studentList)
            {
                if (s.Score >= 5)
                {
                    passCount++;
                }
                else
                {
                    failCount++;
                }
            }

            // Hiện thông báo thống kê
            string msg = $"Tổng số sinh viên: {total}\n" +
                         $"- Số sinh viên Đạt (>= 5): {passCount}\n" +
                         $"- Số sinh viên Không đạt (< 5): {failCount}";

            MessageBox.Show(msg, "Thống kê tình hình học tập", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // Tạo form biểu đồ
            ChartForm chart = new ChartForm();
            // Truyền dữ liệu đã tính sang ChartForm
            chart.SetData(passCount, failCount);
            // Hiển thị form
            chart.ShowDialog();
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem có dòng nào đang được chọn không
            if (dgvStudents.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa!");
                return;
            }

            // 2. Lấy ID của dòng đang chọn
            string studentID = dgvStudents.CurrentRow.Cells["ID"].Value.ToString();

            // 3. Hỏi xác nhận cho chắc chắn
            DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa sinh viên có mã {studentID} không?",
                                                  "Xác nhận xóa",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // 4. Tìm và xóa trong List
                Student studentToRemove = studentList.Find(s => s.ID == studentID);
                if (studentToRemove != null)
                {
                    studentList.Remove(studentToRemove);

                    // 5. Cập nhật lại bảng
                    LoadDataToGrid();
                    MessageBox.Show("Đã xóa thành công!");

                    // Xóa trắng các ô nhập liệu
                    txtID.Text = ""; txtName.Text = ""; txtClass.Text = "";
                }
            }
        }
    }
}