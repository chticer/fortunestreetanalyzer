using fortunestreetanalyzer.DatabaseModels.fortunestreet;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace fortunestreetanalyzer;

public class LoadAnalyzerInstanceHelper
{
    private class PostRollsTurnDataModel
    {
        public int TurnIndex { get; set; }
        public int CharacterIndex { get; set; }
        public List<GetPostRollsTVF> GetPostRollsTVFResults { get; set; }
    }

    public static Global.LoadAnalyzerInstance LoadGameSettings(long analyzerInstanceID, FortuneStreetAppContext fortuneStreetAppContext)
    {
        List<Rules> rules = fortuneStreetAppContext.Rules.OrderBy(name => name.Name).ToList();

        List<Boards> boards = fortuneStreetAppContext.Boards.OrderBy(name => name.Name).ToList();

        List<Colors> miiColors = fortuneStreetAppContext.Colors.OrderBy(id => id.ID).ToList();

        Global.LoadAnalyzerInstance loadAnalyzerInstanceGameSettings = new Global.LoadAnalyzerInstance
        {
            Data = new Global.AnalyzerDataModel
            {
                GameSettingsData = new Global.AnalyzerDataModel.GameSettingsDataModel
                {
                    RuleData = new Global.AnalyzerDataModel.GameSettingsDataModel.RuleDataModel
                    {
                        ID = rules.FirstOrDefault().ID
                    },
                    BoardData = new Global.AnalyzerDataModel.GameSettingsDataModel.BoardDataModel
                    {
                        ID = boards.FirstOrDefault().ID
                    },
                    ColorData = new Global.AnalyzerDataModel.GameSettingsDataModel.ColorDataModel
                    {
                        ID = miiColors.FirstOrDefault().ID
                    }
                }
            }
        };

        GameSettings gameSettingResult = fortuneStreetAppContext.GameSettings.SingleOrDefault(analyzer_instance_id => analyzer_instance_id.AnalyzerInstanceID == analyzerInstanceID);

        if (gameSettingResult != null)
        {
            Rules ruleResult = rules.SingleOrDefault(id => id.ID == gameSettingResult.RuleID);

            loadAnalyzerInstanceGameSettings.Data.GameSettingsData.RuleData.ID = ruleResult.ID;
            loadAnalyzerInstanceGameSettings.Data.GameSettingsData.RuleData.Name = ruleResult.Name;

            BoardCharacteristics boardCharacteristicResult = fortuneStreetAppContext.BoardCharacteristics.SingleOrDefault(board_characteristic => board_characteristic.RuleID == gameSettingResult.RuleID && board_characteristic.BoardID == gameSettingResult.BoardID);

            loadAnalyzerInstanceGameSettings.Data.GameSettingsData.RuleData.NetWorthThreshold = boardCharacteristicResult.NetWorthThreshold;
            loadAnalyzerInstanceGameSettings.Data.GameSettingsData.RuleData.StandingThreshold = boardCharacteristicResult.StandingThreshold;

            Boards boardResult = boards.SingleOrDefault(id => id.ID == gameSettingResult.BoardID);

            loadAnalyzerInstanceGameSettings.Data.GameSettingsData.BoardData.ID = boardResult.ID;
            loadAnalyzerInstanceGameSettings.Data.GameSettingsData.BoardData.Name = boardResult.Name;

            Colors colorResult = miiColors.SingleOrDefault(id => id.ID == gameSettingResult.MiiColorID);

            loadAnalyzerInstanceGameSettings.Data.GameSettingsData.ColorData.ID = colorResult.ID;
            loadAnalyzerInstanceGameSettings.Data.GameSettingsData.ColorData.SystemColor = colorResult.SystemColor;
            loadAnalyzerInstanceGameSettings.Data.GameSettingsData.ColorData.CharacterColor = colorResult.CharacterColor;
        }

        loadAnalyzerInstanceGameSettings.Response =
            "<div id=\"game-selection\">" +
                "<div>" +
                    "<div>" +
                        "<h5>Rules</h5>" +
                    "</div>" +

                    "<div>" +
                        "<select class=\"form-select" + (gameSettingResult != null ? " disabled" : "") + "\"" + (gameSettingResult != null ? " disabled=\"disabled\"" : "") + ">" +

                            string.Join("", rules.Select(rule => "<option value=\"" + rule.ID + "\"" + (loadAnalyzerInstanceGameSettings.Data.GameSettingsData.RuleData.ID == rule.ID ? " selected=\"selected\"" : "") + ">" + rule.Name + "</option>")) +

                        "</select>" +
                    "</div>" +
                "</div>" +

                "<div>" +
                    "<div>" +
                        "<h5>Boards</h5>" +
                    "</div>" +

                    "<div>" +
                        "<select class=\"form-select" + (gameSettingResult != null ? " disabled" : "") + "\"" + (gameSettingResult != null ? " disabled=\"disabled\"" : "") + ">" +

                            string.Join("", boards.Select(board => "<option value=\"" + board.ID + "\"" + (loadAnalyzerInstanceGameSettings.Data.GameSettingsData.BoardData.ID == board.ID ? " selected=\"selected\"" : "") + ">" + board.Name + "</option>")) +

                        "</select>" +
                    "</div>" +
                "</div>" +

                "<div>" +
                    "<div>" +
                        "<h5>Mii Color</h5>" +
                    "</div>" +

                    "<div class=\"color-selection" + (gameSettingResult != null ? " disabled" : "") + "\">" +

                        string.Join("", miiColors.Select
                        (
                            color =>
                                "<div>" +
                                    "<input type=\"hidden\" data-colorid=\"" + color.ID + "\" />" +

                                    "<div style=\"background-color: #" + color.SystemColor + ";\"" + (loadAnalyzerInstanceGameSettings.Data.GameSettingsData.ColorData.ID == color.ID ? " class=\"selected\"" : "") + "></div>" +
                                "</div>"
                        )) +

                    "</div>" +
                "</div>" +

                (
                    gameSettingResult == null
                    ?
                    Global.CreateConfirmationActions("center-items", new List<string>
                    {
                        "<button type=\"button\" class=\"btn btn-lg btn-primary\" name=\"confirm\">Confirm</button>"
                    })
                    :
                    ""
                ) +

            "</div>";

        return loadAnalyzerInstanceGameSettings;
    }

