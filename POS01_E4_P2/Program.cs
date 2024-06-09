using E4_P2;

//Does not quite return the same value as the seed should, maybe an OS issue?

{
    Console.WriteLine($"Prüfe die Tipps auf Duplikate...");
    var lottoTipp = new LottoTipp();
    lottoTipp.AddQuicktipps(1000);
    for (int i = 0; i < 1000; i++)
    {
        var tipp = lottoTipp.GetTipp(i);
        if (tipp.Distinct().Count() != 6)
        {
            Console.Error.WriteLine($"FEHLER! Der Tipp {string.Join(",", tipp)} hat Duplikate!");
            return;
        }
        if (tipp.Max() > 45)
        {
            Console.Error.WriteLine($"FEHLER! Der Tipp {string.Join(",", tipp)} hat Zahlen über 45.");
            return;
        }
        if (tipp.Min() < 1)
        {
            Console.Error.WriteLine($"FEHLER! Der Tipp {string.Join(",", tipp)} hat Zahlen unter 1.");
            return;
        }
    }
}

{
    var lottoTipp = new LottoTipp();
    lottoTipp.AddQuicktipps(5);
    Console.WriteLine($"Generiere 5 Tipps...");
    for (int i = 0; i < 5; i++)
    {
        var tipp = lottoTipp.GetTipp(i);
        Console.WriteLine($"Tipp {i}: {string.Join(" ", tipp)}");
    }
}

{
    Console.WriteLine($"Generiere 1 000 000 Tipps und zähle die 6er, 5er, ...");
    var usedMemory = GC.GetTotalMemory(forceFullCollection: true);
    var lottoTipp = new LottoTipp();
    lottoTipp = new LottoTipp();
    lottoTipp.AddQuicktipps(1_000_000);
    var drawnNumbers = new int[] { 4, 2, 1, 8, 32, 16 };
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    for (int i = 0; i < 1_000_000; i++)
    {
        var rightNumbers = lottoTipp.CheckTipp(i, drawnNumbers);
        if (rightNumbers >= 5)
        {
            var tipp = lottoTipp.GetTipp(i);
            Console.WriteLine($"Tipp {i:000 000} hat {rightNumbers} Richtige: {string.Join(" ", tipp)}");

        }
    }
    stopwatch.Stop();
    Console.WriteLine($"Berechnung nach {stopwatch.ElapsedMilliseconds} ms beendet.");
    Console.WriteLine($"{(GC.GetTotalMemory(forceFullCollection: true) - usedMemory)/1048576M:0.00} MBytes belegt.");
}

// TODO: Implementiere die Klasse. Füge notwendige Properties und interne Listen hinzu.
namespace E4_P2
{
    class LottoTipp
    {
        private readonly Random _random = new Random(906);  // Fixed Seed, erzeugt immer die selbe Sequenz an Werten.
        private List<int[]> TippsList = new List<int[]>();

        public LottoTipp() {
            TippsList.Clear();
            AddQuicktipps(1);
        }
        /// <summary>
        /// Property; Gibt die Anzahl der gespeicherten Tipps zurück.
        /// </summary>
        public int TippCount => TippsList.Count;

        /// <summary>
        /// Gibt den nten gespeicherten Tipp als Array zurück. Der erste Tipp hat die Nummer 0.
        /// </summary>
        public int[] GetTipp(int number) { return TippsList[number]; }
    
        /// <summary>
        /// Generiert 6 zufällige Zahlen zwischen 1 und 45 ohne Kollision.
        /// </summary>
        private int[] GetNumbers() {
            int[] returnArray = new int[6];
            int i = 0;
            while (i < 6) {
                int numb = _random.Next(1,45);
                if (!returnArray.Contains(numb)) {
                    returnArray[i] = numb;
                    i++;
                }
            }
            return returnArray;
        }

        /// <summary>
        /// Fügt die in count übergebene Anzahl an Tipps zur internen Tippliste hinzu.
        /// </summary>
        /// <param name="count"></param>
        public void AddQuicktipps(int count)
        {
            for (int i = 0; i < count; i++) {
                TippsList.Add(GetNumbers());
            }
        }

        /// <summary>
        /// Prüft, wie viele Richtige der nte Tipp hat. Die Tippnummer beginnt bei 0
        /// (0 ist also der erste Tipp, ...).
        /// </summary>
        public int CheckTipp(int tippNr, int[] drawnNumbers) {
            int returnInt = 0;
            for (int i = 0; i < 6; i++) {
                if (TippsList[tippNr].Contains(drawnNumbers[i])) {
                    returnInt++;
                }
            }
            return returnInt;
        }
    }
}
