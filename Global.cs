using fortunestreetanalyzer.DatabaseModels.fortunestreet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography;

namespace fortunestreetanalyzer;

public class Global
{
    public readonly static string AZURE_STORAGE_URL = "https://projectsecretply10004.blob.core.windows.net/main/";
    public readonly static string GITHUB_URL = "https://github.com/chticer/fortunestreetanalyzer/";

    public class AnalyzerDataModel
    {
        public long AnalyzerInstanceID { get; set; }
        public string AnalyzerInstanceName { get; set; }
        public DateTime AnalyzerInstanceStarted { get; set; }
        public GameSettingsDataModel GameSettingsData { get; set; }
        public CharacterDataModel CharacterData { get; set; }
        public List<SpaceDataModel> SpaceData { get; set; }
        public List<SpaceTypeDataModel> SpaceTypeData { get; set; }
        public List<ShopDataModel> ShopData { get; set; }
        public List<DistrictDataModel> DistrictData { get; set; }

        public class GameSettingsDataModel
        {
            public RuleDataModel RuleData { get; set; }
            public BoardDataModel BoardData { get; set; }
            public ColorDataModel ColorData { get; set; }
            public List<List<TurnDataModel>> TurnData { get; set; }

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
                public List<TurnPlayerDataModel> TurnPlayerData { get; set; }
                public List<List<TurnCharacterPropertiesDataModel>> TurnCameoCharactersData { get; set; }

                public class TurnCharacterPropertiesDataModel
                {
                    public long SpaceIndexCurrent { get; set; }
                    public long? SpaceIndexFrom { get; set; }
                    public byte LayoutIndex { get; set; }
                    public byte? DieRollValue { get; set; }
                    public List<byte> DieRollRestrictions { get; set; }
                    public List<string> Logs { get; set; }
                }

                public class TurnPlayerDataModel : TurnCharacterPropertiesDataModel
                {
                    public byte Level { get; set; }
                    public byte Placing { get; set; }
                    public int ReadyCash { get; set; }
                    public int TotalShopValue { get; set; }
                    public int TotalStockValue { get; set; }
                    public int NetWorth { get; set; }
                    public List<long> OwnedShopIndices { get; set; }
                    public byte TotalSuitCards { get; set; }
                    public List<string> CollectedSuits { get; set; }
                    public byte ArcadeIndex { get; set; }
                }
            }
        }

        public class CharacterDataModel
        {
            public List<CharacterPlayerDataModel> PlayerData { get; set; }
            public List<CharacterPropertiesDataModel> CameoCharacterData { get; set; }

            public class CharacterPropertiesDataModel
            {
                public long ID { get; set; }
                public string PortraitURL { get; set; }
                public string Name { get; set; }
            }

            public class CharacterPlayerDataModel : CharacterPropertiesDataModel
            {
                public byte? TurnOrderValue { get; set; }
                public ColorDataModel ColorData { get; set; }

                public class ColorDataModel
                {
                    public long ID { get; set; }
                    public string CharacterColor { get; set; }
                }
            }
        }

        public class SpaceDataModel
        {
            public long ID { get; set; }
            public AdditionalPropertiesDataModel AdditionalPropertiesData { get; set; }
            public List<SpaceLayoutDataModel> SpaceLayoutData { get; set; }
            public List<SpaceConstraintDataModel> SpaceConstraintData { get; set; }
            public long SpaceTypeIndex { get; set; }
            public long? ShopIndex { get; set; }
            public long? DistrictIndex { get; set; }

            public class AdditionalPropertiesDataModel
            {
                public ShopDataModel ShopData { get; set; }
                public SuitDataModel SuitData { get; set; }

                public class ShopDataModel
                {
                    public long? OwnerCharacterIndex { get; set; }
                    public int? Price { get; set; }
                    public decimal PriceFactor { get; set; }
                    public int? PriceFixed { get; set; }
                    public bool Closed { get; set; }
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

            public class SpaceConstraintDataModel
            {
                public long SpaceIndexFrom { get; set; }
                public List<long> SpaceIndicesTo { get; set; }
                public long LayoutIndex { get; set; }
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
        public List<SpaceConstraintIndexDataModel> SpaceConstraintIndexData { get; set; }
        public List<SpaceTypeIndexDataModel> SpaceTypeIndexData { get; set; }
        public List<ShopIndexDataModel> ShopIndexData { get; set; }
        public List<DistrictIndexDataModel> DistrictIndexData { get; set; }

        public class SpaceIndexDataModel
        {
            public long Index { get; set; }
            public Spaces SpaceData { get; set; }
        }

        public class SpaceConstraintIndexDataModel
        {
            public long Index { get; set; }
            public SpaceConstraints SpaceConstraintData { get; set; }
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

    public class LoadAnalyzerInstance
    {
        public string Type { get; set; }
        public AnalyzerDataModel Data { get; set; }
        public string Response { get; set; }

