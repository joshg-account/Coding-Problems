using System.Reflection.Metadata;

namespace BarberShop
{
    internal class Program
    {
        static readonly object chairLock = new();
        static public Random rand = new Random();
        static public bool BarberAwake;
        static public float timeSpeed = 1f;
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
            Thread barb = new Thread(BarberThinking);
            threads.Add(barb);
            barb.Start();
            while (customersID < customersWalkIn)
            {
                Customer t = new Customer(customersID);
                Thread th = new Thread(() => CustomerThinking(t));
                Console.WriteLine("Customer #" + (customersID + 1) + " has entered the shop");
                if (chair == null)
                {
                    lock (chairLock)
                    {
                        if (BarberAwake == false)
                        {
                            Console.WriteLine("Customer #" + (customersID + 1) + " has woken the barber");
                            BarberAwake = true;
                        }
                        chair = t;
                        threads.Add(th);
                        Console.WriteLine("Customer #" + (customersID + 1) + " has sat on the chair");
                    }
                }
                else if (waiting.Count < waitingChairs)
                {
                    waiting.Add(t);
                    threads.Add(th);
                    Console.WriteLine("Customer #" + (customersID + 1) + " is now waiting");
                }
                else
                {
                    Console.WriteLine("Customer #" + (customersID + 1) + " has left the shop");
                    customersExited++;
                }

                customersID++;
                th.Start();
                Thread.Sleep(rand.Next(500,1500));
            }
            while (waiting.Count > 0 || chair != null)
            {
                Thread.Sleep(300);
            }
            Console.WriteLine("The End");
            Console.Write("Press Any Key To Exit.");
            Console.ReadKey(); 
        }
        static void BarberThinking()
        {
            while (customersID > customersWalkIn)
            {
                if (waiting.Count == 0 && chair == null)
                {
                    Console.WriteLine("Barber has fallen asleep!");
                    BarberAwake = false;
                }
                Thread.Sleep(50);
            }
        }
        static void CustomerThinking(Customer c)
        {
            while (c.inShop)
            {
                if (c == chair)
                {
                    Thread.Sleep(rand.Next(200,2000));
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
                            if (BarberAwake == false)
                            {
                                Console.WriteLine("Customer #" + (customersID + 1) + " has woken the barber");
                                BarberAwake = true;
                            }
                            chair = c;
                            waiting.Remove(c);
                            Console.WriteLine("Customer #" + (c.ID + 1) + " has sat on the chair");
                        }
                    }
                }
                Thread.Sleep(50);
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