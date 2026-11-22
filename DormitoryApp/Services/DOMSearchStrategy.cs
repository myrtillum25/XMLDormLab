using System.Xml;
using DormitoryApp.Models;

namespace DormitoryApp.Services;

// 1. Стратегія пошуку через DOM (завантажує весь XML у пам'ять у вигляді дерева XmlDocument)
public class DomSearchStrategy : IXmlSearchStrategy
{
    public List<Student> Search(string xmlPath, SearchFilter filter)
    {
        var results = new List<Student>();

        if (!File.Exists(xmlPath))
            return results;

        var xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlPath);

        // Пошук всіх вузлів <Student>
        XmlNodeList? studentNodes = xmlDoc.SelectNodes("//Student");
        if (studentNodes == null)
            return results;

        foreach (XmlNode node in studentNodes)
        {
            if (node.Attributes == null)
                continue;

            // Зчитування атрибути студента
            string fullName = node.Attributes["fullName"]?.Value ?? "";
            string faculty = node.Attributes["faculty"]?.Value ?? "";
            string department = node.Attributes["department"]?.Value ?? "";
            int.TryParse(node.Attributes["roomNumber"]?.Value, out int roomNumber);
            int.TryParse(node.Attributes["course"]?.Value, out int course);
            DateTime.TryParse(node.Attributes["checkInDate"]?.Value, out DateTime checkInDate);
            string benefit = node.Attributes["benefit"]?.Value ?? "";
            bool isRoomHead = (node.Attributes["isRoomHead"]?.Value?.ToLower() == "true");
            string phone = node.Attributes["phone"]?.Value ?? "";

            // Перевірка фільтри
            if (MatchesFilter(fullName, faculty, department, roomNumber, course, benefit, filter))
            {
                results.Add(new Student
                {
                    FullName = fullName,
                    Faculty = faculty,
                    Department = department,
                    RoomNumber = roomNumber,
                    Course = course,
                    CheckInDate = checkInDate,
                    Benefit = benefit,
                    IsRoomHead = isRoomHead,
                    Phone = phone
                });
            }
        }

        return results;
    }

    private static bool MatchesFilter(string fullName, string faculty, string department,
        int roomNumber, int course, string benefit, SearchFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.FullName) &&
            !fullName.Contains(filter.FullName, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrWhiteSpace(filter.Faculty) &&
            !faculty.Equals(filter.Faculty, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrWhiteSpace(filter.Department) &&
            !department.Contains(filter.Department, StringComparison.OrdinalIgnoreCase))
            return false;

        if (filter.RoomNumber.HasValue && roomNumber != filter.RoomNumber.Value)
            return false;

        if (filter.Course.HasValue && course != filter.Course.Value)
            return false;

        if (!string.IsNullOrWhiteSpace(filter.Benefit) &&
            !benefit.Contains(filter.Benefit, StringComparison.OrdinalIgnoreCase))
            return false;

        // Пошук за загальним ключовим словом
        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            string kw = filter.Keyword;
            bool inAll = fullName.Contains(kw, StringComparison.OrdinalIgnoreCase)
                      || faculty.Contains(kw, StringComparison.OrdinalIgnoreCase)
                      || department.Contains(kw, StringComparison.OrdinalIgnoreCase)
                      || benefit.Contains(kw, StringComparison.OrdinalIgnoreCase)
                      || roomNumber.ToString().Contains(kw);
            if (!inAll) return false;
        }

        return true;
    }
}