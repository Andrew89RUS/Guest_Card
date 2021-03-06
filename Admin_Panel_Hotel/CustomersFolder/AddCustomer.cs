﻿using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Admin_Panel_Hotel.CustomersFolder
{
    public partial class AddCustomer : Form
    {
        /// <summary>
        /// Уникальный номер заказчика. -1 если заказчик ещё не добавлен в БД.
        /// </summary>
        private static long CustomerId = -1;
        private static long LocationId = 0;
        private static int EmailsCount = 1;

        public AddCustomer()
        {
            InitializeComponent();

            OpenCustomerInfoPanel();

            // Установка подсказок в текстовых полях.
            Functions.SetPlaceholderTextBox(NameTextBox, "Наименование организации");
            Functions.SetPlaceholderTextBox(ContractNumberTextBox, "Номер договора");
            Functions.SetPlaceholderTextBox(LocationNameTextBox, "Название локации");
            Functions.SetPlaceholderTextBox(RoomCountTextBox, "Количество комнат");
            Functions.SetPlaceholderTextBox(BedsCountTextBox, "Количество мест");
            Functions.SetPlaceholderTextBox(EmailNameTextBox0, "Имя заказчика");
            Functions.SetPlaceholderTextBox(EmailTextBox0, "Электронная почта заказчика");
            Functions.SetPlaceholderTextBox(SubDivisionNameTextBox, "Наименование организации");
            Functions.SetPlaceholderTextBox(SubDivisionEmailNameTextBox0, "Имя заказчика");
            Functions.SetPlaceholderTextBox(SubDivisionEmailTextBox0, "Электронная почта");

            // Установка подсказки для полей с датами.
            Functions.SetPlaceholderDateTimePicker(FromContractTimeDateTimePicker, "Введите дату");
            Functions.SetPlaceholderDateTimePicker(ToContractTimeDateTimePicker, "Введите дату");

            Functions.NewlineProcessing(RoomsDataGridView, new string[] { "1", "Номер комнаты", "Количество спальных мест", null });

            // Установка ограничений для текстовых полей.
            Functions.OnlyNumbersInTextBox(CardCountTextBox);
            Functions.OnlyNumbersInTextBox(RoomCountTextBox);
            Functions.OnlyNumbersInTextBox(BedsCountTextBox);

            // Установка подсказок обязательных полей.
            NameErrorProvider.SetError(NameTextBox, "* - обязательное поле");
            NameErrorProvider.SetIconPadding(NameTextBox, 10);
            ContractNumberErrorProvider.SetError(ContractNumberTextBox, "* - обязательное поле");
            ContractNumberErrorProvider.SetIconPadding(ContractNumberTextBox, 10);
            ToContractTimeErrorProvider.SetError(ToContractTimeDateTimePicker, "* - обязательное поле");
            ToContractTimeErrorProvider.SetIconPadding(ToContractTimeDateTimePicker, 10);
            FromContractTimeErrorProvider.SetError(FromContractTimeDateTimePicker, "* - обязательное поле");
            FromContractTimeErrorProvider.SetIconPadding(FromContractTimeDateTimePicker, 10);
            LocationNameErrorProvider.SetError(LocationNameTextBox, "* - обязательное поле");
            LocationNameErrorProvider.SetIconPadding(LocationNameTextBox, 10);
            RoomCountErrorProvider.SetError(RoomCountTextBox, "* - обязательное поле");
            RoomCountErrorProvider.SetIconPadding(RoomCountTextBox, 10);
            BedsCountErrorProvider.SetError(BedsCountTextBox, "* - обязательное поле");
            BedsCountErrorProvider.SetIconPadding(BedsCountTextBox, 10);
            EmailNamesErrorProvider.SetError(EmailNameTextBox0, "* - обязательное поле");
            EmailNamesErrorProvider.SetIconPadding(EmailNameTextBox0, 10);
            EmailsErrorProvider.SetError(EmailTextBox0, "* - обязательное поле");
            EmailsErrorProvider.SetIconPadding(EmailTextBox0, 10);
            SubDivisionNameErrorProvider.SetError(SubDivisionNameTextBox, "* - обязательное поле");
            SubDivisionNameErrorProvider.SetIconPadding(SubDivisionNameTextBox, 10);
            SubDivisionEmailNameErrorProvider.SetError(SubDivisionEmailNameTextBox0, "* - обязательное поле");
            SubDivisionEmailNameErrorProvider.SetIconPadding(SubDivisionEmailNameTextBox0, 10);
            SubDivisionEmailErrorProvider.SetError(SubDivisionEmailTextBox0, "* - обязательное поле");
            SubDivisionEmailErrorProvider.SetIconPadding(SubDivisionEmailTextBox0, 10);
        }

        #region Данные заказчика

        /// <summary>
        /// Обработка события, когда текстовое поле перестаёт быть активным полем.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameTextBox_Leave(object sender, EventArgs e)
        {
            if (NameTextBox.Text.Trim().Length > 0 && NameTextBox.Text != NameTextBox.Tag.ToString()) // Если в текстовом поле есть текст, который не является подсказкой.
            {
                NameErrorProvider.Icon = Properties.Resources.check_mark;
            }
            else
            {
                NameErrorProvider.Icon = Properties.Resources.exclamation_mark;
            }
        }

        /// <summary>
        /// Обработка события, когда текстовое поле перестаёт быть активным полем.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContractNumberTextBox_Leave(object sender, EventArgs e)
        {
            if (ContractNumberTextBox.Text.Trim().Length > 0 && ContractNumberTextBox.Text != ContractNumberTextBox.Tag.ToString()) // Если в текстовом поле есть текст, который не является подсказкой.
            {
                ContractNumberErrorProvider.Icon = Properties.Resources.check_mark;
            }
            else
            {
                ContractNumberErrorProvider.Icon = Properties.Resources.exclamation_mark;
            }
        }

        /// <summary>
        /// Обработка события, когда текстовое поле перестаёт быть активным полем.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FromContractTimeDateTimePicker_Leave(object sender, EventArgs e)
        {
            if (FromContractTimeDateTimePicker.Format == DateTimePickerFormat.Short
                && FromContractTimeDateTimePicker.Value != FromContractTimeDateTimePicker.MinDate
                && FromContractTimeDateTimePicker.Value < ToContractTimeDateTimePicker.Value) // Если в поле введена корректная дата, то удаляем обозначение заполнения.
            {
                FromContractTimeErrorProvider.Icon = Properties.Resources.check_mark;
            }
            else if(FromContractTimeDateTimePicker.Value >= ToContractTimeDateTimePicker.Value)
            {
                FromContractTimeErrorProvider.Icon = Properties.Resources.exclamation_mark;
                MessageBox.Show("Дата начала срока договора должна быть меньше окончания срока договора!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                FromContractTimeErrorProvider.Icon = Properties.Resources.exclamation_mark;
            }

            if (ToContractTimeDateTimePicker.Format == DateTimePickerFormat.Short
                && ToContractTimeDateTimePicker.Value != ToContractTimeDateTimePicker.MinDate
                && FromContractTimeDateTimePicker.Value < ToContractTimeDateTimePicker.Value) // Если в поле введена корректная дата, то удаляем обозначение заполнения.
            {
                ToContractTimeErrorProvider.Icon = Properties.Resources.check_mark;
            }
        }

        /// <summary>
        /// Обработка события, когда текстовое поле перестаёт быть активным полем.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToContractTimeDateTimePicker_Leave(object sender, EventArgs e)
        {
            if (ToContractTimeDateTimePicker.Format == DateTimePickerFormat.Short
                && ToContractTimeDateTimePicker.Value != ToContractTimeDateTimePicker.MinDate
                && FromContractTimeDateTimePicker.Value < ToContractTimeDateTimePicker.Value) // Если в поле введена корректная дата, то удаляем обозначение заполнения.
            {
                ToContractTimeErrorProvider.Icon = Properties.Resources.check_mark;
            }
            else if(FromContractTimeDateTimePicker.Value >= ToContractTimeDateTimePicker.Value)
            {
                ToContractTimeErrorProvider.Icon = Properties.Resources.exclamation_mark;
                MessageBox.Show("Дата начала срока договора должна быть меньше окончания срока договора!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                ToContractTimeErrorProvider.Icon = Properties.Resources.exclamation_mark;
            }

            if (FromContractTimeDateTimePicker.Format == DateTimePickerFormat.Short
                && FromContractTimeDateTimePicker.Value != FromContractTimeDateTimePicker.MinDate
                && FromContractTimeDateTimePicker.Value < ToContractTimeDateTimePicker.Value) // Если в поле введена корректная дата, то удаляем обозначение заполнения.
            {
                FromContractTimeErrorProvider.Icon = Properties.Resources.check_mark;
            }
        }

        /// <summary>
        /// Обработка окончания редактирования поля с эл.почтой.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmailTextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox.TextLength > 0 && textBox.Text != textBox.Tag.ToString() && RegexUtilities.IsValidEmail(textBox.Text.Trim()))
            {
                EmailsErrorProvider.Clear();
            }
            else if (textBox.TextLength > 0 && textBox.Text != textBox.Tag.ToString())
            {
                MessageBox.Show($"Электронная почта \"{textBox.Text}\" введена некорректно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                EmailsErrorProvider.SetError(textBox, "* - обязательное поле");
                EmailsErrorProvider.SetIconPadding(textBox, 10);
                textBox.Focus();
            }
        }

        /// <summary>
        /// Обработка окончания редактирования поля с именем эл.почты.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmailNameTextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox.Text.Trim().Length > 0 && textBox.Text != textBox.Tag.ToString()) // Если введёно имя эл.почты в последнее поле.
            {
                EmailNamesErrorProvider.Clear();
            }
            else
            {
                EmailNamesErrorProvider.SetError(textBox, "* - обязательное поле");
                EmailNamesErrorProvider.SetIconPadding(textBox, 10);
            }
        }

        /// <summary>
        /// Обработка события нажатия на ссылочную надпись.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddEmailLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AddEmailsFields(EmailsPanel, AddEmailLinkLabel);
        }

        /// <summary>
        /// Обработка события нажатия на кнопку.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerInfoNextButton_Click(object sender, EventArgs e)
        {
            CheckAndAddCustomerInfo();
        }

        private void CheckAndAddCustomerInfo()
        {
            TextBox lastEmailNameTextBox = EmailsPanel.Controls[EmailsPanel.Controls.Count - 2] as TextBox;
            TextBox lastEmailTextBox = EmailsPanel.Controls[EmailsPanel.Controls.Count - 1] as TextBox;

            if (NameTextBox.TextLength > 0 && NameTextBox.Text.Trim() != NameTextBox.Tag.ToString()
                && ContractNumberTextBox.TextLength > 0 && ContractNumberTextBox.Text.Trim() != ContractNumberTextBox.Tag.ToString()
                && FromContractTimeDateTimePicker.Format == DateTimePickerFormat.Short
                && ToContractTimeDateTimePicker.Format == DateTimePickerFormat.Short
                && FromContractTimeDateTimePicker.Value < ToContractTimeDateTimePicker.Value
                && lastEmailNameTextBox.TextLength > 0 && lastEmailNameTextBox.Text.Trim() != lastEmailNameTextBox.Tag.ToString()
                && RegexUtilities.IsValidEmail(lastEmailTextBox.Text.Trim())) // Проверка заполнения всех обязательных полей на первом шаге.
            {
                OpenSubDivisionsPanel();
                SubDivisionsButton.Enabled = true;

                long event_id = Functions.SqlInsert($"INSERT INTO event(start_at, end_at, ev_type) VALUES({Functions.ToUnixTime(FromContractTimeDateTimePicker.Value)}, {Functions.ToUnixTime(ToContractTimeDateTimePicker.Value)}), 9");
                CustomerId = Functions.SqlInsert($"INSERT INTO customer_legal_info(name, dogovor, event_id) VALUES('{NameTextBox.Text}', '{ContractNumberTextBox.Text}', {event_id})");
            }
        }

        /// <summary>
        /// Обработка события нажатия на кнопку.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerInfoButton_Click(object sender, EventArgs e)
        {
            OpenCustomerInfoPanel();
        }

        /// <summary>
        /// Отображение панели с данными заказчика.
        /// </summary>
        private void OpenCustomerInfoPanel()
        {
            CustomerInfoPanel.Visible = true;
            SubDivisionsPanel.Visible = false;
            AddCustomerLocationPanel.Visible = false;
            CardPropertiesPanel.Visible = false;

            CustomerInfoButton.BackgroundImage = Properties.Resources.BlueCircle;
            CustomerInfoButton.ForeColor = Color.White;

            SubDivisionsButton.BackgroundImage = Properties.Resources.GrayCircle;
            SubDivisionsButton.ForeColor = Color.Black;

            AddLocationsButton.BackgroundImage = Properties.Resources.GrayCircle;
            AddLocationsButton.ForeColor = Color.Black;

            CardPropertiesButton.BackgroundImage = Properties.Resources.GrayCircle;
            CardPropertiesButton.ForeColor = Color.Black;
        }

        #endregion

        #region Данные подрядных организаций заказчика

        /// <summary>
        /// Обработка события нажатия на кнопку.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubDivisionsNextButton_Click(object sender, EventArgs e)
        {
            if (SubDivisionsDataGridView.Rows.Count > 0)
            {
                OpenAddCustomerLocationPanel();
            }
            else
            {
                MessageBox.Show("Вы должны добавить хотя бы одну подрядную организацию!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обработка события нажатия на кнопку.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubDivisionsButton_Click(object sender, EventArgs e)
        {
            OpenSubDivisionsPanel();
        }

        /// <summary>
        /// Отображение панели с добавлением подрядных организаций заказчика.
        /// </summary>
        private void OpenSubDivisionsPanel()
        {
            CustomerInfoPanel.Visible = false;
            SubDivisionsPanel.Visible = true;
            AddCustomerLocationPanel.Visible = false;
            CardPropertiesPanel.Visible = false;

            CustomerInfoButton.BackgroundImage = Properties.Resources.GrayCircle;
            CustomerInfoButton.ForeColor = Color.Black;

            SubDivisionsButton.BackgroundImage = Properties.Resources.BlueCircle;
            SubDivisionsButton.ForeColor = Color.White;

            AddLocationsButton.BackgroundImage = Properties.Resources.GrayCircle;
            AddLocationsButton.ForeColor = Color.Black;

            CardPropertiesButton.BackgroundImage = Properties.Resources.GrayCircle;
            CardPropertiesButton.ForeColor = Color.Black;
        }

        /// <summary>
        /// Обработка события нажатия на ссылочную надпись.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSubDivisionLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (SubDivisionNameTextBox.TextLength > 0 && SubDivisionNameTextBox.Text != SubDivisionNameTextBox.Tag.ToString()
                && SubDivisionEmailNameTextBox0.TextLength > 0 && SubDivisionEmailNameTextBox0.Text != SubDivisionEmailNameTextBox0.Tag.ToString()
                && SubDivisionEmailTextBox0.TextLength > 0 && SubDivisionEmailTextBox0.Text != SubDivisionEmailTextBox0.Tag.ToString()
                && RegexUtilities.IsValidEmail(SubDivisionEmailTextBox0.Text.Trim())) // Если заполнены все обязательные поля и эл.почта введена корректно.
            {
                SubDivisionsDataGridView.Rows.Add(SubDivisionsDataGridView.Rows.Count + 1
                    , SubDivisionNameTextBox.Text.Trim()
                    , SubDivisionEmailNameTextBox0.Text.Trim()
                    , SubDivisionEmailTextBox0.Text.Trim()
                    , null);

                SubDivisionNameTextBox.Text = SubDivisionNameTextBox.Tag.ToString();
                SubDivisionNameTextBox.ForeColor = Color.Silver;

                SubDivisionEmailNameTextBox0.Text = SubDivisionEmailNameTextBox0.Tag.ToString();
                SubDivisionEmailNameTextBox0.ForeColor = Color.Silver;
                SubDivisionEmailNameErrorProvider.Icon = Properties.Resources.exclamation_mark;

                SubDivisionEmailTextBox0.Text = SubDivisionEmailTextBox0.Tag.ToString();
                SubDivisionEmailTextBox0.ForeColor = Color.Silver;
                SubDivisionEmailErrorProvider.Icon = Properties.Resources.exclamation_mark;
            }
        }

        /// <summary>
        /// Обработка события изменения текста в текстовом поле.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubDivisionNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (SubDivisionNameTextBox.TextLength > 0 && SubDivisionNameTextBox.Text != SubDivisionNameTextBox.Tag.ToString())
            {
                SubDivisionNameErrorProvider.Icon = Properties.Resources.check_mark;
            }
            else
            {
                SubDivisionNameErrorProvider.Icon = Properties.Resources.exclamation_mark;
            }
        }

        /// <summary>
        /// Обработка события, когда текстовое поле перестаёт быть активным.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubDivisionEmailNameTextBox0_Leave(object sender, EventArgs e)
        {
            if (SubDivisionEmailNameTextBox0.Text.Trim().Length > 0 && SubDivisionEmailNameTextBox0.Text != SubDivisionEmailNameTextBox0.Tag.ToString()) // Если введёно имя эл.почты.
            {
                SubDivisionEmailNameErrorProvider.Icon = Properties.Resources.check_mark;
            }
            else
            {
                SubDivisionEmailNameErrorProvider.Icon = Properties.Resources.exclamation_mark;
            }
        }

        /// <summary>
        /// Обработка события, когда текстовое поле перестаёт быть активным.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubDivisionEmailTextBox0_Leave(object sender, EventArgs e)
        {
            if (SubDivisionEmailTextBox0.TextLength > 0 && SubDivisionEmailTextBox0.Text != SubDivisionEmailTextBox0.Tag.ToString() && RegexUtilities.IsValidEmail(SubDivisionEmailTextBox0.Text.Trim()))
            {
                SubDivisionEmailErrorProvider.Icon = Properties.Resources.check_mark;
            }
            else if (SubDivisionEmailTextBox0.TextLength > 0 && SubDivisionEmailTextBox0.Text != SubDivisionEmailTextBox0.Tag.ToString())
            {
                MessageBox.Show($"Электронная почта \"{SubDivisionEmailTextBox0.Text}\" введена некорректно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SubDivisionEmailErrorProvider.Icon = Properties.Resources.exclamation_mark;
                SubDivisionEmailTextBox0.Focus();
            }
        }

        /// <summary>
        /// Обработка движения мыши в ячейке таблицы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubDivisionsDataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 1 || e.ColumnIndex == 4)
            {
                SubDivisionsDataGridView.Cursor = Cursors.Hand;
            }
            else
            {
                SubDivisionsDataGridView.Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Добавить локацию

        /// <summary>
        /// Обработка события нажатия на кнопку.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddLocationsButton_Click(object sender, EventArgs e)
        {
            OpenAddCustomerLocationPanel();
        }

        /// <summary>
        /// Обработка события нажатия на кнопку.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddLocationNextButton_Click(object sender, EventArgs e)
        {
            if (LocationsDataGridView.Rows.Count > 0)
            {
                OpenCardPropertiesPanel();
                CardPropertiesButton.Enabled = true;
            }
        }

        /// <summary>
        /// Отображение панели с добавлением локаций заказчика.
        /// </summary>
        private void OpenAddCustomerLocationPanel()
        {
            CustomerInfoPanel.Visible = false;
            SubDivisionsPanel.Visible = false;
            AddCustomerLocationPanel.Visible = true;
            CardPropertiesPanel.Visible = false;

            CustomerInfoButton.BackgroundImage = Properties.Resources.GrayCircle;
            CustomerInfoButton.ForeColor = Color.Black;

            SubDivisionsButton.BackgroundImage = Properties.Resources.GrayCircle;
            SubDivisionsButton.ForeColor = Color.Black;

            AddLocationsButton.BackgroundImage = Properties.Resources.BlueCircle;
            AddLocationsButton.ForeColor = Color.White;

            CardPropertiesButton.BackgroundImage = Properties.Resources.GrayCircle;
            CardPropertiesButton.ForeColor = Color.Black;
        }

        /// <summary>
        /// Проверка заполнения обязательных полей и добавление локации в базу данных.
        /// </summary>
        /// <returns>
        /// True - если все обязательные поля заполнены и локация добавлена в таблицу. False - если заполнены не все обязательные поля.
        /// </returns>
        private bool AddLocation()
        {
            if (LocationNameTextBox.TextLength > 0 && LocationNameTextBox.Text != LocationNameTextBox.Tag.ToString()
                && RoomCountTextBox.TextLength > 0 && RoomCountTextBox.Text != RoomCountTextBox.Tag.ToString()
                && BedsCountTextBox.TextLength > 0 && BedsCountTextBox.Text != BedsCountTextBox.Tag.ToString()
                && CheckAddRoomsData()) // Если все обязательные поля заполнены.
            {
                LocationsDataGridView.Rows.Add(LocationsDataGridView.RowCount + 1
                    , LocationNameTextBox.Text
                    , RoomCountTextBox.Text.Trim()
                    , BedsCountTextBox.Text.Trim()
                    , CardCountTextBox.Text.Trim());
                LocationId++;

                for (int i = 0; i < RoomsDataGridView.RowCount; i++)
                {
                    // TODO: Сделать поиск существующих комнат.
                    if (true) // Если в БД есть такие комнаты.
                    {
                        // TODO: Сделать добавление комнат в локацию.
                        //Functions.SqlInsert($"INSERT INTO ");
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка заполнения данных в добавленных комнатах.
        /// </summary>
        /// <returns>True - если все данные заполнены. False - если есть незаполненные области.</returns>
        private bool CheckAddRoomsData()
        {
            // Проверка заполнения всех видимых ячеек.
            for (int i = 0; i < RoomsDataGridView.Rows.Count - 1; i++)
            {
                try
                {
                    // Если в ячейке нет данных или написан текст подсказки.
                    if (RoomsDataGridView[1, i].Value == null
                        || RoomsDataGridView[1, i].Value.ToString().Trim().ToLower() == RoomsDataGridView.Columns[1].ToolTipText.ToLower()
                        || !int.TryParse(RoomsDataGridView[1, i].Value.ToString().Trim().ToLower(), out int roomNumber)
                        || RoomsDataGridView[2, i].Value == null
                        || RoomsDataGridView[2, i].Value.ToString().Trim().ToLower() == RoomsDataGridView.Columns[2].ToolTipText.ToLower()
                        || !int.TryParse(RoomsDataGridView[2, i].Value.ToString().Trim().ToLower(), out int bedsCount))
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                    throw;
                }
            }
            return true;
        }

        /// <summary>
        /// Обработка события изменения текста в текстовом поле.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocationNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (LocationNameTextBox.TextLength > 0 && LocationNameTextBox.Text != LocationNameTextBox.Tag.ToString())
            {
                LocationNameErrorProvider.Icon = Properties.Resources.check_mark;
            }
            else
            {
                LocationNameErrorProvider.Icon = Properties.Resources.exclamation_mark;
            }
        }

        /// <summary>
        /// Обработка события изменения текста в текстовом поле.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomCountTextBox_TextChanged(object sender, EventArgs e)
        {
            if (RoomCountTextBox.TextLength > 0 && RoomCountTextBox.Text != RoomCountTextBox.Tag.ToString())
            {
                RoomCountErrorProvider.Icon = Properties.Resources.check_mark;
            }
            else
            {
                RoomCountErrorProvider.Icon = Properties.Resources.exclamation_mark;
            }
        }

        /// <summary>
        /// Обработка события изменения текста в текстовом поле.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BedsCountTextBox_TextChanged(object sender, EventArgs e)
        {
            if (BedsCountTextBox.TextLength > 0 && BedsCountTextBox.Text != BedsCountTextBox.Tag.ToString() && int.TryParse(BedsCountTextBox.Text, out int bedsCount))
            {
                BedsCountErrorProvider.Icon = Properties.Resources.check_mark;

                CardCountTextBox.Text = (((bedsCount * 10) / 100) + bedsCount).ToString(); // Расчёт количества карт по формуле: количество мест + 10%.
            }
            else
            {
                BedsCountErrorProvider.Icon = Properties.Resources.exclamation_mark;
                CardCountTextBox.Text = "0";
            }
        }

        /// <summary>
        /// Обработка нажатия на ссылочную надпись.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddLocationLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (AddLocation())
            {
                LocationNameTextBox.Text = LocationNameTextBox.Tag.ToString();
                RoomCountTextBox.Text = RoomCountTextBox.Tag.ToString();
                BedsCountTextBox.Text = BedsCountTextBox.Tag.ToString();
                CardCountTextBox.Text = "0";
                AddRoomsLabel.Visible = false;
                RoomsDataGridView.Visible = false;
            }
            else
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Отобразить панель с добавлением комнат.
        /// </summary>
        private void VisibleAddRooms()
        {
            AddCustomerLocationPanel.Focus();
            RoomsDataGridView.Rows.Clear();

            if (LocationNameTextBox.TextLength > 0 && LocationNameTextBox.Text != LocationNameTextBox.Tag.ToString()
                && RoomCountTextBox.TextLength > 0 && RoomCountTextBox.Text != RoomCountTextBox.Tag.ToString()
                && BedsCountTextBox.TextLength > 0 && BedsCountTextBox.Text != BedsCountTextBox.Tag.ToString()) // Если заполнены все обязательные поля.
            {
                for (int i = 0; i < Convert.ToInt32(RoomCountTextBox.Text); i++)
                {
                    int row = RoomsDataGridView.Rows.Add();
                    RoomsDataGridView[1, row].ErrorText = "* - обязательное поле";
                    RoomsDataGridView[2, row].ErrorText = "* - обязательное поле";
                    RoomsDataGridView[3, row].Value = LocationId;
                }

                AddRoomsLabel.Visible = true;
                RoomsDataGridView.Visible = true;
            }
        }

        /// <summary>
        /// Обработка окончания редактирования ячейки.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Если закончилось редактирование не первого столбца (номер строки) и в ячейке нет текста, или есть текст, который является подсказкой.
            if (e.ColumnIndex != 0 && (RoomsDataGridView[e.ColumnIndex, e.RowIndex].Value == null || (RoomsDataGridView[e.ColumnIndex, e.RowIndex].Value != null
                && (RoomsDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Trim() == RoomsDataGridView.Columns[e.ColumnIndex].ToolTipText || !int.TryParse(RoomsDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString(), out int count)))))
            {
                RoomsDataGridView[e.ColumnIndex, e.RowIndex].ErrorText = "* - обязательное поле";
            }
            else
            {
                RoomsDataGridView[e.ColumnIndex, e.RowIndex].ErrorText = null;
                AddLocation();
            }
        }

        /// <summary>
        /// Обработка события, когда текстовое поле перестаёт быть активным.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocationNameTextBox_Leave(object sender, EventArgs e)
        {
            VisibleAddRooms();
        }

        /// <summary>
        /// Обработка движения мыши в ячейке таблицы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocationsDataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 1 || e.ColumnIndex == 5)
            {
                LocationsDataGridView.Cursor = Cursors.Hand;
            }
            else
            {
                LocationsDataGridView.Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Добавить свойства карт

        private void AllProperties_Customer_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GiveOutLinen_Customer_CheckBox.Checked = AllProperties_Customer_CheckBox.Checked;
            Set1_Customer_CheckBox.Checked = AllProperties_Customer_CheckBox.Checked;
            Set2_Customer_CheckBox.Checked = AllProperties_Customer_CheckBox.Checked;
            Instructed_Customer_CheckBox.Checked = AllProperties_Customer_CheckBox.Checked;
            ResidenceRules_Customer_CheckBox.Checked = AllProperties_Customer_CheckBox.Checked;
            FireSafety_Customer_CheckBox.Checked = AllProperties_Customer_CheckBox.Checked;
        }

        private void CardPropertiesButton_Click(object sender, EventArgs e)
        {
            OpenCardPropertiesPanel();
        }

        /// <summary>
        /// Отображение панели с выбором свойств карт заказчика.
        /// </summary>
        private void OpenCardPropertiesPanel()
        {
            CustomerInfoPanel.Visible = false;
            SubDivisionsPanel.Visible = false;
            AddCustomerLocationPanel.Visible = false;
            CardPropertiesPanel.Visible = true;

            CustomerInfoButton.BackgroundImage = Properties.Resources.GrayCircle;
            CustomerInfoButton.ForeColor = Color.Black;

            SubDivisionsButton.BackgroundImage = Properties.Resources.GrayCircle;
            SubDivisionsButton.ForeColor = Color.Black;

            AddLocationsButton.BackgroundImage = Properties.Resources.GrayCircle;
            AddLocationsButton.ForeColor = Color.Black;

            CardPropertiesButton.BackgroundImage = Properties.Resources.BlueCircle;
            CardPropertiesButton.ForeColor = Color.White;
        }

        private void Set1HelpButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("В \"Комплект-1\" входит:\nНаволочка, подушка, одеяло, пододеяльник.", "Комплект-1", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void Set2HelpButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("В \"Комплект-2\" входит:\nПростыня, подушка, одеяло.", "Комплект-2", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        #endregion

        /// <summary>
        /// Добавить поля электронных почт.
        /// </summary>
        /// <param name="emailsPanel">Панель, на которой находятся поля электронной почты.</param>
        /// <param name="activateButton">Кнопка, которой активировали создание полей.</param>
        private void AddEmailsFields(Panel emailsPanel, LinkLabel activateButton)
        {
            TextBox lastEmailTextBox = emailsPanel.Controls[emailsPanel.Controls.Count - 1] as TextBox;
            TextBox lastEmailNameTextBox = emailsPanel.Controls[emailsPanel.Controls.Count - 2] as TextBox;

            if (lastEmailNameTextBox.Text != lastEmailNameTextBox.Tag.ToString() && lastEmailTextBox.Text != lastEmailTextBox.Tag.ToString())
            {
                TextBox emailNameTextBox = new TextBox();
                emailNameTextBox.Size = lastEmailNameTextBox.Size;
                emailNameTextBox.Margin = lastEmailNameTextBox.Margin;
                emailNameTextBox.Location = new Point(0, lastEmailNameTextBox.Location.Y + lastEmailNameTextBox.Size.Height + lastEmailNameTextBox.Margin.Top);
                emailNameTextBox.Name = $"EmailNameTextBox{EmailsCount}";
                emailsPanel.Controls.Add(emailNameTextBox);

                TextBox emailTextBox = new TextBox();
                emailTextBox.Size = lastEmailTextBox.Size;
                emailTextBox.Margin = lastEmailTextBox.Margin;
                emailTextBox.Location = new Point(lastEmailTextBox.Location.X, lastEmailTextBox.Location.Y + lastEmailTextBox.Size.Height + lastEmailTextBox.Margin.Top);
                emailTextBox.Name = $"EmailTextBox{EmailsCount}";
                emailsPanel.Controls.Add(emailTextBox);

                Functions.SetPlaceholderTextBox(emailNameTextBox, lastEmailNameTextBox.Tag.ToString());
                Functions.SetPlaceholderTextBox(emailTextBox, lastEmailTextBox.Tag.ToString());

                EmailNamesErrorProvider.SetError(emailNameTextBox, "* - обязательное поле");
                EmailNamesErrorProvider.SetIconPadding(emailNameTextBox, 10);
                EmailsErrorProvider.SetError(emailTextBox, "* - обязательное поле");
                EmailsErrorProvider.SetIconPadding(emailTextBox, 10);

                emailNameTextBox.Leave += new EventHandler(EmailNameTextBox_Leave);
                emailTextBox.Leave += new EventHandler(EmailTextBox_Leave);

                activateButton.Location = new Point(activateButton.Location.X, activateButton.Location.Y + lastEmailNameTextBox.Margin.Top + lastEmailNameTextBox.Size.Height);

                EmailsCount++;
            }
        }

        /// <summary>
        /// Обработка события нажатия на кнопку.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCustomerButton_Click(object sender, EventArgs e)
        {
            if (true)
            {
                MessageBox.Show("Функция в разработке.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // UNDONE: Добавление заказчика в базу данных.
                if (CheckCustomerInfo())
                {
                    long eventId = AddContractTimeToDB();

                    if (eventId > 0)
                    {
                        long packageRulesId = -1;

                        
                    }
                    else
                    {
                        MessageBox.Show("Возникла непредвиденная ошибка при добавлении события в базу данных.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Не все обязательные поля заполнены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Создание события "Срок договора".
        /// </summary>
        /// <returns>Возвращает id созданного события.</returns>
        private long AddContractTimeToDB()
        {
            long eventId = -1;
            long eventTypeId = GetEventTypeId();

            DateTimeOffset fromContractTime = FromContractTimeDateTimePicker.Value;
            DateTimeOffset toContractTime = ToContractTimeDateTimePicker.Value;

            MySqlCommand insert = new MySqlCommand($"INSERT INTO event(start_at, end_at, ev_type) VALUES ({fromContractTime.ToUnixTimeSeconds()}, {toContractTime.ToUnixTimeSeconds()}, {eventTypeId})");

            if (insert.ExecuteNonQuery() > 0)
            {
                eventId = insert.LastInsertedId;
            }

            return eventId;
        }

        /// <summary>
        /// Получить id типа события "Срок договора".
        /// </summary>
        /// <returns>Возвращает id типа события.</returns>
        private static long GetEventTypeId()
        {
            MySqlCommand select = new MySqlCommand("SELECT id FROM event_type WHERE name = 'Срок договора'", Functions.Connection);
            MySqlDataReader reader = select.ExecuteReader();

            long eventTypeId = -1;

            while (reader.Read())
            {
                eventTypeId = (long)reader[0];
                break;
            }
            reader.Close();

            // Если Тип события "Срок договора" не найден в базе.
            if (eventTypeId < 0)
            {
                MySqlCommand insert = new MySqlCommand("INSERT INTO event_type(name) VALUES ('Срок договора')", Functions.Connection);

                if (insert.ExecuteNonQuery() > 0) // Если количество добавленных строк больше 0.
                {
                    eventTypeId = insert.LastInsertedId;
                }
            }

            return eventTypeId;
        }

        /// <summary>
        /// Проверка заполнения полей при добавлении заказчика.
        /// </summary>
        /// <returns></returns>
        private bool CheckCustomerInfo()
        {
            // UNDONE: Сделать проверку заполнения обязательных полей.
            TextBox lastEmailNameTextBox = EmailsPanel.Controls[EmailsPanel.Controls.Count - 2] as TextBox;
            TextBox lastEmailTextBox = EmailsPanel.Controls[EmailsPanel.Controls.Count - 1] as TextBox;
            
            if (NameTextBox.TextLength > 0 && NameTextBox.Text != "Наименование организации"
                && ContractNumberTextBox.TextLength > 0 && ContractNumberTextBox.Text != "Номер договора"
                && LocationNameTextBox.TextLength > 0 && LocationNameTextBox.Text != "Локация"
                && lastEmailNameTextBox.TextLength > 0 && lastEmailNameTextBox.Text.Trim() != lastEmailNameTextBox.Tag.ToString()
                && RegexUtilities.IsValidEmail(lastEmailTextBox.Text.Trim())
                && SubDivisionsDataGridView.Rows.Count > 0
                && LocationsDataGridView.Rows.Count > 0) // Если все обязательные поля заполнены корректно.
            {
                return true;
            }
            else // Если какое-либо или все обязательные поля незаполнены.
            {
                return false;
            }
        }
    }
}