using fortunestreetanalyzer.DatabaseModels.fortunestreet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace fortunestreetanalyzer.Pages.train;

public class IndexModel : PageModel
{
    private readonly FortuneStreetAppContext _fortuneStreetAppContext;

    public class StartupDataModel
    {
        public long? id { get; set; }
    }

    public class SavePreRollTurnDataModel
    {
        public List<PreRolls> PreRollsRecords { get; set; }
    }

    public class SavePostRollTurnDataModel
    {
        public PostRolls PostRollsRecord { get; set; }
    }

    public class ResetTurnModel
    {
        public PreRolls PreRollsRecord { get; set; }
    }

    public IndexModel(FortuneStreetAppContext fortuneStreetAppContext)
    {
        _fortuneStreetAppContext = fortuneStreetAppContext;
    }

    public JsonResult OnPostStartup([FromBody] StartupDataModel startupDataParameter)
    {
        Global.Response response = new Global.Response();

        try
        {
            List<CurrentAnalyzerInstancesTVF> userAnalyzerInstances = Global.FindUserAnalyzerInstances(startupDataParameter.id != null ? (long) startupDataParameter.id : 0, new List<string> { "train" }, _fortuneStreetAppContext);

            if (userAnalyzerInstances == null)
            {
                response.AlertData = new Global.Response.Alert
                {
                    Type = "alert-danger",
                    Title = "Unable to verify the analyzer instance ID."
                };
                response.Error = true;

                return new JsonResult(response);
            }

            CurrentAnalyzerInstancesTVF currentUserAnalyzerInstance = userAnalyzerInstances.FirstOrDefault();

            response.HTMLResponse = JsonSerializer.Serialize(Global.LoadAnalyzerInstanceDataResponse(currentUserAnalyzerInstance != null ? currentUserAnalyzerInstance.AnalyzerInstanceID : (long) Global.CreateAnalyzerInstanceID("train", null, _fortuneStreetAppContext), new List<string> { "train" }, _fortuneStreetAppContext).FirstOrDefault());

            return new JsonResult(response);
        }
        catch (Exception e)
        {
            return Global.ServerErrorResponse(e);
        }
    }

    public JsonResult OnPostSaveGameSettings([FromBody] Global.AnalyzerDataModel saveGameSettingsParameter)
    {
        return SaveAnalyzerInstanceHelper.SaveGameSettings(saveGameSettingsParameter, _fortuneStreetAppContext);
    }

    public JsonResult OnPostSaveTurnOrderDeterminationSettings([FromBody] Global.AnalyzerDataModel saveTurnOrderDeterminationSettingsParameter)
    {
        return SaveAnalyzerInstanceHelper.SaveTurnOrderDeterminationSettings(saveTurnOrderDeterminationSettingsParameter, _fortuneStreetAppContext);
    }

    public JsonResult OnPostSavePreRollTurnData([FromBody] SavePreRollTurnDataModel savePreRollTurnDataParameter)
    {
        return SaveAnalyzerInstanceHelper.SavePreRollTurnData(savePreRollTurnDataParameter.PreRollsRecords, _fortuneStreetAppContext);
    }

    public JsonResult OnPostSavePostRollTurnData([FromBody] SavePostRollTurnDataModel savePostRollTurnDataParameter)
    {
        return SaveAnalyzerInstanceHelper.SavePostRollTurnData(savePostRollTurnDataParameter.PostRollsRecord, _fortuneStreetAppContext);
    }

    public JsonResult OnPostResetTurn([FromBody] ResetTurnModel resetTurnParameter)
    {
        Global.Response response = new Global.Response();

        try
        {
            long analyzerInstanceID = resetTurnParameter.PreRollsRecord.AnalyzerInstanceID;

            List<GetPreRollsTVF> getPreRollsTVFResults = _fortuneStreetAppContext.GetPreRollsTVF.FromSqlRaw("SELECT * FROM getprerolls_tvf({0})", analyzerInstanceID).Where(turn_number => turn_number.TurnNumber == resetTurnParameter.PreRollsRecord.TurnNumber).ToList();

            List<GetPostRollsTVF> getPostRollsTVFResults = _fortuneStreetAppContext.GetPostRollsTVF.FromSqlRaw("SELECT * FROM getpostrolls_tvf({0})", analyzerInstanceID).Where(turn_number => turn_number.TurnNumber == resetTurnParameter.PreRollsRecord.TurnNumber - 1).ToList();

            _fortuneStreetAppContext.PreRolls.AddRange(getPreRollsTVFResults.Select(result => new PreRolls
            {
                AnalyzerInstanceID = analyzerInstanceID,
                CharacterID = result.CharacterID,
                SpaceIDCurrent = result.SpaceIDCurrent,
                SpaceIDFrom = result.SpaceIDFrom,
                TurnResetFlag = true,
                TurnNumber = result.TurnNumber,
                LayoutIndex = result.LayoutIndex,
                Level = result.Level,
                Placing = result.Placing,
                ReadyCash = result.ReadyCash,
                TotalShopValue = result.TotalShopValue,
                TotalStockValue = result.TotalStockValue,
                NetWorth = result.NetWorth,
                OwnedShopIndices = result.OwnedShopIndices,
                TotalSuitCards = result.TotalSuitCards,
                CollectedSuits = result.CollectedSuits,
                ArcadeIndex = result.ArcadeIndex,
                DieRollRestrictions = result.DieRollRestrictions
            }));

            _fortuneStreetAppContext.PostRolls.AddRange(getPostRollsTVFResults.Select(result => new PostRolls
            {
                AnalyzerInstanceID = analyzerInstanceID,
                CharacterID = result.CharacterID,
                SpaceIDLandedOn = result.SpaceIDLandedOn,
                TurnResetFlag = true,
                TurnNumber = result.TurnNumber,
                DieRollValue = result.DieRollValue,
                Logs = result.Logs
            }));

            _fortuneStreetAppContext.SaveChanges();

            response.HTMLResponse = JsonSerializer.Serialize(new
            {
                Data = LoadAnalyzerInstanceHelper.LoadAnalyzerData(analyzerInstanceID, _fortuneStreetAppContext).GameSettingsData.TurnData
            });

            return new JsonResult(response);
        }
        catch (Exception e)
        {
            return Global.ServerErrorResponse(e);
        }
    }
}
