namespace DiningPhilosophers
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
            while (customersID + 1 < customersWalkIn)
            {
                Customer t = new Customer(customersID);
                Console.WriteLine("Customer #" + customersID + " has entered the shop");
                if (chair == null)
                {
                    lock (chairLock)
                    {
                        chair = t;
                        Console.WriteLine("Customer #" + customersID + " has sat on the chair");
                    }
                }
                else if (waiting.Count < waitingChairs)
                {
                    waiting.Add(t);
                    threads.Add(new Thread(() => CustomerThinking(t)));
                    Console.WriteLine("Customer #" + customersID + " is now waiting");
                }
                else
                {
                    Console.WriteLine("Customer #" + customersID + " has left the shop");
                    customersExited++;
                }
                customersID++;
                Thread.Sleep(500);
            }
        }
        static void CustomerThinking(Customer c)
        {
            
        }
    }
    class Customer
    {
        public int ID;
        public Customer(int ID)
        {
            this.ID = ID;
        }
    }
}