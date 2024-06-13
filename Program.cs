using ClassLibrary1;
using System;
using ClassLibrary12;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Threading.Channels;


namespace Lab14
{
    internal class Program
    {
        /// <summary>
        /// Ввод числа 
        /// </summary>
        static int Number(int minValue, int maxValue, string msg = "") // Ввод числа от minValue до maxValue
        {
            Console.Write(msg + $" (целое число от {minValue} до {maxValue}): ");
            int number;
            bool isConvert;
            do
            {
                //string buf = Console.ReadLine();
                isConvert = int.TryParse(Console.ReadLine(), out number);
                if (!isConvert || number < minValue || number > maxValue)
                    Console.WriteLine("Неправильно введено число. \nПопробуйте еще раз.");
            } while (!isConvert || number < minValue || number > maxValue);

            return number;
        }
        /// <summary>
        /// Средний вес планет
        /// </summary>
        public static double AveragePlanetWeight(IEnumerable<CelestialBody> collection)
        {
            return (from body in collection
                where body is Planet
                select body.Weight).Average();
        }
        /// <summary>
        /// Сумма спутников
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static int SumSatellites(IEnumerable<CelestialBody> collection)
        {
            return collection
                .Where(celbody => celbody is Planet)
                .Select(planet => ((Planet)planet).Satellites) //Получение всех спутников всех планет
                .Sum(); //Подсчет общего количества спутников
        }
        /// <summary>
        /// Количество планет
        /// </summary>
        public static int CountPlanets(IEnumerable<CelestialBody> collection)
        {
            return (from celbody in collection
            where celbody is Planet
            select celbody).Count();
        }
        public static int CountStars(IEnumerable<CelestialBody> collection)
        {
            return collection
                .Where(celbody => celbody is Star)
                .Count();
        }
        public static IEnumerable<IGrouping<int, CelestialBody>> GroupById(IEnumerable<CelestialBody> collection)
        {
            return collection
                .OrderBy(celbody => celbody.id.Number)  //Сортировка по id
                .GroupBy(celbody => celbody.id.Number);
        }
        static void Main(string[] args)
        {
            int answer;
            do
            {
                Console.WriteLine("\n1. Часть 1");
                Console.WriteLine("2. Часть 2");
                Console.WriteLine("3. Выход");
                answer = Number(1, 3, "Выберите номер задания");

                switch (answer)
                {
                    case 1: // Часть 1
                    {
                        Queue<Galaxy> Universe = new Queue<Galaxy>(); // Создание коллекции
                        for (int i = 0; i < 3; i++)
                        {
                            Galaxy galaxy = new Galaxy();
                            galaxy.MakeGalaxy();
                            Universe.Enqueue(galaxy);
                        }

                        Console.WriteLine("\nКоллекция сформирована");

                        do
                        {
                            Console.WriteLine("\n0. Печать небесных тел");
                            Console.WriteLine("1. Max Min температура звезды");
                            Console.WriteLine("2. Пересечение галактик");
                            Console.WriteLine("3. Группировка данных ");
                            Console.WriteLine("4. Получние нового типа - обьем");
                            Console.WriteLine("5. Соединение (Join)");
                            Console.WriteLine("6. Назад");
                            answer = Number(0, 6, "Выберите номер задания");

                            switch (answer)
                            {
                                case 0:
                                {
                                    foreach (var galaxy in Universe)
                                    {
                                        foreach (var celbody in galaxy.ContentsGalaxy.Values)
                                        {
                                            Console.WriteLine(celbody);
                                        }
                                    }

                                    break;
                                }
                                case 1: //Max Min температура звезды
                                {
                                    double MaxTemperature = Galaxy.MaxTemperature(Universe);
                                    Console.WriteLine($"\nМаксимальная температура звезды: {MaxTemperature} (LINQ)");

                                    double MinTemperature = Galaxy.MinTemperature(Universe);
                                    Console.WriteLine(
                                        $"Минимальная температура звезды: {MinTemperature} (Методы расширения)");
                                    break;
                                }
                                case 2: //Обьединение и пересечение галактик
                                {
                                    var galaxy1 = Universe.ElementAt(0); //Первый элемент очереди
                                    var galaxy2 = Universe.ElementAt(1); //Второй элемент очереди
                                    CelestialBody celbody = new CelestialBody("CommonItem", 1, 1, 1); //Общий элемент
                                    galaxy1.Add(celbody); //Добавление в галактики одинаковых элементов 
                                    galaxy2.Add(celbody);

                                    var intersect =
                                        Galaxy.Intersect(galaxy1, galaxy2); //Пересечение двух галактик, LINQ
                                    Console.WriteLine("\nПересечение двух галактик");
                                    foreach (var item in intersect)
                                    {
                                        Console.WriteLine(item);
                                    }

                                    break;
                                }
                                case 3: //Группировка
                                {
                                    var group1 = Galaxy.GroupByType(Universe); //Группировка данных по типу
                                    Console.WriteLine("\nГруппировка данных по типу");
                                    foreach (var group in group1)
                                    {
                                        Console.WriteLine($"Тип {group.Key}"); //Печать ключа группировки
                                        foreach (var item in group)
                                            Console.WriteLine($" - {item}"); //Печать элементо
                                    }

                                    var group2 = Galaxy.GroupByRadius(Universe); //Группировка по радиусу, LINQ
                                    Console.WriteLine("\nГруппировка данных по радиусу:");
                                    foreach (var group in group2)
                                    {
                                        Console.WriteLine(group.Key); //Печать ключа группировки
                                        foreach (var item in group)
                                            Console.WriteLine($" - {item}"); //Печать элементов
                                    }

                                    break;
                                }
                                case 4: //Обьем - новый тип данных
                                {
                                    var Volumes = Galaxy.Volume(Universe);
                                    foreach (var item in Volumes) //Печать обьема
                                    {
                                        Console.WriteLine($"{item.Name}, Объем: {(Int128)item.volume}");
                                    }

                                    break;
                                }
                                case 5:  //Соединение (Join)
                                {

                                    List<GalaxyInfo> galaxyInfo = new List<GalaxyInfo>(); //Лист для соединения 
                                    foreach (var galaxy in Universe)
                                    {
                                        GalaxyInfo inf = new GalaxyInfo();
                                        inf.RandomInit();
                                        inf.Name = galaxy.Name; //Установка имени для соединения
                                        galaxyInfo.Add(inf);
                                    }

                                    var joinInf = from galaxy in Universe
                                        join t in galaxyInfo on galaxy.Name equals t.Name
                                        select new
                                        {
                                            Name = "Галактика " + galaxy.Name + ", " + galaxy.ContentsGalaxy.Count +
                                                   " Небесных тел",
                                            Info = t.Type + ", Адрес: " + t.Address
                                        };

                                    Console.WriteLine("\nОбьединенная информация");
                                    foreach (var item in joinInf)
                                    {
                                        Console.WriteLine(item.Name + ", " + item.Info);
                                    }

                                    break;
                                }
                            }
                        } while (answer != 6);

                        break;
                    }
                    case 2: // Часть 2
                    {
                        MyCollection<CelestialBody> collection = new MyCollection<CelestialBody>(15);
                        for (int i = 0; i < 5; i++) // Небесное тело
                        {
                            CelestialBody C = new CelestialBody();
                            C.RandomInit();
                            collection.Add(C);
                        }

                        for (int i = 5; i < 10; i++) // Звезды
                        {
                            Star S = new Star();
                            S.RandomInit();
                            collection.Add(S);
                        }

                        for (int i = 10; i < 15; i++) // Планеты
                        {
                            Planet P = new Planet();
                            P.RandomInit();
                            collection.Add(P);
                        }

                        for (int i = 15; i < 20; i++) // Газовые гиганты
                        {
                            GasGigant G = new GasGigant();
                            G.RandomInit();
                            collection.Add(G);
                        }
                        Console.WriteLine("\nКоллекция сформирована");

                        do
                        {
                            Console.WriteLine("\n0. Печать небесных тел");
                            Console.WriteLine("1. Сумма спутников и средний вес планет");
                            Console.WriteLine("2. Количество планет и звезд");
                            Console.WriteLine("3. Группировка данных ");
                            Console.WriteLine("4. Назад");
                            answer = Number(0, 4, "Выберите номер задания");

                            switch (answer)
                            {
                                case 0: //Печать коллекции
                                {
                                    foreach (var item in collection)
                                    {
                                        Console.WriteLine(item);
                                    }
                                    break;
                                }
                                case 1:  //Сумма спутников и средний вес планет
                                        {
                                    var PlanetWeight = AveragePlanetWeight(collection);
                                    Console.WriteLine($"\nСредний вес планет {PlanetWeight}");

                                    int sumSatellites = SumSatellites(collection);
                                    Console.WriteLine($"\nСумма спутников планет {sumSatellites}");
                                    break;
                                }
                                case 2:  //Количество планет и звезд
                                        {
                                    var starCount = CountStars(collection);
                                    Console.WriteLine($"\nКоличество звезд {starCount}");

                                    var planetCount = CountPlanets(collection);
                                    Console.WriteLine($"\nКоличество планет {planetCount}");
                                    break;
                                }
                                case 3:  //Группировка данных по id
                                        {
                                    var group = GroupById(collection);
                                    Console.WriteLine("\nГруппировка небесных тел по ID:");
                                    foreach (var gr in group)
                                    {
                                        Console.WriteLine($"ID {gr.Key}");
                                        foreach (var item in gr)
                                        {
                                            Console.WriteLine($" - {item}");
                                        }
                                    }
                                    break;
                                }
                                }
                        } while (answer != 4);
                        break;
                    } 
                }
            } while (answer != 3);
        }
    }
}
