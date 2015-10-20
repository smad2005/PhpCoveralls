using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using phpCoveralls.Models;

namespace phpCoveralls
{
    internal class Program
    {
        private const string COVERALLS_REPO_TOKEN = "COVERALLS_REPO_TOKEN";
        private static string gitRootFolder;

        private static void Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 4)
            {
                DrawHeader();
                return;
            }

            var coverallsModelcs = new CoverallsModelcs();
            coverallsModelcs.RepoToken = Environment.GetEnvironmentVariable(COVERALLS_REPO_TOKEN);

            if (string.IsNullOrEmpty(coverallsModelcs.RepoToken))
            {
                Log.ErrorWriteLine($"Environment variable '{COVERALLS_REPO_TOKEN}' is empty");
            }

            gitRootFolder = Path.GetFullPath(args[0]);
            if (!Directory.Exists(gitRootFolder))
            {
                Log.ErrorWriteLine("Path to git root folder doesn't exist.");
            }

            var xmlPath = Path.GetFullPath(args[1]);
            if (!File.Exists(xmlPath))
            {
                Log.ErrorWriteLine($"File {xmlPath} not found.");
            }

            if (args.Length >= 3 && !string.IsNullOrEmpty(args[2]))
            {
                coverallsModelcs.ServiceName = args[2];
            }
            var gitInfo = new GitInfo(gitRootFolder);
            coverallsModelcs.Git.Head = gitInfo.GetLastCommitInfo();

            coverallsModelcs.Git.Branch = args.Length >= 4 && !string.IsNullOrEmpty(args[3])
                ? args[3]
                : gitInfo.GetCurrentBranchName();

            coverallsModelcs.ServiceBranch = coverallsModelcs.Git.Branch;

            var xdoc = XDocument.Load(xmlPath);
            ConvertCloverXMLtoJSON(xdoc, coverallsModelcs);
            var bytes = coverallsModelcs.ToJson();
            HttpUploadFile("https://coveralls.io/api/v1/jobs", bytes);
        }

        private static void DrawHeader()
        {
            Console.WriteLine("{0} tool for coveralls.io", Assembly.GetExecutingAssembly().GetName().Name);
            var exeName = Assembly.GetExecutingAssembly().ManifestModule.Name;
            Action lineFunct = () => Console.WriteLine(new string('-', 15));
            lineFunct();
            Console.WriteLine("using:");
            Console.WriteLine($"$env:{COVERALLS_REPO_TOKEN}='your_key'");
            Console.WriteLine("{0} pathToGitRootFolder pathToCloverXml [CIName] [branchName]", exeName);
            lineFunct();
            Console.WriteLine($"e.g:");
            Console.WriteLine("{0} .. bin/clover.xml appveyor master", exeName);
            Console.WriteLine("{0} .. bin/clover.xml appveyor", exeName);
            Console.WriteLine("{0} .. bin/clover.xml", exeName);
            Console.WriteLine("{0} ./ clover.xml", exeName);
            Console.WriteLine("{0} src bin/logs/clover.xml", exeName);
        }

        private static void ConvertCloverXMLtoJSON(XDocument xdoc, CoverallsModelcs coverallsModelcs)
        {
            foreach (XElement projects in (xdoc.Nodes().First() as XElement).Nodes())
            {
                foreach (XElement file in projects.Nodes())
                {
                    if (file.Name == "file")
                    {
                        var sourceFileModel = new SourceFileModel();
                        sourceFileModel.Name = file.Attribute("name").Value;
                        var coverage = new PhpList();

                        foreach (XElement line in file.Nodes())
                        {
                            if (line.Name == "line")
                                coverage[Convert.ToInt32(line.Attribute("num").Value)] =
                                    Convert.ToInt32(line.Attribute("count").Value);
                            else
                            {
                                coverage.Normalize(Convert.ToInt32(line.Attribute("loc").Value));
                            }
                        }
                        sourceFileModel.SourceDigest = GetMd5(sourceFileModel.Name);
                        sourceFileModel.Name = ToRelativePath(sourceFileModel.Name);
                        sourceFileModel.Coverage = coverage;
                        coverallsModelcs.SourceFiles.Add(sourceFileModel);
                    }
                }
            }
        }

        private static string ToRelativePath(string path)
        {
            path = Path.GetFullPath(path);
            return path.Replace(gitRootFolder, string.Empty).Replace("\\", "/").TrimStart('/');
        }

        private static string GetMd5(string path)
        {
            MD5 hashProvider = new MD5CryptoServiceProvider();
            using (var fs = new FileStream(path, FileMode.Open))
            {
                return string.Concat(hashProvider.ComputeHash(fs).Select(y => y.ToString("x2")));
            }
        }

        public static void HttpUploadFile(string url, byte[] bytes, string fieldName = "json_file",
            string fileName = "result.json")
        {
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            var boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            var wr = (HttpWebRequest) WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            using (var rs = wr.GetRequestStream())
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                var headerTemplate =
                    "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";

                var header = string.Format(headerTemplate, fieldName, fileName, "application/json");
                var headerbytes = Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);
                rs.Write(bytes, 0, bytes.Length);
                var dt = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(dt, 0, dt.Length);
            }

            try
            {
                wr.GetResponse().Close();
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    Log.ErrorWriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
                }
            }
        }
    }
}