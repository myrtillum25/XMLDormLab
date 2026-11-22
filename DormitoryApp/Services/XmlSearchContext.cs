using DormitoryApp.Models;

namespace DormitoryApp.Services;

// Контекст для керування поточною стратегією пошуку
public class XmlSearchContext
{
    private IXmlSearchStrategy strategy;

    // За замовчуванням LINQ to XML
    public XmlSearchContext(IXmlSearchStrategy? defaultStrategy = null)
    {
        strategy = defaultStrategy ?? new LinqToXmlSearchStrategy();
    }

    // Дозволяє змінити алгоритм на льоту
    public void SetStrategy(IXmlSearchStrategy strategy)
    {
        this.strategy = strategy;
    }

    public List<Student> ExecuteSearch(string xmlPath, SearchFilter filter)
    {
        return strategy.Search(xmlPath, filter);
    }
}