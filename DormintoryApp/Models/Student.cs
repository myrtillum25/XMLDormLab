using System;

namespace DormitoryApp.Models;

public class Student
{
    public string FullName { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int RoomNumber { get; set; }
    public int Course { get; set; }
    public DateTime CheckInDate { get; set; }
    public string Benefit { get; set; } = string.Empty;
    public bool IsRoomHead { get; set; }
    public string Phone { get; set; } = string.Empty;

    // Опис для виводу або перевірки
    public string GetInfo()
    {
        string head = IsRoomHead ? " [Староста]" : "";
        string benefit = !string.IsNullOrWhiteSpace(Benefit) ? $" (Пільга: {Benefit})" : "";
        return $"{FullName}{head} — {Faculty}, {Department}, Кімната {RoomNumber}, {Course} курс{benefit}";
    }
}