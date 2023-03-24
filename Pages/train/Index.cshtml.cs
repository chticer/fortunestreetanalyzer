using fortunestreetanalyzer.DatabaseModels.fortunestreet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace fortunestreetanalyzer.Pages.train;

public class IndexModel : PageModel
{
    private readonly FortuneStreetAppContext _fortuneStreetAppContext;
    private readonly FortuneStreetSavePreRollContext _fortuneStreetSavePreRollContext;
    private readonly FortuneStreetSavePostRollContext _fortuneStreetSavePostRollContext;

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
        public TurnIterators TurnIteratorsRecord { get; set; }
    }

    public class NewTurnModel
    {
        public TurnIterators TurnIteratorsRecord { get; set; }
    }

    public IndexModel(FortuneStreetAppContext fortuneStreetAppContext, FortuneStreetSavePreRollContext fortuneStreetSavePreRollContext, FortuneStreetSavePostRollContext fortuneStreetSavePostRollContext)
    {
        _fortuneStreetAppContext = fortuneStreetAppContext;
        _fortuneStreetSavePreRollContext = fortuneStreetSavePreRollContext;
        _fortuneStreetSavePostRollContext = fortuneStreetSavePostRollContext;
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
        return SaveAnalyzerInstanceHelper.SaveTurnOrderDeterminationSettings(saveTurnOrderDeterminationSettingsParameter, _fortuneStreetAppContext, _fortuneStreetSavePreRollContext);
    }

    public JsonResult OnPostSavePreRollTurnData([FromBody] SavePreRollTurnDataModel savePreRollTurnDataParameter)
    {
        return SaveAnalyzerInstanceHelper.SavePreRollTurnData(savePreRollTurnDataParameter.PreRollsRecords, _fortuneStreetSavePreRollContext);
    }

    public JsonResult OnPostSavePostRollTurnData([FromBody] SavePostRollTurnDataModel savePostRollTurnDataParameter)
    {
        return SaveAnalyzerInstanceHelper.SavePostRollTurnData(savePostRollTurnDataParameter.PostRollsRecord, _fortuneStreetSavePostRollContext);
    }

    public JsonResult OnPostResetTurn([FromBody] ResetTurnModel resetTurnParameter)
    {
        Global.Response response = new Global.Response();

        try
        {
            List<CurrentTurnIteratorsTVF> currentTurnIteratorsTVFResults = _fortuneStreetAppContext.CurrentTurnIteratorsTVF.FromSqlRaw("SELECT * FROM currentturniterators_tvf()").Where(result => result.AnalyzerInstanceID == resetTurnParameter.TurnIteratorsRecord.AnalyzerInstanceID).ToList();

            int maximumTurnResetCounter = currentTurnIteratorsTVFResults.Select(turn_reset_counter => turn_reset_counter.TurnResetCounter).Max();

            currentTurnIteratorsTVFResults = currentTurnIteratorsTVFResults.Where(turn_number => turn_number.TurnNumber == resetTurnParameter.TurnIteratorsRecord.TurnNumber).ToList();

            List<long> resetTurnInitialTurnIteratorIDs = currentTurnIteratorsTVFResults.Select(id => id.ID).ToList();

            List<PreRolls> preRollsRecords = _fortuneStreetAppContext.PreRolls.Where(turn_iterator_id => resetTurnInitialTurnIteratorIDs.Contains(turn_iterator_id.TurnIteratorID)).GroupBy(turn_iterator_id => turn_iterator_id.TurnIteratorID).Select(result => result.FirstOrDefault()).ToList();

            List<TurnIterators> turnIteratorsRecords = currentTurnIteratorsTVFResults.Select(result => new TurnIterators
            {
                AnalyzerInstanceID = result.AnalyzerInstanceID,
                CharacterID = result.CharacterID,
                TurnResetCounter = maximumTurnResetCounter + 1,
                TurnNumber = result.TurnNumber,
                TurnOrder = result.TurnOrder
            }).ToList();

            _fortuneStreetAppContext.TurnIterators.AddRange(turnIteratorsRecords);

            _fortuneStreetAppContext.SaveChanges();

            foreach (PreRolls currentPreRollsRecord in preRollsRecords)
            {
                CurrentTurnIteratorsTVF currentTurnIteratorsTVFResult = currentTurnIteratorsTVFResults.SingleOrDefault(result => result.ID == currentPreRollsRecord.TurnIteratorID);

                TurnIterators currentTurnIteratorsRecord = turnIteratorsRecords.SingleOrDefault(record => record.CharacterID == currentTurnIteratorsTVFResult.CharacterID);

                _fortuneStreetAppContext.PreRolls.Add(new PreRolls
                {
                    TurnIteratorID = currentTurnIteratorsRecord.ID,
                    SpaceIDCurrent = currentPreRollsRecord.SpaceIDCurrent,
                    SpaceIDFrom = currentPreRollsRecord.SpaceIDFrom,
                    LayoutIndex = currentPreRollsRecord.LayoutIndex,
                    Level = currentPreRollsRecord.Level,
                    Placing = currentPreRollsRecord.Placing,
                    ReadyCash = currentPreRollsRecord.ReadyCash,
                    TotalShopValue = currentPreRollsRecord.TotalShopValue,
                    TotalStockValue = currentPreRollsRecord.TotalStockValue,
                    NetWorth = currentPreRollsRecord.NetWorth,
                    OwnedShopIndices = currentPreRollsRecord.OwnedShopIndices,
                    TotalSuitCards = currentPreRollsRecord.TotalSuitCards,
                    CollectedSuits = currentPreRollsRecord.CollectedSuits,
                    ArcadeIndex = currentPreRollsRecord.ArcadeIndex
                });
            }

            _fortuneStreetAppContext.SaveChanges();

            response.HTMLResponse = JsonSerializer.Serialize(new
            {
                Data = LoadAnalyzerInstanceHelper.LoadAnalyzerData(resetTurnParameter.TurnIteratorsRecord.AnalyzerInstanceID, _fortuneStreetAppContext).GameSettingsData.TurnData
            });

            return new JsonResult(response);
        }
        catch (Exception e)
        {
            return Global.ServerErrorResponse(e);
        }
    }

    public JsonResult OnPostNewTurn([FromBody] NewTurnModel newTurnParameter)
    {
        Global.Response response = new Global.Response();

        try
        {
            List<GetPreRollsTVF> getPreRollsTVFResults = _fortuneStreetAppContext.GetPreRollsTVF.FromSqlRaw("SELECT * FROM getprerolls_tvf({0})", newTurnParameter.TurnIteratorsRecord.AnalyzerInstanceID).Where(turn_number => turn_number.TurnNumber == newTurnParameter.TurnIteratorsRecord.TurnNumber - 1).ToList();

            List<CurrentTurnIteratorsTVF> currentTurnIteratorsTVFResults = _fortuneStreetAppContext.CurrentTurnIteratorsTVF.FromSqlRaw("SELECT * FROM currentturniterators_tvf()").Where(result => result.AnalyzerInstanceID == newTurnParameter.TurnIteratorsRecord.AnalyzerInstanceID && result.TurnNumber == newTurnParameter.TurnIteratorsRecord.TurnNumber - 1).ToList();

            List<TurnIterators> turnIteratorsRecords = currentTurnIteratorsTVFResults.Select(result => new TurnIterators
            {
                AnalyzerInstanceID = result.AnalyzerInstanceID,
                CharacterID = result.CharacterID,
                TurnResetCounter = result.TurnResetCounter,
                TurnNumber = newTurnParameter.TurnIteratorsRecord.TurnNumber,
                TurnOrder = result.TurnOrder
            }).ToList();

            _fortuneStreetAppContext.TurnIterators.AddRange(turnIteratorsRecords);

            _fortuneStreetAppContext.SaveChanges();

            foreach (GetPreRollsTVF currentGetPreRollsTVFResult in getPreRollsTVFResults)
            {
                TurnIterators currentTurnIteratorsRecord = turnIteratorsRecords.SingleOrDefault(character_id => character_id.CharacterID == currentGetPreRollsTVFResult.CharacterID);

                _fortuneStreetAppContext.PreRolls.Add(new PreRolls
                {
                    TurnIteratorID = currentTurnIteratorsRecord.ID,
                    SpaceIDCurrent = currentGetPreRollsTVFResult.SpaceIDCurrent,
                    SpaceIDFrom = currentGetPreRollsTVFResult.SpaceIDFrom,
                    LayoutIndex = currentGetPreRollsTVFResult.LayoutIndex,
                    Level = currentGetPreRollsTVFResult.Level,
                    Placing = currentGetPreRollsTVFResult.Placing,
                    ReadyCash = currentGetPreRollsTVFResult.ReadyCash,
                    TotalShopValue = currentGetPreRollsTVFResult.TotalShopValue,
                    TotalStockValue = currentGetPreRollsTVFResult.TotalStockValue,
                    NetWorth = currentGetPreRollsTVFResult.NetWorth,
                    OwnedShopIndices = currentGetPreRollsTVFResult.OwnedShopIndices,
                    TotalSuitCards = currentGetPreRollsTVFResult.TotalSuitCards,
                    CollectedSuits = currentGetPreRollsTVFResult.CollectedSuits,
                    ArcadeIndex = currentGetPreRollsTVFResult.ArcadeIndex
                });
            }

            _fortuneStreetAppContext.SaveChanges();

            response.HTMLResponse = JsonSerializer.Serialize(new
            {
                Data = LoadAnalyzerInstanceHelper.LoadAnalyzerData(newTurnParameter.TurnIteratorsRecord.AnalyzerInstanceID, _fortuneStreetAppContext).GameSettingsData.TurnData.LastOrDefault()
            });

            return new JsonResult(response);
        }
        catch (Exception e)
        {
            return Global.ServerErrorResponse(e);
        }
    }
}
