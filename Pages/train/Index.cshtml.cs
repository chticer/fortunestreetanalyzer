using fortunestreetanalyzer.DatabaseModels.fortunestreet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;

namespace fortunestreetanalyzer.Pages.train
{
    public class IndexModel : PageModel
    {
        private readonly FortuneStreetAppContext _fortuneStreetAppContext;
        private readonly FortuneStreetSaveAnalyzerInstanceLogContext _fortuneStreetSaveAnalyzerInstanceLogContext;

        public class StartupDataModel
        {
            public long? id { get; set; }
        }

        public class SaveAnalyzerDataModel
        {
            public List<string> keys { get; set; }
            public Global.AnalyzerDataModel analyzerData { get; set; }
        }

        public IndexModel(FortuneStreetAppContext fortuneStreetAppContext, FortuneStreetSaveAnalyzerInstanceLogContext fortuneStreetSaveAnalyzerInstanceLogContext)
        {
            _fortuneStreetAppContext = fortuneStreetAppContext;
            _fortuneStreetSaveAnalyzerInstanceLogContext = fortuneStreetSaveAnalyzerInstanceLogContext;
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

                response.HTMLResponse = JsonSerializer.Serialize(Global.RebuildAnalyzerInstanceDataResponse(true, true, currentUserAnalyzerInstance != null ? currentUserAnalyzerInstance.AnalyzerInstanceID : (long) Global.CreateAnalyzerInstanceID("train", null, _fortuneStreetAppContext), new List<string> { "train" }, _fortuneStreetAppContext).FirstOrDefault());

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                return Global.ServerErrorResponse(e);
            }
        }

        public JsonResult OnGetLoadGameData()
        {
            Global.Response response = new Global.Response();

            try
            {
                response.HTMLResponse = JsonSerializer.Serialize(RebuildAnalyzerInstanceHelper.LoadGameData(null, _fortuneStreetAppContext));

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                return Global.ServerErrorResponse(e);
            }
        }

        public JsonResult OnPostSaveGameData([FromBody] Global.AnalyzerDataModel saveGameDataParameter)
        {
            Global.Response response = new Global.Response();

            try
            {
                Rules rulesResult = _fortuneStreetAppContext.Rules.SingleOrDefault(id => id.ID == saveGameDataParameter.GameData.RuleData.ID);

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

                Boards boardsResult = _fortuneStreetAppContext.Boards.SingleOrDefault(id => id.ID == saveGameDataParameter.GameData.BoardData.ID);

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

                Colors colorsResult = _fortuneStreetAppContext.Colors.SingleOrDefault(id => id.ID == saveGameDataParameter.GameData.ColorData.ID);

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

                _fortuneStreetAppContext.GameSettings.Add(new GameSettings
                {
                    AnalyzerInstanceID = saveGameDataParameter.AnalyzerInstanceID,
                    RuleID = rulesResult.ID,
                    BoardID = boardsResult.ID,
                    MiiColorID = colorsResult.ID
                });

                _fortuneStreetAppContext.SaveChanges();

                BoardCharacteristics boardCharacteristicResult = _fortuneStreetAppContext.BoardCharacteristics.SingleOrDefault(board_characteristic => board_characteristic.RuleID == rulesResult.ID && board_characteristic.BoardID == boardsResult.ID);

                response.HTMLResponse = JsonSerializer.Serialize(new
                {
                    Data = new Global.AnalyzerDataModel
                    {
                        GameData = new Global.AnalyzerDataModel.GameDataModel
                        {
                            RuleData = new Global.AnalyzerDataModel.GameDataModel.RuleDataModel
                            {
                                ID = rulesResult.ID,
                                Name = rulesResult.Name,
                                StandingThreshold = boardCharacteristicResult.StandingThreshold,
                                NetWorthThreshold = boardCharacteristicResult.NetWorthThreshold
                            },
                            BoardData = new Global.AnalyzerDataModel.GameDataModel.BoardDataModel
                            {
                                ID = boardsResult.ID,
                                Name = boardsResult.Name,
                                SalaryStart = boardCharacteristicResult.SalaryStart,
                                SalaryIncrease = boardCharacteristicResult.SalaryIncrease,
                                MaxDieRoll = boardCharacteristicResult.MaxDieRoll
                            },
                            ColorData = new Global.AnalyzerDataModel.GameDataModel.ColorDataModel
                            {
                                ID = colorsResult.ID,
                                SystemColor = colorsResult.SystemColor,
                                CharacterColor = colorsResult.CharacterColor
                            }
                        }
                    }
                });

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                return Global.ServerErrorResponse(e);
            }
        }

