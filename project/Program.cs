using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
namespace project
{

    class Program
    {
        public static HashSet<string> vertices;
        public static Dictionary<string, List<string>> links;
        public static Dictionary<string, Dictionary<string, List<string>>> final_dict;
        public static Dictionary<string, string> color ;
        public static Dictionary<string, int> d ;
        public static Stopwatch TimeFile;
        public static Stopwatch algorithmTime;
        public static string Movie_path;
        public static string[] Queries;
        public static string query_path;
        public static string solu_path;
        public static void filereader(string movie)
        {
            var lines = File.ReadLines(movie);
            var splitLines = lines.Select(line => line.Split('/')).ToArray();
            links = new Dictionary<string, List<string>>();
            vertices = new HashSet<string>();


            foreach (string[] x in splitLines)
            {
                links.Add(x[0], new List<string>());
                for (int i = 1; i < x.Length; i++)
                {
                    links[x[0]].Add(x[i]);
                    vertices.Add(x[i]);
                }

            }

        }
        public static void adjecency()
        {
            final_dict = new Dictionary<string, Dictionary<string, List<string>>>();
            foreach (var (key, value) in links)
            {

                for (int i = 0; i < value.Count; i++)
                {

                    // add final dict
                    if (!final_dict.ContainsKey(value[i]))
                    {
                        final_dict.Add(value[i], new Dictionary<string, List<string>>());
                        for (int j = 0; j < value.Count; j++)
                        {
                            if (j == i) continue;
                            final_dict[value[i]].Add(value[j], new List<string>() { key });

                        }
                    }

                    else
                    {
                        for (int j = 0; j < value.Count; j++)
                        {
                            if (j == i) continue;

                            if (final_dict[value[i]].ContainsKey(value[j]))
                            {
                                final_dict[value[i]][value[j]].Add(key);
                            }
                            else
                            {
                                final_dict[value[i]].Add(value[j], new List<string>() { key });
                            }

                        }
                    }


                }

            }
        }
        public static void Bfs(string [] query_f,string soul_path, int op)
        {
            string source, dst;

            //intialize vertics
            color = new Dictionary<string, string>();
            d = new Dictionary<string, int>();
            foreach (string actor in vertices)
            {
                color.Add(actor, "white");
                d.Add(actor, 1000000000);

            }
            //watch.Start();
            int count;
            foreach (string line in query_f)
            {
                string[] words = line.Split('/');
                source = words[0];
                dst = words[1];

                //output variables
                int dos;
                List<string> chain = new List<string>();
                int RS;

                // bfs code
                bool movie_change = false;
                string movie_name = " ";
                bool found = false;
                string name;
                Dictionary<string, List<string>> parent = new Dictionary<string, List<string>>();
                List<string> lis = new List<string>();
                lis.Add(" ");
                lis.Add(" ");
                lis.Add("0");
                parent.Add(source, lis);

                // for re intialize 
                List<string> used = new List<string>();

                // intialize

                d[source] = 0;
                color[source] = "gray";
                used.Add(source);

                Queue<string> Q = new Queue<string>();
                Q.Enqueue(source);

                while (Q.Count != 0)
                {
                    name = Q.Dequeue();
                    foreach (var (key, molist) in final_dict[name])
                    {
                       
                        if (color[key] == "white")
                        {
                            color[key] = "gray";
                            d[key] = d[name] + 1;
                            used.Add(key);


                            //add in  final dict 
                            int rs = Int16.Parse(parent[name][2]);
                            List<string> pa = new List<string>();

                            pa.Add(name);
                            pa.Add(molist[0]);

                            // rs += adj[name].Where(x => x.Equals(v)).Count();
                            rs += final_dict[key][name].Count;

                            pa.Add((Int16.Parse(parent[name][2]) + final_dict[key][name].Count).ToString());

                            parent.Add(key, pa);
                            Q.Enqueue(key);

                        }


                        else if (color[key] == "gray" && d[key] == d[name] + 1)
                        {
                            int old_rs = Int16.Parse(parent[key][2]);
                            int new_rs = Int16.Parse(parent[name][2]);
                            //new_rs += adj[name].Where(x => x.Equals(v)).Count();
                            new_rs += final_dict[key][name].Count;
                            if (new_rs > old_rs)
                            {
                                parent[key].Clear();
                                parent[key].Add(name);
                                parent[key].Add(molist[0]);
                                parent[key].Add(new_rs.ToString());

                            }


                        }

                        if (op == 1)
                        {
                            if (d[dst] < d[name] + 1)
                            {

                                found = true;
                                break;
                            }
                        }



                    }
                    color[name] = "black";
                    if (found == true)
                    {
                        break;
                    }

                }

                dos = d[dst];


                RS = Int16.Parse(parent[dst][2]);
                string parent_actor;
                string movie_link;
                
                string step = dst;

                List<string> chain_movies = new List<string>();
                List<string> chain_actors = new List<string>();
                chain_actors.Add(dst);
                for (int i = 0; i < dos; i++)
                {
                    parent_actor = parent[step][0];
                    movie_link = parent[step][1];

                    chain_movies.Insert(0, movie_link);
                    chain_actors.Insert(0, parent[step][0]);
                    step = parent_actor;
                }




                //reintializitong 
                using (StreamWriter writer = File.AppendText(soul_path))
                {
                    writer.WriteLine(words[0] + "/" + words[1]);
                    writer.WriteLine("DoS = " + dos + ", RS = " + RS);


                    writer.Write("CHAIN OF ACTORS: ");
                    for (int i = 0; i < chain_actors.Count; i++)
                    {

                        if (i == chain_actors.Count - 1)
                        {
                            writer.Write(chain_actors[i]);
                            writer.WriteLine("");
                        }
                        else
                        {
                            writer.Write(chain_actors[i] + " -> ");
                        }
                    }


                    writer.Write("CHAIN OF MOVIES:  => ");

                    for (int i = 0; i < chain_movies.Count; i++)
                    {

                        if (i == chain_movies.Count - 1)
                        {
                            writer.Write(chain_movies[i] + " =>");

                        }
                        else
                        {
                            writer.Write(chain_movies[i] + " => ");
                        }
                    }


                    writer.WriteLine("");
                    writer.WriteLine("");

                }


                foreach (string actor in used)
                {
                    color[actor] = "white";
                    d[actor] = 1000000000;
                }
            }

            //watch.Stop();
            //Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");


        }
        static void Main(string[] args)
        {

            int opt;
            Console.WriteLine("Enter the number 1 for optimize or 0 for not optimized");

            opt = int.Parse(Console.ReadLine());
            int testcase;
             Console.WriteLine("Enter the number you need:\n[1]Sample\n[2]Complete");
             testcase = int.Parse(Console.ReadLine());
            
            switch (testcase)
             {
                #region Sample
                case 1:
                    Movie_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Sample\movies1.txt";
                    query_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Sample\queries1.txt";
                    solu_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\sol\SampleSoul.txt";
                    //Console.WriteLine("Enter the path of the Movies");
                    //Movie_path = Console.ReadLine();
                    //Console.WriteLine("Enter the path of the Queries");
                    //query_path = Console.ReadLine();
                    //Console.WriteLine("Enter the path that you want to save the solution ");
                    //solu_path = Console.ReadLine();
                    Queries = File.ReadAllLines(query_path);
                    TimeFile = Stopwatch.StartNew();
                    filereader(Movie_path);
                    TimeFile.Stop();
                    Console.WriteLine($"Execution Time for reading file: {TimeFile.ElapsedMilliseconds / 1000} in sec");
                    
                    adjecency();
                    algorithmTime = Stopwatch.StartNew();
                    Bfs(Queries, solu_path,opt);
                    algorithmTime.Stop();
                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds / 1000} in sec");


                    break;
                #endregion
                #region Complete
                case 2:
                    Console.WriteLine("Enter the number you need:\n[1]Small\n[2]Medium\n[3]large\n[4]Extreme");
                    int test= int.Parse(Console.ReadLine());
                    switch(test)
                    {
                        #region Small
                        case 1:
                            Console.WriteLine("Enter the number you need:\n[1]Case 1\n[2]Case 2");
                            int test_small = int.Parse(Console.ReadLine());
                            switch(test_small)
                            {
                                case 1:
                                    Movie_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\small\Case1\Movies193.txt";
                                    query_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\small\Case1\queries110.txt";
                                    solu_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\sol\SmallCase1.txt";
                                    //Console.WriteLine("Enter the path of the Movies");
                                    //Movie_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path of the Queries");
                                    //query_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path that you want to save the solution ");
                                    //solu_path = Console.ReadLine();
                                    Queries = File.ReadAllLines(query_path);
                                    TimeFile = Stopwatch.StartNew();
                                    filereader(Movie_path);
                                    TimeFile.Stop();
                                    Console.WriteLine($"Execution Time for reading file: {TimeFile.ElapsedMilliseconds / 1000} in sec");
                                    
                                    adjecency();
                                    algorithmTime = Stopwatch.StartNew();
                                    Bfs(Queries, solu_path,opt);
                                    algorithmTime.Stop();
                                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds / 1000} in sec");
                                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds} in ms");

                                    break;
                                case 2:
                                    Movie_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\small\Case2\Movies187.txt";
                                    query_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\small\Case2\queries50.txt";
                                    solu_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\sol\SmallCase2.txt";
                                    //Console.WriteLine("Enter the path of the Movies");
                                    //Movie_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path of the Queries");
                                    //query_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path that you want to save the solution ");
                                    //solu_path = Console.ReadLine();
                                    Queries = File.ReadAllLines(query_path);
                                    TimeFile = Stopwatch.StartNew();
                                    filereader(Movie_path);
                                    TimeFile.Stop();
                                    Console.WriteLine($"Execution Time for reading file: {TimeFile.ElapsedMilliseconds / 1000} in sec");
                                  
                                    adjecency();
                                    algorithmTime = Stopwatch.StartNew();
                                    Bfs(Queries, solu_path, opt);
                                    algorithmTime.Stop();
                                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds / 1000} in sec");
                                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds} in ms");

