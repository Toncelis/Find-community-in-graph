using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleGraphCommunities
{
    class Program
    {
        static void Main(string[] args)
        {

            // for each string in file 
            // Dictionary.Add string => Nod

            int coreNodIndex;
            int stepsAmount;

            Dictionary<int, Nod> AllNods = BuildNodCloud(@"../../../GraphRepresentation.txt");

            Console.Write("Граф будет считан из файда GraphRepresentation.txt.\n" +
                "Каждая строка описывает один узел \"индекс_узла : связанный_узел1 связанный_узел2 ...\"\n\n" +
                "Введите индекс стартовой вершины сообщества (целое число): ");
            while (!int.TryParse(Console.ReadLine(), out coreNodIndex) && AllNods.ContainsKey(coreNodIndex))
            {
                Console.Write("Неправильный ввод, либо узел с введенным индексом отсутствует в созданном графе. \n" +
                    "Введите индекс стартовой вершины сообщества (целое число): ");
            }

            Console.Write("Введите ширину сообщества(натуральное число): ");
            while (!int.TryParse(Console.ReadLine(), out stepsAmount))
            {
                Console.Write("Неправильный ввод.\n" +
                    "Введите ширину сообщества (натуральное число): ");
            }

            List<int> result = BuildCommunity(coreNodIndex, stepsAmount, AllNods);

            foreach (var i in result)
                Console.WriteLine(i);

            Console.ReadKey();
        }

        /// <summary>
        /// Поиск узлов из множества validNods, удаленных от starter на количестве ребер не более stepsAmount. 
        /// </summary>
        /// <param name="starter">индекс начального узла</param>
        /// <param name="stepsAmount">максимальное количестве ребер</param>
        /// <param name="validNods">рассматриваемые узлы</param>
        /// <param name="AllNods">общее множество узлов со связями</param>
        /// <returns></returns>
        static List<int> BurnWave (int starter, int stepsAmount, List<int> validNods, Dictionary<int, Nod> AllNods)
        {
            List<int> result = new List<int> ();

            foreach (int i in validNods)
            {
                AllNods[i].Value = 0;
            }

            List<int> burningNods = new List<int>();
            burningNods.Add(starter);
            AllNods[starter].Value = -1;

            List<int> stepResult = new List<int>();

            for (int currentStep=1; currentStep<=stepsAmount; currentStep++)
            {
                foreach (int currentNodIndex in burningNods)
                {
                    foreach (int adjacentNodIndex in AllNods[currentNodIndex].Connections)
                    {
                        if (AllNods[adjacentNodIndex].Value==0)
                        {
                            AllNods[adjacentNodIndex].Value = currentStep;
                            stepResult.Add(adjacentNodIndex);
                        }
                    }
                }
                burningNods = new List<int> (stepResult);
                stepResult.Clear();
            }

            foreach (int i in validNods)
            {
                if (AllNods[i].Value != 0)
                {
                    result.Add(i); 
                    AllNods[i].Value = 0; 
                }
            }

            return result;
        }

        static void ToConsole (string str, List<int> list)
        {
            Console.WriteLine(str);
            foreach (var i in list)
            {
                Console.WriteLine(i);
            }
        }

        /// <summary>
        /// Построение одного из вариантов сообществ в множестве AllNods, опирающегося на starter и шириной в stepsAmount ребер
        /// </summary>
        /// <param name="starter">индекс начального узла</param>
        /// <param name="stepsAmount">ширина в ребрах</param>
        /// <param name="AllNods">общее множество узлов со связями</param>
        /// <returns></returns>
        static List<int> BuildCommunity (int starter, int stepsAmount, Dictionary<int, Nod> AllNods)
        {
            List<int> currentCommunity = new List<int> (AllNods.Keys);
            List<int> checkedNods = new List<int> { starter };
            List<int> newCommunity = new List<int>(BurnWave(starter, stepsAmount, currentCommunity, AllNods));
            currentCommunity = new List<int> (newCommunity);
            //ToConsole("Начальная выборка узлов.", currentCommunity);
            List<int> uncheckedNods =new List<int>( Difference(currentCommunity, checkedNods) );
            while (uncheckedNods.Count > 0)
            {
                checkedNods.Add(uncheckedNods[0]);
                newCommunity = new List<int>(BurnWave(starter, stepsAmount, currentCommunity, AllNods));
                currentCommunity = new List<int>(newCommunity); 
                uncheckedNods = new List<int>(Difference(currentCommunity, checkedNods));
                //ToConsole("Текущее сообщество.", currentCommunity);
            }
            return currentCommunity;
        }

        /// <summary>
        /// Построение множества, дополняющего small до big.
        /// </summary>
        /// <param name="big"></param>
        /// <param name="small"></param>
        /// <returns></returns>
        static List<int> Difference (List<int> big, List<int> small)
        {
            List<int> result = new List<int>();
            foreach (var element in big)
            {
                if (!small.Contains(element))
                {
                    result.Add(element);
                }
            }
            return result;
        }

        /// <summary>
        /// Построение множества узлов из файла 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        static Dictionary<int,Nod> BuildNodCloud (string filePath)
        {
            Dictionary<int, Nod> result = new Dictionary<int, Nod> ();

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs, Encoding.UTF8);
            Nod tempNod = new Nod();
            string stringNod;

            while (sr.Peek() >=0)
            {
                stringNod = sr.ReadLine();
                tempNod = new Nod(stringNod);
                result.Add(tempNod.Value, tempNod);
            }
            sr.Close();
            fs.Close();
            return result;
        }
    }
}
