namespace CigaretteSmokers
{
    internal class Program
    {
        static readonly object chairLock = new();
        static public int waitingChairs;
        static public int customersWalkIn;
        static public int customersID = 0;
        static public int customersExited = 0;
        static public List<Customer> waiting = new List<Customer>();
        static public List<Thread> threads = new List<Thread>();
        static public Customer? chair = null;
        static void Main(string[] args)
        {
            Console.Write("How many waiting chairs are there? \n>> ");
            waitingChairs = Convert.ToInt32(Console.ReadLine());
            Console.Write("How many customers can walk in? \n>> ");
            customersWalkIn = Convert.ToInt32(Console.ReadLine());
            while (customersID < customersWalkIn)
            {
                Customer t = new Customer(customersID);
                Console.WriteLine("Customer #" + (customersID + 1) + " has entered the shop");
                if (chair == null)
                {
                    lock (chairLock)
                    {
                        chair = t;
                        threads.Add(new Thread(() => CustomerThinking(t)));
                        Console.WriteLine("Customer #" + (customersID + 1) + " has sat on the chair");
                    }
                }
                else if (waiting.Count < waitingChairs)
                {
                    waiting.Add(t);
                    threads.Add(new Thread(() => CustomerThinking(t)));
                    Console.WriteLine("Customer #" + (customersID + 1) + " is now waiting");
                }
                else
                {
                    Console.WriteLine("Customer #" + (customersID + 1) + " has left the shop");
                    customersExited++;
                }
                customersID++;
                Thread.Sleep(500);
            }
        }
        static void CustomerThinking(Customer c)
        {
            while (c.inShop)
            {
                if (c == chair)
                {
                    Thread.Sleep(1000);
                    threads.Remove(Thread.CurrentThread);
                    Console.WriteLine("Customer #" + (c.ID + 1) + " has finished and has left the shop");
                    c.inShop = false;
                    chair = null;
                }
                else
                {
                    if (chair == null)
                    {
                        lock (chairLock)
                        {
                            chair = c;
                            waiting.Remove(c);
                            Console.WriteLine("Customer #" + (c.ID + 1) + " has sat on the chair");
                        }
                    }
                }
            }
            
            
        }
    }
    class Customer
    {
        public int ID;
        public bool inShop = true;
        public Customer(int ID)
        {
            this.ID = ID;
        }
    }
}