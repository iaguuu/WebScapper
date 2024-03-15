using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;

public class Functions
{
    private Functions()
    { }
    private static readonly Random random = new Random();

    public static string encodeBase64(string value)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }
    public static string decodeBase64(string value, string encoding = null)
    {
        if (encoding == null) { return Encoding.UTF8.GetString(Convert.FromBase64String(value)); }

        byte[] bytes = Convert.FromBase64String(value);
        return Encoding.GetEncoding(encoding).GetString(bytes);
    }
    public string randomString(int OpcionalLength = 0)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZÇabcdefghijklmnopqrstuvwxyç0123456789";
        char[] stringChars = new char[(OpcionalLength == 0) ? random.Next(0, 1000) : OpcionalLength];
        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }
        return new string(stringChars);
    }
    public string TextToFindBetween(string source, Regex regexPattern, int index = 0)
    {
        Match match = regexPattern.Match(source);
        if (!match.Success) { return ""; }
        return match.Groups[index].Value;
    }
    public string ReplaceLastOccurrence(string source, string find, string replace)
    {
        int place = source.LastIndexOf(find);
        if (place == -1) return source;
        return source.Remove(place, find.Length).Insert(place, replace);
    }

    public static bool IsValidJson(string strInput)
    {
        if (string.IsNullOrWhiteSpace(strInput)) { return false; }
        strInput = strInput.Trim();
        if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || (strInput.StartsWith("[") && strInput.EndsWith("]")))
        {
            try
            {
                var obj = JToken.Parse(strInput);
                return true;
            }
            catch
            {
                return false;
            }
        }
        return false;
    }

    public static DataTable JsonToTable(string Json)
    {
        DataTable iDataTable = JsonConvert.DeserializeObject<DataTable>(Json);
        return iDataTable;
    }
    public static DataTable ConvertCsvStringToDataTable(bool isFilePath, string CSVContent, char CharDelimiter)
    {
        string[] Lines;
        if (isFilePath) { Lines = File.ReadAllLines(CSVContent); } else { Lines = CSVContent.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); }
        string[] Fields = Lines[0].Split(new char[] { CharDelimiter });
        int Cols = Fields.GetLength(0);
        DataTable dt = new DataTable();
        //1st row must be column names; force lower case to ensure matching later on.
        for (int i = 0; i < Cols; i++)
        {
            dt.Columns.Add(Fields[i].ToLower(), typeof(string));
        }
        DataRow Row;
        for (int i = 1; i < Lines.GetLength(0); i++)
        {
            Fields = Lines[i].Split(new char[] { CharDelimiter });
            Row = dt.NewRow();
            for (int f = 0; f < Cols; f++)
                Row[f] = Fields[f];
            dt.Rows.Add(Row);
        }
        return dt;
    }

    public void WriteTextToTxt(string fullFilePath, string fileName, string text)
    {
        try
        {
            DirectoryInfo ObjSearchDir = new DirectoryInfo(fullFilePath);
            if (!ObjSearchDir.Exists) { ObjSearchDir.Create(); }

            using (StreamWriter sw = new StreamWriter(Path.Combine(fullFilePath, fileName), true))
            {
                sw.WriteLine(text);
            }
        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }
    }
}