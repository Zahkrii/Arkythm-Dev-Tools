namespace ChartConvertor.Core.Contracts.Services;

public interface IFileService
{
    T Read<T>(string folderPath, string fileName);

    T Read<T>(string filePath);

    string ReadString(string filePath);

    void Save<T>(string folderPath, string fileName, T content);

    void Delete(string folderPath, string fileName);
}