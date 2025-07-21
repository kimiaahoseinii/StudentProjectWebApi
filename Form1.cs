using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentApp2
{
    public partial class Form1 : Form
    {

        HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:5001/api/students")
        };
        public Form1()
        {
            InitializeComponent();
        }


        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                var students = await _client.GetFromJsonAsync<List<Student>>("api/students");
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = students;
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در بارگذاری لیست: " + ex.Message);
            }
        }




        private async void btnGet_Click(object sender, EventArgs e)
        {
            var response = await _client.GetAsync("");
            var json = await response.Content.ReadAsStringAsync();
            var students = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Student>>(json);
            dataGridView1.DataSource = students;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            var student = new Student
            {
                FullName = txtName1.Text,
                NationalCode = txtCode1.Text,
                BirthDate = DateTime.Parse(txtDate1.Text)
            };

            var content = new StringContent(JsonConvert.SerializeObject(student), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("", content);

            if (response.IsSuccessStatusCode)
                MessageBox.Show("دانشجو با موفقیت اضافه شد.");

            btnGet_Click(null, null);
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            var student = new Student
            {
                StudentId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Id"].Value),
                FullName = txtName1.Text,
                NationalCode = txtCode1.Text,
                BirthDate = DateTime.Parse(txtDate1.Text)
            };

            var content = new StringContent(JsonConvert.SerializeObject(student), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"/{student.Id}", content);

            if (response.IsSuccessStatusCode)
                MessageBox.Show("ویرایش انجام شد.");

            btnGet_Click(null, null);
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Id"].Value);
            var response = await _client.DeleteAsync($"/{id}");

            if (response.IsSuccessStatusCode)
                MessageBox.Show("دانشجو حذف شد.");

            btnGet_Click(null, null);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            txtName1.Text = dataGridView1.CurrentRow.Cells["FullName"].Value.ToString();
            txtCode1.Text = dataGridView1.CurrentRow.Cells["NationalCode"].Value.ToString();
            txtDate1.Text = Convert.ToDateTime(dataGridView1.CurrentRow.Cells["BirthDate"].Value).ToString("yyyy-MM-dd");
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private async void txtName1_TextChanged(object sender, EventArgs e)
        {
            var students = await _client.GetFromJsonAsync<List<Student>>("api/students");
            string searchText = txtName1.Text.Trim().ToLower();
            var filtered = students
                .Where(s => s.FullName != null && s.FullName.ToLower().Contains(searchText))
                .ToList();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = filtered;
        }

        private async void txtCode1_TextChanged(object sender, EventArgs e)
        {
            var students = await _client.GetFromJsonAsync<List<Student>>("api/students");
            string searchText = txtCode1.Text.Trim();
            var filtered = students
                .Where(s => s.NationalCode != null && s.NationalCode.Contains(searchText))
                .ToList();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = filtered;
        }

        private async  void txtDate1_TextChanged(object sender, EventArgs e)
        {
            var students = await _client.GetFromJsonAsync<List<Student>>("api/students");
            string searchText = txtDate1.Text.Trim();
            var filtered = students
                .Where(s => s.BirthDate.ToString("yyyy/MM/dd").Contains(searchText))
                .ToList();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = filtered;
        }
    }

    internal class Student
    {
        public string FullName { get; set; }
        public string NationalCode { get; set; }
        public DateTime BirthDate { get; set; }
        public int StudentId { get; internal set; }
        public object Id { get; internal set; }
    }
}

 
 
