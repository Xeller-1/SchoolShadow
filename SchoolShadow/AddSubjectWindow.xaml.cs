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
using System.Xml.Linq;

namespace SchoolShadow
{
    /// <summary>
    /// Логика взаимодействия для AddSubjectWindow.xaml
    /// </summary>
    public partial class AddSubjectWindow : Window
    {
        public AddSubjectWindow()
        {
            InitializeComponent();
            LoadGroups();
        }
        private void LoadGroups()
        {
            using (var context = new ScheduleSystemEntities())
            {
                var groups = context.Groups.ToList();
                GroupsComboBox.ItemsSource = groups;
                GroupsComboBox.DisplayMemberPath = "Groups_Name";
                GroupsComboBox.SelectedValuePath = "Groups_GroupID";
            }
        }

        // Обработчик нажатия на кнопку "Добавить"
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string subjectName = SubjectNameTextBox.Text;
                int complexity = int.Parse(SubjectComplexityTextBox.Text);
                int hours = int.Parse(SubjectHoursTextBox.Text);
                int groupId = (int)GroupsComboBox.SelectedValue;
                int load = int.Parse(CurriculumLoadTextBox.Text);

                // Создание нового предмета
                using (var context = new ScheduleSystemEntities())
                {
                    // Добавляем новый предмет
                    var newSubject = new Subjects
                    {
                        Subjects_Name = subjectName,
                        Subjects_Complexity = complexity,
                        Subjects_Hours = hours
                    };

                    context.Subjects.Add(newSubject);
                    context.SaveChanges(); // Сохраняем новый предмет в базе

                    // Привязываем предмет к группе через Curriculum
                    var curriculum = new Curriculum
                    {
                        Curriculum_GroupID = groupId,
                        Curriculum_SubjectID = newSubject.Subjects_SubjectID,
                        Curriculum_Hours = hours,
                        Curriculum_Load = load
                    };

                    context.Curriculum.Add(curriculum);
                    context.SaveChanges(); // Сохраняем связь между группой и предметом
                }

                MessageBox.Show("Предмет успешно добавлен!");
                this.Close(); // Закрыть окно после добавления
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении предмета: {ex.Message}");
            }
        }
    }
}
