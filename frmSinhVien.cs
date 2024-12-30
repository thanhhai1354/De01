using De01.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace De01
{
    public partial class frmSinhVien : Form
    {
        private SinhVienContextDB contextDB;
        private SinhVien selectedSinhvien;

        private List<SinhVien> addedSinhViens = new List<SinhVien>();
        private List<SinhVien> updatedSinhViens = new List<SinhVien>();
        private List<SinhVien> deletedSinhViens = new List<SinhVien>();

        public frmSinhVien()
        {
            InitializeComponent();
        }

        private void frmSinhVien_Load(object sender, EventArgs e)
        {
            LoadForm();  
        }

        private void LoadForm()
        {
            try
            {
                contextDB = new SinhVienContextDB();
                List<SinhVien> listSinhVien = contextDB.SinhVien.ToList();
                List<Lop> listLop = contextDB.Lop.ToList();
                FillLopCMB(listLop);
                BindGrid(listSinhVien);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillLopCMB(List<Lop> listLop)
        {
            this.cmbLop.DataSource = listLop;
            this.cmbLop.DisplayMember = "TenLop";
            this.cmbLop.ValueMember = "MaLop";
        }

        private void BindGrid(List<SinhVien> sinhViens)
        {
            dgvSinhVien.Rows.Clear();
            foreach(var item in sinhViens)
            {
                int index = dgvSinhVien.Rows.Add();
                dgvSinhVien.Rows[index].Cells[0].Value = item.MaSV;
                dgvSinhVien.Rows[index].Cells[1].Value = item.HoTen;
                dgvSinhVien.Rows[index].Cells[2].Value = item.NgaySinh;
                dgvSinhVien.Rows[index].Cells[3].Value = item.Lop != null ? item.Lop.TenLop : "Công Nghệ Thông Tin";
            }
        }



        private void ClearForm()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            txtTim.Clear();
            cmbLop.SelectedIndex = -1;
        }

        private void dgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSinhVien.Rows[e.RowIndex];
                string maSV = row.Cells[0].Value.ToString();
                selectedSinhvien = contextDB.SinhVien.FirstOrDefault(s => s.MaSV == maSV);

                if (selectedSinhvien != null)
                {
                    txtMaSV.Text = selectedSinhvien.MaSV.ToString();
                    txtHoTen.Text = selectedSinhvien.HoTen.ToString();
                    dtNgaySinh.Text = selectedSinhvien.NgaySinh.ToString();
                    cmbLop.SelectedValue = selectedSinhvien.MaLop;

                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                SinhVien sinhvien = new SinhVien()
                {
                    MaSV = txtMaSV.Text,
                    HoTen = txtHoTen.Text,
                    NgaySinh = dtNgaySinh.Value,
                    MaLop = Convert.ToString(cmbLop.SelectedValue)
                };
                
                btnKluu.Enabled = true;
                btnLuu.Enabled = true;

                addedSinhViens.Add(sinhvien);

                List<SinhVien> currentList = contextDB.SinhVien.ToList();
                currentList.Add(sinhvien);
                BindGrid(currentList);

                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (selectedSinhvien != null)
            {
                try
                {
                    DialogResult result = MessageBox.Show($"Bạn có chắc muốn xóa sinh viên có mã số {selectedSinhvien.MaSV}?", "Thông báo", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        btnKluu.Enabled = true;
                        btnLuu.Enabled = true;
                        deletedSinhViens.Add(selectedSinhvien);
                        List<SinhVien> currentList = contextDB.SinhVien.ToList();
                        currentList.Remove(selectedSinhvien);
                        BindGrid(currentList);
                        ClearForm();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sinh viên để xóa");
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (selectedSinhvien != null)
            {
                try
                {
                    selectedSinhvien.MaSV = txtMaSV.Text;
                    selectedSinhvien.HoTen = txtHoTen.Text;
                    selectedSinhvien.NgaySinh = dtNgaySinh.Value;
                    selectedSinhvien.MaLop = Convert.ToString(cmbLop.SelectedValue);

                    if (!updatedSinhViens.Contains(selectedSinhvien))
                    {
                        updatedSinhViens.Add(selectedSinhvien);
                    }

                    btnKluu.Enabled = true;
                    btnLuu.Enabled = true;
                    BindGrid(contextDB.SinhVien.ToList());
                    ClearForm();
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show(ex.Message);
                }
            }else
            {
                MessageBox.Show("Vui lòng chọn một sinh viên để sửa");
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                if (addedSinhViens.Count > 0)
                {
                    contextDB.SinhVien.AddRange(addedSinhViens);
                }

                foreach (var sv in updatedSinhViens)
                {
                    contextDB.Entry(sv).State = System.Data.Entity.EntityState.Modified;
                }

                foreach (var sv in deletedSinhViens)
                {
                    contextDB.SinhVien.Remove(sv);
                }

                contextDB.SaveChanges();
                btnLuu.Enabled = false;
                btnKluu.Enabled = false;
                addedSinhViens.Clear();
                updatedSinhViens.Clear();
                deletedSinhViens.Clear();

                BindGrid(contextDB.SinhVien.ToList());
                MessageBox.Show("Lưu thay đổi thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
        }

        private void btnKluu_Click(object sender, EventArgs e)
        {
            LoadForm();
            btnLuu.Enabled = false;
            btnKluu.Enabled = false;
            addedSinhViens.Clear();
            updatedSinhViens.Clear();
            deletedSinhViens.Clear();
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            try
            {
                string searchValue = txtTim.Text.Trim();
                List<SinhVien> filteredSinhViens;

                if (string.IsNullOrEmpty(searchValue))
                {
                    filteredSinhViens = contextDB.SinhVien.ToList();
                }
                else
                {
                    filteredSinhViens = contextDB.SinhVien
                        .Where(sv => sv.HoTen.Contains(searchValue))
                        .ToList();
                }

                BindGrid(filteredSinhViens);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmSinhVien_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show($"Bạn có chắc muốn đóng form?", "Thông báo", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
