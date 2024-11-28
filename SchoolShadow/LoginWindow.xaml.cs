using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCrypt.Net;
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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UsernamePlaceholder.Visibility = string.IsNullOrWhiteSpace(UsernameTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrWhiteSpace(PasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }


        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            try
            {
                // Проверка пароля с хэшом, который был сохранен в базе данных
                return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при верификации пароля: {ex.Message}");
                return false;
            }
        }



        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;

            // Получаем контекст в пределах блока using
            using (var context = ScheduleSystemEntities.GetContext())
            {
                var user = context.Users
                                  .FirstOrDefault(u => u.Username == username); // Проверяем пользователя по введенному логину

                if (user != null)
                {
                    // Если пользователь найден, проверяем пароль
                    string enteredPassword = password;
                    string storedHash = user.PasswordHash;

                    if (VerifyPassword(enteredPassword, storedHash))  // Проверяем пароль
                    {
                        // Успешный вход
                        MessageBox.Show("Вход успешен!");

                        

                        // Переход на DashboardPage в MainFrame
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();  // Открываем главное окно

                        this.Close();
                    }
                    else
                    {
                        // Неверный пароль
                        MessageBox.Show("Неверный пароль.");
                    }
                }
                else
                {
                    // Псевдопользователь не найден
                    MessageBox.Show("Пользователь не найден.");
                }
            }
        }

        private void UpdatePasswordsButton_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
