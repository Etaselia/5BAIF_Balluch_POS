﻿using _02LinQE1.Model;
using static _02LinQE1.ProgramChecker;

namespace _02LinQE1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using ExamsDb db = ExamsDb.FromSeed();
            var results = JsonDocument.Parse(File.ReadAllText("/home/eta/RiderProjects/5BAIF_Balluch_POS/POS02-E2/results.json", System.Text.Encoding.UTF8)).RootElement;
            // *************************************************************************************
            // MUSTERBEISPIELE
            // *************************************************************************************
            // 1. Suche den Schüler mit der ID 1003
            //    Where liefert IEnumerable, also immer eine Collecion.
            //    Deswegen brauchen wir First, um auf das erste Element zugreifen
            //    zu können.
            Student demo1 = db.Students.Where(s => s.Id == 1003).First();
            demo1.WriteJson("(1) Schüler mit der ID 1003");
            // 2. Welcher Schüler hat die Id 999?
            //    First liefert eine Exception, da die Liste leer ist.
            //    FirstOrDefault liefert in diesem Fall den Standardwert (null).
            Student? demo2 = db.Students.Where(s => s.Id == 999).FirstOrDefault();

            // 3. Wie viele Schüler sind in der Liste?
            int demo3 = db.Students.Count();
            demo3.WriteJson("(3) Schüler in der Liste");

            // 4. Wie viele Schüler gehen in die 3BHIF?
            int demo4 = db.Students.Where(s => s.Schoolclass == "3BHIF").Count();
            //    Für Count existiert eine Überladung, die auch eine Filterfunktion
            //    bekommen kann.
            demo4 = db.Students.Count(s => s.Schoolclass == "3BHIF");
            demo4.WriteJson("(4) Schüler der 3BHIF");

            // *************************************************************************************
            // ÜBUNGEN
            // *************************************************************************************
            // Genereller Hinweis: Schreibe nach deiner Abfrage - wenn sie eine Collection liefert - .ToList().
            // Sonst kann es zu Exceptions bei CheckJsonAndWrite kommen, da in wirklichkeit ein OR Mapper
            // dahintersteckt und eine Datenbankfrage ausgewertet wird. ToList() lädt das Ergebnis vorab
            // in den Speicher.
            
            // 1. Welche Note hat die Prüferin FAV bei ihrer schlechtesten Prüfung vergeben.
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result1 (kein var verwenden!).
            int result1 = db.Exams.Where(e => e.Examinator == "FAV").Max(e => e.Grade);
            CheckJsonAndWrite(result1, results.GetProperty("Result1"), "Beispiel 1");

            // 2. Welchen Notendurchschnitt haben die weiblichen Schülerinnen in POS?
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result2 (kein var verwenden!).
            double result2 = db.Students.Where(s => s.Gender == Gender.Female).SelectMany(s => s.Exams).Where(e => e.Subject == "POS").Average(e => e.Grade);
            CheckJsonAndWrite(result2, results.GetProperty("Result2"), "Beispiel 2");

            // 3. Welche Schüler haben 6 oder mehr Prüfungen? Gib eine Liste von Schülern zurück und gib Sie aus.
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result3 (kein var verwenden!)
            IEnumerable<Student> result3 = db.Students.Where(s => s.Exams.Count() >= 6).ToList();
            CheckJsonAndWrite(result3, results.GetProperty("Result3"), "Beispiel 3");

            // 4. Welche Schüler haben eine DBI Prüfung? Gib eine Liste von Schülern zurück und gib sie aus.
            //    Hinweis: kombiniere Where und Any, indem Any in der Where Funktion verwendet wird.
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result4 (kein var verwenden!).
            IEnumerable<Student> result4 = db.Students.Where(s => s.Exams.Any(e => e.Subject == "DBI")).ToList();
            CheckJsonAndWrite(result4, results.GetProperty("Result4"), "Beispiel 4");

            // 5. Gibt es Schüler, die nur in POS eine Prüfung haben?
            //    Gib eine Liste von Schülern zurück und gib sie aus.
            //    Hinweis: kombiniere Where und All, indem All in der Where Funktion verwendet wird.
            //    All gibt auch Schüler aus, die keine Prüfung haben. Dies kann so belassen werden.
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result5 (kein var verwenden!).
            IEnumerable<Student> result5 = db.Students.Where(s => s.Exams.All(e => e.Subject == "POS")).ToList();
            CheckJsonAndWrite(result5, results.GetProperty("Result5"), "Beispiel 5");

            // 6. Welche Schüler haben keine POS Prüfung? Gib eine Liste von Schülern zurück und gib sie aus.
            //    Hinweis: kombinieren Where und Any, indem Any in der Where Funktion verwendet wird.
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result6 (kein var verwenden!).
            IEnumerable<Student> result6 = db.Students.Where(s => s.Exams.All(e => e.Subject != "POS")).ToList();
            CheckJsonAndWrite(result6, results.GetProperty("Result6"), "Beispiel 6");

            // 7. Welche Schüler haben überhaupt keine Prüfung? Gib eine Liste von Schülern zurück und gib sie aus.
            //     Hinweis: Verwende Any statt dem Count Property.
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result7 (kein var verwenden!).
            IEnumerable<Student> result7 = db.Students.Where(s => !s.Exams.Any()).ToList();
            CheckJsonAndWrite(result7, results.GetProperty("Result7"), "Beispiel 7");

            // 8. Welche Schüler hatten in Juni AM Prüfungen? Gib eine Liste von Prüfungen zurück.
            //     Hinweis: Verwende das Month Property des Datum Feldes.
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result8 (kein var verwenden!).
            IEnumerable<Exam> result8 = db.Exams.Where(e => e.Subject == "AM" && e.Date.Month == 6).ToList();
            CheckJsonAndWrite(result8, results.GetProperty("Result8"), "Beispiel 8");

            // 9. Welche Schüler haben in E nur negative Prüfungen? Überlege, warum man diesen Filterausdruck
            //     nicht zusammengesetzt schreiben kann.
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result9 (kein var verwenden!).
            IEnumerable<Student> result9 = db.Students.Where(s => s.Exams.Where(e => e.Subject == "E").All(e => e.Grade > 4)).ToList();
            CheckJsonAndWrite(result9, results.GetProperty("Result9a"), results.GetProperty("Result9b"), "Beispiel 9");

            // 10. Welche Schüler haben im Mittel bessere DBI Prüfungen als D Prüfungen. Anders gesagt: Bei wem
            //     ist der Notenschnitt der DBI Prüfungen kleiner als der Notenschnitt der D Prüfungen.
            //     Gib eine Liste von Schülern zurück, auf die das zutrifft.
            //     Hinweise:
            //       -) Wenn mit Where gefiltert wird, kann es sein, dass eine leere Liste zurückkommt.
            //       -) Average kann nur von einer Liste mit Elementen aufgerufen werden.
            //       -) Erstelle daher eine Lambda Expression mit try und catch im inneren, die false im Fehlerfall liefert.
            // Schreibe das Ergebnis mit dem richtigen Datentyp in die Variable result10 (kein var verwenden!).
            // Beginne deine Abfrage mit db.Students.ToList(), da für diesen Ausdruck alle Daten
            // vorher in den Speicher geladen werden müssen.
            IEnumerable<Student> result10 = db.Students.ToList().Where(s => 
            {
                try 
                { 
                    return s.Exams.Where(e => e.Subject == "DBI").Average(e => e.Grade) < s.Exams.Where(e => e.Subject == "D").Average(e => e.Grade); 
                } 
                catch 
                { 
                    return false; 
                }
            });
            CheckJsonAndWrite(result10, results.GetProperty("Result10"), "Beispiel 10");

            WriteSummary();
        }
    }
}