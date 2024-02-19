using System.Text;

namespace ECGFileService;

internal class BufferedFileStream : IDisposable
{
    private readonly FileStream _fileStream;
    private readonly byte[] _buffer;

    public BufferedFileStream(FileStream fileStream, int bufferSize = 1024)
    {
        _fileStream = fileStream;
        _buffer = new byte[bufferSize];
    }

    public string Read(long offset, int width)
    {
        if (width > _buffer.Length) throw new Exception("width超出Buffer的大小");
        _fileStream.Seek(offset, SeekOrigin.Begin);
        var count = _fileStream.Read(_buffer, 0, width);
        if (count != width) throw new Exception($"读取到的字符长度和宽度不同, offset = {offset}, width = {width}");
        return Encoding.UTF8.GetString(_buffer, 0, count);
    }


    public async Task<string> ReadAsync(long offset, int width,
        CancellationToken cancellationToken = default)
    {
        if (width > _buffer.Length) throw new Exception("width超出Buffer的大小");
        _fileStream.Seek(offset, SeekOrigin.Begin);
        var count = await _fileStream.ReadAsync(_buffer.AsMemory(0, width), cancellationToken);
        if (count != width) throw new Exception($"读取到的字符长度和宽度不同, offset = {offset}, width = {width}");
        return Encoding.UTF8.GetString(_buffer, 0, count);
    }


    public void Dispose()
    {
        _fileStream.Dispose();
    }
}