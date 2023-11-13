namespace E8;

class FileLogger : ILogger
{
    private string fileName;

    public FileLogger(string fileName)
    {
        this.fileName = fileName;
    }

    public void Log(string message)
    {
        System.IO.File.AppendAllText(fileName, message + Environment.NewLine);
    }
}