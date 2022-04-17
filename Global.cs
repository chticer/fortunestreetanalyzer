using fortunestreetanalyzer.DatabaseModels.fortunestreet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;

namespace fortunestreetanalyzer
{
    public class Global
    {
        public readonly static string AZURE_STORAGE_URL = "https://projectsecretply10004.blob.core.windows.net/main/";
        public readonly static string GITHUB_URL = "https://github.com/chticer/fortunestreetanalyzer/";

        public class AnalyzerDataModel
        {
            public long AnalyzerInstanceID { get; set; }
            public string AnalyzerInstanceName { get; set; }
            public DateTime AnalyzerInstanceStarted { get; set; }
            public GameDataModel GameData { get; set; }
            public List<CharacterDataModel> CharacterData { get; set; }
            public List<SpaceDataModel> SpaceData { get; set; }
            public List<SpaceTypeDataModel> SpaceTypeData { get; set; }
            public List<ShopDataModel> ShopData { get; set; }
            public List<DistrictDataModel> DistrictData { get; set; }

            public class GameDataModel
            {
                public RuleDataModel RuleData { get; set; }
                public BoardDataModel BoardData { get; set; }
                public ColorDataModel ColorData { get; set; }
                public List<TurnDataModel> TurnData { get; set; }

                public class RuleDataModel
                {
                    public long ID { get; set; }
                    public string Name { get; set; }
                    public byte StandingThreshold { get; set; }
                    public short NetWorthThreshold { get; set; }
                }

                public class BoardDataModel
                {
                    public long ID { get; set; }
                    public string Name { get; set; }
                    public short ReadyCashStart { get; set; }
                    public short SalaryStart { get; set; }
                    public short SalaryIncrease { get; set; }
                    public byte MaxDieRoll { get; set; }
                }

                public class ColorDataModel
                {
                    public long ID { get; set; }
                    public string SystemColor { get; set; }
                    public string CharacterColor { get; set; }
                }

                public class TurnDataModel
                {
                    public List<CharacterTurnDataModel> CharacterTurnData { get; set; }

                    public class CharacterTurnDataModel
                    {
                        public CharacterDataModel CharacterData { get; set; }
                        public byte SpaceLayoutIndex { get; set; }
                        public byte DieRollValue { get; set; }
                        public List<TradesDataModel> TradesData { get; set; }

                        public class TradesDataModel
                        {
                            public bool Success { get; set; }
                            public CharacterDataModel CharacterData { get; set; }
                            public List<ShopDataModel> ShopDataExchangeFrom { get; set; }
                            public List<ShopDataModel> ShopDataExchangeTo { get; set; }
                            public List<int> ReadyCashExchangeOffers { get; set; }
                        }
                    }
                }
            }

            public class CharacterDataModel
            {
                public long ID { get; set; }
                public string PortraitURL { get; set; }
                public string Name { get; set; }
                public byte TurnOrderValue { get; set; }
                public ColorDataModel ColorData { get; set; }
                public long SpaceIndex { get; set; }
                public byte Level { get; set; }
                public byte Placing { get; set; }
                public int ReadyCash { get; set; }
                public int TotalShopValue { get; set; }
                public int TotalStockValue { get; set; }
                public int NetWorth { get; set; }
                public List<long> OwnedShopIndices { get; set; }
                public SuitDataModel OwnedSuits { get; set; }

                public class ColorDataModel
                {
                    public long ID { get; set; }
                    public string GameColor { get; set; }
                }

                public class SuitDataModel
                {
                    public byte TotalSuitCards { get; set; }
                    public List<string> SuitNames { get; set; }
                }
            }

            public class SpaceDataModel
            {
                public long ID { get; set; }
                public AdditionalPropertiesDataModel AdditionalPropertiesData { get; set; }
                public List<SpaceLayoutDataModel> SpaceLayoutData { get; set; }
                public long SpaceTypeIndex { get; set; }
                public long? ShopIndex { get; set; }
                public long? DistrictIndex { get; set; }

                public class AdditionalPropertiesDataModel
                {
                    public ShopDataModel ShopData { get; set; }
                    public SuitDataModel SuitData { get; set; }

                    public class ShopDataModel
                    {
                        public short? OwnerCharacterIndex { get; set; }
                        public short? Price { get; set; }
                    }

                    public class SuitDataModel
                    {
                        public string Name { get; set; }
                        public bool? Rotate { get; set; }
                    }
                }

                public class SpaceLayoutDataModel
                {
                    public decimal CenterXFactor { get; set; }
                    public decimal CenterYFactor { get; set; }
                }
            }

