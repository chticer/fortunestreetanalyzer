using fortunestreetanalyzer.DatabaseModels.fortunestreet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace fortunestreetanalyzer.Pages
{
    public class IndexModel : PageModel
    {
        private readonly FortuneStreetAppContext _fortuneStreetAppContext;

        public class SaveAnalyzerDataModel
        {
            public List<string> keys { get; set; }
            public Global.AnalyzerDataModel analyzerData { get; set; }
        }

        public IndexModel(FortuneStreetAppContext fortuneStreetAppContext)
        {
            _fortuneStreetAppContext = fortuneStreetAppContext;
        }

        public JsonResult OnGetLoadAnalyzerInstance()
        {
            Global.Response response = new Global.Response();

            try
            {
                string userIPAddress = Global.GetUserIPAddress();

                List<CurrentAnalyzerInstancesTVF> currentAnalyzerInstancesTVFResults = _fortuneStreetAppContext.CurrentAnalyzerInstancesTVF.FromSqlInterpolated($"SELECT * FROM currentanalyzerinstances_tvf() WHERE status = 'in_progress'").OrderByDescending(timestampadded => timestampadded.TimestampAdded).ToList();

                foreach (CurrentAnalyzerInstancesTVF currentAnalyzerInstancesTVFResult in currentAnalyzerInstancesTVFResults)
                {
                    if (!Global.HashComparison(userIPAddress, currentAnalyzerInstancesTVFResult.IPAddress))
                        currentAnalyzerInstancesTVFResults.Remove(currentAnalyzerInstancesTVFResult);
                }

                response.HTMLResponse =
                    "<div>" +

                        (
                            currentAnalyzerInstancesTVFResults.Count > 0
                            ?
                            "<div class=\"horizontal-items\">" +
                                "<div>" +
                                    "<select class=\"form-select\">" +

                                        string.Join("", currentAnalyzerInstancesTVFResults.Select(results => "<option value=\"" + results.AnalyzerInstanceID + "\">" + results.TimestampAdded.ToLocalTime().ToString("MMMM d, yyyy h:mm:ss tt") + "</option>")) +

                                    "</select>" +
                                "</div>" +

                                "<div>" +
                                    "<button type=\"button\" class=\"btn btn-primary\" name=\"load\">Load</button>" +
                                "</div>" +
                            "</div>"
                            :
                            "<div>No previous analyzer instances found. Start a new one by clicking the \"Create\" button below.</div>"
                        ) +

                        "<div>" +

                            Global.CreateConfirmationActions("center-items", new List<string>
                            {
                                "<button type=\"button\" class=\"btn btn-lg btn-primary\" name=\"create\">Create</button>"
                            }) +

                        "</div>" +
                    "</div>";

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                return Global.ServerErrorResponse(e);
            }
        }

        public JsonResult OnGetLoadGameSelection()
        {
            Global.Response response = new Global.Response();

            try
            {
                List<Rules> rules = _fortuneStreetAppContext.Rules.OrderBy(name => name.Name).ToList();

                List<Boards> boards = _fortuneStreetAppContext.Boards.OrderBy(name => name.Name).ToList();

                List<Colors> miiColors = _fortuneStreetAppContext.Colors.OrderBy(id => id.ID).ToList();

                response.HTMLResponse = JsonSerializer.Serialize(new
                {
                    Data = new Global.AnalyzerDataModel
                    {
                        GameSelection = new Global.AnalyzerDataModel.GameSelectionDataModel
                        {
                            RuleData = new Global.AnalyzerDataModel.GameSelectionDataModel.RuleDataModel
                            {
                                ID = rules.FirstOrDefault().ID
                            },
                            BoardData = new Global.AnalyzerDataModel.GameSelectionDataModel.BoardDataModel
                            {
                                ID = boards.FirstOrDefault().ID
                            },
                            ColorData = new Global.AnalyzerDataModel.GameSelectionDataModel.ColorDataModel
                            {
                                ID = miiColors.FirstOrDefault().ID
                            }
                        }
                    },
                    Response =
                        "<div id=\"game-selection\">" +
                            "<div>" +
                                "<div>" +
                                    "<h5>Rules</h5>" +
                                "</div>" +

                                "<div>" +
                                    "<select class=\"form-select\">" +

                                        string.Join("", rules.Select(rule => "<option value=\"" + rule.ID + "\">" + rule.Name + "</option>")) +

                                    "</select>" +
                                "</div>" +
                            "</div>" +

                            "<div>" +
                                "<div>" +
                                    "<h5>Boards</h5>" +
                                "</div>" +

                                "<div>" +
                                    "<select class=\"form-select\">" +

                                        string.Join("", boards.Select(board => "<option value=\"" + board.ID + "\">" + board.Name + "</option>")) +

                                    "</select>" +
                                "</div>" +
                            "</div>" +

                            "<div>" +
                                "<div>" +
                                    "<h5>Mii Color</h5>" +
                                "</div>" +

                                "<div class=\"color-selection center-items\">" +

                                    string.Join("", miiColors.Select
                                    (
                                        (color, index) =>
                                            "<div>" +
                                                "<input type=\"hidden\" data-colorid=\"" + color.ID + "\" />" +

                                                "<div style=\"background-color: #" + color.MiiColor + "\"" + (index == 0 ? " class=\"selected\"" : "") + "></div>" +
                                            "</div>"
                                    )) +

                                "</div>" +
                            "</div>" +

                            Global.CreateConfirmationActions("center-items", new List<string>
                            {
                                "<button type=\"button\" class=\"btn btn-lg btn-primary\" name=\"confirm\">Confirm</button>"
                            }) +

                        "</div>"
                });

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                return Global.ServerErrorResponse(e);
            }
        }

        public JsonResult OnPostSaveGameSelection([FromBody] Global.AnalyzerDataModel.GameSelectionDataModel saveGameSelectionParameters)
        {
            Global.Response response = new Global.Response();

            try
            {
                Rules rulesResult = _fortuneStreetAppContext.Rules.SingleOrDefault(id => id.ID == saveGameSelectionParameters.RuleData.ID);

                if (rulesResult == null)
                {
                    response.AlertData = new Global.Response.Alert
                    {
                        Type = "alert-error",
                        Title = "Invalid rule selection."
                    };
                    response.Error = true;

                    return new JsonResult(response);
                }

                Boards boardsResult = _fortuneStreetAppContext.Boards.SingleOrDefault(id => id.ID == saveGameSelectionParameters.BoardData.ID);

                if (boardsResult == null)
                {
                    response.AlertData = new Global.Response.Alert
                    {
                        Type = "alert-error",
                        Title = "Invalid board selection."
                    };
                    response.Error = true;

                    return new JsonResult(response);
                }

                Colors colorsResult = _fortuneStreetAppContext.Colors.SingleOrDefault(id => id.ID == saveGameSelectionParameters.ColorData.ID);

                if (colorsResult == null)
                {
                    response.AlertData = new Global.Response.Alert
                    {
                        Type = "alert-error",
                        Title = "Invalid mii color selection."
                    };
                    response.Error = true;

                    return new JsonResult(response);
                }

                long? analyzerInstanceID = Global.CreateAnalyzerInstanceID(_fortuneStreetAppContext);

                if (analyzerInstanceID == null)
                {
                    response.AlertData = new Global.Response.Alert
                    {
                        Type = "alert-error",
                        Title = "Unable to create an analyzer instance ID."
                    };
                    response.Error = true;

                    return new JsonResult(response);
                }

                _fortuneStreetAppContext.GameSettings.Add(new GameSettings
                {
                    AnalyzerInstanceID = (long) analyzerInstanceID,
                    RuleID = rulesResult.ID,
                    BoardID = boardsResult.ID,
                    MiiColorID = colorsResult.ID
                });

                _fortuneStreetAppContext.SaveChanges();

                response.HTMLResponse = JsonSerializer.Serialize(new
                {
                    Data = new Global.AnalyzerDataModel
                    {
                        AnalyzerInstanceID = (long) analyzerInstanceID,
                        GameSelection = new Global.AnalyzerDataModel.GameSelectionDataModel
                        {
                            RuleData = new Global.AnalyzerDataModel.GameSelectionDataModel.RuleDataModel
                            {
                                ID = rulesResult.ID,
                                Name = rulesResult.Name
                            },
                            BoardData = new Global.AnalyzerDataModel.GameSelectionDataModel.BoardDataModel
                            {
                                ID = boardsResult.ID,
                                Name = boardsResult.Name
                            },
                            ColorData = new Global.AnalyzerDataModel.GameSelectionDataModel.ColorDataModel
                            {
                                ID = colorsResult.ID,
                                MiiColor = colorsResult.MiiColor,
                                GameColor = colorsResult.GameColor
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
                List<GetBoardCharactersTVF> getBoardCharactersTVFResults = _fortuneStreetAppContext.GetBoardCharactersTVF.FromSqlInterpolated($"SELECT * FROM getboardcharacters_tvf({loadCharactersParameter.ID})").ToList();

                List<Global.AnalyzerDataModel.CharacterDataModel> characters = new List<Global.AnalyzerDataModel.CharacterDataModel>
                {
                    new Global.AnalyzerDataModel.CharacterDataModel
                    {
                        Name = "You"
                    }
                }.Union(getBoardCharactersTVFResults.Select(result => new Global.AnalyzerDataModel.CharacterDataModel
                {
                    ID = result.CharacterID,
                    SpriteURL = Global.AZURE_STORAGE_URL + result.CharacterSpriteURL,
                    Name = result.Name
                })).ToList();

                response.HTMLResponse = JsonSerializer.Serialize(new
                {
                    Data = new Global.AnalyzerDataModel
                    {
                        CharacterData = characters
                    },
                    Response =
                        "<div id=\"player-turn-determination\">" +
                            "<div>" +

                                string.Join("", characters.Select
                                (
                                    character =>
                                        "<div>" +
                                            "<div></div>" +

                                            "<div>" + character.Name + "</div>" +

                                            "<div>" +
                                                "<div>" +

                                                    string.Join("", Enumerable.Range(0, 10).Select
                                                    (
                                                        digit =>
                                                            "<span>" +
                                                                "<button type=\"button\" class=\"btn btn-outline-primary btn-lg\" value=\"" + (digit * 10) + "\">" + digit + "</button>" +
                                                            "</span>"
                                                    )) +

                                                "</div>" +

                                                "<div>" +

                                                    string.Join("", Enumerable.Range(0, 10).Select
                                                    (
                                                        digit =>
                                                            "<span>" +
                                                                "<button type=\"button\" class=\"btn btn-outline-primary btn-lg\" value=\"" + digit + "\">" + digit + "</button>" +
                                                            "</span>"
                                                    )) +

                                                "</div>" +
                                            "</div>" +
                                        "</div>"
                                )) +

                            "</div>" +

                            Global.CreateConfirmationActions("center-items", new List<string>
                            {
                                "<button type=\"button\" class=\"btn btn-lg btn-primary disabled\" name=\"confirm\" disabled=\"disabled\">Confirm</button>"
                            }) +

                        "</div>"
                });

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                return Global.ServerErrorResponse(e);
            }
        }

        public JsonResult OnPostSaveCharacterData([FromBody] Global.AnalyzerDataModel loadCharacterColorsParameter)
        {
            Global.Response response = new Global.Response();

            try
            {
                _fortuneStreetAppContext.TurnOrderDetermination.AddRange(loadCharacterColorsParameter.CharacterData.Select(character => new TurnOrderDetermination
                {
                    AnalyzerInstanceID = loadCharacterColorsParameter.AnalyzerInstanceID,
                    CharacterID = character.ID,
                    Value = character.TurnOrderValue
                }));

                _fortuneStreetAppContext.SaveChanges();

                List<GetCharacterColorsTVF> getCharacterColorsTVFResults = _fortuneStreetAppContext.GetCharacterColorsTVF.FromSqlInterpolated($"SELECT * FROM getcharactercolors_tvf({loadCharacterColorsParameter.AnalyzerInstanceID})").ToList();

                List<Global.AnalyzerDataModel.CharacterDataModel> characters = getCharacterColorsTVFResults.Select(result => new Global.AnalyzerDataModel.CharacterDataModel
                {
                    ID = result.CharacterID,
                    TurnOrderValue = result.Value,
                    ColorData = new Global.AnalyzerDataModel.CharacterDataModel.ColorDataModel
                    {
                        ID = result.ColorIDAssigned
                    }
                }).ToList();

                List<GetBoardCharactersTVF> getBoardCharactersTVFResults = _fortuneStreetAppContext.GetBoardCharactersTVF.FromSqlInterpolated($"SELECT * FROM getboardcharacters_tvf({loadCharacterColorsParameter.GameSelection.BoardData.ID})").ToList();

                List<long> characterColorIDs = characters.Select(character => character.ColorData.ID).ToList();

                List<Colors> colors = _fortuneStreetAppContext.Colors.Where(color => characterColorIDs.Contains(color.ID)).ToList();

                foreach (Global.AnalyzerDataModel.CharacterDataModel currentCharacter in characters)
                {
                    GetBoardCharactersTVF currentGetBoardCharactersTVFResult = getBoardCharactersTVFResults.SingleOrDefault(result => result.CharacterID == currentCharacter.ID);

                    if (currentGetBoardCharactersTVFResult != null)
                    {
                        currentCharacter.SpriteURL = Global.AZURE_STORAGE_URL + currentGetBoardCharactersTVFResult.CharacterSpriteURL;
                        currentCharacter.Name = currentGetBoardCharactersTVFResult.Name;
                    }

                    Colors currentColor = colors.SingleOrDefault(color => color.ID == currentCharacter.ColorData.ID);

                    currentCharacter.ColorData.GameColor = currentColor.GameColor;
                }

                response.HTMLResponse = JsonSerializer.Serialize(new
                {
                    Data = new Global.AnalyzerDataModel
                    {
                        CharacterData = characters.Select(character => new Global.AnalyzerDataModel.CharacterDataModel
                        {
                            ID = character.ID,
                            SpriteURL = character.SpriteURL,
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

        public JsonResult OnPostSaveAnalyzerData([FromBody] SaveAnalyzerDataModel saveAnalyzerParameter)
        {
            Global.Response response = new Global.Response();

            try
            {
                long? analyzerInstanceID = Global.VerifyAnalyzerInstanceID(saveAnalyzerParameter.analyzerData.AnalyzerInstanceID, _fortuneStreetAppContext);

                if (analyzerInstanceID == null)
                    throw new Exception();

                List<PropertyInfo> analyzerDataModelProperties = typeof(Global.AnalyzerDataModel).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();

                foreach (PropertyInfo currentAnalyzerDataModelProperty in analyzerDataModelProperties)
                {
                    if (saveAnalyzerParameter.keys.Contains(currentAnalyzerDataModelProperty.Name))
                        _fortuneStreetAppContext.AnalyzerInstanceLogs.Add(new AnalyzerInstanceLogs
                        {
                            AnalyzerInstanceID = (long) analyzerInstanceID,
                            Key = currentAnalyzerDataModelProperty.Name,
                            Value = JsonSerializer.Serialize(currentAnalyzerDataModelProperty.GetValue(saveAnalyzerParameter.analyzerData, null))
                        });
                }

                _fortuneStreetAppContext.SaveChanges();

                response.HTMLResponse = JsonSerializer.Serialize(new
                {
                    Data = new Global.AnalyzerDataModel
                    {
                        AnalyzerInstanceID = (long) analyzerInstanceID
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
