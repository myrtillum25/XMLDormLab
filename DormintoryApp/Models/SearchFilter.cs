namespace DormitoryApp.Models;

public class SearchFilter
{
    public string? FullName { get; set; }
    public string? Faculty { get; set; }
    public string? Department { get; set; }
    public int? RoomNumber { get; set; }
    public int? Course { get; set; }
    public string? Benefit { get; set; }
    public string? Keyword { get; set; } // Для швидкого пошуку за ключовим словом

    // Перевіряє, чи хоч щось введено у фільтрі
    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(FullName)
            && string.IsNullOrWhiteSpace(Faculty)
            && string.IsNullOrWhiteSpace(Department)
            && !RoomNumber.HasValue
            && !Course.HasValue
            && string.IsNullOrWhiteSpace(Benefit)
            && string.IsNullOrWhiteSpace(Keyword);
    }
}