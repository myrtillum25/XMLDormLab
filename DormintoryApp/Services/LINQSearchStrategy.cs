using System.Xml.Linq;
using DormitoryApp.Models;

namespace DormitoryApp.Services;

// 3. Стратегія пошуку через LINQ to XML (використовує XDocument та декларативні LINQ-запити)
public class LinqToXmlSearchStrategy : IXmlSearchStrategy
{
    public List<Student> Search(string xmlPath, SearchFilter filter)
    {
        if (!File.Exists(xmlPath))
            return new List<Student>();

        var doc = XDocument.Load(xmlPath);

        var query = from s in doc.Descendants("Student")
                    let fullName = (string?)s.Attribute("fullName") ?? ""
                    let faculty = (string?)s.Attribute("faculty") ?? ""
                    let department = (string?)s.Attribute("department") ?? ""
                    let roomNumber = (int?)s.Attribute("roomNumber") ?? 0
                    let course = (int?)s.Attribute("course") ?? 0
                    let checkInDate = (DateTime?)s.Attribute("checkInDate") ?? DateTime.MinValue
                    let benefit = (string?)s.Attribute("benefit") ?? ""
                    let isRoomHead = ((string?)s.Attribute("isRoomHead"))?.ToLower() == "true"
                    let phone = (string?)s.Attribute("phone") ?? ""
                    where (string.IsNullOrWhiteSpace(filter.FullName) || fullName.Contains(filter.FullName, StringComparison.OrdinalIgnoreCase))
                       && (string.IsNullOrWhiteSpace(filter.Faculty) || faculty.Equals(filter.Faculty, StringComparison.OrdinalIgnoreCase))
                       && (string.IsNullOrWhiteSpace(filter.Department) || department.Contains(filter.Department, StringComparison.OrdinalIgnoreCase))
                       && (!filter.RoomNumber.HasValue || roomNumber == filter.RoomNumber.Value)
                       && (!filter.Course.HasValue || course == filter.Course.Value)
                       && (string.IsNullOrWhiteSpace(filter.Benefit) || benefit.Contains(filter.Benefit, StringComparison.OrdinalIgnoreCase))
                       && (string.IsNullOrWhiteSpace(filter.Keyword) ||
                           fullName.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase) ||
                           faculty.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase) ||
                           department.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase) ||
                           benefit.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase) ||
                           roomNumber.ToString().Contains(filter.Keyword))
                    select new Student
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
                    };

        return query.ToList();
    }
}