        public LoadAnalyzerInstance()
        {
            Data = new AnalyzerDataModel();
            Response = string.Empty;
        }
    }

    public static List<CurrentAnalyzerInstancesTVF> FindUserAnalyzerInstances(long? analyzerInstanceID, List<string> analyzerInstanceTypes, FortuneStreetContext fortuneStreetContext)
    {
        try
        {
            string userIPAddress = GetUserIPAddress();

            List<CurrentAnalyzerInstancesTVF> analyzerInstancesTVFInProgressResults = fortuneStreetContext.CurrentAnalyzerInstancesTVF.FromSqlRaw("SELECT * FROM currentanalyzerinstances_tvf()").Where(result => Equals(result.Status, "in_progress") && (analyzerInstanceID == null || result.AnalyzerInstanceID == analyzerInstanceID) && (analyzerInstanceTypes == null || analyzerInstanceTypes.Contains(result.Type))).ToList();

            List<long> userAnalyzerInstanceIDs = new List<long>();

            foreach (CurrentAnalyzerInstancesTVF currentAnalyzerInstancesTVFInProgressResult in analyzerInstancesTVFInProgressResults)
            {
                if (HashComparison(userIPAddress, currentAnalyzerInstancesTVFInProgressResult.IPAddress))
                    userAnalyzerInstanceIDs.Add(currentAnalyzerInstancesTVFInProgressResult.ID);
            }

            if (userAnalyzerInstanceIDs.Count == 0)
                return new List<CurrentAnalyzerInstancesTVF>();

            return fortuneStreetContext.CurrentAnalyzerInstancesTVF.FromSqlRaw("SELECT * FROM currentanalyzerinstances_tvf()").Where(analyzerinstanceid => userAnalyzerInstanceIDs.Contains(analyzerinstanceid.AnalyzerInstanceID)).OrderByDescending(id => id.ID).ToList();
        }
        catch
        {
            return null;
        }
    }

    public static List<LoadAnalyzerInstance> LoadAnalyzerInstanceDataResponse(long? analyzerInstanceID, List<string> analyzerInstanceTypes, FortuneStreetAppContext fortuneStreetAppContext)
    {
        try
        {
            List<LoadAnalyzerInstance> loadAnalyzerInstances = new List<LoadAnalyzerInstance>();

            List<CurrentAnalyzerInstancesTVF> currentAnalyzerInstancesTVFResults = FindUserAnalyzerInstances(analyzerInstanceID, analyzerInstanceTypes, fortuneStreetAppContext);

            foreach (CurrentAnalyzerInstancesTVF currentAnalyzerInstancesTVFResult in currentAnalyzerInstancesTVFResults)
            {
                LoadAnalyzerInstance currentLoadAnalyzerInstanceDataResponse = new LoadAnalyzerInstance();

                currentLoadAnalyzerInstanceDataResponse.Type = currentAnalyzerInstancesTVFResult.Type;

                currentLoadAnalyzerInstanceDataResponse.Data.AnalyzerInstanceID = currentAnalyzerInstancesTVFResult.AnalyzerInstanceID;
                currentLoadAnalyzerInstanceDataResponse.Data.AnalyzerInstanceName = currentAnalyzerInstancesTVFResult.Name;
                currentLoadAnalyzerInstanceDataResponse.Data.AnalyzerInstanceStarted = currentAnalyzerInstancesTVFResult.TimestampAdded;

                try
                {
                    LoadAnalyzerInstance loadAnalyzerInstanceGameSettings = LoadAnalyzerInstanceHelper.LoadGameSettings(currentAnalyzerInstancesTVFResult.AnalyzerInstanceID, fortuneStreetAppContext);

                    currentLoadAnalyzerInstanceDataResponse.Data.GameSettingsData = loadAnalyzerInstanceGameSettings.Data.GameSettingsData;

                    currentLoadAnalyzerInstanceDataResponse.Response += loadAnalyzerInstanceGameSettings.Response;

                    if (!string.IsNullOrEmpty(loadAnalyzerInstanceGameSettings.Data.GameSettingsData.RuleData.Name))
                    {
                        LoadAnalyzerInstance loadAnalyzerInstanceTurnOrderDeterminationSettings = LoadAnalyzerInstanceHelper.LoadTurnOrderDeterminationSettings(currentAnalyzerInstancesTVFResult.AnalyzerInstanceID, fortuneStreetAppContext);

                        currentLoadAnalyzerInstanceDataResponse.Data.CharacterData = loadAnalyzerInstanceTurnOrderDeterminationSettings.Data.CharacterData;

                        currentLoadAnalyzerInstanceDataResponse.Response += loadAnalyzerInstanceTurnOrderDeterminationSettings.Response;

                        if (loadAnalyzerInstanceTurnOrderDeterminationSettings.Data.CharacterData.PlayerData.FirstOrDefault().TurnOrderValue != null)
                        {
                            Global.AnalyzerDataModel analyzerData = LoadAnalyzerInstanceHelper.LoadAnalyzerData(currentAnalyzerInstancesTVFResult.AnalyzerInstanceID, fortuneStreetAppContext);

                            currentLoadAnalyzerInstanceDataResponse.Data.GameSettingsData.BoardData.ReadyCashStart = analyzerData.GameSettingsData.BoardData.ReadyCashStart;
                            currentLoadAnalyzerInstanceDataResponse.Data.GameSettingsData.BoardData.SalaryStart = analyzerData.GameSettingsData.BoardData.SalaryStart;
                            currentLoadAnalyzerInstanceDataResponse.Data.GameSettingsData.BoardData.SalaryIncrease = analyzerData.GameSettingsData.BoardData.SalaryIncrease;
                            currentLoadAnalyzerInstanceDataResponse.Data.GameSettingsData.BoardData.MaxDieRoll = analyzerData.GameSettingsData.BoardData.MaxDieRoll;
                            currentLoadAnalyzerInstanceDataResponse.Data.GameSettingsData.TurnData = analyzerData.GameSettingsData.TurnData;

                            currentLoadAnalyzerInstanceDataResponse.Data.SpaceData = analyzerData.SpaceData;

                            currentLoadAnalyzerInstanceDataResponse.Data.SpaceTypeData = analyzerData.SpaceTypeData;

                            currentLoadAnalyzerInstanceDataResponse.Data.ShopData = analyzerData.ShopData;

                            currentLoadAnalyzerInstanceDataResponse.Data.DistrictData = analyzerData.DistrictData;
                        }
                    }

                    loadAnalyzerInstances.Add(currentLoadAnalyzerInstanceDataResponse);
                }
                catch
                {
                }
            }

            return loadAnalyzerInstances;
        }
        catch
        {
            return null;
        }
    }

