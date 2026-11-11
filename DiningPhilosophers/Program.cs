using System.Security.Cryptography.X509Certificates;

namespace DiningPhilosophers
{
    internal class Program
    {
        public static int thoughts = 0;
        public static bool processing = false;
        public static List<Philospoher> atendees = new List<Philospoher>();
        static void Main(string[] args)
        {
            
            List<Thread> threads = new List<Thread>();
            Console.Write("How many philosophers are at the table?\n>> ");
            int philosophers = Convert.ToInt32(Console.ReadLine());
            Console.Write("How many thoughts will the philosophers have?\n>> ");
            thoughts = Convert.ToInt32(Console.ReadLine());
            processing = true;
            for (int i = 0; i < philosophers; i++)
            {
                atendees.Add(new Philospoher(i));
            }
            foreach (Philospoher p in atendees)
            {
                Thread t = new Thread(() => Thinking(p));
                threads.Add(t);
                t.Start();
            }
            processing = false;
        }
        public static void CheckPlates(int ID)
        {
            bool food = false;
            foreach (Philospoher p in atendees)
            {
                if (p.food) food = true;
            }
            Console.WriteLine("food eaten by philosopher " + (ID + 1));
            Thread.Sleep(100);
            if (!food) 
            {
                Console.WriteLine("All Philosphers are full");        
                Console.Write("Press Any Key To Exit.");     
                Console.ReadKey(); // prevents terminal from instantly closing
            }
        }
        public static void Thinking(Philospoher p)
        {
            while (processing) Thread.Sleep(100);
            while (!p.eating && p.food)
            {
                var (l, r) = p.Philosophise();
                if (l == r)
                {
                    bool leftFork = false;
                    bool rightFork = false;
                    if (l == 0)
                    {
                        leftFork = atendees[atendees.Count - 1].eating;
                        rightFork = atendees[l + 1].eating;
                    }
                    else if (l == atendees.Count - 1)
                    {
                        leftFork = atendees[l - 1].eating;
                        rightFork = atendees[0].eating;
                    }
                    else
                    {
                        leftFork = atendees[l - 1].eating;
                        rightFork = atendees[l + 1].eating;
                    }
                    if (!leftFork && !rightFork) p.eating = true;
                    else Thread.Sleep(100);
                }
            }
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(1);
            }
            p.eating = false;
            p.food = false;
            CheckPlates(p.ID);
        }

    }
    class Philospoher
    {
        Random rand = new Random();
        public int ID;
        public bool eating = false;
        public bool food = true;
        public Philospoher(int ID)
        {
            this.ID = ID;
        }
        public (int, int) Philosophise()
        {
            return (this.ID, rand.Next(0, (Program.atendees.Count + Program.thoughts)));
        }
    }
}