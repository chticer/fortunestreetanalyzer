﻿using fortunestreetanalyzer.DatabaseModels.fortunestreet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
                            RuleID = rules.FirstOrDefault().ID,
                            BoardID = boards.FirstOrDefault().ID,
                            ColorID = miiColors.FirstOrDefault().ID
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

                response.AlertData = new Global.Response.Alert
                {
                    Type = "alert-success",
                    Title = "Saved data...",
                    Descriptions = new List<string>()
                };
            }
            catch
            {
                response.AlertData = new Global.Response.Alert
                {
                    Type = "alert-warning",
                    Title = "Cannot save data...",
                    Descriptions = new List<string>()
                };
            }

            return new JsonResult(response);
        }
    }
}
