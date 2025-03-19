using System;
using System.IO;
using System.Reflection;

abstract class Table
{
    protected int Number; 
    protected Table(int number)
    {
        Number = number;
    }

    public abstract void Rezer();
}

class RestaurantTable : Table
{
    private bool isRezer;
    private static int AllTables = 0;

    public int Seats { get; private set; }
    public string Description { get; set; }

    public RestaurantTable(int number, int seats) : base(number)
    {
        Seats = seats;
        isRezer = false;
        AllTables++;
    }

    public RestaurantTable() : this(0, 0) 
    {
    }

    public void ReserveTable()
    {
        isRezer = true;
        Console.WriteLine($"Столик {Number} занят");
    }

    private void SecretMethod()
    {
        Console.WriteLine("Типо приватный метод");
    }

    public override void Rezer()
    {
        Console.WriteLine($"Столик {Number} зарезервирован");
    }

    public static int GetTotalTables()
    {
        return AllTables;
    }


    public void DisplayInfo()
    {
        Console.WriteLine($"Столик {Number}, Мест: {Seats}, Описание: {Description}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        RestaurantTable table1 = new RestaurantTable(1, 4);
        RestaurantTable table2 = new RestaurantTable(2, 6);

        table1.Description = "У окна";
        table1.DisplayInfo();
        table1.ReserveTable();

        table2.Description = "В центре зала";
        table2.DisplayInfo();
        table2.Rezer();

        Console.WriteLine($"Всего столиков: {RestaurantTable.GetTotalTables()}");


        Type restaurantTableType = typeof(RestaurantTable);
        // a) 
        Console.WriteLine("Информация о членах класса RestaurantTable:");
        Console.WriteLine("----------------------------------------");

        Console.WriteLine("\nПоля:");
        foreach (FieldInfo field in restaurantTableType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
            Console.WriteLine($"- {field.FieldType} {field.Name} ({(field.IsPublic ? "Public" : "Non-Public")})");
        }

        Console.WriteLine("\nСвойства:");
        foreach (PropertyInfo property in restaurantTableType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
            Console.WriteLine($"- {property.PropertyType} {property.Name} ({(property.CanRead ? "Read" : "")} {(property.CanWrite ? "Write" : "")})");
        }

        Console.WriteLine("\nМетоды:");
        foreach (MethodInfo method in restaurantTableType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
            Console.WriteLine($"- {method.ReturnType} {method.Name} ({(method.IsPublic ? "Public" : "Non-Public")})");
        }

        // b) HTML
        GenerateHtml(typeof(Table), "Table.html");
        GenerateHtml(typeof(RestaurantTable), "RestaurantTable.html");

        // 2
        // a)
        Console.WriteLine("\nСписок конструкторов и их параметров:");
        ConstructorInfo[] constructors = restaurantTableType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (ConstructorInfo constr in constructors)
        {
            Console.WriteLine($"Конструктор: {constr.Name}, Модификатор доступа: {(constr.IsPublic ? "Public" : "Non-Public")}");
            Console.WriteLine("Параметры:");
            foreach (ParameterInfo parameter in constr.GetParameters())
            {
                Console.WriteLine($"- Имя: {parameter.Name}, Тип: {parameter.ParameterType}");
            }
            Console.WriteLine();
        }

        Console.WriteLine("Создание объектов:");
        ConstructorInfo Params = restaurantTableType.GetConstructor(new Type[] { typeof(int), typeof(int) });
        if (Params != null)
        {
            object tableWithParams = Params.Invoke(new object[] { 1, 4 });
            Console.WriteLine("Объект создан с помощью конструктора с параметрами.");
        }

        Console.WriteLine("\nСписок методов класса:");
        MethodInfo[] methods = restaurantTableType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        foreach (MethodInfo method in methods)
        {
            Console.WriteLine($"Метод: {method.Name}, Модификатор доступа: {(method.IsPublic ? "Public" : "Non-Public")}");
        }

        Console.WriteLine("\nВызов приватного метода:");
        MethodInfo secret = restaurantTableType.GetMethod("SecretMethod", BindingFlags.Instance | BindingFlags.NonPublic);
        if (secret != null)
        {
            object tableInstance = Activator.CreateInstance(restaurantTableType, new object[] { 2, 6 });
            if (tableInstance != null)
            {
                secret.Invoke(tableInstance, null);
            }
            else
            {
                Console.WriteLine("Не удалось создать экземпляр класса.");
            }
        }
    }

    static void GenerateHtml(Type type, string fileName)
    {
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            writer.WriteLine("<html>");
            writer.WriteLine("<head><title>Documentation for " + type.Name + "</title></head>");
            writer.WriteLine("<body>");
            writer.WriteLine("<h1>" + type.Name + "</h1>");
            writer.WriteLine("<hr>");

            // Публичные элементы
            writer.WriteLine("<h2>Public Members</h2>");
            writer.WriteLine("<h3>Fields</h3>");
            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                writer.WriteLine("<p>" + field.FieldType + " " + field.Name + "</p>");
            }

            writer.WriteLine("<h3>Properties</h3>");
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                writer.WriteLine("<p>" + property.PropertyType + " " + property.Name + "</p>");
            }

            writer.WriteLine("<h3>Methods</h3>");
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                if (!method.IsSpecialName)
                {
                    writer.WriteLine("<p>" + method.ReturnType + " " + method.Name + "</p>");
                }
            }

            // Приватные и защищенные элементы
            writer.WriteLine("<h2>Non-Public Members</h2>");
            writer.WriteLine("<h3>Fields</h3>");
            foreach (FieldInfo field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                writer.WriteLine("<p>" + field.FieldType + " " + field.Name + "</p>");
            }

            writer.WriteLine("<h3>Properties</h3>");
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                writer.WriteLine("<p>" + property.PropertyType + " " + property.Name + "</p>");
            }

            writer.WriteLine("<h3>Methods</h3>");
            foreach (MethodInfo method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                if (!method.IsSpecialName)
                {
                    writer.WriteLine("<p>" + method.ReturnType + " " + method.Name + "</p>");
                }
            }

            writer.WriteLine("</body>");
            writer.WriteLine("</html>");
        }
    }
}