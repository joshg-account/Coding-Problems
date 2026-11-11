namespace TheReadersWritersProblem
{
    internal class Program
    {
        static Random rand = new Random();
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
        }
        static void ThinkingReader(Reader r)
        {
            int i = 0;
            int timeOpen = 0;
            while (processing) Thread.Sleep(100);
            while (i < 10)
            {
                foreach (Reader l in readersList)
                {
                    if (l.read > timeOpen) timeOpen = l.read;
                }
                if (r.reading)
                {
                    if (r.read == r.readTime)
                    {
                        r.reading = false;
                        r.readTime = 0;
                        bool others = false;
                        foreach (Reader h in readersList)
                        {
                            if (h.reading) others = true;
                        }
                        if (!others) fileOpen = false;
                    }
                    else r.read++;
                }
                else if (timeOpen < 5 && r.CanRead() && !fileLastUserWriter)
                {
                    r.read = timeOpen;
                    r.reading = true;
                    i++;
                    Console.WriteLine("Reader " + r.ID + " is reading!");
                }
                else if (r.CanRead())
                {
                    r.read = 0;
                    r.readTime = rand.Next(2,8);
                    r.reading = true;
                    fileLastUserWriter = false;
                    fileOpen = true;
                    i++;
                    Console.WriteLine("Reader " + r.ID + " is reading!");
                }
                Thread.Sleep(100);
            }
        }
        static void ThinkingWriter(Writer w)
        {
            int i = 0;
            int timeOpen = 0;
            while (processing) Thread.Sleep(100);
            while (i < 10)
            {
                foreach (Writer l in writersList)
                {
                    if (l.write > timeOpen) timeOpen = l.write;
                }
                if (w.writing)
                {
                    if (w.write == w.writeTime)
                    {
                        w.writing = false;
                        w.writeTime = 0;
                        bool others = false;
                        foreach (Writer h in writersList)
                        {
                            if (h.writing) others = true;
                        }
                        if (!others) fileOpen = false;
                    }
                    else w.write++;
                }
                else if (w.CanWrite() && timeOpen == 0)
                {
                    w.write = 0;
                    w.writeTime = rand.Next(3,10);
                    w.writing = true;
                    fileLastUserWriter = true;
                    fileOpen = true;
                    i++;
                    Console.WriteLine("Writer " + w.ID + " is writing!");
                }
                Thread.Sleep(100);
            }
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