    public static IndexDataModel GetIndexData(long ruleID, long boardID, FortuneStreetAppContext fortuneStreetAppContext)
    {
        List<Spaces> spaceResults = fortuneStreetAppContext.Spaces.Where(space => space.RuleID == ruleID && space.BoardID == boardID).ToList();

        List<long> spaceIDs = spaceResults.Select(id => id.ID).ToList();

        List<SpaceConstraints> spaceConstraintResults = fortuneStreetAppContext.SpaceConstraints.Where(space_id => spaceIDs.Contains(space_id.SpaceID)).ToList();

        List<long> spaceTypeIDs = spaceResults.Select(space_type_id => space_type_id.SpaceTypeID).Distinct().ToList();

        List<SpaceTypes> spaceTypeResults = fortuneStreetAppContext.SpaceTypes.Where(id => spaceTypeIDs.Contains(id.ID)).ToList();

        List<long> shopIDs = spaceResults.Where(shop_id => shop_id.ShopID != null).Select(shop_id => (long) shop_id.ShopID).Distinct().ToList();

        List<Shops> shopResults = fortuneStreetAppContext.Shops.Where(id => shopIDs.Contains(id.ID)).ToList();

        IndexDataModel indexData = new IndexDataModel
        {
            SpaceIndexData = spaceResults.Select((space_result, space_index) => new IndexDataModel.SpaceIndexDataModel
            {
                Index = space_index,
                SpaceData = space_result
            }).ToList(),
            SpaceConstraintIndexData = spaceConstraintResults.Select((space_constraint_result, space_constraint_index) => new IndexDataModel.SpaceConstraintIndexDataModel
            {
                Index = space_constraint_index,
                SpaceConstraintData = space_constraint_result
            }).ToList(),
            SpaceTypeIndexData = spaceTypeResults.Select((space_type_result, space_type_index) => new IndexDataModel.SpaceTypeIndexDataModel
            {
                Index = space_type_index,
                SpaceTypeData = space_type_result
            }).ToList(),
            ShopIndexData = shopResults.Select((shop_result, shop_index) => new IndexDataModel.ShopIndexDataModel
            {
                Index = shop_index,
                ShopData = shop_result
            }).ToList(),
            DistrictIndexData = new List<IndexDataModel.DistrictIndexDataModel>()
        };

        List<long> districtIDs = spaceResults.Where(district_id => district_id.DistrictID != null).Select(district_id => (long) district_id.DistrictID).Distinct().ToList();

        if (districtIDs.Count > 0)
        {
            List<Districts> districtResults = fortuneStreetAppContext.Districts.Where(id => districtIDs.Contains(id.ID)).ToList();

            indexData.DistrictIndexData = districtResults.Select((district_result, district_index) => new IndexDataModel.DistrictIndexDataModel
            {
                Index = district_index,
                DistrictData = district_result
            }).ToList();
        }

        return indexData;
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

        byte[] hashBytes = new Rfc2898DeriveBytes(input, saltBytes, 1000, HashAlgorithmName.SHA256).GetBytes(32);

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

        byte[] hashBytes = new Rfc2898DeriveBytes(input, saltBytes, 1000, HashAlgorithmName.SHA256).GetBytes(32);

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
