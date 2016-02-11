// example execute: scriptcs clean-tfs-bindings.csx -- C:\Directory

string folderToClean = null;
if (Env.ScriptArgs.Any())
{
    folderToClean = Env.ScriptArgs[0];
}

folderToClean = folderToClean ?? Directory.GetCurrentDirectory();
Console.WriteLine("Cleaning folder {0}", folderToClean);

var files = Directory.GetFiles(folderToClean, "*.vssscc", SearchOption.AllDirectories).ToList();
Console.WriteLine("Found {0} to delete", files.Count());
files.ForEach(t => {
    File.Delete(t);
    Console.WriteLine("Deleted file {0}", t);
});