    public static Global.LoadAnalyzerInstance LoadTurnOrderDeterminationSettings(long analyzerInstanceID, FortuneStreetAppContext fortuneStreetAppContext)
    {
        GameSettings gameSettingResult = fortuneStreetAppContext.GameSettings.SingleOrDefault(analyzer_instance_id => analyzer_instance_id.AnalyzerInstanceID == analyzerInstanceID);

        List<GetBoardCharactersTVF> getBoardCharactersTVFResults = fortuneStreetAppContext.GetBoardCharactersTVF.FromSqlRaw("SELECT * FROM getboardcharacters_tvf({0})", gameSettingResult.BoardID).ToList();

        List<Characters> cameoCharacters = fortuneStreetAppContext.Characters.Where(rank => string.IsNullOrEmpty(rank.Rank)).ToList();

        Global.AnalyzerDataModel.CharacterDataModel characterData = new Global.AnalyzerDataModel.CharacterDataModel
        {
            PlayerData = new List<Global.AnalyzerDataModel.CharacterDataModel.CharacterPlayerDataModel>
            {
                new Global.AnalyzerDataModel.CharacterDataModel.CharacterPlayerDataModel
                {
                    Name = "You"
                }
            }.Union(getBoardCharactersTVFResults.Select(result => new Global.AnalyzerDataModel.CharacterDataModel.CharacterPlayerDataModel
            {
                ID = result.CharacterID,
                PortraitURL = Global.AZURE_STORAGE_URL + result.CharacterPortraitURI,
                Name = result.Name
            })).ToList(),
            CameoCharacterData = cameoCharacters.Select(result => new Global.AnalyzerDataModel.CharacterDataModel.CharacterPropertiesDataModel
            {
                ID = result.ID,
                PortraitURL = Global.AZURE_STORAGE_URL + result.CharacterPortraitURI,
                Name = result.Name
            }).ToList()
        };

        List<GetCharacterColorsTVF> getCharacterColorsTVFResults = fortuneStreetAppContext.GetCharacterColorsTVF.FromSqlRaw("SELECT * FROM getcharactercolors_tvf({0})", analyzerInstanceID).ToList();

        if (getCharacterColorsTVFResults.Count > 0)
        {
            List<long> colorIDsAssigned = getCharacterColorsTVFResults.Select(color_id_assigned => color_id_assigned.ColorIDAssigned).ToList();

            List<Colors> colorsAssignedResults = fortuneStreetAppContext.Colors.Where(color => colorIDsAssigned.Contains(color.ID)).ToList();

            foreach (GetCharacterColorsTVF currentGetCharacterColorsTVFResult in getCharacterColorsTVFResults)
            {
                Colors currentColorsAssignedResult = colorsAssignedResults.SingleOrDefault(id => id.ID == currentGetCharacterColorsTVFResult.ColorIDAssigned);

                Global.AnalyzerDataModel.CharacterDataModel.CharacterPlayerDataModel currentCharacterPlayer = characterData.PlayerData.SingleOrDefault(id => id.ID == currentGetCharacterColorsTVFResult.CharacterID);

                currentCharacterPlayer.TurnOrderValue = currentGetCharacterColorsTVFResult.Value;
                currentCharacterPlayer.ColorData = new Global.AnalyzerDataModel.CharacterDataModel.CharacterPlayerDataModel.ColorDataModel
                {
                    ID = currentColorsAssignedResult.ID,
                    CharacterColor = currentColorsAssignedResult.CharacterColor
                };
            }
        }

        Global.AnalyzerDataModel.CharacterDataModel characterDataOrdered = new Global.AnalyzerDataModel.CharacterDataModel
        {
            PlayerData = characterData.PlayerData,
            CameoCharacterData = characterData.CameoCharacterData
        };

        if (getCharacterColorsTVFResults.Count > 0)
            characterDataOrdered.PlayerData = characterDataOrdered.PlayerData.OrderByDescending(turn_order_value => turn_order_value.TurnOrderValue).ToList();

        return new Global.LoadAnalyzerInstance
        {
            Data = new Global.AnalyzerDataModel
            {
                CharacterData = characterDataOrdered
            },
            Response =
                "<div id=\"player-turn-determination\"" + (getCharacterColorsTVFResults.Count > 0 ? " class=\"disabled\"" : "") + ">" +
                    "<div>" +

                        string.Join("", characterData.PlayerData.Select
                        (
                            character =>
                                "<div>" +
                                    "<div class=\"character-portrait-icon\">" +

                                        (
                                            !string.IsNullOrWhiteSpace(character.PortraitURL)
                                            ?
                                            "<img src=\"" + character.PortraitURL + "\" alt=\"Character Portrait for " + character.Name + "\" />"
                                            :
                                            ""
                                        ) +

                                    "</div>" +

                                    "<div>" + character.Name + "</div>" +

                                    "<div>" +
                                        "<div>" +

                                            string.Join("", Enumerable.Range(0, 10).Select
                                            (
                                                digit =>
                                                    "<span>" +
                                                        "<button type=\"button\" class=\"btn btn-outline-primary btn-lg" + (getCharacterColorsTVFResults.Count > 0 && character.TurnOrderValue / 10 == digit ? " active" : "") + "\" value=\"" + (digit * 10) + "\">" + digit + "</button>" +
                                                    "</span>"
                                            )) +

                                        "</div>" +

                                        "<div>" +

                                            string.Join("", Enumerable.Range(0, 10).Select
                                            (
                                                digit =>
                                                    "<span>" +
                                                        "<button type=\"button\" class=\"btn btn-outline-primary btn-lg" + (getCharacterColorsTVFResults.Count > 0 && character.TurnOrderValue % 10 == digit ? " active" : "") + "\" value=\"" + digit + "\">" + digit + "</button>" +
                                                    "</span>"
                                            )) +

                                        "</div>" +
                                    "</div>" +
                                "</div>"
                        )) +

                    "</div>" +

                    (
                        getCharacterColorsTVFResults.Count == 0
                        ?
                        Global.CreateConfirmationActions("center-items", new List<string>
                        {
                            "<button type=\"button\" class=\"btn btn-lg btn-primary\" name=\"confirm\">Confirm</button>"
                        })
                        :
                        ""
                    ) +

                "</div>"
        };
    }

