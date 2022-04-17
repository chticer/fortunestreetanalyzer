using fortunestreetanalyzer.DatabaseModels.fortunestreet;
using Microsoft.EntityFrameworkCore;

namespace fortunestreetanalyzer
{
    public class RebuildAnalyzerInstanceHelper
    {
        public static Global.RebuildAnalyzerInstance LoadGameData(Global.AnalyzerDataModel.GameDataModel previousGameData, FortuneStreetAppContext fortuneStreetAppContext)
        {
            List<Rules> rules = fortuneStreetAppContext.Rules.OrderBy(name => name.Name).ToList();

            List<Boards> boards = fortuneStreetAppContext.Boards.OrderBy(name => name.Name).ToList();

            List<Colors> miiColors = fortuneStreetAppContext.Colors.OrderBy(id => id.ID).ToList();

            Global.RebuildAnalyzerInstance rebuildAnalyzerInstance = new Global.RebuildAnalyzerInstance
            {
                Data = new Global.AnalyzerDataModel
                {
                    GameData =
                    previousGameData != null
                    ?
                    previousGameData
                    :
                    new Global.AnalyzerDataModel.GameDataModel
                    {
                        RuleData = new Global.AnalyzerDataModel.GameDataModel.RuleDataModel
                        {
                            ID = rules.FirstOrDefault().ID
                        },
                        BoardData = new Global.AnalyzerDataModel.GameDataModel.BoardDataModel
                        {
                            ID = boards.FirstOrDefault().ID
                        },
                        ColorData = new Global.AnalyzerDataModel.GameDataModel.ColorDataModel
                        {
                            ID = miiColors.FirstOrDefault().ID
                        }
                    }
                },

            };

            rebuildAnalyzerInstance.Response =
                "<div id=\"game-selection\">" +
                    "<div>" +
                        "<div>" +
                            "<h5>Rules</h5>" +
                        "</div>" +

                        "<div>" +
                            "<select class=\"form-select" + (previousGameData != null ? " disabled" : "") + "\"" + (previousGameData != null ? " disabled=\"disabled\"" : "") + ">" +

                                string.Join("", rules.Select(rule => "<option value=\"" + rule.ID + "\"" + (rebuildAnalyzerInstance.Data.GameData.RuleData.ID == rule.ID ? " selected=\"selected\"" : "") + ">" + rule.Name + "</option>")) +

                            "</select>" +
                        "</div>" +
                    "</div>" +

                    "<div>" +
                        "<div>" +
                            "<h5>Boards</h5>" +
                        "</div>" +

                        "<div>" +
                            "<select class=\"form-select" + (previousGameData != null ? " disabled" : "") + "\"" + (previousGameData != null ? " disabled=\"disabled\"" : "") + ">" +

                                string.Join("", boards.Select(board => "<option value=\"" + board.ID + "\"" + (rebuildAnalyzerInstance.Data.GameData.BoardData.ID == board.ID ? " selected=\"selected\"" : "") + ">" + board.Name + "</option>")) +

                            "</select>" +
                        "</div>" +
                    "</div>" +

                    "<div>" +
                        "<div>" +
                            "<h5>Mii Color</h5>" +
                        "</div>" +

                        "<div class=\"color-selection" + (previousGameData != null ? " disabled" : "") + "\">" +

                            string.Join("", miiColors.Select
                            (
                                color =>
                                    "<div>" +
                                        "<input type=\"hidden\" data-colorid=\"" + color.ID + "\" />" +

                                        "<div style=\"background-color: #" + color.SystemColor + ";\"" + (rebuildAnalyzerInstance.Data.GameData.ColorData.ID == color.ID ? " class=\"selected\"" : "") + "></div>" +
                                    "</div>"
                            )) +

                        "</div>" +
                    "</div>" +

                    (
                        previousGameData == null
                        ?
                        Global.CreateConfirmationActions("center-items", new List<string>
                        {
                            "<button type=\"button\" class=\"btn btn-lg btn-primary\" name=\"confirm\">Confirm</button>"
                        })
                        :
                        ""
                    ) +

                "</div>";

            return rebuildAnalyzerInstance;
        }

        public static Global.RebuildAnalyzerInstance LoadCharacters(Global.AnalyzerDataModel previousAnalyzerData, FortuneStreetAppContext fortuneStreetAppContext)
        {
            List<GetBoardCharactersTVF> getBoardCharactersTVFResults = fortuneStreetAppContext.GetBoardCharactersTVF.FromSqlInterpolated($"SELECT * FROM getboardcharacters_tvf({previousAnalyzerData.GameData.BoardData.ID})").ToList();

            List<Global.AnalyzerDataModel.CharacterDataModel> characters = new List<Global.AnalyzerDataModel.CharacterDataModel>
            {
	            new Global.AnalyzerDataModel.CharacterDataModel
	            {
		            Name = "You"
	            }
            }.Union(getBoardCharactersTVFResults.Select(result => new Global.AnalyzerDataModel.CharacterDataModel
            {
	            ID = result.CharacterID,
	            PortraitURL = Global.AZURE_STORAGE_URL + result.CharacterPortraitURI,
	            Name = result.Name
            })).ToList();

            Global.RebuildAnalyzerInstance rebuildAnalyzerInstance = new Global.RebuildAnalyzerInstance
            {
                Data = new Global.AnalyzerDataModel
                {
                    CharacterData = characters
                }
            };

            rebuildAnalyzerInstance.Response =
                "<div id=\"player-turn-determination\"" + (previousAnalyzerData.CharacterData != null ? " class=\"disabled\"" : "") + ">" +
			        "<div>" +

				        string.Join("", characters.Select
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
												        "<button type=\"button\" class=\"btn btn-outline-primary btn-lg" + (previousAnalyzerData.CharacterData != null && previousAnalyzerData.CharacterData.SingleOrDefault(id => id.ID == character.ID).TurnOrderValue / 10 == digit ? " active" : "") + "\" value=\"" + (digit * 10) + "\">" + digit + "</button>" +
											        "</span>"
									        )) +

								        "</div>" +

								        "<div>" +

									        string.Join("", Enumerable.Range(0, 10).Select
									        (
										        digit =>
											        "<span>" +
												        "<button type=\"button\" class=\"btn btn-outline-primary btn-lg" + (previousAnalyzerData.CharacterData != null && previousAnalyzerData.CharacterData.SingleOrDefault(id => id.ID == character.ID).TurnOrderValue % 10 == digit ? " active" : "") + "\" value=\"" + digit + "\">" + digit + "</button>" +
											        "</span>"
									        )) +

								        "</div>" +
							        "</div>" +
						        "</div>"
				        )) +

			        "</div>" +

                    (
                        previousAnalyzerData.CharacterData == null
                        ?
                        Global.CreateConfirmationActions("center-items", new List<string>
                        {
                            "<button type=\"button\" class=\"btn btn-lg btn-primary\" name=\"confirm\">Confirm</button>"
                        })
                        :
                        ""
                    ) +

                "</div>";

            return rebuildAnalyzerInstance;
        }
    }
}
