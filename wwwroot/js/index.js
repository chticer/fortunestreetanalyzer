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

    function alertScroll()
    {
        let alertNotificationContainer = settingsContainer.find(".alert.notification");

        if (alertNotificationContainer.length > 0)
        {
            alertNotificationContainer.removeClass("sticky");

            alertNotificationContainer.css({ top: "" });

            if (alertNotificationContainer.offset().top < $(window).scrollTop())
            {
                alertNotificationContainer.addClass("sticky");

                alertNotificationContainer.css({ top: ($(window).scrollTop() - alertNotificationContainer.parent().offset().top) + "px" });
            }
        }

        let alertSaveContainer = settingsContainer.find(".alert.save");

        if (alertSaveContainer.length > 0)
        {
            alertSaveContainer.removeClass("sticky");

            alertSaveContainer.css({ top: "" });

            if (alertSaveContainer.offset().top + alertSaveContainer.innerHeight() > $(window).scrollTop() + $(window).innerHeight() - alertSaveContainer.innerHeight())
            {
                alertSaveContainer.addClass("sticky");

                alertSaveContainer.css({ top: ($(window).scrollTop() + $(window).innerHeight() - alertSaveContainer.innerHeight() - alertSaveContainer.parent().offset().top) + "px" });
            }
        }
    }

    function alertNotificationMessageDisplay(alert)
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
                settingsContainer.find(".alert.notification").remove();

                let alertMessage =  "<div class=\"alert alert-dismissible " + alert["Type"] + " notification\">" +
                                        "<button type=\"button\" class=\"btn-close\" data-bs-dismiss=\"alert\"></button>" +

                                        "<div>" +
                                            "<strong>" + alert["Title"] + "</strong>" +
                                        "</div>" +

                                        "<div>" + descriptions.join("") + "</div>" +
                                    "</div>";

                settingsContainer.prepend(alertMessage);

                alertScroll();
            }
        }
    }

    function alertSaveMessageDisplay(alert)
    {
        if (alert !== null)
        {
            settingsContainer.find(".alert.save").remove();

            let alertMessage =  "<div class=\"alert alert-dismissible " + alert["Type"] + " save\">" +
                                    "<div>" +
                                        "<strong>" + alert["Title"] + "</strong>" +
                                    "</div>" +
                                "</div>";

            settingsContainer.append(alertMessage);

            alertScroll();

            let alertSaveContainer = settingsContainer.find(".alert.save");

            setTimeout(function ()
            {
                alertSaveContainer.animate({ opacity: 0 }, 3000, function ()
                {
                    alertSaveContainer.remove();
                });
            }, 1000);
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
        alertSaveMessageDisplay
        (
            {
                Type: "alert-primary",
                Title: "Saving to database..."
            }
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
            alertSaveMessageDisplay(response["AlertData"]);
        }).fail(function ()
        {
            alertSaveMessageDisplay
            (
                {
                    Type: "alert-warning",
                    Title: "Cannot save data..."
                }
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

            alertNotificationMessageDisplay(response["AlertData"]);

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

        alertNotificationMessageDisplay(response["AlertData"]);

        if (!response["Error"])
        {
            settingsContainer.append(response["HTMLResponse"]);

            settingsContainer.find("button[name=\"load\"]").on("click", loadAnalyzerInstanceData);

            settingsContainer.find("button[name=\"create\"]").on("click", loadGameSelection);
        }
    });

    $(window).on("scroll", alertScroll);
});