        public JsonResult OnPostLoadCharacters([FromBody] Boards loadCharactersParameter)
        {
            Global.Response response = new Global.Response();

            try
            {
                response.HTMLResponse = JsonSerializer.Serialize(RebuildAnalyzerInstanceHelper.LoadCharacters
                (
                    new Global.AnalyzerDataModel
                    {
                        GameData = new Global.AnalyzerDataModel.GameDataModel
                        {
                            BoardData = new Global.AnalyzerDataModel.GameDataModel.BoardDataModel
                            {
                                ID = loadCharactersParameter.ID
                            }
                        }
                    },
                    _fortuneStreetAppContext
                ));

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                return Global.ServerErrorResponse(e);
            }
        }

        public JsonResult OnPostSaveCharacterData([FromBody] Global.AnalyzerDataModel saveCharacterDataParameter)
        {
            Global.Response response = new Global.Response();

            try
            {
                _fortuneStreetAppContext.TurnOrderDetermination.AddRange(saveCharacterDataParameter.CharacterData.Select(character => new TurnOrderDetermination
                {
                    AnalyzerInstanceID = saveCharacterDataParameter.AnalyzerInstanceID,
                    CharacterID = character.ID,
                    Value = character.TurnOrderValue
                }));

                _fortuneStreetAppContext.SaveChanges();

                List<GetCharacterColorsTVF> getCharacterColorsTVFResults = _fortuneStreetAppContext.GetCharacterColorsTVF.FromSqlInterpolated($"SELECT * FROM getcharactercolors_tvf({saveCharacterDataParameter.AnalyzerInstanceID})").ToList();

                List<Global.AnalyzerDataModel.CharacterDataModel> characters = getCharacterColorsTVFResults.Select(result => new Global.AnalyzerDataModel.CharacterDataModel
                {
                    ID = result.CharacterID,
                    TurnOrderValue = result.Value,
                    ColorData = new Global.AnalyzerDataModel.CharacterDataModel.ColorDataModel
                    {
                        ID = result.ColorIDAssigned
                    }
                }).ToList();

                List<GetBoardCharactersTVF> getBoardCharactersTVFResults = _fortuneStreetAppContext.GetBoardCharactersTVF.FromSqlInterpolated($"SELECT * FROM getboardcharacters_tvf({saveCharacterDataParameter.GameData.BoardData.ID})").ToList();

                List<long> characterColorIDs = characters.Select(character => character.ColorData.ID).ToList();

                List<Colors> colors = _fortuneStreetAppContext.Colors.Where(color => characterColorIDs.Contains(color.ID)).ToList();

                foreach (Global.AnalyzerDataModel.CharacterDataModel currentCharacter in characters)
                {
                    GetBoardCharactersTVF currentGetBoardCharactersTVFResult = getBoardCharactersTVFResults.SingleOrDefault(result => result.CharacterID == currentCharacter.ID);

                    if (currentGetBoardCharactersTVFResult != null)
                    {
                        currentCharacter.PortraitURL = Global.AZURE_STORAGE_URL + currentGetBoardCharactersTVFResult.CharacterPortraitURI;
                        currentCharacter.Name = currentGetBoardCharactersTVFResult.Name;
                    }

                    Colors currentColor = colors.SingleOrDefault(color => color.ID == currentCharacter.ColorData.ID);

                    currentCharacter.ColorData.GameColor = currentColor.CharacterColor;
                }

                response.HTMLResponse = JsonSerializer.Serialize(new
                {
                    Data = new Global.AnalyzerDataModel
                    {
                        CharacterData = characters.Select(character => new Global.AnalyzerDataModel.CharacterDataModel
                        {
                            ID = character.ID,
                            PortraitURL = character.PortraitURL,
                            Name = character.Name,
                            TurnOrderValue = character.TurnOrderValue,
                            ColorData = new Global.AnalyzerDataModel.CharacterDataModel.ColorDataModel
                            {
                                ID = character.ColorData.ID,
                                GameColor = character.ColorData.GameColor
                            }
                        }).ToList()
                    }
                });

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                return Global.ServerErrorResponse(e);
            }
        }

