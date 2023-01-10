using fortunestreetanalyzer.DatabaseModels.fortunestreet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace fortunestreetanalyzer;

public class SaveAnalyzerInstanceHelper
{
    public static JsonResult SaveGameSettings(Global.AnalyzerDataModel saveGameSettingsParameter, FortuneStreetAppContext fortuneStreetAppContext)
    {
        Global.Response response = new Global.Response();

        try
        {
            Rules rulesResult = fortuneStreetAppContext.Rules.SingleOrDefault(id => id.ID == saveGameSettingsParameter.GameSettingsData.RuleData.ID);

            if (rulesResult == null)
            {
                response.AlertData = new Global.Response.Alert
                {
                    Type = "alert-danger",
                    Title = "Invalid rule selection."
                };
                response.Error = true;

                return new JsonResult(response);
            }

            Boards boardsResult = fortuneStreetAppContext.Boards.SingleOrDefault(id => id.ID == saveGameSettingsParameter.GameSettingsData.BoardData.ID);

            if (boardsResult == null)
            {
                response.AlertData = new Global.Response.Alert
                {
                    Type = "alert-danger",
                    Title = "Invalid board selection."
                };
                response.Error = true;

                return new JsonResult(response);
            }

            Colors colorsResult = fortuneStreetAppContext.Colors.SingleOrDefault(id => id.ID == saveGameSettingsParameter.GameSettingsData.ColorData.ID);

            if (colorsResult == null)
            {
                response.AlertData = new Global.Response.Alert
                {
                    Type = "alert-danger",
                    Title = "Invalid mii color selection."
                };
                response.Error = true;

                return new JsonResult(response);
            }

            fortuneStreetAppContext.GameSettings.Add(new GameSettings
            {
                AnalyzerInstanceID = saveGameSettingsParameter.AnalyzerInstanceID,
                RuleID = rulesResult.ID,
                BoardID = boardsResult.ID,
                MiiColorID = colorsResult.ID
            });

            fortuneStreetAppContext.SaveChanges();

            BoardCharacteristics boardCharacteristicResult = fortuneStreetAppContext.BoardCharacteristics.SingleOrDefault(board_characteristic => board_characteristic.RuleID == rulesResult.ID && board_characteristic.BoardID == boardsResult.ID);

            Global.LoadAnalyzerInstance loadAnalyzerInstanceTurnOrderDeterminationSettings = LoadAnalyzerInstanceHelper.LoadTurnOrderDeterminationSettings(saveGameSettingsParameter.AnalyzerInstanceID, fortuneStreetAppContext);

            response.HTMLResponse = JsonSerializer.Serialize(new
            {
                Data = new Global.AnalyzerDataModel
                {
                    GameSettingsData = new Global.AnalyzerDataModel.GameSettingsDataModel
                    {
                        RuleData = new Global.AnalyzerDataModel.GameSettingsDataModel.RuleDataModel
                        {
                            ID = rulesResult.ID,
                            Name = rulesResult.Name,
                            StandingThreshold = boardCharacteristicResult.StandingThreshold,
                            NetWorthThreshold = boardCharacteristicResult.NetWorthThreshold
                        },
                        BoardData = new Global.AnalyzerDataModel.GameSettingsDataModel.BoardDataModel
                        {
                            ID = boardsResult.ID,
                            Name = boardsResult.Name,
                            ReadyCashStart = boardCharacteristicResult.ReadyCashStart,
                            SalaryStart = boardCharacteristicResult.SalaryStart,
                            SalaryIncrease = boardCharacteristicResult.SalaryIncrease,
                            MaxDieRoll = boardCharacteristicResult.MaxDieRoll
                        },
                        ColorData = new Global.AnalyzerDataModel.GameSettingsDataModel.ColorDataModel
                        {
                            ID = colorsResult.ID,
                            SystemColor = colorsResult.SystemColor,
                            CharacterColor = colorsResult.CharacterColor
                        }
                    },
                    CharacterData = loadAnalyzerInstanceTurnOrderDeterminationSettings.Data.CharacterData
                },
                Response = loadAnalyzerInstanceTurnOrderDeterminationSettings.Response
            });

            return new JsonResult(response);
        }
        catch (Exception e)
        {
            return Global.ServerErrorResponse(e);
        }
    }

