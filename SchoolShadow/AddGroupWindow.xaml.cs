using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SchoolShadow
{
    /// <summary>
    /// Логика взаимодействия для AddGroupWindow.xaml
    /// </summary>
    public partial class AddGroupWindow : Window
    {
        public AddGroupWindow()
        {
            InitializeComponent();
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на корректность введенных данных
            if (string.IsNullOrWhiteSpace(GroupNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(GroupYearTextBox.Text) ||
                !int.TryParse(GroupYearTextBox.Text, out int year))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            // Добавление данных в базу
            try
            {
                var group = new Groups
                {
                    Groups_Name = GroupNameTextBox.Text,
                    Groups_Year = year,
                    
                };

                MainWindow.GetContext().Groups.Add(group);
                MainWindow.GetContext().SaveChanges();

                MessageBox.Show("Класс успешно добавлена.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении класса: {ex.Message}");
            }
        }
    }
}
