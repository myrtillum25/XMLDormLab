using DormitoryApp.Models;

namespace DormitoryApp.Services;

// Спільний інтерфейс для всіх алгоритмів парсингу XML (патерн Strategy)
public interface IXmlSearchStrategy
{
    List<Student> Search(string xmlPath, SearchFilter filter);
}