        public JsonResult OnPostLoadSettingsOnGameStart([FromBody] Global.AnalyzerDataModel.GameDataModel loadSettingsOnGameStartParameter)
        {
            Global.Response response = new Global.Response();

            try
            {
                BoardCharacteristics boardCharacteristicResult = _fortuneStreetAppContext.BoardCharacteristics.SingleOrDefault(board_characteristic => board_characteristic.RuleID == loadSettingsOnGameStartParameter.RuleData.ID && board_characteristic.BoardID == loadSettingsOnGameStartParameter.BoardData.ID);

                List<Spaces> spaceResults = _fortuneStreetAppContext.Spaces.Where(space => space.RuleID == loadSettingsOnGameStartParameter.RuleData.ID && space.BoardID == loadSettingsOnGameStartParameter.BoardData.ID).ToList();

                List<long> spaceIDs = spaceResults.Select(id => id.ID).ToList();

                List<SpaceLayouts> spaceLayoutResults = _fortuneStreetAppContext.SpaceLayouts.Where(space_id => spaceIDs.Contains(space_id.SpaceID)).OrderBy(layout_index => layout_index.LayoutIndex).ToList();

                List<long> spaceTypeIDs = spaceResults.Select(space_type_id => space_type_id.SpaceTypeID).Distinct().ToList();

                List<SpaceTypes> spaceTypeResults = _fortuneStreetAppContext.SpaceTypes.Where(id => spaceTypeIDs.Contains(id.ID)).ToList();

                List<long> shopIDs = spaceResults.Where(shop_id => shop_id.ShopID != null).Select(shop_id => (long) shop_id.ShopID).Distinct().ToList();

                List<Shops> shopResults = _fortuneStreetAppContext.Shops.Where(id => shopIDs.Contains(id.ID)).ToList();

                List<long> districtIDs = spaceResults.Where(district_id => district_id.DistrictID != null).Select(district_id => (long) district_id.DistrictID).Distinct().ToList();

                List<Districts> districtResults = _fortuneStreetAppContext.Districts.Where(id => districtIDs.Contains(id.ID)).ToList();

                Global.IndexDataModel indexData = new Global.IndexDataModel
                {
                    SpaceIndexData = spaceResults.Select((space_result, space_index) => new Global.IndexDataModel.SpaceIndexDataModel
                    {
                        Index = space_index,
                        SpaceData = space_result
                    }).ToList(),
                    SpaceTypeIndexData = spaceTypeResults.Select((space_type_result, space_type_index) => new Global.IndexDataModel.SpaceTypeIndexDataModel
                    {
                        Index = space_type_index,
                        SpaceTypeData = space_type_result
                    }).ToList(),
                    ShopIndexData = shopResults.Select((shop_result, shop_index) => new Global.IndexDataModel.ShopIndexDataModel
                    {
                        Index = shop_index,
                        ShopData = shop_result
                    }).ToList(),
                    DistrictIndexData = new List<Global.IndexDataModel.DistrictIndexDataModel>()
                };

                if (districtIDs.Count > 0)
                    indexData.DistrictIndexData = districtResults.Select((district_result, district_index) => new Global.IndexDataModel.DistrictIndexDataModel
                    {
                        Index = district_index,
                        DistrictData = district_result
                    }).ToList();

                List<Global.AnalyzerDataModel.CharacterDataModel> characterData = Enumerable.Range(0, 4).Select(value => new Global.AnalyzerDataModel.CharacterDataModel
                {
                    Level = 1,
                    Placing = 1,
                    ReadyCash = boardCharacteristicResult.ReadyCashStart,
                    TotalShopValue = 0,
                    TotalStockValue = 0,
                    NetWorth = boardCharacteristicResult.ReadyCashStart,
                    OwnedShopIndices = new List<long>(),
                    OwnedSuits = new Global.AnalyzerDataModel.CharacterDataModel.SuitDataModel
                    {
                        TotalSuitCards = 0,
                        SuitNames = new List<string>()
                    }
                }).ToList();

                List<Global.AnalyzerDataModel.SpaceDataModel> spaceData = new List<Global.AnalyzerDataModel.SpaceDataModel>();

                foreach (Global.IndexDataModel.SpaceIndexDataModel currentSpaceIndexData in indexData.SpaceIndexData)
                {
                    if (currentSpaceIndexData.SpaceData.SpaceType.Name.Equals("bank"))
                    {
                        foreach (Global.AnalyzerDataModel.CharacterDataModel currentCharacterData in characterData)
                            currentCharacterData.SpaceIndex = currentSpaceIndexData.Index;
                    }

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
                        SpaceTypeIndex = indexData.SpaceTypeIndexData.SingleOrDefault(space_type_index_result => space_type_index_result.SpaceTypeData.ID == currentSpaceIndexData.SpaceData.SpaceTypeID).Index,
                        ShopIndex = currentShopIndexData?.Index,
                        DistrictIndex = currentDistrictIndexData?.Index
                    });
                }

