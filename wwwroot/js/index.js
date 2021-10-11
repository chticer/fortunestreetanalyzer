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

            let alertMessage =  "<div class=\"alert " + alert["Type"] + " save\">" +
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

            if (!response["Error"])
            {
                let JSONResponse = JSON.parse(response["HTMLResponse"]);

                analyzerData["AnalyzerInstanceID"] = JSONResponse["Data"]["AnalyzerInstanceID"];
            }
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

    function loadGameData()
    {
        $("#settings-content").append(loadingDisplay());

        ajaxCall
        (
            "GET",
            "LoadGameData",
            {}
        ).done(function (response)
        {
            $("#settings-content > .loading").remove();

            alertNotificationMessageDisplay(response["AlertData"]);

            if (!response["Error"])
            {
                let JSONResponse = JSON.parse(response["HTMLResponse"]);

                analyzerData = JSONResponse["Data"];

                $("#settings-content").append(JSONResponse["Response"]);

                $("#game-selection > div:first-of-type select").on("change", function ()
                {
                    analyzerData["GameData"]["RuleData"]["ID"] = Number($(this).val());
                });

                $("#game-selection > div:nth-of-type(2) select").on("change", function ()
                {
                    analyzerData["GameData"]["BoardData"]["ID"] = Number($(this).val());
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
                        if (analyzerData["GameData"]["ColorData"]["ID"] !== colorSelectionInputData["ColorID"])
                        {
                            analyzerData["GameData"]["ColorData"]["ID"] = colorSelectionInputData["ColorID"];

                            currentColorContainer.parent().find(".selected").removeClass("selected");

                            $(this).addClass("selected");
                        }
                    });
                });

                $("#game-selection > div:last-of-type button[name=\"confirm\"]").on("click", function ()
                {
                    $(this).closest(".confirmation-actions").remove();

                    let gameSelectionRulesSelectContainer = $("#game-selection > div:first-of-type select");

                    gameSelectionRulesSelectContainer.addClass("disabled");
                    gameSelectionRulesSelectContainer.prop("disabled", true);

                    let gameSelectionBoardsSelectContainer = $("#game-selection > div:nth-of-type(2) select");

                    gameSelectionBoardsSelectContainer.addClass("disabled");
                    gameSelectionBoardsSelectContainer.prop("disabled", true);

                    let gameSelectionColorSelectionContainer = $("#game-selection > div:nth-of-type(3) > .color-selection");

                    gameSelectionColorSelectionContainer.addClass("disabled");
                    gameSelectionColorSelectionContainer.children().children().off("click");

                    $("#settings-content").append(loadingDisplay());

                    ajaxCall
                    (
                        "POST",
                        "SaveGameData",
                        analyzerData["GameData"]
                    ).done(function (response)
                    {
                        $("#settings-content > .loading").remove();

                        alertNotificationMessageDisplay(response["AlertData"]);

                        if (!response["Error"])
                        {
                            let JSONResponse = JSON.parse(response["HTMLResponse"]);

                            analyzerData["AnalyzerInstanceID"] = JSONResponse["Data"]["AnalyzerInstanceID"];
                            analyzerData["GameData"] = JSONResponse["Data"]["GameData"];

                            saveAnalyzerData([ "GameData" ]);

                            initializeDetermineCharacterSettings();
                        }
                    });
                });
            }
        });
    }

    function initializeDetermineCharacterSettings()
    {
        $("#settings-content").append(loadingDisplay());

        alertScroll();

        ajaxCall
        (
            "POST",
            "LoadCharacters",
            {
                ID: analyzerData["GameData"]["BoardData"]["ID"]
            }
        ).done(function (response)
        {
            $("#settings-content > .loading").remove();

            alertNotificationMessageDisplay(response["AlertData"]);

            if (!response["Error"])
            {
                let JSONResponse = JSON.parse(response["HTMLResponse"]);

                analyzerData["CharacterData"] = JSONResponse["Data"]["CharacterData"];

                $("#settings-content").append(JSONResponse["Response"]);

                let playerTurnDeterminationConfirmButton = $("#player-turn-determination > div:last-of-type button[name=\"confirm\"]");

                let playerTurnDeterminationValues =
                [
                    [ null, null ],
                    [ null, null ],
                    [ null, null ],
                    [ null, null ]
                ];

                let playerTurnDeterminationSumValues = [];

                let playerTurnDeterminationConfirmButtonFlag = false;

                $("#player-turn-determination > div:first-of-type > div > div:last-of-type button").on("click", function ()
                {
                    let playerTurnDeterminationColumnValue = Number($(this).val());

                    let playerTurnDeterminationColumnContainer = $(this).parent().parent();

                    let playerTurnDeterminationCharacterIndex = playerTurnDeterminationColumnContainer.parent().parent().index();
                    let playerTurnDeterminationColumnIndex = playerTurnDeterminationColumnContainer.index();

                    if (playerTurnDeterminationValues[playerTurnDeterminationCharacterIndex][playerTurnDeterminationColumnIndex] === null || playerTurnDeterminationValues[playerTurnDeterminationCharacterIndex][playerTurnDeterminationColumnIndex] !== playerTurnDeterminationColumnValue)
                    {
                        playerTurnDeterminationValues[playerTurnDeterminationCharacterIndex][playerTurnDeterminationColumnIndex] = playerTurnDeterminationColumnValue;

                        playerTurnDeterminationColumnContainer.find(".active").removeClass("active");

                        $(this).addClass("active");

                        playerTurnDeterminationSumValues = $.map(playerTurnDeterminationValues, function (value)
                        {
                            let sum = 0;

                            for (let currentValue of value)
                            {
                                if (currentValue === null)
                                {
                                    sum = null;

                                    break;
                                }

                                sum += currentValue;
                            }

                            return [ sum ];
                        });

                        for (let i = 0; i < playerTurnDeterminationValues.length; ++i)
                        {
                            let playerTurnDeterminationCharacterContainer = $("#player-turn-determination > div:first-of-type > div:nth-of-type(" + (i + 1) + ")");

                            playerTurnDeterminationCharacterContainer.find(".disabled").each(function ()
                            {
                                $(this).prop("disabled", false);
                                $(this).removeClass("disabled");
                            });

                            for (let j = 0; j < playerTurnDeterminationSumValues.length; ++j)
                            {
                                if (i !== j && playerTurnDeterminationSumValues[j] !== null)
                                {
                                    let currentPlayerTurnDeterminationColumnValues =
                                    [
                                        Math.floor(playerTurnDeterminationSumValues[j] / 10),
                                        playerTurnDeterminationSumValues[j] % 10
                                    ];

                                    if (playerTurnDeterminationValues[i][0] !== null && playerTurnDeterminationValues[i][0] === currentPlayerTurnDeterminationColumnValues[0] * 10)
                                        playerTurnDeterminationCharacterContainer.children().last().children().eq(1).children().eq(currentPlayerTurnDeterminationColumnValues[1]).children().first().each(function ()
                                        {
                                            $(this).prop("disabled", true);
                                            $(this).addClass("disabled");
                                        });

                                    if (playerTurnDeterminationValues[i][1] !== null && playerTurnDeterminationValues[i][1] === currentPlayerTurnDeterminationColumnValues[1])
                                        playerTurnDeterminationCharacterContainer.children().last().children().eq(0).children().eq(currentPlayerTurnDeterminationColumnValues[0]).children().first().each(function ()
                                        {
                                            $(this).prop("disabled", true);
                                            $(this).addClass("disabled");
                                        });
                                }
                            }
                        }

                        if (!playerTurnDeterminationConfirmButtonFlag)
                        {
                            for (let currentPlayerTurnDeterminationSumValue of playerTurnDeterminationSumValues)
                            {
                                if (currentPlayerTurnDeterminationSumValue === null)
                                    return;
                            }

                            playerTurnDeterminationConfirmButtonFlag = true;

                            playerTurnDeterminationConfirmButton.each(function ()
                            {
                                $(this).prop("disabled", false);
                                $(this).removeClass("disabled");
                            });
                        }
                    }
                });

                playerTurnDeterminationConfirmButton.on("click", function ()
                {
                    $(this).closest(".confirmation-actions").remove();

                    for (let i = 0; i < playerTurnDeterminationSumValues.length; ++i)
                        analyzerData["CharacterData"][i]["TurnOrderValue"] = playerTurnDeterminationSumValues[i];

                    ajaxCall
                    (
                        "POST",
                        "SaveCharacterData",
                        analyzerData
                    ).done(function (response)
                    {
                        alertNotificationMessageDisplay(response["AlertData"]);

                        if (!response["Error"])
                        {
                            let JSONResponse = JSON.parse(response["HTMLResponse"]);

                            analyzerData["CharacterData"] = JSONResponse["Data"]["CharacterData"];

                            saveAnalyzerData([ "CharacterData" ]);

                            initializeGameStartSettings();
                        }
                    });
                });
            }
        });
    }

    function initializeGameStartSettings()
    {
        let playerTurnDeterminationContainer = $("#player-turn-determination > div:first-of-type");

        playerTurnDeterminationContainer.addClass("disabled");
        playerTurnDeterminationContainer.find("button").off("click");

        ajaxCall
        (
            "POST",
            "LoadSettingsOnGameStart",
            analyzerData["GameData"]
        ).done(function (response)
        {
            alertNotificationMessageDisplay(response["AlertData"]);

            if (!response["Error"])
            {
                let JSONResponse = JSON.parse(response["HTMLResponse"]);

                analyzerData["GameData"]["RuleData"]["StandingThreshold"] = JSONResponse["GameData"]["RuleData"]["StandingThreshold"];
                analyzerData["GameData"]["RuleData"]["NetWorthThreshold"] = JSONResponse["GameData"]["RuleData"]["NetWorthThreshold"];
                analyzerData["GameData"]["BoardData"]["ReadyCashStart"] = JSONResponse["GameData"]["BoardData"]["ReadyCashStart"];
                analyzerData["GameData"]["BoardData"]["SalaryStart"] = JSONResponse["GameData"]["BoardData"]["SalaryStart"];
                analyzerData["GameData"]["BoardData"]["SalaryIncrease"] = JSONResponse["GameData"]["BoardData"]["SalaryIncrease"];
                analyzerData["GameData"]["BoardData"]["MaxDieRoll"] = JSONResponse["GameData"]["BoardData"]["MaxDieRoll"];
                analyzerData["GameData"]["TurnData"] = JSONResponse["GameData"]["TurnData"];
                analyzerData["SpaceLayoutIndex"] = JSONResponse["SpaceLayoutIndex"];
                analyzerData["DistrictData"] = JSONResponse["DistrictData"];
                analyzerData["ShopData"] = JSONResponse["ShopData"];
                analyzerData["SpaceData"] = JSONResponse["SpaceData"];
                analyzerData["SpaceTypeData"] = JSONResponse["SpaceTypeData"];
            }
        });
    }

    $("#stock-districts-subpanel").hide();

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

            settingsContainer.find("button[name=\"load\"]").on("click", function ()
            {
                settingsContainer.empty().append("<div id=\"settings-content\"></div>");

                loadAnalyzerInstanceData();
            });

            settingsContainer.find("button[name=\"create\"]").on("click", function ()
            {
                settingsContainer.empty().append("<div id=\"settings-content\"></div>");

                loadGameData();
            });
        }
    });

    $(window).on("scroll", alertScroll);
});
