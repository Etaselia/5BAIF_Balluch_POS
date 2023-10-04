using System;

namespace TypesDemo.Application
{
    class TypeExercise
    {
        /// <summary>
        /// Geben Sie die Fäche des Rechtecks (Länge x Breite) zurück.
        /// Wenn ein Wert (Länge oder Breite) null ist, soll das Ergebnis
        /// auch null sein. Ersetzen Sie ? durch den korrekten Datentyp.
        /// </summary>
        public double? BerechneFlaeche(double? laenge, double? breite)
        {
            return laenge * breite;
        }

        /// <summary>
        /// Geben Sie die Fäche des Rechtecks (Länge x Breite) zurück.
        /// Wenn ein Wert (Länge oder Breite) null ist, soll das Ergebnis
        /// 0 sein.
        /// </summary>
        public double BerechneFlaeche2(double? laenge, double? breite)
        {
            if (laenge != null && breite != null)
            {
                return  (double)(laenge * breite);
            }
            return 0;
        }

        /// <summary>
        /// Berechnen Sie den Preis nach folgender Vorschrift:
        /// In steuerProdukt und steuerKategorie sind Steuersätze als Faktoren
        /// gespeichert, also 1.2 für 20%. Sie müssen daher bei der Berechnung
        /// nur den Preis mit diesem Wert multiplizieren.
        /// 
        /// Ist ein Wert für steuerProdukt gesetzt (nicht null), so ist nur dieser
        /// Wert für die Berechnung heranzuziehen (also nettopreis x steuerProdukt).
        /// 
        /// Ist kein Wert für steuerProdukt gesetzt, so ist der Wert in steuerKategorie
        /// heranzuziehen (nettopreis x steuerKategorie).
        /// 
        /// Sind beide Werte nicht gesetzt, ist der Nettopreis x 1.2 zurückzugeben.
        /// 
        /// Verwenden Sie den ?? Operator.
        /// </summary>
        public decimal BerechnePreis(decimal nettopreis, decimal? steuerProdukt, decimal? steuerKategorie)
        {
            if (steuerProdukt.HasValue)
            {
                // Wenn steuerProdukt gesetzt ist, berechne den Preis mit diesem Wert.
                return nettopreis * steuerProdukt.Value;
            }
            else if (steuerKategorie.HasValue)
            {
                // Wenn steuerKategorie gesetzt ist, berechne den Preis mit diesem Wert.
                return nettopreis * steuerKategorie.Value;
            }
            else
            {
                // Wenn beide Werte nicht gesetzt sind, berechne den Preis mit einem Standardwert von 1.2 (20% Steuersatz).
                return nettopreis * 1.2m;
            }
        }

        /// <summary>
        /// Geben Sie die durchschnittliche Schülerzahl pro Klasse zurück. Sie
        /// berechnet sich aus schuelerGesamt / klassenGesamt.
        /// </summary>
        public double BerechneSchuelerProKlasse(int schuelerGesamt, int klassenGesamt)
        {
            try
            {
                return  (double) schuelerGesamt / (double) klassenGesamt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Geben Sie ein Achtel (also wert / 8) des übergebenen Wertes
        /// zurück. Achten Sie auf den Datentyp des Rückgabewertes.
        /// Kann in dieser Funktion eine Exception auftreten?
        /// </summary>
        public int BerechneAchtel(long wert)
        {
            try
            {
                return (int)(wert / 8);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Berechne die Länge des gesamten Namens (Vor- und Zuname).
        /// Ist vorname oder zuname null, so ist für diesen Teil des Namens die Länge 0
        /// anzunehmen.
        /// </summary>
        public int BerechneLaenge(string? vorname, string? nachname)
        {
            int lenght = 0;
            if (vorname != null)
            {
                lenght += vorname.Length;
            }

            if (nachname != null)
            {
                lenght += nachname.Length;
            }

            return lenght;
        }

    }
    class Program
    {
        static void Main(string[] args)
        {

            TypeExercise typeExercise = new TypeExercise();

            Console.WriteLine("BerechneFlaeche(3,4):              " + typeExercise.BerechneFlaeche(3, 4));
            Console.WriteLine("BerechneFlaeche(3,null):           " + typeExercise.BerechneFlaeche(3, null));
            Console.WriteLine("BerechneFlaeche2(3,null):          " + typeExercise.BerechneFlaeche2(3, null));

            Console.WriteLine("BerechnePreis(100,1.2,null):       " + typeExercise.BerechnePreis(100, 1.2M, null));
            Console.WriteLine("BerechnePreis(100,1.2,1.1):        " + typeExercise.BerechnePreis(100, 1.2M, 1.1M));
            Console.WriteLine("BerechnePreis(100,null,1.1):       " + typeExercise.BerechnePreis(100, null, 1.1M));
            Console.WriteLine("BerechnePreis(100,null,null):      " + typeExercise.BerechnePreis(100, null, null));

            Console.WriteLine("BerechneSchuelerProKlasse(100, 6): " + typeExercise.BerechneSchuelerProKlasse(100, 6));
            Console.WriteLine("BerechneAchtel(120):               " + typeExercise.BerechneAchtel(120));
            Console.WriteLine("BerechneLaenge(null, nachname):    " + typeExercise.BerechneLaenge(null, "nachname"));            
        }
    }
}
