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
    /// Логика взаимодействия для AddRoomWindow.xaml
    /// </summary>
    public partial class AddRoomWindow : Window
    {
        public AddRoomWindow()
        {
            InitializeComponent();
            LoadTeachers();
        }

        // Метод для загрузки списка преподавателей
        private void LoadTeachers()
        {
            using (var context = new ScheduleSystemEntities())
            {
                var teachers = context.Teachers.ToList();
                TeacherComboBox.ItemsSource = teachers;
                TeacherComboBox.DisplayMemberPath = "Teachers_FullName";  // Отображаем имя преподавателя
                TeacherComboBox.SelectedValuePath = "Teachers_TeacherID";  // Значение, которое будет привязано
            }
        }

        // Обработчик нажатия на кнопку "Добавить"
        private void AddRoomButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string roomName = RoomNameTextBox.Text;
                int capacity = int.Parse(RoomCapacityTextBox.Text);
                string equipment = RoomEquipmentTextBox.Text;
                int? teacherId = (int?)TeacherComboBox.SelectedValue; // Может быть null, если преподаватель не выбран
                

                // Создание новой аудитории
                using (var context = new ScheduleSystemEntities())
                {
                    var newRoom = new Rooms
                    {
                        Rooms_Name = roomName,
                        Rooms_Capacity = capacity,
                        Rooms_Equipment = equipment,
                        Rooms_TeacherID = (int)teacherId,
                       
                    };

                    // Добавляем новую аудиторию в базу данных
                    context.Rooms.Add(newRoom);
                    context.SaveChanges(); // Сохраняем изменения в базе данных

                    MessageBox.Show("Аудитория успешно добавлена!");
                    this.Close(); // Закрыть окно после добавления
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении аудитории: {ex.Message}");
            }
        }
    }
}