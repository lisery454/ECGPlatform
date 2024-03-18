using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


using var streamReader = new StreamReader(@"E:\Data\毕业设计\template_data.json");
var allText = streamReader.ReadToEnd();
// allText = allText.Replace("\"[", "[");
// allText = allText.Replace("]\"", "]");
// allText = allText.Replace("\\\"", "\"");
// allText = allText.Replace(@"\\", "\\");
// using var streamWriter = new StreamWriter(@"C:\Users\lxl11\Desktop\new_data.json");
// streamWriter.Write(allText);

var jArray = JArray.Parse(allText);

var a = "\u5ba4";

Console.WriteLine(a);