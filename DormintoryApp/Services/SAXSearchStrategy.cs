using System.Xml;
using DormitoryApp.Models;

namespace DormitoryApp.Services;

// 2. Стратегія пошуку через SAX (потоковий послідовний XmlReader, що не вантажить увесь файл у пам'ять)
public class SaxSearchStrategy : IXmlSearchStrategy
{
    public List<Student> Search(string xmlPath, SearchFilter filter)
    {
        var results = new List<Student>();

        if (!File.Exists(xmlPath))
            return results;

        using var reader = XmlReader.Create(xmlPath);

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Student")
            {
                string fullName = reader.GetAttribute("fullName") ?? "";
                string faculty = reader.GetAttribute("faculty") ?? "";
                string department = reader.GetAttribute("department") ?? "";
                int.TryParse(reader.GetAttribute("roomNumber"), out int roomNumber);
                int.TryParse(reader.GetAttribute("course"), out int course);
                DateTime.TryParse(reader.GetAttribute("checkInDate"), out DateTime checkInDate);
                string benefit = reader.GetAttribute("benefit") ?? "";
                bool isRoomHead = (reader.GetAttribute("isRoomHead")?.ToLower() == "true");
                string phone = reader.GetAttribute("phone") ?? "";

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