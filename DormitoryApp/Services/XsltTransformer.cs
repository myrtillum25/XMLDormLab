using System.Xml.Xsl;

namespace DormitoryApp.Services;

public class XsltTransformer
{
    // Перетворює XML-документ у HTML-сторінку за допомогою XSL-файлу
    public void Transform(string xmlPath, string xslPath, string htmlOutputPath)
    {
        if (!File.Exists(xmlPath))
            throw new FileNotFoundException($"Файл {xmlPath} не знайдено!");

        if (!File.Exists(xslPath))
            throw new FileNotFoundException($"Файл {xslPath} не знайдено!");

        var xslt = new XslCompiledTransform();
        xslt.Load(xslPath);

        xslt.Transform(xmlPath, htmlOutputPath);
    }
}