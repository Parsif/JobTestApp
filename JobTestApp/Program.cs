using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JobTestApp
{
    class Program
    {

        internal class Employee
        {
            [JsonProperty(PropertyName = "id")]
             public int Id { get; set; }

             [JsonProperty(PropertyName = "name")]
             public string Name { get; set; }

             [JsonProperty(PropertyName = "supervisorId")]
             public int SupervisorId { get; set; } = -1;

             [JsonProperty(PropertyName = "count")]
             public int Count { get; set; }

             [JsonProperty(PropertyName = "type")]
             public string Type { get; set; }

             public bool ShouldSerializeSupervisorId()
             {
                 return SupervisorId != -1;
             }

        }

        internal class Node
        {
            public Employee Employee { get; set; }
            public Node Parent { get; set; }
            public List<Node> Children { get; set; } = new List<Node>();

            public Node(Employee employee)
            {
                Employee = employee;
            }

            public int Count()
            {
                foreach (var child in Children)
                {
                 
                    var numOfChildren = child.Count();
                    if (child.Employee.Type == "department")
                    {
                        Employee.Count += numOfChildren;
                    }
                    else
                    {
                        Employee.Count += numOfChildren + 1;
                    }
                }

                return Employee.Count;
            }
        }


        static void Main(string[] args)
        {
            var employeesList = ReadFromJson("data.json");
            if (employeesList is null)
            {
                return;
            }
            var nodes = new Dictionary<int, Node>();

            foreach (var employee in employeesList)
            {
                nodes.Add(employee.Id, new Node(employee));
            }

            Node root = null;
            foreach (var node in nodes.Values)
            {
                int supId = node.Employee.SupervisorId;
                if (supId == -1)
                {
                    root = node;
                    continue;
                }
                nodes[supId].Children.Add(node);
            }

            if (root is null)
            {
                Console.WriteLine("No root was found");
                return;
            }
            root.Count();
            WriteToJson(employeesList);
            Console.Read();
        }

        static List<Employee> ReadFromJson(string filename)
        {
            string jsonString;
            try
            {
                using (var streamReader = new StreamReader(filename))
                {
                    jsonString = streamReader.ReadToEnd();
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Input file not found");
                return null;
            }

            
            return JsonConvert.DeserializeObject<List<Employee>>(jsonString);
            
       
        }


        static void WriteToJson(List<Employee> employees)
        {
            string jsonString = JsonConvert.SerializeObject(employees, Formatting.Indented);
            Console.WriteLine(jsonString);

            using (var streamWriter = new StreamWriter($"../../result.json"))
            {
                streamWriter.WriteLine(jsonString);
            }
        }

    }


}

