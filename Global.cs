﻿using fortunestreetanalyzer.DatabaseModels.fortunestreet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;

namespace fortunestreetanalyzer
{
    public class Global
    {
        public readonly static string GITHUB_URL = "https://github.com/chticer/fortunestreetanalyzer/";

        public class AnalyzerDataModel
        {
            public long AnalyzerInstanceID { get; set; }
            public GameSelectionDataModel GameSelection { get; set; }

            public class GameSelectionDataModel
            {
                public long RuleID { get; set; }
                public long BoardID { get; set; }
                public long ColorID { get; set; }
            }
        }

        public class Response
        {
            public Alert AlertData { get; set; }
            public bool Error { get; set; }
            public string HTMLResponse { get; set; }

            public class Alert
            {
                public string Type { get; set; }
                public string Title { get; set; }
                public List<string> Descriptions { get; set; }
            }

            public Response()
            {
                AlertData = null;
                Error = false;
                HTMLResponse = string.Empty;
            }
        }

        public static string CreateConfirmationActions(string alignmentClass, List<string> buttonTags)
        {
            return
                "<div class=\"confirmation-actions" + (!string.IsNullOrEmpty(alignmentClass) ? " " + alignmentClass : "") + "\">" +

                    string.Join("", buttonTags.Select(buttontag => "<div>" + buttontag + "</div>")) +

                "</div>";
        }

        public static long? VerifyAnalyzerInstanceID(long analyzerInstanceID, FortuneStreetAppContext fortuneStreetAppContext)
        {
            try
            {
                CurrentAnalyzerInstancesTVF currentAnalyzerInstancesTVFResult = fortuneStreetAppContext.CurrentAnalyzerInstancesTVF.FromSqlInterpolated($"SELECT * FROM currentanalyzerinstances_tvf() WHERE analyzer_instance_id = {analyzerInstanceID} AND status = 'in_progress'").FirstOrDefault();

                string userIPAddress = GetUserIPAddress();

                if (currentAnalyzerInstancesTVFResult == null || !HashComparison(userIPAddress, currentAnalyzerInstancesTVFResult.IPAddress))
                {
                    AnalyzerInstances analyzerInstanceRecord = new AnalyzerInstances
                    {
                        IPAddress = Hash(userIPAddress),
                        Status = "in_progress"
                    };

                    fortuneStreetAppContext.AnalyzerInstances.Add(analyzerInstanceRecord);

                    fortuneStreetAppContext.SaveChanges();

                    analyzerInstanceRecord.AnalyzerInstanceID = analyzerInstanceRecord.ID;

                    fortuneStreetAppContext.SaveChanges();

                    return analyzerInstanceRecord.AnalyzerInstanceID;
                }

                return currentAnalyzerInstancesTVFResult.AnalyzerInstanceID;
            }
            catch
            {
                return null;
            }
        }

        private static string Hash(string input)
        {
            byte[] saltBytes;

            new RNGCryptoServiceProvider().GetBytes(saltBytes = new byte[16]);

            byte[] hashBytes = new Rfc2898DeriveBytes(input, saltBytes, 1000).GetBytes(32);

            byte[] hashValue = new byte[saltBytes.Length + hashBytes.Length];

            Array.Copy(saltBytes, 0, hashValue, 0, saltBytes.Length);
            Array.Copy(hashBytes, 0, hashValue, saltBytes.Length, hashBytes.Length);

            return Convert.ToBase64String(hashValue);
        }

        public static bool HashComparison(string input, string storedInput)
        {
            byte[] storedHashBytes = Convert.FromBase64String(storedInput);

            byte[] saltBytes = new byte[16];

            Array.Copy(storedHashBytes, 0, saltBytes, 0, saltBytes.Length);

            byte[] hashBytes = new Rfc2898DeriveBytes(input, saltBytes, 1000).GetBytes(32);

            for (int i = 0; i < hashBytes.Length; ++i)
            {
                if (storedHashBytes[i + saltBytes.Length] != hashBytes[i])
                    return false;
            }

            return true;
        }

        public static string GetUserIPAddress()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault().ToString();
        }

        public static JsonResult ServerErrorResponse(Exception e)
        {
            return new JsonResult(new Response
            {
                AlertData = new Response.Alert
                {
                    Type = "alert-danger",
                    Title = "Server Issues!",
                    Descriptions = new List<string>
                    {
                        "An error message was received from a server request.",
                        "Error Message: " + e.Message,
                        e.InnerException != null ? "<div>Additional Error Information:</div><div>" + e.InnerException.Message + "</div>" : null,
                        "Please try again. If this error continues to occur, submit this <a href=\"" + GITHUB_URL + "issues/new/choose\" alt=\"GitHub Issue\" target=\"_blank\">issue</a> on GitHub with the information provided above."
                    }
                },
                Error = true
            });
        }
    }
}