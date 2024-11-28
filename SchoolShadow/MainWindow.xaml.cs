using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Linq;
using BCrypt.Net;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace SchoolShadow
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadData();
            LoadSchedule();
        }

        private static ScheduleSystemEntities _context;

        public static ScheduleSystemEntities GetContext()
        {
            if (_context == null)
                _context = new ScheduleSystemEntities();

            return _context;
        }

        private void LoadSchedule()
        {
            try
            {
                var scheduleData = _context.Schedule
                    .Select(s => new
                    {
                        s.Schedule_ScheduleID,
                        GroupName = s.Groups.Groups_Name,
                        SubjectName = s.Subjects.Subjects_Name,
                        TeacherName = s.Teachers.Teachers_FullName,
                        RoomName = s.Rooms.Rooms_Name,
                        s.Schedule_StartTime,
                        s.Schedule_EndTime
                    }).ToList();

                ScheduleDataGrid.ItemsSource = scheduleData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке расписания: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void LoadData()
        {
            
            UpdateAllData();

        }

        // Метод обновления всех таблиц
        private void UpdateAllData()
        {
            var teachers = GetContext().Teachers.ToList();
            var subjects = GetContext().Subjects.ToList();
            var rooms = GetContext().Rooms.ToList();
            var groups = GetContext().Groups.ToList();
            var curriculum = GetContext().Curriculum.ToList();
            

            // Заполнение таблицы Groups
            GroupsDataGrid.ItemsSource = groups;

            // Заполнение таблицы Teachers
            TeachersDataGrid.ItemsSource = teachers
                .Select(t => new
                {
                    t.Teachers_TeacherID,
                    t.Teachers_FullName,
                    SubjectName = subjects.FirstOrDefault(s => s.Subjects_SubjectID == t.Teachers_SubjectID)?.Subjects_Name ?? "Без предмета"
                }).ToList();

            // Заполнение таблицы Rooms
            RoomsDataGrid.ItemsSource = rooms
                .Select(r => new
                {
                    r.Rooms_RoomID,
                    r.Rooms_Name,
                    r.Rooms_Capacity,
                    r.Rooms_Equipment,
                    r.Rooms_IsShared,
                    TeacherName = r.Rooms_TeacherID != null // Если `Rooms_TeacherID` — nullable
                        ? teachers.FirstOrDefault(t => t.Teachers_TeacherID == r.Rooms_TeacherID)?.Teachers_FullName
                        : "Без преподавателя"
                }).ToList();

            // Заполнение ComboBox для фильтрации по классам
            ClassFilterComboBox.ItemsSource = groups
                .Select(g => new { g.Groups_GroupID, g.Groups_Name })
                .ToList();
            ClassFilterComboBox.DisplayMemberPath = "Groups_Name";
            ClassFilterComboBox.SelectedValuePath = "Groups_GroupID";
            ClassFilterComboBox.SelectedIndex = -1; // Сбрасываем выбор

            // Заполнение таблицы Subjects
            SubjectDataGrid.ItemsSource = subjects
                .Select(s => new
                {
                    s.Subjects_SubjectID,
                    s.Subjects_Name,
                    s.Subjects_Complexity,
                    s.Subjects_Hours,
                    GroupName = curriculum
                        .Where(c => c.Curriculum_SubjectID == s.Subjects_SubjectID)
                        .Select(c => groups.FirstOrDefault(g => g.Groups_GroupID == c.Curriculum_GroupID)?.Groups_Name)
                        .FirstOrDefault() ?? "Не указано"
                }).ToList();

        }




        private void ClassFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClassFilterComboBox.SelectedValue == null)
            {
                UpdateAllData(); // Если класс не выбран, показываем все данные
                return;
            }

            int selectedGroupId = (int)ClassFilterComboBox.SelectedValue;

            var subjects = GetContext().Subjects.ToList();
            var groups = GetContext().Groups.ToList();
            var curriculum = GetContext().Curriculum.ToList();

            // Фильтрация предметов по выбранному классу
            SubjectDataGrid.ItemsSource = subjects
                .Where(s => curriculum.Any(c => c.Curriculum_SubjectID == s.Subjects_SubjectID && c.Curriculum_GroupID == selectedGroupId))
                .Select(s => new
                {
                    s.Subjects_SubjectID,
                    s.Subjects_Name,
                    s.Subjects_Complexity,
                    s.Subjects_Hours,
                    GroupName = groups.FirstOrDefault(g => g.Groups_GroupID == selectedGroupId)?.Groups_Name ?? "Не указано"
                }).ToList();
            
        }

        

        // Добавление группы
        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            var addGroupWindow = new AddGroupWindow();
            addGroupWindow.ShowDialog();

            // Обновление DataGrid после добавления
            GroupsDataGrid.ItemsSource = GetContext().Groups.ToList();
            UpdateAllData();
        }

        // Добавление преподавателя
        private void AddTeacher_Click(object sender, RoutedEventArgs e)
        {
            var addTeacherWindow = new AddTeacherWindow();
            addTeacherWindow.ShowDialog();

            // Обновление DataGrid после добавления преподавателя
            TeachersDataGrid.ItemsSource = MainWindow.GetContext().Teachers.ToList();
            UpdateAllData();
        }

        private void AddSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            // Открытие окна для добавления нового предмета
            AddSubjectWindow addSubjectWindow = new AddSubjectWindow();
            addSubjectWindow.ShowDialog(); // Ожидаем, пока окно не закроется

            UpdateAllData();

        }

        // Добавление аудитории
        private void AddRoom_Click(object sender, RoutedEventArgs e)
        {
            AddRoomWindow addRoomWindow = new AddRoomWindow();
            addRoomWindow.ShowDialog(); // Ожидаем, пока окно не закроется

            UpdateAllData();
        }

        

        // Генерация расписания
        private void GenerateSchedule_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var scheduleGenerator = new ScheduleGenerator(_context);

                // Выбор первого класса для генерации расписания
                var firstClassId = 1;
                if (firstClassId != null)
                {
                    scheduleGenerator.GenerateScheduleForClass(firstClassId);
                    MessageBox.Show("Расписание успешно сгенерировано!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadSchedule();
                }
                else
                {
                    MessageBox.Show("Классы не найдены в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при генерации расписания: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            UpdateAllData();
        }

        // Метод для получения контекста
        


        



    }
}
