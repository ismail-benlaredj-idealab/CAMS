using System;
using System.Buffers;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection.PortableExecutable;
using Microsoft.AnalysisServices.AdomdClient;

class Program
{
    static void Main()
    {
        string connectionString = "Data Source=IDEALAB6; Initial Catalog=Analysis Services Tutorial;";
        //string mdxQuery = "SELECT \r\n  NON EMPTY {  [Customer].[First Name].[First Name]} ON COLUMNS, \r\n  NON EMPTY { \r\n    [Measures].[Internet Sales-Sales Amount] * [Product].[Product Name].[Product Name]\r\n  } ON ROWS \r\nFROM \r\n  [Analysis Services Tutorial]";

        string mdxQuery = "SELECT \r\nNON EMPTY { [Date].[Calendar Date].[Calendar Year]} ON COLUMNS,\r\nNON EMPTY {  [Measures].[Internet Sales-Sales Amount] * [Product].[Product Name].[Product Name]} ON ROWS\r\nFROM \r\n[Analysis Services Tutorial]";

        String fileResultPath = "C:\\Users\\idealab6\\source\\repos\\Mdx-app\\Mdx-app\\Resources\\";


        List<String> h1 = new List<String> { };



        using (AdomdConnection connection = new AdomdConnection(connectionString))
        {
            connection.Open();
            using (AdomdCommand command = new AdomdCommand(mdxQuery, connection))
            {
                using (AdomdDataReader reader = command.ExecuteReader())
                {
                    using (StreamWriter writer = new StreamWriter(Path.Combine(fileResultPath, "Cube.csv")))
                    {
                        using (StreamWriter writerH2 = new StreamWriter(Path.Combine(fileResultPath, "hierarchy_2.csv")))
                        {
                            // Write the header row
                            bool skipFirstRow = true;

                            while (reader.Read())
                            {
                                if (skipFirstRow)
                                {
                                    using (StreamWriter writerH1 = new StreamWriter(Path.Combine(fileResultPath, "hierarchy_1.csv")))
                                    {
                                        for (int i = 2; i < reader.FieldCount; i++) // Start from the 3rd column
                                        {
                                            writerH1.Write(reader.GetName(i));
                                            if (i < reader.FieldCount - 1) writerH1.WriteLine(",");
                                        }
                                    }
                                    skipFirstRow = false;
                                    continue;
                                }
                                for (int i = 1; i < reader.FieldCount; i++)
                                {
                                    if (i == 1)
                                    {
                                        //writerH2.WriteLine(reader.GetName(i) + reader.GetString(i));
                                        writerH2.WriteLine(reader.GetString(i));
                                        if (i < reader.FieldCount - 1) writer.Write(",");
                                    }
                                    else
                                    {
                                        //writer.Write(reader[i]);
                                        writer.Write($"{Math.Round(Convert.ToDouble(reader[i]),4)}");
                                        if (i < reader.FieldCount - 1) writer.Write(",");
                                    }

                                }
                                writer.WriteLine();
                            }
                        }
                    }
                }
            }
        }



        List<string> part_1 = GetRowByValue(Path.Combine(fileResultPath, "hierarchy_1.csv"));
        List<string>  Part_2 = GetRowByValue(Path.Combine(fileResultPath, "hierarchy_2.csv"));
        generateReadings(part_1, Part_2, Path.Combine(fileResultPath, "readings.dat"));
        ///////////////
        ///
        List<string[]> csvData = new List<string[]>();


        using (var reader = new StreamReader(Path.Combine(fileResultPath, "Cube.csv")))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                csvData.Add(values);
            }
            reader.Close();
        }


        using (var reader = new StreamReader(Path.Combine(fileResultPath, "readings.dat")))
        {
            int rowIndex = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split("#,#");

                var ind1 = FindRowIndexInCsv(Path.Combine(fileResultPath, "hierarchy_1.csv"), values[0]);
                var ind2 = FindRowIndexInCsv(Path.Combine(fileResultPath, "hierarchy_2.csv"), values[1]);
            
                var updateValue=0;
                if ( values[2]!=null)
                {
                    updateValue= int.Parse(values[2]);
                }



                csvData= UpdateCellValue(csvData, ind1, ind2, updateValue);
               
            }
        }
        updateCSVRes(csvData);
    }

    static List<string> GetRowByValue(string csvFile)
    {
        
       List<string> readList = new List<string>();

        //Console.WriteLine(num);
        using (var reader = new StreamReader(csvFile))
        {
            int listSize = GetRowCount(csvFile);
        for (int i = 0; i<10000; i++)
        {
                {
                    Random random = new Random();
                    int randomRow = random.Next(1, listSize);
                    string line = null;
                    for (int j = 1; j <= randomRow; j++)
                    {
                        line = reader.ReadLine();
                    }

                    if (line != null)
                    {
                        string[] values = line.Split(',');
                        readList.Add(values[0]);
                    }
                    else
                    {
                        Console.WriteLine();
                    }

                    reader.BaseStream.Position = 0;
                    reader.DiscardBufferedData();
                }
            }
        }
 
        return readList;   
    }


    static int GetRowCount(string csvFile)
    {
        int rowCount = 0;

        using (var reader = new StreamReader(csvFile))
        {
            while (!reader.EndOfStream)
            {
                reader.ReadLine();
                rowCount++;
            }
        }
        return rowCount;
    }

    static void generateReadings(List<string> list1, List<string> list2, string filePath)
    {
        Random random = new Random();
      

        if (list1.Count != list2.Count)
        {
            throw new ArgumentException("The lists must have the same length.");
        }


        using (StreamWriter writer = new StreamWriter(filePath))
        {
        
            for (int i = 0; i < list1.Count; i++)
            {
                int randomValue = random.Next(500, 3000);
                string combinedLine = $"{list1[i]}#,#{list2[i]}#,#{randomValue}";

                writer.WriteLine(combinedLine);
            }
        }
    }

    static int FindRowIndexInCsv(string csvFile, string searchValue)
    {
        using (var reader = new StreamReader(csvFile))
        {
            int rowIndex = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                foreach (var value in values)
                {
                    if (value.Equals(searchValue, StringComparison.OrdinalIgnoreCase))
                    {
                        return rowIndex;
                    }
                }
                rowIndex++;
            }
        }
        return -1; // Value not found
    }



    static List<string[]> UpdateCellValue(List<string[]>  csvData, int column , int row, int amount)
    {
        row = Convert.ToInt32(row);
        column = Convert.ToInt32(column);


        if (csvData[row][column] != "")
        {
           double currentValue = Math.Round(double.Parse(csvData[row][column]),3);
            currentValue += amount;
            csvData[row][column] = currentValue.ToString();
        }
        else
        {
            csvData[row][column] = amount.ToString();

        }
        //Console.WriteLine($"Updated the value {row} {column} ==> {amount}.");
        return csvData;
    }

    static void updateCSVRes(List<string[]> csvData)
    {
        // Write the updated data back to the CSV file
        using (var finalWriter = new StreamWriter(Path.Combine("C:\\Users\\idealab6\\source\\repos\\Mdx-app\\Mdx-app\\Resources\\", "updatesCube.csv")))
        {
            foreach (var rowValues in csvData)
            {
                finalWriter.WriteLine(string.Join(",", rowValues));
            }
            finalWriter.Close();
            finalWriter.Dispose();
        }

       
    }


}
