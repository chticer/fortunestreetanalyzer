using fortunestreetanalyzer.DatabaseModels.fortunestreet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace fortunestreetanalyzer.Pages;

public class IndexModel : PageModel
{
    private readonly FortuneStreetAppContext _fortuneStreetAppContext;

    public class CreateAnalyzerDataModel
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public IndexModel(FortuneStreetAppContext fortuneStreetAppContext)
    {
        _fortuneStreetAppContext = fortuneStreetAppContext;
    }

    public JsonResult OnGetStartup()
    {
        Global.Response response = new Global.Response();

        try
        {
            List<Global.LoadAnalyzerInstance> loadAnalyzerInstances = Global.LoadAnalyzerInstanceDataResponse(null, new List<string> { "train", "predict" }, _fortuneStreetAppContext);

            if (loadAnalyzerInstances == null)
                throw new Exception("Cannot connect to the database.");

            response.HTMLResponse = JsonSerializer.Serialize(new
            {
                Data = new
                {
                    Train = loadAnalyzerInstances.Where(analyzertraininstance => string.Equals(analyzertraininstance.Type, "train")).Select(analyzertraininstance => analyzertraininstance.Data),
                    Predict = loadAnalyzerInstances.Where(analyzertraininstance => string.Equals(analyzertraininstance.Type, "predict")).Select(analyzertraininstance => analyzertraininstance.Data)
                },
                Response = new
                {
                    Train =
                    (
                        loadAnalyzerInstances.Count > 0
                        ?
                        "<div>" +
                            "<div>Select an analyzer instance then click the \"Load\" button below to continue a previous analyzer instance.</div>" +

                            "<div id=\"train-previous-analyzer-instances\">" +
                                "<div>" +
                                    "<div>" +
                                        (loadAnalyzerInstances.Count > 1 ? "<span class=\"fas fa-arrow-left\"></span>" : "") +
                                    "</div>" +

                                    "<div>1 of " + loadAnalyzerInstances.Count.ToString("N0") + "</div>" +

                                    "<div>" +
                                        (loadAnalyzerInstances.Count > 1 ? "<span class=\"fas fa-arrow-right\"></span>" : "") +
                                    "</div>" +
                                "</div>" +

                                "<div></div>" +
                            "</div>" +
                        "</div>"
                        :
                        ""
                    ) +

                    "<div>" +
                        "<div>Enter a name for the analyzer instance (optional) then click the \"Create\" button below to start a new analyzer instance.</div>" +

                        "<div id=\"train-create-analyzer-instance\" class=\"create-analyzer-instance\">" +
		                        "<div>" +
			                        "<input type=\"text\" class=\"form-control form-control-lg\" name=\"train-create-analyzer-instance-name\" placeholder=\"Enter a name.\" />" +
		                        "</div>" +

		                        "<div>" +
			                        "<button type=\"button\" class=\"btn btn-primary btn-lg\" name=\"train-create-analyzer-instance-create\">Create</button>" +
		                        "</div>" +
                        "</div>" +
                    "</div>"
                }
            });

            return new JsonResult(response);
        }
        catch (Exception e)
        {
            return Global.ServerErrorResponse(e);
        }
    }

    public JsonResult OnPostCreateAnalyzerInstance([FromBody] CreateAnalyzerDataModel createAnalyzerDataParameter)
    {
        Global.Response response = new Global.Response();

        try
        {
            long? analyzerInstanceID = Global.CreateAnalyzerInstanceID(createAnalyzerDataParameter.type, !string.IsNullOrWhiteSpace(createAnalyzerDataParameter.name) ? createAnalyzerDataParameter.name : null, _fortuneStreetAppContext);

            if (analyzerInstanceID == null)
            {
                response.AlertData = new Global.Response.Alert
                {
                    Type = "alert-danger",
                    Descriptions = new List<string>
                    {
                        "Unable to create an analyzer instance ID."
                    }
                };
                response.Error = true;

                return new JsonResult(response);
            }

            response.AlertData = new Global.Response.Alert
            {
                Type = "alert-success",
                Descriptions = new List<string>
                {
                    "<strong>Redirecting...</strong>",
                    "{redirect-placeholder} if you have not been redirected."
                }
            };

            response.HTMLResponse = JsonSerializer.Serialize(new
            {
                Data = new Global.AnalyzerDataModel
                {
                    AnalyzerInstanceID = (long) analyzerInstanceID
                }
            });

            return new JsonResult(response);
        }
        catch (Exception e)
        {
            return Global.ServerErrorResponse(e);
        }
    }
}
