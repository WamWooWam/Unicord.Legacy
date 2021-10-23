Did you know the modern .NET Core SDK allows targeting Windows Phone 7+? I didn't either! But it's great you can just specify `wp75` as a target framework and the SDK does the rest!

... except it doesn't, because it looks for reference assemblies in the wrong place.

Reference assemblies for WP7.5 are kept in `C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Silverlight\v4.0\Profile\WindowsPhone71`, but the .NET SDK looks for them in `C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\WindowsPhone\v7.5`, erroring out if they don't exist (which they won't).

You can get around this by copying the files in the former directory to the latter, creating one if it doesn't already exist, but to avoid this hassle and the requirement of the .NET Core SDK/VS2017+, this directory contains a local NuGet feed of packages built by yours truly. The source code these are based upon still lives in the `..\Libraries` folder.