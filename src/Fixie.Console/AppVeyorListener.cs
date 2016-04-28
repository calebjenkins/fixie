﻿using Fixie.Execution;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Script.Serialization;

namespace Fixie.ConsoleRunner
{
    public class AppVeyorListener :
        Handler<AssemblyStarted>,
        Handler<CaseSkipped>,
        Handler<CasePassed>,
        Handler<CaseFailed>
    {
        readonly string url;
        readonly HttpClient client;
        string fileName;

        public AppVeyorListener()
            : this(Environment.GetEnvironmentVariable("APPVEYOR_API_URL"), new HttpClient())
        {
        }

        public AppVeyorListener(string url, HttpClient client)
        {
            this.url = new Uri(new Uri(url), "api/tests").ToString();
            this.client = client;
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void Handle(AssemblyStarted message)
        {
            fileName = Path.GetFileName(message.Location);
        }

        public void Handle(CaseSkipped message)
        {
            var caseCompleted = (CaseCompleted)message;

            Post(new TestResult
            {
                testFramework = "Fixie",
                fileName = fileName,
                testName = caseCompleted.Name,
                outcome = "Skipped",
                durationMilliseconds = caseCompleted.Duration.TotalMilliseconds.ToString("0"),
                StdOut = caseCompleted.Output,
                ErrorMessage = caseCompleted.SkipReason,
                ErrorStackTrace = null
            });
        }

        public void Handle(CasePassed message)
        {
            var caseCompleted = (CaseCompleted)message;

            Post(new TestResult
            {
                testFramework = "Fixie",
                fileName = fileName,
                testName = caseCompleted.Name,
                outcome = "Passed",
                durationMilliseconds = caseCompleted.Duration.TotalMilliseconds.ToString("0"),
                StdOut = caseCompleted.Output,
                ErrorMessage = null,
                ErrorStackTrace = null
            });
        }

        public void Handle(CaseFailed message)
        {
            var caseCompleted = (CaseCompleted)message;

            Post(new TestResult
            {
                testFramework = "Fixie",
                fileName = fileName,
                testName = caseCompleted.Name,
                outcome = "Failed",
                durationMilliseconds = caseCompleted.Duration.TotalMilliseconds.ToString("0"),
                StdOut = caseCompleted.Output,
                ErrorMessage = caseCompleted.Exceptions.PrimaryException.DisplayName,
                ErrorStackTrace = caseCompleted.Exceptions.CompoundStackTrace
            });
        }

        void Post(TestResult result)
        {
            var content = new JavaScriptSerializer().Serialize(result);
            client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"))
                  .ContinueWith(x => x.Result.EnsureSuccessStatusCode())
                  .Wait();
        }

        public class TestResult
        {
            public string testFramework { get; set; }
            public string fileName { get; set; }
            public string testName { get; set; }
            public string outcome { get; set; }
            public string durationMilliseconds { get; set; }
            public string StdOut { get; set; }
            public string ErrorMessage { get; set; }
            public string ErrorStackTrace { get; set; }
        }
    }
}