    public static JsonResult SaveTurnOrderDeterminationSettings(Global.AnalyzerDataModel saveTurnOrderDeterminationSettingsParameter, FortuneStreetAppContext fortuneStreetAppContext)
    {
        Global.Response response = new Global.Response();

        try
        {
            fortuneStreetAppContext.TurnOrderDetermination.AddRange(saveTurnOrderDeterminationSettingsParameter.CharacterData.PlayerData.Select(player => new TurnOrderDetermination
            {
                AnalyzerInstanceID = saveTurnOrderDeterminationSettingsParameter.AnalyzerInstanceID,
                CharacterID = player.ID,
                Value = (byte) player.TurnOrderValue
            }));

            fortuneStreetAppContext.SaveChanges();

            BoardCharacteristics boardCharacteristicResult = fortuneStreetAppContext.BoardCharacteristics.SingleOrDefault(board_characteristic => board_characteristic.RuleID == saveTurnOrderDeterminationSettingsParameter.GameSettingsData.RuleData.ID && board_characteristic.BoardID == saveTurnOrderDeterminationSettingsParameter.GameSettingsData.BoardData.ID);

            Global.IndexDataModel indexData = Global.GetIndexData(saveTurnOrderDeterminationSettingsParameter.GameSettingsData.RuleData.ID, saveTurnOrderDeterminationSettingsParameter.GameSettingsData.BoardData.ID, fortuneStreetAppContext);

            List<long> spaceIDs = indexData.SpaceIndexData.Select(space_index => space_index.SpaceData.ID).ToList();

            List<SpaceLayouts> spaceLayoutResults = fortuneStreetAppContext.SpaceLayouts.Where(space_id => spaceIDs.Contains(space_id.SpaceID)).ToList();

            List<Global.AnalyzerDataModel.GameSettingsDataModel.TurnDataModel> initialTurnData = Enumerable.Range(0, saveTurnOrderDeterminationSettingsParameter.CharacterData.PlayerData.Count).Select(value => new Global.AnalyzerDataModel.GameSettingsDataModel.TurnDataModel
            {
                TurnPlayerData = new List<Global.AnalyzerDataModel.GameSettingsDataModel.TurnDataModel.TurnPlayerDataModel>
                {
                    new Global.AnalyzerDataModel.GameSettingsDataModel.TurnDataModel.TurnPlayerDataModel
                    {
                        LayoutIndex = 0,
                        Level = 1,
                        Placing = 1,
                        ReadyCash = boardCharacteristicResult.ReadyCashStart,
                        TotalShopValue = 0,
                        TotalStockValue = 0,
                        NetWorth = boardCharacteristicResult.ReadyCashStart,
                        OwnedShopIndices = new List<long>(),
                        TotalSuitCards = 0,
                        CollectedSuits = new List<string>(),
                        ArcadeIndex = 0
                    }
                },
                TurnCameoCharactersData = new List<List<Global.AnalyzerDataModel.GameSettingsDataModel.TurnDataModel.TurnCharacterPropertiesDataModel>>()
            }).ToList();

            List<Global.AnalyzerDataModel.SpaceDataModel> spaceData = new List<Global.AnalyzerDataModel.SpaceDataModel>();

            foreach (Global.IndexDataModel.SpaceIndexDataModel currentSpaceIndexData in indexData.SpaceIndexData)
            {
                if (currentSpaceIndexData.SpaceData.SpaceType.Name.Equals("bank"))
                {
                    foreach (Global.AnalyzerDataModel.GameSettingsDataModel.TurnDataModel currentInitialTurnData in initialTurnData)
                        currentInitialTurnData.TurnPlayerData.FirstOrDefault().SpaceIndexCurrent = currentSpaceIndexData.Index;
                }

                List<Global.IndexDataModel.SpaceConstraintIndexDataModel> currentSpaceConstraintIndexData = indexData.SpaceConstraintIndexData.Where(space_constraint_index_result => space_constraint_index_result.SpaceConstraintData.SpaceID == currentSpaceIndexData.SpaceData.ID).ToList();

                Global.IndexDataModel.ShopIndexDataModel currentShopIndexData = indexData.ShopIndexData.SingleOrDefault(shop_index_result => currentSpaceIndexData.SpaceData.ShopID != null && shop_index_result.ShopData.ID == (long) currentSpaceIndexData.SpaceData.ShopID);

                Global.IndexDataModel.DistrictIndexDataModel currentDistrictIndexData = indexData.DistrictIndexData.SingleOrDefault(district_index_result => currentSpaceIndexData.SpaceData.DistrictID != null && district_index_result.DistrictData.ID == (long) currentSpaceIndexData.SpaceData.DistrictID);

                spaceData.Add(new Global.AnalyzerDataModel.SpaceDataModel
                {
                    ID = currentSpaceIndexData.SpaceData.ID,
                    AdditionalPropertiesData = !string.IsNullOrWhiteSpace(currentSpaceIndexData.SpaceData.AdditionalProperties) ? JsonSerializer.Deserialize<Global.AnalyzerDataModel.SpaceDataModel.AdditionalPropertiesDataModel>(currentSpaceIndexData.SpaceData.AdditionalProperties) : null,
                    SpaceLayoutData = spaceLayoutResults.Where(space_id => space_id.SpaceID == currentSpaceIndexData.SpaceData.ID).Select(space_layout_result => new Global.AnalyzerDataModel.SpaceDataModel.SpaceLayoutDataModel
                    {
                        CenterXFactor = space_layout_result.CenterXFactor,
                        CenterYFactor = space_layout_result.CenterYFactor
                    }).ToList(),
                    SpaceConstraintData = currentSpaceConstraintIndexData.GroupBy(space_constraint_index_result => new { space_constraint_index_result.SpaceConstraintData.SpaceIDFrom, space_constraint_index_result.SpaceConstraintData.LayoutIndex }).Select(space_constraint_groupby => new Global.AnalyzerDataModel.SpaceDataModel.SpaceConstraintDataModel
                    {
                        SpaceIndexFrom = indexData.SpaceIndexData.FirstOrDefault(space_index_result => space_index_result.SpaceData.ID == space_constraint_groupby.Key.SpaceIDFrom).Index,
                        SpaceIndicesTo = indexData.SpaceIndexData.Where(space_index_result => currentSpaceConstraintIndexData.Where(space_constraint_index_result => space_constraint_index_result.SpaceConstraintData.SpaceIDFrom == space_constraint_groupby.Key.SpaceIDFrom).Select(space_constraint_index_result => space_constraint_index_result.SpaceConstraintData.SpaceIDTo).Contains(space_index_result.SpaceData.ID)).Select(index => index.Index).ToList(),
                        LayoutIndex = space_constraint_groupby.Key.LayoutIndex
                    }).ToList(),
                    SpaceTypeIndex = indexData.SpaceTypeIndexData.SingleOrDefault(space_type_index_result => space_type_index_result.SpaceTypeData.ID == currentSpaceIndexData.SpaceData.SpaceTypeID).Index,
                    ShopIndex = currentShopIndexData?.Index,
                    DistrictIndex = currentDistrictIndexData?.Index
                });
            }

            List<GetCharacterColorsTVF> getCharacterColorsTVFResults = fortuneStreetAppContext.GetCharacterColorsTVF.FromSqlRaw("SELECT * FROM getcharactercolors_tvf({0})", saveTurnOrderDeterminationSettingsParameter.AnalyzerInstanceID).ToList();

            List<Colors> colorsResults = fortuneStreetAppContext.Colors.ToList();

            Global.AnalyzerDataModel analyzerData = new Global.AnalyzerDataModel
            {
                GameSettingsData = new Global.AnalyzerDataModel.GameSettingsDataModel
                {
                    TurnData = new List<List<Global.AnalyzerDataModel.GameSettingsDataModel.TurnDataModel>> { initialTurnData }
                },
                CharacterData = new Global.AnalyzerDataModel.CharacterDataModel
                {
                    PlayerData = getCharacterColorsTVFResults.Select(result => new Global.AnalyzerDataModel.CharacterDataModel.CharacterPlayerDataModel
                    {
                        ID = result.CharacterID,
                        ColorData = new Global.AnalyzerDataModel.CharacterDataModel.CharacterPlayerDataModel.ColorDataModel
                        {
                            ID = result.ColorIDAssigned,
                            CharacterColor = colorsResults.SingleOrDefault(color => color.ID == result.ColorIDAssigned).CharacterColor
                        }
                    }).ToList()
                },
                SpaceData = spaceData,
                SpaceTypeData = indexData.SpaceTypeIndexData.Select(space_type_index_result => new Global.AnalyzerDataModel.SpaceTypeDataModel
                {
                    ID = space_type_index_result.SpaceTypeData.ID,
                    Name = space_type_index_result.SpaceTypeData.Name,
                    Icon = space_type_index_result.SpaceTypeData.Icon,
                    Title = space_type_index_result.SpaceTypeData.Title,
                    Description = space_type_index_result.SpaceTypeData.Description
                }).ToList(),
                ShopData = indexData.ShopIndexData.Select(shop_index_result => new Global.AnalyzerDataModel.ShopDataModel
                {
                    ID = shop_index_result.ShopData.ID,
                    Name = shop_index_result.ShopData.Name,
                    Value = shop_index_result.ShopData.Value
                }).ToList(),
                DistrictData = indexData.DistrictIndexData.Select(district_index_result => new Global.AnalyzerDataModel.DistrictDataModel
                {
                    ID = district_index_result.DistrictData.ID,
                    Name = district_index_result.DistrictData.Name,
                    Color = district_index_result.DistrictData.Color
                }).ToList()
            };

            SavePreRollTurnData
            (
                initialTurnData.Select((turn_result, turn_index) => new PreRolls
                {
                    AnalyzerInstanceID = saveTurnOrderDeterminationSettingsParameter.AnalyzerInstanceID,
                    CharacterID = analyzerData.CharacterData.PlayerData[turn_index].ID,
                    SpaceIDCurrent = analyzerData.SpaceData[(int) turn_result.TurnPlayerData.FirstOrDefault().SpaceIndexCurrent].ID,
                    TurnNumber = (byte) analyzerData.GameSettingsData.TurnData.Count,
                    LayoutIndex = turn_result.TurnPlayerData.FirstOrDefault().LayoutIndex,
                    Level = turn_result.TurnPlayerData.FirstOrDefault().Level,
                    Placing = turn_result.TurnPlayerData.FirstOrDefault().Placing,
                    ReadyCash = turn_result.TurnPlayerData.FirstOrDefault().ReadyCash,
                    TotalShopValue = turn_result.TurnPlayerData.FirstOrDefault().TotalShopValue,
                    TotalStockValue = turn_result.TurnPlayerData.FirstOrDefault().TotalStockValue,
                    NetWorth = turn_result.TurnPlayerData.FirstOrDefault().NetWorth,
                    OwnedShopIndices = JsonSerializer.Serialize(turn_result.TurnPlayerData.FirstOrDefault().OwnedShopIndices),
                    TotalSuitCards = turn_result.TurnPlayerData.FirstOrDefault().TotalSuitCards,
                    CollectedSuits = JsonSerializer.Serialize(turn_result.TurnPlayerData.FirstOrDefault().CollectedSuits),
                    ArcadeIndex = turn_result.TurnPlayerData.FirstOrDefault().ArcadeIndex,
                    DieRollRestrictions = turn_result.TurnPlayerData.FirstOrDefault().DieRollRestrictions != null ? JsonSerializer.Serialize(turn_result.TurnPlayerData.FirstOrDefault().DieRollRestrictions) : null
                }).ToList(),
                fortuneStreetAppContext
            );

            response.HTMLResponse = JsonSerializer.Serialize(new
            {
                Data = analyzerData
            });

            return new JsonResult(response);
        }
        catch (Exception e)
        {
            return Global.ServerErrorResponse(e);
        }
    }

    public static JsonResult SavePreRollTurnData(List<PreRolls> preRollsRecords, FortuneStreetAppContext fortuneStreetAppContext)
    {
        try
        {
            fortuneStreetAppContext.PreRolls.AddRange(preRollsRecords);

            fortuneStreetAppContext.SaveChanges();

            return new JsonResult(new Global.Response());
        }
        catch (Exception e)
        {
            return Global.ServerErrorResponse(e);
        }
    }

    public static JsonResult SavePostRollTurnData(PostRolls postRollsRecord, FortuneStreetAppContext fortuneStreetAppContext)
    {
        try
        {
            fortuneStreetAppContext.PostRolls.Add(postRollsRecord);

            fortuneStreetAppContext.SaveChanges();

            return new JsonResult(new Global.Response());
        }
        catch (Exception e)
        {
            return Global.ServerErrorResponse(e);
        }
    }
}
