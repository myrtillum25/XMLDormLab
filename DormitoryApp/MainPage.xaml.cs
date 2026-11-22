using System.Xml.Linq;
using DormitoryApp.Models;
using DormitoryApp.Services;

namespace DormitoryApp;

public partial class MainPage : ContentPage
{
    private string currentXmlPath = "";
    private readonly XmlSearchContext searchContext = new();
    private readonly XsltTransformer transformer = new();

    public MainPage()
    {
        InitializeComponent();

        // Спробуємо знайти Dormitory.xml поруч із виконуваним файлом
        string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dormitory.xml");
        if (File.Exists(defaultPath))
        {
            LoadFile(defaultPath);
        }
    }

    // 1. Вибір XML файлу через системний діалог
    private async void OnSelectFileClicked(object sender, EventArgs e)
    {
        try
        {
            var options = new PickOptions
            {
                PickerTitle = "Оберіть XML-файл бази даних гуртожитку",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".xml" } },
                    { DevicePlatform.Android, new[] { "text/xml", "application/xml" } },
                    { DevicePlatform.iOS, new[] { "public.xml" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.xml" } }
                })
            };

            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                LoadFile(result.FullPath);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Не вдалося відкрити файл: {ex.Message}", "OK");
        }
    }

    private void LoadFile(string path)
    {
        currentXmlPath = path;
        FilePathEntry.Text = path;
        PopulateDynamicFilters(path);
    }

    // 2. ДИНАМІЧНА ПІДГРУЗКА ДАНИХ (Вимога завдання):
    // Зчитує з обраного файлу унікальні факультети та кімнати і заповнює Picker-и
    private void PopulateDynamicFilters(string xmlPath)
    {
        try
        {
            if (!File.Exists(xmlPath)) return;

            var doc = XDocument.Load(xmlPath);
            var students = doc.Descendants("Student").ToList();

            // Збираємо унікальні факультети
            var faculties = students
                .Select(s => (string?)s.Attribute("faculty"))
                .Where(f => !string.IsNullOrWhiteSpace(f))
                .Distinct()
                .OrderBy(f => f)
                .ToList();

            FacultyPicker.Items.Clear();
            FacultyPicker.Items.Add("Усі факультети");
            foreach (var fac in faculties)
                FacultyPicker.Items.Add(fac);
            FacultyPicker.SelectedIndex = 0;

            // Збираємо унікальні кімнати
            var rooms = students
                .Select(s => (string?)s.Attribute("roomNumber"))
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Distinct()
                .OrderBy(r => int.TryParse(r, out int n) ? n : 0)
                .ToList();

            RoomPicker.Items.Clear();
            RoomPicker.Items.Add("Усі кімнати");
            foreach (var room in rooms)
                RoomPicker.Items.Add(room);
            RoomPicker.SelectedIndex = 0;

            // Скидаємо курс
            CoursePicker.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            DisplayAlert("Увага", $"Помилка зчитування атрибутів: {ex.Message}", "OK");
        }
    }

    // 3. ПОШУК ЗА ПАТЕРНОМ СТРАТЕГІЯ
    private async void OnSearchClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(currentXmlPath) || !File.Exists(currentXmlPath))
        {
            await DisplayAlert("Увага", "Будь ласка, спочатку оберіть файл Dormitory.xml!", "OK");
            return;
        }

        // 1. Обираємо стратегію залежно від перемикача
        if (DomRadio.IsChecked)
            searchContext.SetStrategy(new DomSearchStrategy());
        else if (SaxRadio.IsChecked)
            searchContext.SetStrategy(new SaxSearchStrategy());
        else
            searchContext.SetStrategy(new LinqToXmlSearchStrategy());

        // 2. Формуємо об'єкт фільтра
        var filter = new SearchFilter();

        if (FacultyPicker.SelectedIndex > 0)
            filter.Faculty = FacultyPicker.SelectedItem?.ToString();

        if (RoomPicker.SelectedIndex > 0 && int.TryParse(RoomPicker.SelectedItem?.ToString(), out int roomNum))
            filter.RoomNumber = roomNum;

        if (CoursePicker.SelectedIndex > 0)
            filter.Course = CoursePicker.SelectedIndex; // 1 курс = індекс 1 тощо

        if (!string.IsNullOrWhiteSpace(KeywordEntry.Text))
            filter.Keyword = KeywordEntry.Text.Trim();

        // 3. Виконуємо пошук через контекст
        List<Student> results = searchContext.ExecuteSearch(currentXmlPath, filter);

        // 4. Відображаємо результати
        ResultsCollectionView.ItemsSource = results;
        ResultsCountLabel.Text = $"Знайдено записів: {results.Count}";

        if (results.Count == 0)
        {
            await DisplayAlert("Результат", "За вказаними критеріями студентів не знайдено.", "OK");
        }
    }

    // 4. ОЧИЩЕННЯ (Кнопка Clear за вимогою завдання)
    private void OnClearClicked(object sender, EventArgs e)
    {
        FacultyPicker.SelectedIndex = 0;
        RoomPicker.SelectedIndex = 0;
        CoursePicker.SelectedIndex = 0;
        KeywordEntry.Text = "";
        ResultsCollectionView.ItemsSource = null;
        ResultsCountLabel.Text = "Знайдено записів: 0";
    }

    // 5. ТРАНСФОРМАЦІЯ В HTML ЧЕРЕЗ XSLT
    private async void OnTransformToHtmlClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(currentXmlPath) || !File.Exists(currentXmlPath))
        {
            await DisplayAlert("Увага", "Оберіть XML-файл для трансформації!", "OK");
            return;
        }

        try
        {
            string folder = Path.GetDirectoryName(currentXmlPath) ?? AppDomain.CurrentDomain.BaseDirectory;
            string xslPath = Path.Combine(folder, "Transform.xsl");

            if (!File.Exists(xslPath))
            {
                // Спробуємо в каталозі додатку
                xslPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Transform.xsl");
            }

            if (!File.Exists(xslPath))
            {
                await DisplayAlert("Помилка", "Файл стилів Transform.xsl не знайдено поряд з XML чи додатком!", "OK");
                return;
            }

            string htmlPath = Path.Combine(folder, "DormitoryReport.html");

            transformer.Transform(currentXmlPath, xslPath, htmlPath);

            bool open = await DisplayAlert("Успіх!",
                $"HTML-звіт успішно створено:\n{htmlPath}\n\nБажаєте відкрити його у браузері?",
                "Так", "Ні");

            if (open)
            {
                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(htmlPath)
                });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка трансформації", ex.Message, "OK");
        }
    }

    // 6. ПІДТВЕРДЖЕННЯ ПРИ ВИХОДІ (Вимога завдання)
    private async void OnExitClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Підтвердження", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
        if (answer)
        {
            Application.Current?.Quit();
        }
    }
}