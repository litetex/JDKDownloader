# JDKDownloader <img src="assets/JDKD-128.png" height="50" /> [![Latest Version](https://img.shields.io/github/v/release/litetex/JDKDownloader?style=flat-square)](https://github.com/litetex/JDKDownloader/releases)
⚠ The project is in an experimental stage and not stable ⚠

Downloads jdks/jres

### Why don't just download Java via the API of the provider directly?
This is technially possible and also a very good solution.

However there are some problems:
1. Automatically resolving OS and Architecture is not that easily possible on different operating systems.<br>
This would require a lot of ``if`` statements and is also dependent on the platform (Windows → ``batch or powershell`` | Linux → ``shell``)
2. Configuration may differ a lot between providers
3. You get a compressed file (usually ``zip`` or ``tar.gz``), that has to be extracted (you need some kind of compression tool).
Sometimes (e.g. when using AdoptOpenJDK) you also have the problem that the comporessed file doesn't include the jdk directly, because there is a folder:
![demo picture](https://user-images.githubusercontent.com/40789489/115450243-f600db00-a21b-11eb-8e04-05cca103ca00.png)


### Providers
Note: These are just download providers.

#### AdoptOpenJDK
WIP 🔧

### Nuget
| Module | Nuget |
| --- | --- |
| <img src="src/JDKDownloader.Base/icon.png" height="32" /> Base | [![Nuget](https://img.shields.io/nuget/v/Litetex.JDKDownloader.Base?style=flat-square)](https://www.nuget.org/packages/Litetex.JDKDownloader.Base) |
| <img src="src/JDKDownloader.Provider.AdoptOpenJDK/icon.png" height="32" /> Provider.AdoptOpenJDK | [![Nuget](https://img.shields.io/nuget/v/Litetex.JDKDownloader.Provider.AdoptOpenJDK?style=flat-square)](https://www.nuget.org/packages/Litetex.JDKDownloader.Provider.AdoptOpenJDK) |

### Development [![Latest Version](https://img.shields.io/github/v/release/litetex/JDKDownloader?style=flat-square&include_prereleases&label=prerelease)](https://github.com/litetex/JDKDownloader/releases)
| Workflow | Status |
| --- | --- |
| Sonar Build | [![Sonarcloud Status](https://sonarcloud.io/api/project_badges/measure?project=litetex_JDKDownloader&metric=alert_status)](https://sonarcloud.io/dashboard?id=litetex_JDKDownloader) <br>[![Latest workflow runs](https://img.shields.io/github/workflow/status/litetex/JDKDownloader/Sonar%20CI/develop)](https://github.com/litetex/JDKDownloader/actions?query=workflow%3A%22Sonar+CI%22+branch%3Adevelop)  |
| Check Build | [![Latest workflow runs](https://img.shields.io/github/workflow/status/litetex/JDKDownloader/Check%20Build/develop)](https://github.com/litetex/JDKDownloader/actions?query=workflow%3A%22Check+Build%22+branch%3Adevelop) |
| Build Nuget | [![Latest workflow runs](https://img.shields.io/github/workflow/status/litetex/JDKDownloader/Build%20Nuget/develop)](https://github.com/litetex/JDKDownloader/actions?query=workflow%3A%22Build+Nuget%22+branch%3Adevelop) |
| Release | [![master workflow runs](https://img.shields.io/github/workflow/status/litetex/JDKDownloader/Release/master?label=master)](https://github.com/litetex/JDKDownloader/actions?query=workflow%3A%22Release%22+branch%3Amaster) <br>[![master workflow runs](https://img.shields.io/github/workflow/status/litetex/JDKDownloader/Release/master-release-test?label=release-test)](https://github.com/litetex/JDKDownloader/actions?query=workflow%3A%22Release%22+branch%3Amaster-release-test) |