            public class SpaceTypeDataModel
            {
                public long ID { get; set; }
                public string Name { get; set; }
                public string Icon { get; set; }
                public string Title { get; set; }
                public string Description { get; set; }
            }

            public class ShopDataModel
            {
                public long ID { get; set; }
                public string Name { get; set; }
                public int Value { get; set; }
            }

            public class DistrictDataModel
            {
                public long ID { get; set; }
                public string Name { get; set; }
                public string Color { get; set; }
            }
        }

        public class IndexDataModel
        {
            public List<SpaceIndexDataModel> SpaceIndexData { get; set; }
            public List<SpaceTypeIndexDataModel> SpaceTypeIndexData { get; set; }
            public List<ShopIndexDataModel> ShopIndexData { get; set; }
            public List<DistrictIndexDataModel> DistrictIndexData { get; set; }

            public class SpaceIndexDataModel
            {
                public long Index { get; set; }
                public Spaces SpaceData { get; set; }
            }

            public class SpaceTypeIndexDataModel
            {
                public long Index { get; set; }
                public SpaceTypes SpaceTypeData { get; set; }
            }

            public class ShopIndexDataModel
            {
                public long Index { get; set; }
                public Shops ShopData { get; set; }
            }

            public class DistrictIndexDataModel
            {
                public long Index { get; set; }
                public Districts DistrictData { get; set; }
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

        public class RebuildAnalyzerInstance
        {
            public AnalyzerDataModel Data { get; set; }
            public string Response { get; set; }
        }

        public static List<CurrentAnalyzerInstancesTVF> FindUserAnalyzerInstances(long? analyzerInstanceID, List<string> analyzerInstanceTypes, FortuneStreetContext fortuneStreetContext)
        {
            try
            {
                string userIPAddress = GetUserIPAddress();

                List<string> whereClauses = new List<string>();

                if (analyzerInstanceID != null)
                    whereClauses.Add("id = " + analyzerInstanceID);

                if (analyzerInstanceTypes != null)
                    whereClauses.Add("type IN ('" + string.Join("','", analyzerInstanceTypes) + "')");

                List<GetAnalyzerInstancesInProgressTVF> getAnalyzerInstancesInProgressTVFResults = fortuneStreetContext.GetAnalyzerInstancesInProgressTVF.FromSqlRaw("SELECT * FROM getanalyzerinstancesinprogress_tvf()" + (whereClauses.Count > 0 ? " WHERE " + string.Join(" AND ", whereClauses) : "")).ToList();

                List<long> userAnalyzerInstanceIDs = new List<long>();

                foreach (GetAnalyzerInstancesInProgressTVF currentGetAnalyzerInstancesInProgressTVFResult in getAnalyzerInstancesInProgressTVFResults)
                {
                    if (HashComparison(userIPAddress, currentGetAnalyzerInstancesInProgressTVFResult.IPAddress))
                        userAnalyzerInstanceIDs.Add(currentGetAnalyzerInstancesInProgressTVFResult.ID);
                }

                if (userAnalyzerInstanceIDs.Count == 0)
                    return new List<CurrentAnalyzerInstancesTVF>();

                return fortuneStreetContext.CurrentAnalyzerInstancesTVF.FromSqlRaw("SELECT * FROM currentanalyzerinstances_tvf() WHERE id IN (" + string.Join(",", userAnalyzerInstanceIDs) + ") ORDER BY id DESC").ToList();
            }
            catch
            {
                return null;
            }
        }

