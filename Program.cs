namespace QuickTXTSplitter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ArgParser.ParsedArgs parsedArgs = ArgParser.Parse(args);

            DirectoryBuilder.BuildDirectories(parsedArgs.Destination);

            var sourceFilepaths = from filepath
                                  in Directory.GetFiles(parsedArgs.Source.FullName) 
                                  where Path.GetExtension(filepath) == ".txt" 
                                  select filepath;

            Parallel.ForEach(sourceFilepaths, filepath =>
            {
                FileSplitter.Split(filepath, parsedArgs);
            });
        }
    }
}
