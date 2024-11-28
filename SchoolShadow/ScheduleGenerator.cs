using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SchoolShadow
{
    public class ScheduleGenerator
    {
        private readonly ScheduleSystemEntities _context;
        private readonly Random _random;

        public ScheduleGenerator(ScheduleSystemEntities context)
        {
            _context = context;
            _random = new Random();
        }

        public void GenerateScheduleForClass(int classId)
        {
            try
            {
                var classInfo = _context.Groups.FirstOrDefault(g => g.Groups_GroupID == classId);
                if (classInfo == null)
                {
                    Console.WriteLine($"Класс с ID {classId} не найден.");
                    return;
                }

                // Очищаем старое расписание
                var existingSchedule = _context.Schedule.Where(s => s.Schedule_GroupID == classId).ToList();
                _context.Schedule.RemoveRange(existingSchedule);
                _context.SaveChanges();



                // Загружаем данные
                var subjects = _context.Subjects.ToList();
                var teachers = _context.Teachers.ToList();
                var rooms = _context.Rooms.ToList();
                var weeks = _context.Weeks.ToList();

                var lessonStartTime = new TimeSpan(8, 0, 0);
                var lessonDuration = new TimeSpan(0, 40, 0);
                var shortBreak = new TimeSpan(0, 10, 0);
                var lunchBreak = new TimeSpan(0, 30, 0);

                var schedule = new List<Schedule>();

                foreach (var week in weeks)
                {
                    for (int dayOfWeek = 1; dayOfWeek <= 5; dayOfWeek++) // Учебные дни
                    {
                        var dailySubjectsCount = new Dictionary<int, int>(); // Уроки по предметам за день
                        var dailyTeacherAvailability = new HashSet<int>(); // Занятые учителя
                        var dailyRoomAvailability = new HashSet<int>(); // Занятые аудитории

                        var currentTime = lessonStartTime;

                        for (int lessonNumber = 1; lessonNumber <= 6; lessonNumber++)
                        {
                            if (lessonNumber == 3) // Обед
                            {
                                currentTime += lunchBreak;
                                continue;
                            }

                            // Выбираем случайный предмет из доступных
                            var availableSubjects = subjects
                                .Where(s => s.Subjects_Hours > 0 &&
                                            (!dailySubjectsCount.ContainsKey(s.Subjects_SubjectID) || dailySubjectsCount[s.Subjects_SubjectID] < 2))
                                .ToList();

                            if (!availableSubjects.Any())
                                break;

                            var randomSubject = availableSubjects[_random.Next(availableSubjects.Count)];
                            var subjectId = randomSubject.Subjects_SubjectID;

                            // Выбираем случайного учителя, который может вести предмет
                            var availableTeachers = teachers.Where(t => t.Teachers_SubjectID == subjectId && !dailyTeacherAvailability.Contains(t.Teachers_TeacherID)).ToList();
                            if (!availableTeachers.Any())
                                continue;

                            var randomTeacher = availableTeachers[_random.Next(availableTeachers.Count)];
                            var teacherId = randomTeacher.Teachers_TeacherID;

                            // Выбираем случайную аудиторию
                            var availableRooms = rooms.Where(r => r.Rooms_TeacherID == teacherId && !dailyRoomAvailability.Contains(r.Rooms_RoomID)).ToList();

                            if (!availableRooms.Any())
                                continue;
                            var randomRoom = availableRooms[_random.Next(availableRooms.Count)];
                            var roomId = randomRoom.Rooms_RoomID;

                            // Добавляем урок в расписание
                            var lesson = new Schedule
                            {
                                Schedule_GroupID = classId,
                                Schedule_SubjectID = subjectId,
                                Schedule_TeacherID = teacherId,
                                Schedule_RoomID = roomId,
                                Schedule_WeekID = week.Weeks_WeekID,
                                Schedule_StartTime = currentTime,
                                Schedule_EndTime = currentTime + lessonDuration
                            };

                            schedule.Add(lesson);

                            // Обновляем состояние
                            randomSubject.Subjects_Hours -= 1; // Уменьшаем оставшиеся часы
                            dailySubjectsCount[subjectId] = dailySubjectsCount.ContainsKey(subjectId) ? dailySubjectsCount[subjectId] + 1 : 1;
                            dailyTeacherAvailability.Add(teacherId);
                            dailyRoomAvailability.Add(roomId);

                            // Увеличиваем текущее время
                            currentTime += lessonDuration + shortBreak;
                        }
                    }
                }

                // Добавляем расписание в базу
                if (schedule.Any())
                {
                    _context.Schedule.AddRange(schedule);
                    _context.SaveChanges();
                    Console.WriteLine($"Расписание для класса {classInfo.Groups_Name} успешно сгенерировано.");
                }
                else
                {
                    Console.WriteLine("Не удалось создать расписание: недостаточно данных или некорректные параметры.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при генерации расписания: {ex.Message}");
            }
        }
    }
}