        public static List<RebuildAnalyzerInstance> RebuildAnalyzerInstanceDataResponse(bool dataFlag, bool responseFlag, long? analyzerInstanceID, List<string> analyzerInstanceTypes, FortuneStreetAppContext fortuneStreetAppContext)
        {
            try
            {
                if (!dataFlag && !responseFlag)
                    throw new Exception();

                List<RebuildAnalyzerInstance> rebuildAnalyzerInstances = new List<RebuildAnalyzerInstance>();

                List<CurrentAnalyzerInstancesTVF> currentAnalyzerInstancesTVFResults = FindUserAnalyzerInstances(analyzerInstanceID, analyzerInstanceTypes, fortuneStreetAppContext);

                if (currentAnalyzerInstancesTVFResults.Count > 0)
                {
                    if (dataFlag)
                    {
                        List<long> analyzerInstanceIDs = currentAnalyzerInstancesTVFResults.Select(analyzerinstanceid => analyzerinstanceid.AnalyzerInstanceID).ToList();

                        List<CurrentAnalyzerInstanceLogsTVF> currentAnalyzerInstanceLogsTVFResults = fortuneStreetAppContext.CurrentAnalyzerInstanceLogsTVF.FromSqlRaw("SELECT * FROM currentanalyzerinstancelogs_tvf() WHERE analyzer_instance_id IN (" + string.Join(",", analyzerInstanceIDs) + ")").ToList();

                        foreach (CurrentAnalyzerInstancesTVF currentAnalyzerInstancesTVFResult in currentAnalyzerInstancesTVFResults)
                        {
                            Dictionary<string, object> currentAnalyzerInstanceData = new Dictionary<string, object>
                            {
                                { "AnalyzerInstanceID", currentAnalyzerInstancesTVFResult.AnalyzerInstanceID },
                                { "AnalyzerInstanceName", currentAnalyzerInstancesTVFResult.Name },
                                { "AnalyzerInstanceStarted", currentAnalyzerInstancesTVFResult.TimestampAdded }
                            };

                            foreach (CurrentAnalyzerInstanceLogsTVF currentAnalyzerInstanceLogsTVFResult in currentAnalyzerInstanceLogsTVFResults.Where(analyzerinstanceid => analyzerinstanceid.AnalyzerInstanceID == currentAnalyzerInstancesTVFResult.AnalyzerInstanceID))
                                currentAnalyzerInstanceData.Add(currentAnalyzerInstanceLogsTVFResult.Key, JsonSerializer.Deserialize<object>(currentAnalyzerInstanceLogsTVFResult.Value));

                            rebuildAnalyzerInstances.Add(new RebuildAnalyzerInstance
                            {
                                Data = JsonSerializer.Deserialize<AnalyzerDataModel>(JsonSerializer.Serialize(currentAnalyzerInstanceData))
                            });
                        }
                    }

                    if (responseFlag)
                        foreach (RebuildAnalyzerInstance currentRebuildAnalyzerInstance in rebuildAnalyzerInstances)
                        {
                            currentRebuildAnalyzerInstance.Response = string.Empty;

                            if (currentRebuildAnalyzerInstance.Data.GameData != null)
                            {
                                currentRebuildAnalyzerInstance.Response += RebuildAnalyzerInstanceHelper.LoadGameData(currentRebuildAnalyzerInstance.Data.GameData, fortuneStreetAppContext).Response;

                                if (currentRebuildAnalyzerInstance.Data.CharacterData != null)
                                {
                                    currentRebuildAnalyzerInstance.Response += RebuildAnalyzerInstanceHelper.LoadCharacters(currentRebuildAnalyzerInstance.Data, fortuneStreetAppContext).Response;
                                }
                            }
                        }
                }

                return rebuildAnalyzerInstances;
            }
            catch
            {
                return null;
            }
        }

        public static long? CreateAnalyzerInstanceID(string type, string name, FortuneStreetAppContext fortuneStreetAppContext)
        {
            try
            {
                AnalyzerInstances analyzerInstanceRecord = new AnalyzerInstances
                {
                    Type = type,
                    Name = name,
                    IPAddress = Hash(GetUserIPAddress()),
                    Status = "in_progress"
                };

                fortuneStreetAppContext.AnalyzerInstances.Add(analyzerInstanceRecord);

                fortuneStreetAppContext.SaveChanges();

                analyzerInstanceRecord.AnalyzerInstanceID = analyzerInstanceRecord.ID;

                fortuneStreetAppContext.SaveChanges();

                return analyzerInstanceRecord.AnalyzerInstanceID;
            }
            catch
            {
                return null;
            }
        }

        private static string Hash(string input)
        {
            byte[] saltBytes = new byte[16];

            RandomNumberGenerator.Create().GetBytes(saltBytes);

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

        public static string CreateConfirmationActions(string alignmentClass, List<string> buttonTags)
        {
            return
                "<div class=\"confirmation-actions" + (!string.IsNullOrWhiteSpace(alignmentClass) ? " " + alignmentClass : "") + "\">" +

                    string.Join("", buttonTags.Select(buttontag => "<div>" + buttontag + "</div>")) +

                "</div>";
        }

        public static string OrdinalNumberSuffix(int number)
        {
            int onesPlace = number % 10;
            int tensPlace = number % 100;

            if (onesPlace == 1 && tensPlace != 11)
                return "st";

            if (onesPlace == 2 && tensPlace != 12)
                return "nd";

            if (onesPlace == 3 && tensPlace != 13)
                return "rd";

            return "th";
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
