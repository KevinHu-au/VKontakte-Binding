
#load "../common.cake"

var VK_NUGET_VERSION = "1.6.9";

var VK_VERSION = "1.6.9";
var VK_URL = "https://search.maven.org/remotecontent?filepath=com/vk/{0}/{1}/{0}-{1}.aar";
var VK_DOCS_URL = "https://search.maven.org/remotecontent?filepath=com/vk/{0}/{1}/{0}-{1}-javadoc.jar";

var REQUIRED_PACKAGES = new []{
	"androidsdk"
};

var TARGET = Argument ("t", Argument ("target", "Default"));

var buildSpec = new BuildSpec () {
	Libs = new [] {	
		new DefaultSolutionBuilder {
			AlwaysUseMSBuild = true,
			SolutionPath = "./source/VK.Android/VK.Android.sln",
			BuildsOn = BuildPlatforms.Mac,
			OutputFiles = new [] { 
				new OutputFileCopy { FromFile = "./source/VK.Android/bin/Release/VK.Android.dll" }
			}
		}
	},

	Samples = new [] {
		new DefaultSolutionBuilder { SolutionPath = "./sample/VK.Android.Sample/VK.Android.Sample.sln", AlwaysUseMSBuild = true },
	},

	NuGets = new [] {
		new NuGetInfo { NuSpec = "./nuget/VK.Android.nuspec", Version = VK_NUGET_VERSION }
	},

	Components = new [] {
		new Component { ManifestDirectory = "./component" },
	},
};

Task ("externals")
	.WithCriteria (!FileExists (string.Format("./externals/{0}.aar", REQUIRED_PACKAGES[0])))
	.Does (() => 
{
	EnsureDirectoryExists ("./externals/");

	foreach(var packageName in REQUIRED_PACKAGES){
		// Download the VK aar
		DownloadFile (string.Format(VK_URL, packageName, VK_VERSION), string.Format("./externals/{0}.aar", packageName));
		// Download, and unzip the docs .jar
		DownloadFile (string.Format(VK_DOCS_URL, packageName, VK_VERSION), string.Format("./externals/{0}-docs.jar", packageName));
		
		EnsureDirectoryExists (string.Format("./externals/{0}-docs", packageName));
		Unzip (string.Format("./externals/{0}-docs.jar", packageName), string.Format("./externals/{0}-docs", packageName));
	}
});


Task ("clean").IsDependentOn ("clean-base").Does (() => 
{
	if (DirectoryExists ("./externals/"))
		DeleteDirectory ("./externals", true);
});

SetupXamarinBuildTasks (buildSpec, Tasks, Task);

RunTarget (TARGET);
