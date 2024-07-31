//using System.Data;
//using Microsoft.AnalysisServices.AdomdClient;
//using System.Diagnostics;
//using static System.Runtime.InteropServices.JavaScript.JSType;
//using System.Globalization;

//class GetHierarchy
//{
//    static void Main()
//    {
//        string fileResultPath = "C:\\Users\\idealab6\\source\\repos\\Mdx-app\\Mdx-app\\Resources\\";
//        using (StreamWriter outputFile = new StreamWriter(Path.Combine(fileResultPath, "Results.txt")))
//        {
//            using (AdomdConnection connection = new AdomdConnection("Data Source=IDEALAB6; Initial Catalog=Analysis Services Tutorial;"))
//            {
//                connection.Open();
//                // Create a command to retrieve hierarchies metadata

//                AdomdCommand cmd = connection.CreateCommand();
//                cmd.CommandText = "SELECT DISTINCT ALL_MEMBER FROM $SYSTEM.MDSCHEMA_HIERARCHIES where HIERARCHY_UNIQUE_NAME='[Ship Date].[Calendar Year].[All]'";

//                // Execute the command and retrieve the results
//                AdomdDataReader reader = cmd.ExecuteReader();

//                // Iterate through the results and print hierarchy information
//                while (reader.Read())
//                {
//                    for (int i = 0; i < reader.FieldCount; i++)
//                    {
//                        //Console.Write(String.Format("{0}\t", reader[i]));
//                        Console.Write(reader[i]);
//                        //outputFile.WriteLine(reader[i]);
//                    }
//                    Console.WriteLine();

//                }

//                // Close the reader and connection
//                reader.Close();
//                connection.Close();
//            }
//        }
//    }
//}
