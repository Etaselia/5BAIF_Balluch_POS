using System;
using System.Collections.Generic;
using System.Linq;
using Grouping.Model;
using System.Text.Json;

namespace Grouping
{
    class Program
    {
        private static JsonSerializerOptions serializerOptions
            = new JsonSerializerOptions { WriteIndented = false };
        static void Main(string[] args)
        {

            // *************************************************************************************
            // Schreibe in den nachfolgenden Übungen statt der Zeile
            // List<object> result = null!;
            // die korrekte LINQ Abfrage. Verwende den entsprechenden Datentyp statt object.
            // Du kannst eine "schöne" (also eingerückte) Ausgabe der JSON Daten erreichen, indem
            // du die Variable WriteIndented in Zeile 12 auf true setzt.
            //
            // !!HINWEIS!!
            // Beende deine Abfrage immer mit ToList(), damit die Daten für die JSON Serialisierung
            // schon im Speicher sind. Ansonsten würde es auch einen Compilerfehler geben, da
            // WriteJson() eine Liste haben möchte.
            // *************************************************************************************

            ExamsDb db = ExamsDb.FromFiles("/home/eta/RiderProjects/5BAIF_Balluch_POS/POS02-E4/csv");

            {
                var result = db.Exams
                    .GroupBy(e => new { e.TeacherId, e.Teacher.Lastname })
                    .Select(g => new
                    {
                        g.Key.TeacherId,
                        g.Key.Lastname,
                        ExamsCount = g.Count()
                    })
                    .OrderBy(e => e.TeacherId)
                    .Take(3)
                    .ToList();
                Console.WriteLine("MUSTER: Anzahl der Prüfungen pro Lehrer (erste 3 Lehrer).");
                WriteJson(result);
            }

            // *************************************************************************************
            // ÜBUNG 1: Erstelle für jeden Lehrer eine Liste der Fächer, die er unterrichtet. Es
            // sind nur die ersten 10 Datensätze auszugeben. Das kann mit
            // .OrderBy(t=>t.TeacherId).Take(10)
            // am Ende der LINQ Anweisung gemacht werden. Hinweis: Verwende Distinct für die
            // liste der Unterrichtsgegenstände.
            // *************************************************************************************
            {
                // Fetch the necessary data into memory
                var lessonsList = db.Lessons.Select(l => new { l.TeacherId, l.Subject }).ToList();

                // Perform the grouping and other operations in memory
                var result = lessonsList
                    .GroupBy(l => l.TeacherId)
                    .Select(g => new 
                    {
                        TeacherId = g.Key,
                        Subjects = g.Select(l => l.Subject).Distinct()
                    })
                    .OrderBy(t => t.TeacherId)
                    .Take(10)
                    .ToList();
                
                Console.WriteLine("RESULT1");
                WriteJson(result);
            }

            // *************************************************************************************
            // ÜBUNG 2: Die 5AHIF möchte wissen, in welchem Monat sie welche Tests hat.
            //          Hinweis: Mit den Properties Month und Year kann auf das Monat bzw. Jahr
            //          eines DateTime Wertes zugegriffen werden. Die Ausgabe in DisplayMonth kann
            //          $"{mydate.Year:00}-{mydate.Month:00}" (mydate ist zu ersetzen)
            //          erzeugt werden
            // *************************************************************************************
            {
                var result = db.Exams.Where(e => e.SchoolclassId == "5AHIF")
                    .GroupBy(exam => new { exam.Date.Year, exam.Date.Month })
                    .Select(g => new {Exams = g.Select(e => e.Subject), Date = $"{g.Key.Year:0000}-{g.Key.Month:00}" }).ToList();
                Console.WriteLine("RESULT2");
                WriteJson(result);
            }

            // *************************************************************************************
            // ÜBUNG 3: Jeder Schüler der 5AHIF soll eine Übersicht bekommen, welche Tests er pro Fach
            //          abgeschlossen hat.
            //          Es sind nur die ersten 2 Schüler mit OrderBy(p => p.Id).Take(2) am Ende des
            //          Statements auszugeben.
            //          Hinweis: Beachte die Datenstruktur in der Ausgabe.
            //   Pupil                           <-- Zuerst wird der Schüler projiziert (Select)
            //     |
            //     +-- Id
            //         Firstname
            //         Lastname
            //         Exams                     <-- Hier soll nach Subject gruppiert werden
            //           |
            //           +---- Subject           <-- Key der Gruppierung
            //           +---- SubjectExams      <-- Projektion der Gruppierung
            //                    |    
            //                    +------ Teacher
            //                    +------ Date
            //                    +------ Lesson
            // *************************************************************************************
            {
                var result = db.Pupils
                    .Where(p => p.SchoolclassId == "5AHIF") // Filter to include only pupils from 5AHIF
                    .OrderBy(p => p.Id)
                    .Take(2)
                    .Select(p => new 
                    {
                        p.Id,
                        p.Firstname,
                        p.Lastname,
                        Exams = db.Exams
                            .Where(e => e.SchoolclassId == p.SchoolclassId && e.SchoolclassId == p.SchoolclassId) // Get exams for each pupil
                            .GroupBy(e => e.Subject)
                            .Select(examGroup => new 
                            {
                                Subject = examGroup.Key,
                                SubjectExams = examGroup.Select(e => new 
                                {
                                    Teacher = e.Teacher.Lastname, // Access the teacher's lastname
                                    e.Date,
                                    Lesson = e.Period.Lessons.ToList() // Access the lesson via Period
                                }).ToList()
                            }).ToList()
                    })
                    .ToList();

                Console.WriteLine("RESULT3");
                WriteJson(result);
            }



            // *************************************************************************************
            // ÜBUNG 4: Wie viele Klassen sind pro Tag und Stunde gleichzeitig im Haus?
            //          Hinweis: Gruppiere zuerst nach Tag und Stunde in Lesson. Für die Ermittlung
            //          der Klassenanzahl zähle die eindeutigen KlassenIDs, indem mit Distinct eine
            //          Liste dieser IDs (Id) erzeugt wird und dann mit Count() gezählt wird.
            //          Es sind mit OrderByDescending(g=>g.ClassCount).Take(5) nur die 5
            //          "stärksten" Stunden auszugeben.
            // *************************************************************************************
            {
                List<object> result = null!;
                Console.WriteLine("RESULT4");
                WriteJson(result);
            }

            // *************************************************************************************
            // ÜBUNG 5: Wie viele Klassen gibt es pro Abteilung?
            // *************************************************************************************
            {
                List<object> result = null!;
                Console.WriteLine("RESULT5");
                WriteJson(result);
            }

            // *************************************************************************************
            // ÜBUNG 6: Wie die vorige Übung, allerdings sind nur Abteilungen
            //          mit mehr als 10 Klassen auszugeben.
            //          Hinweis: Filtere mit Where nach dem Erstellen der Objekte mit Department
            //                   und Count
            // *************************************************************************************
            {
                List<object> result = null!;
                Console.WriteLine("RESULT6");
                WriteJson(result);
            }

            // *************************************************************************************
            // ÜBUNG 7: Wann ist der letzte Test (Max von Exam.Date) pro Lehrer und Fach der 5AHIF
            //          in der Tabelle Exams?
            {
                List<object> result = null!;
                Console.WriteLine("RESULT7");
                WriteJson(result);
            }
        }

        public static void WriteJson<T>(List<T> result)
        {
            if (result is not null && typeof(T) == typeof(object))
            {
                Console.WriteLine("Warum erstellst du eine Liste von Elementen mit Typ object?");
                return;
            }
            Console.WriteLine(JsonSerializer.Serialize(result, serializerOptions));
        }
    }
}
