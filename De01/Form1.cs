using De01.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace De01
{
    public partial class frmSinhVien : Form
    {

        public frmSinhVien()
        {
            InitializeComponent();

        }
        private void BindGrid(List<Sinhvien> listStudents)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudents)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = item.MaSV;
                dgvStudent.Rows[index].Cells[1].Value = item.HoTenSV;
                dgvStudent.Rows[index].Cells[2].Value = item.NgaySinh;
                dgvStudent.Rows[index].Cells[3].Value = item.Lop.TenLop;
            }
        }
        private void FillClassCombobox(List<Lop> listClasses)
        {
            this.cboLop.DataSource = listClasses;
            this.cboLop.DisplayMember = "TenLop";
            this.cboLop.ValueMember = "MaLop";
        }

        private void frmSinhVien_Load(object sender, EventArgs e)
        {
            StudentContextDB context = new StudentContextDB();
            List<Lop> listClasses = context.Lops.ToList();
            List<Sinhvien> listStudents = context.Sinhviens.ToList();
            BindGrid(listStudents);
            FillClassCombobox(listClasses);
        }

        private bool CheckStudentID(string mssv)
        {
            using (StudentContextDB context = new StudentContextDB())
            {
                return context.Sinhviens.Any(s => s.MaSV == mssv);
            }
        }



        private void btnSua_Click(object sender, EventArgs e)
        {
            string mssv = txtMaSV.Text;
            if (string.IsNullOrEmpty(mssv))
            {
                MessageBox.Show("Vui lòng nhập mã số sinh viên!");
                return;
            }

            using (StudentContextDB context = new StudentContextDB())
            {
                Sinhvien dbEdit = context.Sinhviens.FirstOrDefault(s => s.MaSV == mssv);
                if (dbEdit == null)
                {
                    MessageBox.Show("Mã số sinh viên không tồn tại!");
                    return;
                }

                DialogResult res = MessageBox.Show("Bạn có muốn cập nhật thông tin sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    dbEdit.HoTenSV = txtHotenSV.Text;
                    dbEdit.NgaySinh = dtNgaySinh.Value;
                    dbEdit.MaLop = cboLop.SelectedValue.ToString();

                    context.SaveChanges();
                    MessageBox.Show("Cập nhật dữ liệu thành công!");
                    List<Sinhvien> students = context.Sinhviens.ToList();
                    BindGrid(students);
                    ResetInputField();
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                string mssv = txtMaSV.Text;
                string fullname = txtHotenSV.Text;
                DateTime birthday = dtNgaySinh.Value;
                string malop = cboLop.SelectedValue.ToString();

                if (string.IsNullOrEmpty(mssv) || string.IsNullOrEmpty(fullname))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                DialogResult res = MessageBox.Show("Bạn có muốn lưu sinh viên này vào database?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    using (StudentContextDB context = new StudentContextDB())
                    {
                        // Kiểm tra lớp học có tồn tại không
                        Lop lops = context.Lops.FirstOrDefault(l => l.MaLop == malop);
                        if (lops == null)
                        {
                            MessageBox.Show("Lớp học không tồn tại!");
                            return;
                        }

                        // Thêm sinh viên mới
                        Sinhvien newSV = new Sinhvien
                        {
                            MaSV = mssv,
                            HoTenSV = fullname,
                            NgaySinh = birthday,
                            MaLop = malop,
                            Lop = lops,
                        };

                        context.Sinhviens.Add(newSV);
                        context.SaveChanges();
                        MessageBox.Show("Lưu sinh viên thành công!");

                        // Cập nhật lại danh sách hiển thị
                        List<Sinhvien> listStudents = context.Sinhviens.ToList();
                        BindGrid(listStudents);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu sinh viên: " + ex.Message);
            }
        }

        private void btnKhong_Click(object sender, EventArgs e)
        {

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Bạn có muốn đóng?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            string mssv = txtMaSV.Text;
            string fullname = txtHotenSV.Text;
            DateTime birthday = dtNgaySinh.Value;
            string lophoc = cboLop.Text;
            string malop = cboLop.SelectedValue.ToString();
            //MessageBox.Show(mssv + fullname + birthday + lophoc);
            if (string.IsNullOrEmpty(mssv) || string.IsNullOrEmpty(fullname))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }
            else if (CheckStudentID(mssv))
            {
                MessageBox.Show("Sinh viên đã có trong danh sách!");
                return;
            }

            DataGridViewRow newRow = new DataGridViewRow();
            newRow.Cells.Add(new DataGridViewTextBoxCell { Value = mssv });
            newRow.Cells.Add(new DataGridViewTextBoxCell { Value = fullname });
            newRow.Cells.Add(new DataGridViewTextBoxCell { Value = birthday });
            newRow.Cells.Add(new DataGridViewTextBoxCell { Value = lophoc });

            dgvStudent.Rows.Add(newRow);
            btnLuu.Enabled = true;
            btnKhong.Enabled = true;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                string mssv = txtMaSV.Text;
                if (string.IsNullOrEmpty(mssv))
                {
                    MessageBox.Show("Vui lòng nhập mã số sinh viên cần xoá!");
                    return;
                }
                using (StudentContextDB context = new StudentContextDB())
                {
                    Sinhvien dbDelete = context.Sinhviens.FirstOrDefault(s => s.MaSV == mssv);
                    if (dbDelete == null)
                    {
                        MessageBox.Show("Không tìm thấy sinh viên cần xoá!");
                        return;
                    }

                    DialogResult res = MessageBox.Show("Bạn có muốn xoá sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                        context.Sinhviens.Remove(dbDelete);
                        context.SaveChanges();
                        MessageBox.Show("Xoá sinh viên thành công!");
                        List<Sinhvien> listSV = context.Sinhviens.ToList();
                        BindGrid(listSV);
                        ResetInputField();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ResetInputField()
        {
            txtMaSV.Clear();
            txtHotenSV.Clear();
        }

        private void frmSinhVien_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult res = MessageBox.Show("Bạn có muốn đóng?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void dgvStudent_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStudent.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvStudent.SelectedRows[0];
                txtMaSV.Text = selectedRow.Cells[0].Value?.ToString();
                txtHotenSV.Text = selectedRow.Cells[1].Value?.ToString();
                if (DateTime.TryParse(selectedRow.Cells[2].Value?.ToString(), out DateTime ngaysinh))
                {
                    dtNgaySinh.Value = ngaysinh;
                }

                string tenlop = selectedRow.Cells[3].Value?.ToString();
                cboLop.SelectedIndex = cboLop.FindStringExact(tenlop);
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string searchName = txtHotenSV.Text.Trim();
            if (string.IsNullOrEmpty(searchName))
            {
                MessageBox.Show("Vui lòng nhập họ hoặc tên sinh viên để tìm kiếm!");
                return;
            }

            using (StudentContextDB context = new StudentContextDB())
            {
                // Tìm kiếm sinh viên theo họ hoặc tên
                var filteredStudents = context.Sinhviens
                    .Where(s => s.HoTenSV.ToLower().Contains(searchName.ToLower()))
                    .ToList();

                if (filteredStudents.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sinh viên nào có tên phù hợp!");
                }
                else
                {
                    BindGrid(filteredStudents);
                }
            }
        }

    }
}   

    

