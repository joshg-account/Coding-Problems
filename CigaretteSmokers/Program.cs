namespace CigaretteSmokers
{
    internal class Program
    {
        public static List<String> Table = new List<string>();
        public static List<Thread> threads = new List<Thread>();
        public static Random rand = new Random();
        public static string[] ingredients = new string[3];
        static void Main(string[] args)
        {
            ingredients[0] = "Match";
            ingredients[1] = "Tobacco";
            ingredients[2] = "Paper";
            Thread agent = new Thread(AgentBrain);
            threads.Add(agent);
            agent.Start();
            for (int i = 0; i < 3; i++)
            {
                int ID = i;
                Thread smoker = new Thread(() => SmokerBrain(ID));
                threads.Add(smoker);
                smoker.Start();
            }
            Console.ReadKey();
        }
        static void SmokerBrain(int ingred)
        {
            while (true)
            {
                if (Table.Count > 0 && (Table[0] != ingredients[ingred] && Table[1] != ingredients[ingred]))
                {
                    Console.WriteLine("Smoker #" + (ingred + 1) + " has picked up the ingredients");
                    Console.WriteLine("Smoker #" + (ingred + 1) + " is smoking");
                    Table.Clear();
                    Thread.Sleep(1000);
                    Console.WriteLine("Smoker #" + (ingred + 1) + " is back");
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
        }
        static void AgentBrain()
        {
            while (true)
            {
                if (Table.Count == 0)
                {
                    while (Table.Count < 2)
                    {
                        int ingred = rand.Next(0,3);
                        if (!Table.Contains(ingredients[ingred]))
                        {
                            Table.Add(ingredients[ingred]);
                            Console.WriteLine("Agent has added " + ingredients[ingred] + " to the table.");
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }
    }
}