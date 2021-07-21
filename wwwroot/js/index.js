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

    function alertDisplay(alert, position)
    {
        if (alert !== null)
        {
            let descriptions = $.map(alert["Descriptions"], function (value)
            {
                if (value !== null)
                    return "<div>" + value + "</div>";
            });

            settingsContainer.find(".alert." + position).remove();

            let alertMessage =  "<div class=\"alert alert-dismissible " + alert["Type"] + " " + position + "\">" +
                                    "<button type=\"button\" class=\"btn-close\" data-bs-dismiss=\"alert\"></button>" +

                                    "<div>" +
                                        "<strong>" + alert["Title"] + "</strong>" +
                                    "</div>" +

                                    "<div>" + descriptions.join("") + "</div>" +
                                "</div>";

            if (position === "bottom")
                settingsContainer.append(alertMessage);
            else
                settingsContainer.prepend(alertMessage);

            let alertContainer = settingsContainer.find(".alert." + position);

            if (position === "bottom" && alertContainer.offset().top + alertContainer.innerHeight() > $(window).scrollTop() + $(window).innerHeight() - alertContainer.innerHeight())
            {
                alertContainer.addClass("sticky");

                alertContainer.css({ top: ($(window).scrollTop() + $(window).innerHeight() - alertContainer.innerHeight() - alertContainer.parent().offset().top) + "px" });
            }
            else if (position === "top" && alertContainer.offset().top < $(window).scrollTop())
            {
                alertContainer.addClass("sticky");

                alertContainer.css({ top: ($(window).scrollTop() - alertContainer.parent().offset().top) + "px" });
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
        alertDisplay
        (
            {
                Type: "alert-primary",
                Title: "Saving to database...",
                Descriptions: []
            },
            "bottom"
        );

        ajaxCall
        (
            "POST",
            "SaveAnalyzerData",
            {
                keys: keys,
                analyzerData: analyzerData
            }
        ).done(function (response)
        {
            alertDisplay(response["AlertData"], "bottom");
        }).fail(function ()
        {
            alertDisplay
            (
                {
                    Type: "alert-warning",
                    Title: "Cannot save data...",
                    Descriptions: []
                },
                "bottom"
            );
        });
    }

    function loadAnalyzerInstanceData()
    {
    }

    function loadGameSelection()
    {
        settingsContainer.empty().append(loadingDisplay());

        ajaxCall
        (
            "GET",
            "LoadGameSelection",
            {}
        ).done(function (response)
        {
            settingsContainer.empty();

            alertDisplay(response["AlertData"], "top");

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
        "LoadAnalyzerInstance",
        {}
    ).done(function (response)
    {
        settingsContainer.empty();

        alertDisplay(response["AlertData"], "top");

        if (!response["Error"])
        {
            settingsContainer.append(response["HTMLResponse"]);

            settingsContainer.find("button[name=\"load\"]").on("click", loadAnalyzerInstanceData);

            settingsContainer.find("button[name=\"create\"]").on("click", loadGameSelection);
        }
    });

    $(window).on("scroll", function ()
    {
        let alertTopContainer = settingsContainer.find(".alert.top");

        if (alertTopContainer.length > 0)
        {
            alertTopContainer.removeClass("sticky");

            alertTopContainer.css({ top: "" });

            if (alertTopContainer.offset().top < $(window).scrollTop())
            {
                alertTopContainer.addClass("sticky");

                alertTopContainer.css({ top: ($(window).scrollTop() - alertTopContainer.parent().offset().top) + "px" });
            }
        }

        let alertBottomContainer = settingsContainer.find(".alert.bottom");

        if (alertBottomContainer.length > 0)
        {
            alertBottomContainer.removeClass("sticky");

            alertBottomContainer.css({ top: "" });

            if (alertBottomContainer.offset().top + alertBottomContainer.innerHeight() > $(window).scrollTop() + $(window).innerHeight() - alertBottomContainer.innerHeight())
            {
                alertBottomContainer.addClass("sticky");

                alertBottomContainer.css({ top: ($(window).scrollTop() + $(window).innerHeight() - alertBottomContainer.innerHeight() - alertBottomContainer.parent().offset().top) + "px" });
            }
        }
    });
});
