# PhpCoveralls
The tool for coveralls.io (Windows/php)

---------------
using:
```
$env:COVERALLS_REPO_TOKEN='your_key'
./phpCoveralls.exe pathToGitRootFolder pathToCloverXml [CIName] [branchName]
```
branchName - it's branch name that will be send to coveralls.io (it wont switch to that branch, just send this name)

---------------
e.g:
```
./phpCoveralls.exe .. bin/clover.xml appveyor
./phpCoveralls.exe .. bin/clover.xml appveyor master
./phpCoveralls.exe .. bin/clover.xml
./phpCoveralls.exe ./ clover.xml
./phpCoveralls.exe src bin/logs/clover.xml appveyor
```


# Requirements
- .Net Framework 4.0






