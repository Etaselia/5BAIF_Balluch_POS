using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grouping.Model;
using System.Text.Json;

namespace Grouping
{
    class Program
    {
        static void Main(string[] args)
        {
            TestsData data = TestsData.FromFile("db/tests.json");

            // Exercise 1: How many classes are there per department?
            var result1 = data.Schoolclass
                              .GroupBy(schoolclass => schoolclass.C_Department)
                              .Select(group => new { Department = group.Key, Count = group.Count() });
            Console.WriteLine("RESULT1");
            Console.WriteLine(JsonSerializer.Serialize(result1));

            // Exercise 2: How many, but only departments with more than 10 classes.
            var result2 = result1
                              .Where(department => department.Count > 10);
            Console.WriteLine("RESULT2");
            Console.WriteLine(JsonSerializer.Serialize(result2));

            // Exercise 3: When is the last test (Max of TE_Date) per teacher and subject for the 5AHIF in the Test table?
            var result3 = data.Test
                              .Where(test => test.TE_Class == "5AHIF")
                              .GroupBy(test => new { test.TE_Teacher, test.TE_Subject })
                              .Select(group => new { TeacherSubject = group.Key, LastTestDate = group.Max(test => test.TE_Date) });
            Console.WriteLine("RESULT3");
            Console.WriteLine(JsonSerializer.Serialize(result3));

            // Exercise 4: Filter for the teacher who had the last lesson of the day in a room.
            // This requires a subquery to find the last lesson for that room and day.
            var result4 = data.Lesson
                              .Where(lesson => data.Lesson
                                                .Any(subLesson => subLesson.L_Room == lesson.L_Room &&
                                                                  subLesson.L_Day == lesson.L_Day &&
                                                                  subLesson.L_Hour > lesson.L_Hour) == false &&
                                               lesson.L_Room?.StartsWith("C2") == true &&
                                               lesson.L_Day == 1)
                              .Select(lesson => new { lesson.L_Room, lesson.L_Day, lesson.L_Hour, lesson.L_Teacher, lesson.L_Class });
            Console.WriteLine("RESULT4");
            Console.WriteLine(JsonSerializer.Serialize(result4));

            // Exercise 5: Optimized version of Exercise 4 using a view-like collection and join operation.
            var lastLesson = data.Lesson
                .Where(l => l.L_Room != null) // Ensure L_Room is not null before grouping
                .GroupBy(l => new { l.L_Room, l.L_Day })
                .Select(g => new { Room = g.Key.L_Room, Day = g.Key.L_Day, LastHour = g.Max(l => l.L_Hour) })
                .ToList();

            var result5 = from lesson in data.Lesson
                where lesson.L_Room != null && lesson.L_Room.StartsWith("C2") && lesson.L_Day == 1 // Check for null before using L_Room
                join last in lastLesson 
                    on new { Room = lesson.L_Room, Day = lesson.L_Day, Hour = lesson.L_Hour } 
                    equals new { last.Room, last.Day, Hour = last.LastHour }
                select new { lesson.L_Room, lesson.L_Day, lesson.L_Hour, lesson.L_Teacher, lesson.L_Class };

            
            Console.WriteLine("RESULT5");
            Console.WriteLine(JsonSerializer.Serialize(result5));

            // alright it seems not to serialize the data correctly here, so i added a null check, now it seems to work decently
        }
    }
}