                response.HTMLResponse = JsonSerializer.Serialize(new
                {
                    Data = new Global.AnalyzerDataModel
                    {
                        GameData = new Global.AnalyzerDataModel.GameDataModel
                        {
                            TurnData = new List<Global.AnalyzerDataModel.GameDataModel.TurnDataModel>()
                        },
                        CharacterData = characterData,
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
                    }
                });

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                return Global.ServerErrorResponse(e);
            }
        }

        public JsonResult OnPostSaveAnalyzerData([FromBody] SaveAnalyzerDataModel saveAnalyzerDataParameter)
        {
            Global.Response response = new Global.Response();

            try
            {
                CurrentAnalyzerInstancesTVF currentUserAnalyzerInstance = Global.FindUserAnalyzerInstances(saveAnalyzerDataParameter.analyzerData.AnalyzerInstanceID, new List<string> { "train" }, _fortuneStreetSaveAnalyzerInstanceLogContext).SingleOrDefault(type => Equals(type.Type, "train"));

                if (currentUserAnalyzerInstance == null)
                    throw new Exception();

                _fortuneStreetSaveAnalyzerInstanceLogContext.AnalyzerInstanceLogs.AddRange(typeof(Global.AnalyzerDataModel).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(name => saveAnalyzerDataParameter.keys.Contains(name.Name)).Select(property => new AnalyzerInstanceLogs
                {
                    AnalyzerInstanceID = currentUserAnalyzerInstance.AnalyzerInstanceID,
                    Key = property.Name,
                    Value = JsonSerializer.Serialize(property.GetValue(saveAnalyzerDataParameter.analyzerData, null))
                }));

                _fortuneStreetSaveAnalyzerInstanceLogContext.SaveChanges();

                response.HTMLResponse = JsonSerializer.Serialize(new
                {
                    Data = new Global.AnalyzerDataModel
                    {
                        AnalyzerInstanceID = currentUserAnalyzerInstance.AnalyzerInstanceID,
                        AnalyzerInstanceName = currentUserAnalyzerInstance.Name,
                        AnalyzerInstanceStarted = currentUserAnalyzerInstance.TimestampAdded
                    }
                });

                response.AlertData = new Global.Response.Alert
                {
                    Type = "alert-success",
                    Title = "Saved data..."
                };
            }
            catch
            {
                response.AlertData = new Global.Response.Alert
                {
                    Type = "alert-warning",
                    Title = "Cannot save data..."
                };
                response.Error = true;
            }

            return new JsonResult(response);
        }
    }
}
