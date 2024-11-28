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
    /// Логика взаимодействия для AddTeacherWindow.xaml
    /// </summary>
    public partial class AddTeacherWindow : Window
    {
        public AddTeacherWindow()
        {
            InitializeComponent();
            LoadSubjects();
        }

        private void LoadSubjects()
        {
            // Загружаем предметы из базы данных
            var subjects = MainWindow.GetContext().Subjects.ToList();
            TeacherSubjectComboBox.ItemsSource = subjects;
            TeacherSubjectComboBox.DisplayMemberPath = "Subjects_Name";
            TeacherSubjectComboBox.SelectedValuePath = "Subjects_SubjectID";
        }

        // Обработчик нажатия на кнопку "Добавить"
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на корректность введенных данных
            if (string.IsNullOrWhiteSpace(TeacherFullNameTextBox.Text) || TeacherSubjectComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            // Получаем выбранный предмет
            var selectedSubject = TeacherSubjectComboBox.SelectedItem as Subjects;
            if (selectedSubject == null)
            {
                MessageBox.Show("Ошибка: выбранный предмет некорректен.");
                return;
            }

            // Добавление преподавателя в базу
            try
            {
                var teacher = new Teachers
                {
                    Teachers_FullName = TeacherFullNameTextBox.Text,
                    Teachers_SubjectID = selectedSubject.Subjects_SubjectID
                };

                // Сохраняем изменения в базу
                MainWindow.GetContext().Teachers.Add(teacher);
                MainWindow.GetContext().SaveChanges();

                MessageBox.Show("Преподаватель успешно добавлен.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении преподавателя: {ex.Message}");
            }
        }
    }
}
