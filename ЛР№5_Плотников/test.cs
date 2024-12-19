csharp 
using Newtonsoft.Json; 
using Newtonsoft.Json.Linq; 
using System; 
using System.IO; 
using System.Xml; 
 
public class XmlToJsonConverter 
{ 
    public static string ConvertXmlToJson(string xmlFilePath) 
    { 
        try 
        { 
            XmlDocument xmlDoc = new XmlDocument(); 
            xmlDoc.Load(xmlFilePath); 
 
            // Преобразование XML в JObject 
            JObject jsonObject = XmlToJObject(xmlDoc); 
 
            // Сериализация JObject в JSON строку 
            return JsonConvert.SerializeXmlNode(xmlDoc, Newtonsoft.Json.Formatting.Indented); 
 
        } 
        catch (Exception ex) 
        { 
            Console.WriteLine($"Ошибка при конвертации XML в JSON: {ex.Message}"); 
            return null; // или бросьте исключение, если это подходит для вашего случая 
        } 
    } 
 
    // Вспомогательная функция для обработки атрибутов 
    private static JObject XmlToJObject(XmlNode node) 
    { 
        JObject obj = new JObject(); 
        if (node.Attributes != null) 
        { 
            foreach (XmlAttribute attr in node.Attributes) 
            { 
                obj.Add(attr.Name, attr.Value); 
            } 
        } 
 
        if (node.HasChildNodes) 
        { 
            foreach (XmlNode child in node.ChildNodes) 
            { 
                if (child.NodeType == XmlNodeType.Element) 
                { 
                    // Рекурсивный вызов для дочерних элементов 
                    JToken childToken = XmlToJObject(child); 
                    if (obj.ContainsKey(child.Name)) 
                    { 
                        // Если элемент с таким именем уже существует, создаем массив 
                        JArray array = obj[child.Name] as JArray; 
                        if (array == null) 
                        { 
                            array = new JArray(obj[child.Name]); 
                            obj[child.Name] = array; 
                        } 
                        array.Add(childToken); 
                    } 
                    else 
                    { 
                        obj.Add(child.Name, childToken); 
                    } 
                } 
                else if (child.NodeType == XmlNodeType.Text) 
                { 
                    // Обработка текстовых узлов 
                    obj.Add("#text", child.Value.Trim()); 
                } 
                //можно добавить обработку других типов узлов, если нужно 
            } 
        } 
        return obj; 
    } 
 
    public static void Main(string[] args) 
    { 
        string xmlFilePath = "input.xml"; // Замените на путь к вашему XML файлу 
        string jsonOutput = ConvertXmlToJson(xmlFilePath); 
 
        if (jsonOutput != null) 
        { 
            Console.WriteLine(jsonOutput); 
            File.WriteAllText("output.json", jsonOutput); // Сохранение в файл output.json 
        } 
    } 
} 
