namespace TheReadersWritersProblem
{
    internal class Program
    {
        static Random rand = new Random();
        static readonly object gate = new();
        public static int readers = 0;
        public static int writers = 0;
        public static bool checking = false;
        public static bool fileOpen = false;
        public static bool fileReader = false;
        public static bool fileWriter = false;
        public static bool fileLastUserWriter = false;
        public static bool fileWriterWaiting = false;
        public static bool fileReaderWaiting = false;
        public static bool processing = false;

        public static List<Reader> readersList = new List<Reader>();
        public static List<Writer> writersList = new List<Writer>();
        public static List<Thread> threads = new List<Thread>();

        static void Main(string[] args)
        {
            Console.Write("How many readers are there?\n>> ");
            readers = Convert.ToInt32(Console.ReadLine());
            Console.Write("How many writers are there?\n>> ");
            writers = Convert.ToInt32(Console.ReadLine());
            processing = true;
            for (int i = 0; i < readers; i++)
            {
                readersList.Add(new Reader(i));
            }
            for (int i = 0; i < writers; i++)
            {
                writersList.Add(new Writer(i));
            }
            foreach (Reader r in readersList)
            {
                Thread t = new Thread(() => ThinkingReader(r));
                threads.Add(t);
                t.Start();
            }
            foreach (Writer w in writersList)
            {
                Thread t = new Thread(() => ThinkingWriter(w));
                threads.Add(t);
                t.Start();
            }
            processing = false;
            while (writersList.Count > 0 || readersList.Count > 0)
            {
                Thread.Sleep(300);
            }
            Console.WriteLine("The End");
        }
        static void ThinkingReader(Reader r)
        {
            int started = 0;
            while (processing) Thread.Sleep(100);

            while (started <= 10 || r.reading)          
            {
                lock (gate)
                {
                    int timeOpen = 0;
                    foreach (Reader l in readersList)
                        if (l.reading && l.read > timeOpen) timeOpen = l.read;

                    if (r.reading)
                    {
                        if (r.read >= r.readTime)
                        {
                            r.reading = false;
                            r.read = 0;
                            r.readTime = 0;
                            if (!readersList.Any(h => h.reading)) fileOpen = false;
                        }
                        else r.read++;
                    }
                    else if (started <= 10)
                    {
                        bool canJoin = fileOpen && !fileLastUserWriter && timeOpen < 5;

                        if (canJoin)                    
                        {
                            r.read = timeOpen;
                            r.readTime = timeOpen + Random.Shared.Next(1, 4);   
                            r.reading = true;
                            started++;
                            Console.WriteLine("Reader " + r.ID + " is reading!");
                        }
                        else if (r.CanRead())           
                        {
                            r.read = 0;
                            r.readTime = Random.Shared.Next(2, 8);
                            r.reading = true;
                            fileLastUserWriter = false;
                            fileOpen = true;
                            started++;
                            Console.WriteLine("Reader " + r.ID + " is reading!");
                        }
                    }
                }
                Thread.Sleep(10);
            }
            readersList.Remove(r);
        }
        static void ThinkingWriter(Writer w)
        {
            int started = 0;
            while (processing) Thread.Sleep(100);

            while (started <= 10 || w.writing)
            {
                lock (gate)
                {
                    if (w.writing)
                    {
                        if (w.write >= w.writeTime)
                        {
                            w.writing = false;
                            w.write = 0;
                            w.writeTime = 0;
                            fileOpen = false;      
                        }
                        else w.write++;
                    }
                    else if (started <= 10 && w.CanWrite())
                    {
                        w.write = 0;
                        w.writeTime = Random.Shared.Next(3, 10);
                        w.writing = true;
                        fileLastUserWriter = true;
                        fileOpen = true;
                        started++;
                        Console.WriteLine("Writer " + w.ID + " is writing!");
                    }
                }
                Thread.Sleep(10);  
            }
            writersList.Remove(w);
        }
    }
    class Reader
    {
        Random rand = new Random();
        public int ID;
        public int read = 0;
        public int readTime = 0;
        public bool reading = false;
        public Reader(int ID)
        {
            this.ID = ID;
        }
        public bool CanRead()
        {
            if (!Program.fileOpen && (Program.fileLastUserWriter || Program.writersList.Count() == 0)) 
            {
                return true; 
            }
            else
            {
                return false;
            }
        }
    }
    class Writer
    {
        Random rand = new Random();
        public int ID;
        public int write = 0;
        public int writeTime = 0;
        public bool writing = false;
        public Writer(int ID)
        {
            this.ID = ID;
        }
        public bool CanWrite()
        {
            if (!Program.fileOpen && (!Program.fileLastUserWriter || Program.readersList.Count() == 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}