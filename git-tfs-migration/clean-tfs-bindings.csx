// example execute: scriptcs clean-tfs-bindings.csx -- C:\Directory

// determine the folder in question we are cleaning.
string folderToClean = null;
if (Env.ScriptArgs.Any())
{
    folderToClean = Env.ScriptArgs[0];
}

folderToClean = folderToClean ?? Directory.GetCurrentDirectory();
Console.WriteLine("Cleaning folder {0}", folderToClean);

// 1. remove all extra binding files that are only needed for TFS
var files = Directory.GetFiles(folderToClean, "*.vssscc", SearchOption.AllDirectories).ToList();
files.AddRange(Directory.GetFiles(folderToClean, "*.vspscc", SearchOption.AllDirectories).ToList());
files.AddRange(Directory.GetFiles(folderToClean, "*.user", SearchOption.AllDirectories).ToList());
Console.WriteLine("Found {0} to delete", files.Count());
files.ForEach(t => {
    File.Delete(t);
    Console.WriteLine("Deleted file {0}", t);
});

// 2. find all solution files to remove binding content from them.
// typical format of bindings we are removing from solution files:
// GlobalSection(TeamFoundationVersionControl) = preSolution
//		SccNumberOfProjects = 1
//		SccEnterpriseProvider = {4CA58AB2-18FA-4F8D-95D4-32DDF27D184C}
//		SccTeamFoundationServer = http://tfs.toolboxsolutions.com:8080/tfs/defaultcollection
//		SccProjectUniqueName0 = ToolBox.OneScreen.Retailer.Staging.DataLoad.dtproj
//		SccProjectName0 = .
//		SccAuxPath0 = http://tfs.toolboxsolutions.com:8080/tfs/defaultcollection
//		SccLocalPath0 = .
//		SccProvider0 = {4CA58AB2-18FA-4F8D-95D4-32DDF27D184C}
//	EndGlobalSection
var solutionFiles = Directory.GetFiles(folderToClean, "*.sln", SearchOption.AllDirectories).ToList();
foreach (var solutionFile in solutionFiles)
{
    var contents = File.ReadAllText(solutionFile);
    var bindingStartIndex = contents.IndexOf("GlobalSection(TeamFoundationVersionControl) = preSolution");
    if (bindingStartIndex > 0)
    {
        Console.WriteLine("Found file {0} with TFS bindings in it, removing them.", solutionFile);
        var bindingEndIndex = contents.IndexOf("EndGlobalSection", bindingStartIndex);
        var lengthOfEnding = "EndGlobalSection".Length;
        var newContent = contents.Substring(0, bindingStartIndex) + contents.Substring(bindingEndIndex + lengthOfEnding);
        
        File.WriteAllText(solutionFile, newContent);
    }
}

