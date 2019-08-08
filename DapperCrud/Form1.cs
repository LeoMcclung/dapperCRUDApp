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
using Dapper;

namespace DapperCrud
{
    public partial class Form1 : Form
    {
        SqlConnection sqlCon = new SqlConnection(@"Data Source=DESKTOP-JBDT7TL\SQLEXPRESS;Initial Catalog=DapperCrudDB;Integrated Security=True");

        int contactId = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataGridView();
                Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();
                DynamicParameters param = new DynamicParameters();
                param.Add("@ContactID", contactId);
                param.Add("@Name", txtName.Text.Trim());
                param.Add("@Mobile", txtMobile.Text.Trim());
                param.Add("@Address", txtAddress.Text.Trim());

                sqlCon.Execute("ContactAddOrEdit",param,commandType: CommandType.StoredProcedure);
                if (contactId == 0)
                    MessageBox.Show("Saved Successfully!");
                else
                    MessageBox.Show("Updated Successfully!");
                FillDataGridView();
                Clear();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }
        }

        void FillDataGridView()
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@SearchText", txtSearch.Text.Trim());

            List<Contact> list = sqlCon.Query<Contact>("ContactViewAllOrSearch", param, commandType:CommandType.StoredProcedure).ToList<Contact>();

            dgvContact.DataSource = list;
            dgvContact.Columns[0].Visible = false;
        }

        class Contact
        {
            public int ContactID { get; set; }

            public string Name { get; set; }
            public string Mobile { get; set; }
            public string Address { get; set; }

        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void Clear()
        {
            txtName.Text = txtMobile.Text = txtAddress.Text = "";
            contactId = 0;
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
        }

        private void DgvContact_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if(dgvContact.CurrentRow.Index != -1)
                {
                    contactId = Convert.ToInt32(dgvContact.CurrentRow.Cells[0].Value.ToString());
                    txtName.Text = dgvContact.CurrentRow.Cells[1].Value.ToString();
                    txtMobile.Text = dgvContact.CurrentRow.Cells[2].Value.ToString();
                    txtAddress.Text = dgvContact.CurrentRow.Cells[3].Value.ToString();
                    btnDelete.Enabled = true;
                    btnSave.Text = "Edit";
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ContactID", contactId);
                sqlCon.Execute("ContactDeleteByID", param, commandType: CommandType.StoredProcedure);
                Clear();
                FillDataGridView();
                MessageBox.Show("Deleted Successfully!");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
