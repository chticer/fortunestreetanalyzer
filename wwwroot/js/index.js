$(document).ready(function ()
{
    const applicationURI = $("input[name=\"__ApplicationURI\"]").val();

    $("input[name=\"__ApplicationURI\"]").remove();

    let analyzerData = {};

    const settingsContainer = $("#settings-panel > div:last-of-type");

    function ajaxCall(type, action, data)
    {
        return $.ajax
        (
            {
                type: type,
                url: applicationURI + "/Index?handler=" + action,
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8"
            }
        );
    }

    function alertDisplay(alert)
    {
        if (alert !== null)
        {
            let descriptions = $.map(alert["Descriptions"], function (value)
            {
                if (value !== null)
                    return "<div>" + value + "</div>";
            });

            if (descriptions.length > 0)
            {
                settingsContainer.find(".alert").remove();

                settingsContainer.prepend
                (
                    "<div class=\"alert alert-dismissible " + alert["Type"] + "\">" +
                        "<button type=\"button\" class=\"btn-close\" data-bs-dismiss=\"alert\"></button>" +

                        "<div>" +
                            "<strong>" + alert["Title"] + "</strong>" +
                        "</div>" +

                        "<div>" + descriptions.join("") + "</div>" +
                    "</div>"
                );
            }
        }
    }

    function loadingDisplay()
    {
        return  "<div class=\"loading\">" +
                    "<div>" +
                        "<div>" +
                            "<span class=\"fas fa-spinner fa-pulse fa-fw\"></span>" +
                        "</div>" +

                        "<div>" +
                            "<h4>Loading...</h4>" +
                        "</div>" +
                    "</div>" +

                    "<div>Please wait a few moments.</div>" +
                "</div>";
    }

    function saveAnalyzerData(keys)
    {
        ajaxCall
        (
            "POST",
            "SaveAnalyzerData",
            {
                keys: keys,
                analyzerData: analyzerData
            }
        );

        // TODO
        // Call alertDisplay with "Saving to database..." alert at bottom.
    }

    function initializeDetermineCharacterSettings()
    {
        let gameSelectionRulesSelectContainer = $("#game-selection > div:first-of-type select");

        gameSelectionRulesSelectContainer.addClass("disabled");
        gameSelectionRulesSelectContainer.prop("disabled", true);

        let gameSelectionBoardsSelectContainer = $("#game-selection > div:nth-of-type(2) select");

        gameSelectionBoardsSelectContainer.addClass("disabled");
        gameSelectionBoardsSelectContainer.prop("disabled", true);

        let gameSelectionColorSelectionContainer = $("#game-selection > div:nth-of-type(3) > .color-selection");

        gameSelectionColorSelectionContainer.addClass("disabled");
        gameSelectionColorSelectionContainer.children().children().off("click");
    }

    settingsContainer.append(loadingDisplay());

    ajaxCall
    (
        "GET",
        "LoadGameSelection",
        {}
    ).done(function (response)
    {
        settingsContainer.empty();

        alertDisplay(response["AlertData"]);

        if (!response["Error"])
        {
            let JSONResponse = JSON.parse(response["HTMLResponse"]);

            analyzerData = JSONResponse["Data"];

            settingsContainer.append(JSONResponse["Response"]);

            $("#game-selection > div:first-of-type select").on("change", function ()
            {
                analyzerData["GameSelection"]["RuleID"] = Number($(this).val());
            });

            $("#game-selection > div:nth-of-type(2) select").on("change", function ()
            {
                analyzerData["GameSelection"]["BoardID"] = Number($(this).val());
            });

            $("#game-selection > div:nth-of-type(3) > .color-selection input[type=\"hidden\"]").each(function ()
            {
                let colorSelectionInputContainer = $(this);

                let currentColorContainer = colorSelectionInputContainer.parent();

                let colorSelectionInputData =
                {
                    ColorID: Number(colorSelectionInputContainer.data("colorid"))
                };

                colorSelectionInputContainer.remove();

                currentColorContainer.children().first().on("click", function ()
                {
                    if (analyzerData["GameSelection"]["ColorID"] !== colorSelectionInputData["ColorID"])
                    {
                        analyzerData["GameSelection"]["ColorID"] = colorSelectionInputData["ColorID"];

                        currentColorContainer.parent().find(".selected").removeClass("selected");

                        $(this).addClass("selected");
                    }
                });
            });

            $("#game-selection > div:last-of-type button[name=\"confirm\"]").on("click", function ()
            {
                $(this).closest(".confirmation-actions").remove();

                saveAnalyzerData([ "GameSelection" ]);

                initializeDetermineCharacterSettings();
            });
        }
    });
});
