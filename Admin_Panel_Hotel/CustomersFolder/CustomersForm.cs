﻿using Admin_Panel_Hotel.CustomersFolder;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace Admin_Panel_Hotel.CustomersFolder
{
    public partial class CustomersForm : Form
    {
        public CustomersForm()
        {
            InitializeComponent();

            CustomersComboBox.SelectedIndex = 0;

            Functions.SetPlaceholderTextBox(SearchTextBox, "Поиск");

            LoadCustomers();
        }

        private void LoadCustomers()
        {
            if (Functions.Connection.State == ConnectionState.Open) // Подгрузка всех заказчиков, если соединение с БД открыто.
            {
                MySqlCommand select = new MySqlCommand("SELECT id, name FROM customer_legal_info", Functions.Connection);
                select.CommandTimeout = 86400;

                MySqlDataReader reader = select.ExecuteReader();

                while (reader.Read())
                {
                    CustomersDataGridView.Rows.Add(reader[1].ToString(), null, reader[0].ToString());
                }
                reader.Close();
            }
        }

        private void CustomersDataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                CustomersDataGridView.Cursor = Cursors.Hand;
            }
            else
            {
                CustomersDataGridView.Cursor = Cursors.Default;
            }
        }

        private void CustomersDataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                CustomerInfoForm.CustomerName = CustomersDataGridView[0, e.RowIndex].Value.ToString();
                CustomerInfoForm.CustomerId = Convert.ToInt64(CustomersDataGridView[2, e.RowIndex].Value.ToString());
                Functions.OpenChildForm(new CustomerInfoForm(), MainForm.ContP);
            }
        }
    }
}