    public static Global.AnalyzerDataModel LoadAnalyzerData(long analyzerInstanceID, FortuneStreetAppContext fortuneStreetAppContext)
    {
        GameSettings gameSettingResult = fortuneStreetAppContext.GameSettings.SingleOrDefault(analyzer_instance_id => analyzer_instance_id.AnalyzerInstanceID == analyzerInstanceID);

        BoardCharacteristics boardCharacteristicResult = fortuneStreetAppContext.BoardCharacteristics.SingleOrDefault(board_characteristic => board_characteristic.RuleID == gameSettingResult.RuleID && board_characteristic.BoardID == gameSettingResult.BoardID);

        Global.IndexDataModel indexData = Global.GetIndexData(gameSettingResult.RuleID, gameSettingResult.BoardID, fortuneStreetAppContext);

        List<long> spaceIDs = indexData.SpaceIndexData.Select(space_index => space_index.SpaceData.ID).ToList();

        List<SpaceLayouts> spaceLayoutResults = fortuneStreetAppContext.SpaceLayouts.Where(space_id => spaceIDs.Contains(space_id.SpaceID)).ToList();

        List<GetPreRollsTVF> getPreRollsTVFResults = fortuneStreetAppContext.GetPreRollsTVF.FromSqlRaw("SELECT * FROM getprerolls_tvf({0})", analyzerInstanceID).ToList();

        List<GetPostRollsTVF> getPostRollsTVFResults = fortuneStreetAppContext.GetPostRollsTVF.FromSqlRaw("SELECT * FROM getpostrolls_tvf({0})", analyzerInstanceID).ToList();

        List<List<Global.AnalyzerDataModel.GameSettingsDataModel.TurnDataModel>> turnData = getPreRollsTVFResults.GroupBy(turn_number => turn_number.TurnNumber).Select(turn_group_by => turn_group_by.GroupBy(character_id => character_id.CharacterID).Select(turn_character_group_by => new Global.AnalyzerDataModel.GameSettingsDataModel.TurnDataModel
        {
            TurnPlayerData = turn_character_group_by.Select(turn_character => new Global.AnalyzerDataModel.GameSettingsDataModel.TurnDataModel.TurnPlayerDataModel
            {
                Level = turn_character.Level,
                Placing = turn_character.Placing,
                ReadyCash = turn_character.ReadyCash,
                TotalShopValue = turn_character.TotalShopValue,
                TotalStockValue = turn_character.TotalStockValue,
                NetWorth = turn_character.NetWorth,
                OwnedShopIndices = JsonSerializer.Deserialize<List<long>>(turn_character.OwnedShopIndices),
                TotalSuitCards = turn_character.TotalSuitCards,
                CollectedSuits = JsonSerializer.Deserialize<List<string>>(turn_character.CollectedSuits),
                ArcadeIndex = turn_character.ArcadeIndex,
                SpaceIndexCurrent = indexData.SpaceIndexData.SingleOrDefault(space_index => space_index.SpaceData.ID == turn_character.SpaceIDCurrent).Index,
                SpaceIndexFrom = turn_character.SpaceIDFrom != null ? indexData.SpaceIndexData.SingleOrDefault(space_index => space_index.SpaceData.ID == (long) turn_character.SpaceIDFrom).Index : null,
                LayoutIndex = turn_character.LayoutIndex,
                DieRollRestrictions = turn_character.DieRollRestrictions != null ? JsonSerializer.Deserialize<List<byte>>(turn_character.DieRollRestrictions) : null
            }).ToList(),
            TurnCameoCharactersData = new List<List<Global.AnalyzerDataModel.GameSettingsDataModel.TurnDataModel.TurnCharacterPropertiesDataModel>>()
        }).ToList()).ToList();

        foreach (PostRollsTurnDataModel postRollsTurnData in getPostRollsTVFResults.GroupBy(turn_number => turn_number.TurnNumber).Select((turn_group_by, turn_index) => turn_group_by.GroupBy(character_id => character_id.CharacterID).Select((turn_character_group_by, turn_character_index) => new PostRollsTurnDataModel { TurnIndex = turn_index, CharacterIndex = turn_character_index, GetPostRollsTVFResults = turn_character_group_by.ToList() })))
        {
            for (int i = 0; i < postRollsTurnData.GetPostRollsTVFResults.Count; ++i)
            {
                turnData[postRollsTurnData.TurnIndex][postRollsTurnData.CharacterIndex].TurnPlayerData[i].DieRollValue = postRollsTurnData.GetPostRollsTVFResults[i].DieRollValue;
                turnData[postRollsTurnData.TurnIndex][postRollsTurnData.CharacterIndex].TurnPlayerData[i].Logs = JsonSerializer.Deserialize<List<string>>(postRollsTurnData.GetPostRollsTVFResults[i].Logs);
            }
        }

        List<Global.AnalyzerDataModel.SpaceDataModel> spaceData = new List<Global.AnalyzerDataModel.SpaceDataModel>();

        foreach (Global.IndexDataModel.SpaceIndexDataModel currentSpaceIndexData in indexData.SpaceIndexData)
        {
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

        return new Global.AnalyzerDataModel
        {
            GameSettingsData = new Global.AnalyzerDataModel.GameSettingsDataModel
            {
                BoardData = new Global.AnalyzerDataModel.GameSettingsDataModel.BoardDataModel
                {
                    ReadyCashStart = boardCharacteristicResult.ReadyCashStart,
                    SalaryStart = boardCharacteristicResult.SalaryStart,
                    SalaryIncrease = boardCharacteristicResult.SalaryIncrease,
                    MaxDieRoll = boardCharacteristicResult.MaxDieRoll
                },
                TurnData = turnData
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
    }
}
