
#load "../common.cake"

var TARGET = Argument ("t", Argument ("target", "Default"));

var iosPlatform = "6.0";
var vkiOSSdkVersion = "1.4.6";

var IOS_PODS = new List<string> {
	"source 'https://github.com/CocoaPods/Specs.git'",
	$"platform :ios, '{iosPlatform}'",
	"install! 'cocoapods', :integrate_targets => false",
	"target 'VK.iOS' do",
	$"\tpod 'VK-ios-sdk', '{vkiOSSdkVersion}'",
	"end",
};

string [] IOS_TARGETS = { "VK-ios-sdk" };

var buildSpec = new BuildSpec () {
	Libs = new ISolutionBuilder [] {
		new DefaultSolutionBuilder {
			SolutionPath = "./source/VK.iOS/VK.iOS.sln",
			Configuration = "Release",
			BuildsOn = BuildPlatforms.Mac,
			OutputFiles = new [] { 
				new OutputFileCopy {
					FromFile = "./source/VK.iOS/bin/Release/VK.iOS.dll",
					ToDirectory = "./output/unified/"
				}
			}
		}	
	},

	Samples = new ISolutionBuilder [] {
		new IOSSolutionBuilder {
			SolutionPath = "./samples/VK.iOS.Sample/VK.iOS.Sample.sln",
			Configuration = "Release",
			Platform = "iPhone",
			BuildsOn = BuildPlatforms.Mac }
	},

	NuGets = new [] {
		new NuGetInfo { NuSpec = "./nuget/VK.iOS.nuspec", BuildsOn = BuildPlatforms.Mac},
	},

	Components = new [] {
		new Component { ManifestDirectory = "./component", BuildsOn = BuildPlatforms.Mac},
	},
};

Task ("externals")
	.IsDependentOn ("externals-base")
	.WithCriteria (!FileExists ("./externals/VK-ios-sdk.a"))
	.Does (() => 
{
	EnsureDirectoryExists ("./externals");

	if (CocoaPodVersion () < new System.Version (1, 0))
		IOS_PODS.RemoveAt (2);

	FileWriteLines ("./externals/Podfile", IOS_PODS.ToArray ());

	CocoaPodInstall ("./externals", new CocoaPodInstallSettings { });
	
	// CopyDirectory ("./externals/Pods/vk-ios-sdk/VKSdkResources.bundle", "./externals/VKSdkResources.bundle");

	foreach (var target in IOS_TARGETS)
		BuildXCodeFatLibrary ("./Pods/Pods.xcodeproj", target, Archs.Simulator | Archs.Simulator64 | Archs.ArmV7 | Archs.Arm64, target, $"{target}.a", null, target);	
});

Task ("clean")
	.IsDependentOn ("clean-base")
	.Does (() => 
{
	if (DirectoryExists("./externals"))
		DeleteDirectory ("./externals", true);
});

SetupXamarinBuildTasks (buildSpec, Tasks, Task);

RunTarget (TARGET);
