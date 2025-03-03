using System.IO.Compression;

using var uncompressedFile = File.Open("original.txt", FileMode.Open);
using var compressedFile = File.Create("compressed.gz");
using var gZipStream = new GZipStream(compressedFile, CompressionLevel.Optimal);
uncompressedFile.CopyTo(gZipStream);