                                    break;
                            }


                            break;
                        #endregion
                        #region Medium
                        case 2:
                            Console.WriteLine("Enter the number you need:\n[1]Case 1->Queries85\n[2]Case 1->Queries4000\n[3]case 2->Queries110\n[4]case 2->Queries2000");
                            int test_medium = int.Parse(Console.ReadLine());
                            switch (test_medium)
                            {
                                case 1:
                                    Movie_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\medium\Case1\Movies967.txt";
                                    query_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\medium\Case1\queries85.txt";
                                    solu_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\sol\MediumCase185.txt";
                                    //Console.WriteLine("Enter the path of the Movies");
                                    //Movie_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path of the Queries");
                                    //query_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path that you want to save the solution ");
                                    //solu_path = Console.ReadLine();
                                    Queries = File.ReadAllLines(query_path);
                                    TimeFile = Stopwatch.StartNew();
                                    filereader(Movie_path);
                                    TimeFile.Stop();
                                    Console.WriteLine($"Execution Time for reading file: {TimeFile.ElapsedMilliseconds/1000} in sec");
                                    
                                    adjecency();
                                    algorithmTime = Stopwatch.StartNew();
                                    Bfs(Queries, solu_path, opt);
                                    algorithmTime.Stop();
                                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds/1000}");
                                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds} in ms");

                                    break;
                                case 2:
                                    Movie_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\medium\Case1\Movies967.txt";
                                    query_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\medium\Case1\queries4000.txt";
                                    solu_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\sol\MediumCase14000.txt";
                                    //Console.WriteLine("Enter the path of the Movies");
                                    //Movie_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path of the Queries");
                                    //query_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path that you want to save the solution ");
                                    //solu_path = Console.ReadLine();
                                    Queries = File.ReadAllLines(query_path);
                                    TimeFile = Stopwatch.StartNew();
                                    filereader(Movie_path);
                                    TimeFile.Stop();
                                    Console.WriteLine($"Execution Time for reading file: {TimeFile.ElapsedMilliseconds / 1000} in sec");
                                    
                                    adjecency();
                                    algorithmTime = Stopwatch.StartNew();
                                    Bfs(Queries, solu_path,opt);
                                    algorithmTime.Stop();
                                    Console.WriteLine($"Execution Time for and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds / 1000} in sec");
                                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds} in ms");

                                    break;
                                case 3:
                                    Movie_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\medium\Case2\Movies4736.txt";
                                    query_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\medium\Case2\queries110.txt";
                                    solu_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\sol\MediumCase2110.txt";
                                    //Console.WriteLine("Enter the path of the Movies");
                                    //Movie_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path of the Queries");
                                    //query_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path that you want to save the solution ");
                                    //solu_path = Console.ReadLine();
                                    Queries = File.ReadAllLines(query_path);
                                    TimeFile = Stopwatch.StartNew();
                                    filereader(Movie_path);
                                    TimeFile.Stop();
                                    Console.WriteLine($"Execution Time for reading file: {TimeFile.ElapsedMilliseconds / 1000} in sec");
                                    
                                    adjecency();
                                    algorithmTime = Stopwatch.StartNew();
                                    Bfs(Queries, solu_path,opt);
                                    algorithmTime.Stop();
                                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds / 1000}");
                                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds} in ms");

                                    break;
                                case 4:
                                    Movie_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\medium\Case2\Movies4736.txt";
                                    query_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\medium\Case2\queries2000.txt";
                                    solu_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\sol\MediumCase22000.txt";
                                    //Console.WriteLine("Enter the path of the Movies");
                                    //Movie_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path of the Queries");
                                    //query_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path that you want to save the solution ");
                                    //solu_path = Console.ReadLine();
                                    Queries = File.ReadAllLines(query_path);
                                    TimeFile = Stopwatch.StartNew();
                                    filereader(Movie_path);
                                    TimeFile.Stop();
                                    Console.WriteLine($"Execution Time for reading file: {TimeFile.ElapsedMilliseconds / 1000} in sec");
                                    
                                    adjecency();
                                    algorithmTime = Stopwatch.StartNew();
                                    Bfs(Queries, solu_path, opt);
                                    algorithmTime.Stop();
                                    Console.WriteLine($"Execution Time for and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds / 1000} in sec");
                                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds} in ms");

                                    break;
                            }

                            break;
                        #endregion
                        #region Large
                        case 3:
                            Console.WriteLine("Enter the number you need:\n[1]Query26\n[2]Query600");
                            int test_large = int.Parse(Console.ReadLine());
                            switch (test_large)
                            {
                                case 1:
                                    Movie_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\large\Movies14129.txt";
                                    query_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\large\queries26.txt";
                                    solu_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\sol\large26.txt";
                                    //Console.WriteLine("Enter the path of the Movies");
                                    //Movie_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path of the Queries");
                                    //query_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path that you want to save the solution ");
                                    //solu_path = Console.ReadLine();
                                    Queries = File.ReadAllLines(query_path);
                                    TimeFile = Stopwatch.StartNew();
                                    filereader(Movie_path);
                                    TimeFile.Stop();
                                    Console.WriteLine($"Execution Time for reading file: {TimeFile.ElapsedMilliseconds / 1000} in sec");
                                     
                                    adjecency();
                                    algorithmTime = Stopwatch.StartNew();
                                    Bfs(Queries, solu_path,opt);
                                    algorithmTime.Stop();
                                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds / 1000} in sec");
                                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds} in ms");

                                    break;
                                case 2:
                                    Movie_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\large\Movies14129.txt";
                                    query_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\large\queries600.txt";
                                    solu_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\sol\large600.txt";
                                    //Console.WriteLine("Enter the path of the Movies");
                                    //Movie_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path of the Queries");
                                    //query_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path that you want to save the solution ");
                                    //solu_path = Console.ReadLine();
                                    Queries = File.ReadAllLines(query_path);
                                    TimeFile = Stopwatch.StartNew();
                                    filereader(Movie_path);
                                    TimeFile.Stop();
                                    Console.WriteLine($"Execution Time for reading file: {TimeFile.ElapsedMilliseconds / 1000} in sec");

                                    adjecency();
                                    algorithmTime = Stopwatch.StartNew();
                                    Bfs(Queries, solu_path,opt);
                                    algorithmTime.Stop();
                                    Console.WriteLine($"Execution Time for and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds / 1000} in sec");
                                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds} in ms");

                                    break;
                            }

                            break;
                        #endregion
                        #region Extreme
                        case 4:
                            Console.WriteLine("Enter the number you need:\n[1]Query22\n[2]Query200");
                            int test_ex = int.Parse(Console.ReadLine());
                            switch (test_ex)
                            {
                                case 1:
                                    Movie_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\extreme\Movies122806.txt";
                                    query_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\extreme\queries22.txt";
                                    solu_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\sol\extreme22sol.txt";
                                    //Console.WriteLine("Enter the path of the Movies");
                                    //Movie_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path of the Queries");
                                    //query_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path that you want to save the solution ");
                                    //solu_path = Console.ReadLine();
                                    Queries = File.ReadAllLines(query_path);
                                    TimeFile = Stopwatch.StartNew();
                                    filereader(Movie_path);
                                    TimeFile.Stop();
                                    Console.WriteLine($"Execution Time for reading file: {TimeFile.Elapsed} in sec");
                                
                                    adjecency();
                                    algorithmTime = Stopwatch.StartNew();
                                    Bfs(Queries, solu_path,opt);
                                    algorithmTime.Stop();
                                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds / 1000} in sec");
                                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds} in ms");

                                    break;
                                case 2:
                                    Movie_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\extreme\Movies122806.txt";
                                    query_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\Complete\extreme\queries200.txt";
                                    solu_path = @"D:\edu\third year\second term\algo\project\[4] Small World Phenomenon\Testcases\sol\extreme200.txt";
                                    //Console.WriteLine("Enter the path of the Movies");
                                    //Movie_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path of the Queries");
                                    //query_path = Console.ReadLine();
                                    //Console.WriteLine("Enter the path that you want to save the solution ");
                                    //solu_path = Console.ReadLine();
                                    Queries = File.ReadAllLines(query_path);
                                    TimeFile = Stopwatch.StartNew();
                                    filereader(Movie_path);
                                    TimeFile.Stop();
                                    Console.WriteLine($"Execution Time for reading file: {TimeFile.ElapsedMilliseconds / 1000} in sec");
                                    adjecency();
                                    algorithmTime = Stopwatch.StartNew();
                                    Bfs(Queries, solu_path,opt);
                                    algorithmTime.Stop();
                                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds / 1000} in sec");
                                    Console.WriteLine($"Execution Time for  and BFS[whole algorithm]: {algorithmTime.ElapsedMilliseconds} in ms");

                                    break;
                            }
                            break;
                            #endregion

                    }

                    break;
                 #endregion

            }







        }
    }
}
