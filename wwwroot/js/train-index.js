const URL_QUERY_PARAMETERS = new URLSearchParams(window.location.search);
const SPACE_SQUARE_SIZE = 100;
const SUIT_DATA =
[
    {
        Icon: "spade",
        Color: "1491E3"
    },
    {
        Icon: "heart",
        Color: "FF8BE9"
    },
    {
        Icon: "diamond",
        Color: "FFF02C"
    },
    {
        Icon: "club",
        Color: "2DD936"
    }
];
const SUIT_ORDER = [ "spade", "heart", "diamond", "club" ];
const DIE_DOT_LOCATIONS_DATA =
[
    [
        {
            x: 0.5,
            y: 0.5
        }
    ],
    [
        {
            x: 0.1667,
            y: 0.1667
        },
        {
            x: 0.8333,
            y: 0.8333
        }
    ],
    [
        {
            x: 0.1667,
            y: 0.1667
        },
        {
            x: 0.5,
            y: 0.5
        },
        {
            x: 0.8333,
            y: 0.8333
        }
    ],
    [
        {
            x: 0.1667,
            y: 0.1667
        },
        {
            x: 0.8333,
            y: 0.1667
        },
        {
            x: 0.1667,
            y: 0.8333
        },
        {
            x: 0.8333,
            y: 0.8333
        }
    ],
    [
        {
            x: 0.1667,
            y: 0.1667
        },
        {
            x: 0.8333,
            y: 0.1667
        },
        {
            x: 0.5,
            y: 0.5
        },
        {
            x: 0.1667,
            y: 0.8333
        },
        {
            x: 0.8333,
            y: 0.8333
        }
    ],
    [
        {
            x: 0.1667,
            y: 0.1667
        },
        {
            x: 0.8333,
            y: 0.1667
        },
        {
            x: 0.1667,
            y: 0.5
        },
        {
            x: 0.8333,
            y: 0.5
        },
        {
            x: 0.1667,
            y: 0.8333
        },
        {
            x: 0.8333,
            y: 0.8333
        }
    ],
    [
        {
            x: 0.1667,
            y: 0.1667
        },
        {
            x: 0.8333,
            y: 0.1667
        },
        {
            x: 0.1667,
            y: 0.5
        },
        {
            x: 0.5,
            y: 0.5
        },
        {
            x: 0.8333,
            y: 0.5
        },
        {
            x: 0.1667,
            y: 0.8333
        },
        {
            x: 0.8333,
            y: 0.8333
        }
    ],
    [
        {
            x: 0.1667,
            y: 0.1667
        },
        {
            x: 0.8333,
            y: 0.1667
        },
        {
            x: 0.1667,
            y: 0.5
        },
        {
            x: 0.5,
            y: 0.3333
        },
        {
            x: 0.5,
            y: 0.6667
        },
        {
            x: 0.8333,
            y: 0.5
        },
        {
            x: 0.1667,
            y: 0.8333
        },
        {
            x: 0.8333,
            y: 0.8333
        }
    ]
];

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
                url: applicationURI + "/train/Index?handler=" + action,
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8"
            }
        );
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

            settingsContainer.find(".alert.notification").remove();

            settingsContainer.prepend
            (
                "<div class=\"alert alert-dismissible " + alert["Type"] + " notification\">" +
                    "<button type=\"button\" class=\"btn-close\" data-bs-dismiss=\"alert\"></button>" +

                    "<div>" +
                        "<strong>" + alert["Title"] + "</strong>" +
                    "</div>" +

                    "<div>" + descriptions.join("") + "</div>" +
                "</div>"
            );
        }
    }

    function alertSaveMessageDisplay(alert)
    {
        if (alert !== null)
        {
            settingsContainer.find(".alert.save").remove();

            settingsContainer.append
            (
                "<div class=\"alert " + alert["Type"] + " save\">" +
                    "<div>" +
                        "<strong>" + alert["Title"] + "</strong>" +
                    "</div>" +
                "</div>"
            );

            let alertSaveContainer = settingsContainer.find(".alert.save");

            alertSaveContainer.animate({ opacity: 0 }, 3000, function ()
            {
                alertSaveContainer.remove();
            });
        }
    }

    function createConfirmationActions(alignmentClass, buttonTags)
    {
        return  "<div class=\"confirmation-actions" + (alignmentClass !== null ? " " + alignmentClass : "") + "\">" +

                    $.map(buttonTags, function (value)
                    {
                        return "<div>" + value + "</div>";
                    }).join("") +

                "</div>";
    }

    function ordinalNumberSuffix(number)
    {
        let onesPlace = number % 10;
        let tensPlace = number % 100;

        if (onesPlace === 1 && tensPlace !== 11)
            return "st";

        if (onesPlace === 2 && tensPlace !== 12)
            return "nd";

        if (onesPlace === 3 && tensPlace !== 13)
            return "rd";

        return "th";
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

    function loadGameData()
    {
        $("#settings-content").append("<div>" + loadingDisplay() + "</div>");

        ajaxCall
        (
            "GET",
            "LoadGameData",
            {}
        ).done(function (response)
        {
            $("#settings-content").find(".loading").parent().remove();

            alertNotificationMessageDisplay(response["AlertData"]);

            if (!response["Error"])
            {
                let JSONResponse = JSON.parse(response["HTMLResponse"]);

                analyzerData["GameData"] = JSONResponse["Data"]["GameData"];

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

                    $("#settings-content").append("<div>" + loadingDisplay() + "</div>");

                    ajaxCall
                    (
                        "POST",
                        "SaveGameData",
                        analyzerData
                    ).done(function (response)
                    {
                        $("#settings-content").find(".loading").parent().remove();

                        alertNotificationMessageDisplay(response["AlertData"]);

                        if (!response["Error"])
                        {
                            let JSONResponse = JSON.parse(response["HTMLResponse"]);

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
        $("#settings-content").append("<div>" + loadingDisplay() + "</div>");

        $("#settings-panel").scrollTop($("#settings-panel").prop("scrollHeight"));

        ajaxCall
        (
            "POST",
            "LoadCharacters",
            {
                ID: analyzerData["GameData"]["BoardData"]["ID"]
            }
        ).done(function (response)
        {
            $("#settings-content").find(".loading").parent().remove();

            alertNotificationMessageDisplay(response["AlertData"]);

            if (!response["Error"])
            {
                let JSONResponse = JSON.parse(response["HTMLResponse"]);

                analyzerData["CharacterData"] = JSONResponse["Data"]["CharacterData"];

                $("#settings-content").append(JSONResponse["Response"]);

                let playerTurnDeterminationConfirmButton = $("#player-turn-determination > div:last-of-type button[name=\"confirm\"]");

                playerTurnDeterminationConfirmButton.prop("disabled", true);
                playerTurnDeterminationConfirmButton.addClass("disabled");

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

                            playerTurnDeterminationConfirmButton.prop("disabled", false);
                            playerTurnDeterminationConfirmButton.removeClass("disabled");
                        }
                    }
                });

                playerTurnDeterminationConfirmButton.on("click", function ()
                {
                    $(this).closest(".confirmation-actions").remove();

                    $("#player-turn-determination").addClass("disabled");

                    $("#player-turn-determination button").off("click");

                    for (let i = 0; i < playerTurnDeterminationSumValues.length; ++i)
                        analyzerData["CharacterData"]["PlayerData"][i]["TurnOrderValue"] = playerTurnDeterminationSumValues[i];

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

    function renderPlayerContainer(playerIndex)
    {
        return  "<div>" +
                    "<div class=\"character-portrait-icon small\">" +

                    (
                        analyzerData["CharacterData"]["PlayerData"][playerIndex]["PortraitURL"] !== null
                        ?
                        "<img src=\"" + analyzerData["CharacterData"]["PlayerData"][playerIndex]["PortraitURL"] + "\" alt=\"Character Portrait for " + analyzerData["CharacterData"]["PlayerData"][playerIndex]["Name"] + "\" />"
                        :
                        ""
                    ) +

                    "</div>" +

                    "<div>" + (analyzerData["CharacterData"]["PlayerData"][playerIndex]["Name"] !== null ? analyzerData["CharacterData"]["PlayerData"][playerIndex]["Name"] : "You") + "</div>" +
                "</div>";
    }

    function initializeGameStartSettings()
    {
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

                $.extend
                (
	                true,
	                analyzerData,
                    {
                        GameData: $.extend
                        (
                            true,
                            analyzerData["GameData"],
                            {
                                TurnData: JSONResponse["Data"]["GameData"]["TurnData"]
                            }
                        ),
		                SpaceData: JSONResponse["Data"]["SpaceData"],
		                SpaceTypeData: JSONResponse["Data"]["SpaceTypeData"],
		                ShopData: JSONResponse["Data"]["ShopData"],
		                DistrictData: JSONResponse["Data"]["DistrictData"]
	                }
                );

                saveAnalyzerData([ "GameData", "SpaceData", "SpaceTypeData", "ShopData", "DistrictData" ]);

                saveTurnBeforeRollData(null, initializeGameSetup);
            }
        });
    }

    function saveTurnBeforeRollData(characterIndices, callback)
    {
        let analyzerDataCopy = $.extend(true, {}, analyzerData);

        analyzerDataCopy["GameData"]["TurnData"] = analyzerDataCopy["GameData"]["TurnData"].slice(-1);

        if (characterIndices !== null)
        {
            analyzerDataCopy["CharacterData"]["PlayerData"] = [];
            analyzerDataCopy["GameData"]["TurnData"][0] = [];

            for (let currentCharacterIndex of characterIndices)
            {
                analyzerDataCopy["CharacterData"]["PlayerData"].push(analyzerData["CharacterData"]["PlayerData"][currentCharacterIndex]);
                analyzerDataCopy["GameData"]["TurnData"][0].push(analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1][currentCharacterIndex]);
            }
        }

        ajaxCall
        (
            "POST",
            "SaveTurnBeforeRollData",
            analyzerDataCopy
        ).done(function (response)
        {
            alertNotificationMessageDisplay(response["AlertData"]);

            if (!response["Error"])
                callback();
        });
    }

    let playerTurnCharacterIndex = 0;

    let spaceLayoutIndex = 0;

    let mouseCoordinates =
    {
        x: 0,
        y: 0
    };

    function initializeGameSetup()
    {
        initializeStandings();

        $("#standings-subpanel").show();

        for (let i = 0; i < analyzerData["CharacterData"]["PlayerData"].length; ++i)
        {
            if (analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1][i]["TurnAfterRollData"] === null)
            {
                playerTurnCharacterIndex = i;

                break;
            }
        }

        spaceLayoutIndex = analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1][playerTurnCharacterIndex]["TurnBeforeRollStartData"]["SpaceLayoutIndex"];

        initializeMap();

        $(document).on("mousemove", function (e)
        {
            mouseCoordinates["x"] = e.clientX;
            mouseCoordinates["y"] = e.clientY;
        });

        $(window).on("resize", updateMapSizing);

        initializeTurns();
    }

    function initializeStandings()
    {
        let standingsContainer = $("#standings-subpanel > div:last-of-type");

        standingsContainer.empty();

        for (let i = 0; i < analyzerData["CharacterData"]["PlayerData"].length; ++i)
        {
            standingsContainer.append
            (
                "<div class=\"player-information\" style=\"background-color: #" + analyzerData["CharacterData"]["PlayerData"][i]["ColorData"]["GameColor"] + ";\">" +
                    "<div></div>" +

                    renderPlayerContainer(i) +

                    "<div>" +
                        "<div>" +
                            "<div>Ready Cash</div>" +

                            "<div></div>" +
                        "</div>" +

                        "<div>" +
                            "<div>Total Shop Value</div>" +

                            "<div></div>" +
                        "</div>" +

                        "<div>" +
                            "<div>Total Stock Value</div>" +

                            "<div></div>" +
                        "</div>" +

                        "<div>" +
                            "<div>Net Worth</div>" +

                            "<div></div>" +
                        "</div>" +
                    "</div>" +

                    "<div></div>" +
                "</div>"
            );

            standingsContainer.children().last().children().eq(2).children().eq(2).toggle(analyzerData["GameData"]["RuleData"]["Name"] === "Standard");

            updatePlayerStandings(i);
        }
    }

    function updatePlayerStandings(playerIndex)
    {
        let playerTurnCharacterData = analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1][playerIndex]["TurnBeforeRollCurrentData"];

        let playerContainer = $("#standings-subpanel > div:last-of-type > div:nth-of-type(" + (playerIndex + 1) + ")");

        playerContainer.children().first().empty().append
        (
            "<span>" + playerTurnCharacterData["Placing"] + "</span>" +

            "<span>" +
                "<sup>" + ordinalNumberSuffix(playerTurnCharacterData["Placing"]) + "</sup>" +
            "</span>"
        );

        let playerStatsContainer = playerContainer.children().eq(2);

        playerStatsContainer.children().first().children().last().text(playerTurnCharacterData["ReadyCash"]);
        playerStatsContainer.children().eq(1).children().last().text(playerTurnCharacterData["TotalShopValue"]);

        if (analyzerData["GameData"]["RuleData"]["Name"] === "Standard")
            playerStatsContainer.children().eq(2).children().last().text(playerTurnCharacterData["TotalStockValue"]);

        playerStatsContainer.children().last().children().last().text(playerTurnCharacterData["NetWorth"]);

        playerContainer.children().last().empty().append
        (
            "<div class=\"suit-card\">" +
                "<div>" +
                    "<div></div>" +

                    "<div>" + playerTurnCharacterData["TotalSuitCards"] + "</div>" +
                "</div>" +
            "</div>"
        );

        let playerSuitCardContainer = playerContainer.children().last().find(".suit-card");

        playerSuitCardContainer.children().first().toggle(playerTurnCharacterData["TotalSuitCards"] > 0);

        for (let currentSuitData of SUIT_DATA)
        {
            playerSuitCardContainer.children().first().children().first().append
            (
                "<div>" +
                    "<span class=\"fas fa-" + currentSuitData["Icon"] + "\" style=\"color: #" + currentSuitData["Color"] + ";\"></span>" +
                "</div>"
            );

            playerContainer.children().last().append
            (
                "<div>" +
                    "<span class=\"fas fa-" + currentSuitData["Icon"] + "\" style=\"color: #" + (playerTurnCharacterData["CollectedSuits"].indexOf(currentSuitData["Icon"]) > -1 ? currentSuitData["Color"] : "333") + ";\"></span>" +
                "</div>"
            );
        }
    }

    function updateCirclePosition(element, centerPercentage)
    {
        return {
            left: ((centerPercentage["x"] * $(element).parent().innerWidth() - $(element).innerWidth() / 2) / $(element).parent().innerWidth() * 100).toFixed(4) + "%",
            top: ((centerPercentage["y"] * $(element).parent().innerHeight() - $(element).innerHeight() / 2) / $(element).parent().innerHeight() * 100).toFixed(4) + "%"
        };
    }

    function renderDie(element, value)
    {
        $(element).append("<div></div>");

        let dieDotLocationContainer = $(element).children().last();

        for (let currentDieDotLocationData of DIE_DOT_LOCATIONS_DATA[value - 1])
        {
            dieDotLocationContainer.append("<div></div>");

            let currentDieDotLocationContainer = dieDotLocationContainer.children().last();

            currentDieDotLocationContainer.css(updateCirclePosition(currentDieDotLocationContainer, currentDieDotLocationData));
        }
    }

    function spacePopupDialog(element)
    {
        $(element).show();

        let boardSubpanelContainer = $("#board-subpanel > div:first-of-type");

        let boardSubpanelOffset = boardSubpanelContainer.offset();

        let currentSpaceOffset = $(element).parent().offset();

        let currentSpacePopupDialogOffsetX = $(document).scrollLeft() + mouseCoordinates["x"] + 10;

        if (currentSpacePopupDialogOffsetX + $(element).outerWidth() > boardSubpanelContainer.width())
            currentSpacePopupDialogOffsetX = $(document).scrollLeft() + mouseCoordinates["x"] - 10 - $(element).outerWidth();

        let currentSpacePopupDialogOffsetY = $(document).scrollTop() + mouseCoordinates["y"] - $(element).outerHeight() / 2;

        if (currentSpacePopupDialogOffsetY < boardSubpanelOffset.top)
            currentSpacePopupDialogOffsetY = $(document).scrollTop() + mouseCoordinates["y"];
        else if (currentSpacePopupDialogOffsetY + $(element).outerHeight() > boardSubpanelContainer.height())
            currentSpacePopupDialogOffsetY = $(document).scrollTop() + mouseCoordinates["y"] - $(element).outerHeight();

        $(element).css
        (
            {
                left: (currentSpacePopupDialogOffsetX - currentSpaceOffset.left - parseInt($(element).parent().css("border-left-width"), 10)) + "px",
                top: (currentSpacePopupDialogOffsetY - currentSpaceOffset.top - parseInt($(element).parent().css("border-top-width"), 10)) + "px"
            }
        );
    }

    function createTreeGraphNode(data)
    {
        return {
            Node: data,
            Child: null,
            NextPointer: null
        };
    }

    function traverseTreeGraphDieRollOptionsSpaceInformation(root, spacesTraversed)
    {
        if (root === null)
            return;

        while (root !== null)
        {
            if (root["Node"]["DieRollValue"] - spacesTraversed > 0)
            {
                let currentSpaceInformationContainer = $("#board-subpanel-spaces > div:nth-of-type(" + (root["Node"]["SpaceIndexCurrent"] + 1) + ") > .space-information");

                currentSpaceInformationContainer.show();

                renderDie(currentSpaceInformationContainer.find(".die-rolls"), root["Node"]["DieRollValue"] - spacesTraversed);

                currentSpaceInformationContainer.hide();
            }

            if (root["Child"] !== null)
                traverseTreeGraphDieRollOptionsSpaceInformation(root["Child"], spacesTraversed);

            root = root["NextPointer"];
        }
    }

    function traverseTreeGraphMovePlayerAroundMapSpaces(root)
    {
        let spaceTreeGraphs = [ root["Child"] ];

        root = root["NextPointer"];

        while (root !== null)
        {
            spaceTreeGraphs.push(root);

            root = root["NextPointer"];
        }

        return spaceTreeGraphs;
    }

    let playerTurnCharacterTreeGraph = null;

    function initializeTreeGraphPlayerTurnCharacter()
    {
        let playerTurnCharacterBeforeRollData = analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1][playerTurnCharacterIndex]["TurnBeforeRollCurrentData"];

        playerTurnCharacterTreeGraph = createTreeGraphNode
        (
            {
                SpaceIndexCurrent: playerTurnCharacterBeforeRollData["SpaceIndexCurrent"],
                SpaceIndexFrom: playerTurnCharacterBeforeRollData["SpaceIndexFrom"],
                DieRollValue: 0
            }
        );

        let maxPlayerTurnEligibleDieRollValue = analyzerData["GameData"]["BoardData"]["MaxDieRoll"];

        if (playerTurnCharacterBeforeRollData["DieRollRestrictions"] !== null)
            maxPlayerTurnEligibleDieRollValue = playerTurnCharacterBeforeRollData["DieRollRestrictions"][playerTurnCharacterBeforeRollData["DieRollRestrictions"].length - 1];

        let playerTurnCharacterTreeGraphPreviousParentNodes = [ playerTurnCharacterTreeGraph ];

        for (let currentPlayerTurnEligibleDieRollValue = 1; currentPlayerTurnEligibleDieRollValue <= maxPlayerTurnEligibleDieRollValue; ++currentPlayerTurnEligibleDieRollValue)
        {
            let playerTurnCharacterTreeGraphCurrentParentNodes = [];

            for (let currentPlayerTurnCharacterTreeGraphPreviousParentNode of playerTurnCharacterTreeGraphPreviousParentNodes)
            {
                let currentSpaceConstraintData = $.grep(analyzerData["SpaceData"][currentPlayerTurnCharacterTreeGraphPreviousParentNode["Node"]["SpaceIndexCurrent"]]["SpaceConstraintData"], function (value)
                {
                    return spaceLayoutIndex === value["SpaceLayoutIndex"] && (currentPlayerTurnCharacterTreeGraphPreviousParentNode["Node"]["SpaceIndexFrom"] === null || currentPlayerTurnCharacterTreeGraphPreviousParentNode["Node"]["SpaceIndexFrom"] === value["SpaceIndexFrom"]);
                });

                for (let currentSpaceIndexToValue of [...new Set([].concat(...currentSpaceConstraintData.map((value) => value["SpaceIndicesTo"])))])
                {
                    let currentPlayerTurnCharacterTreeGraphNode = createTreeGraphNode
                    (
                        {
                            SpaceIndexCurrent: currentSpaceIndexToValue,
                            SpaceIndexFrom: currentPlayerTurnCharacterTreeGraphPreviousParentNode["Node"]["SpaceIndexCurrent"],
                            DieRollValue: currentPlayerTurnEligibleDieRollValue
                        }
                    );

                    playerTurnCharacterTreeGraphCurrentParentNodes.push(currentPlayerTurnCharacterTreeGraphNode);

                    if (currentPlayerTurnCharacterTreeGraphPreviousParentNode["Child"] === null)
                    {
                        currentPlayerTurnCharacterTreeGraphPreviousParentNode["Child"] = currentPlayerTurnCharacterTreeGraphNode;

                        continue;
                    }

                    while (currentPlayerTurnCharacterTreeGraphPreviousParentNode["NextPointer"] !== null)
                        currentPlayerTurnCharacterTreeGraphPreviousParentNode = currentPlayerTurnCharacterTreeGraphPreviousParentNode["NextPointer"];

                    currentPlayerTurnCharacterTreeGraphPreviousParentNode["NextPointer"] = currentPlayerTurnCharacterTreeGraphNode;
                }
            }

            playerTurnCharacterTreeGraphPreviousParentNodes = playerTurnCharacterTreeGraphCurrentParentNodes;
        }
    }

    let maxCenterXFactor = 0;
    let maxCenterYFactor = 0;

    function initializeMap()
    {
        $("#board-subpanel").empty().append
        (
            "<div>" +
                "<div id=\"board-subpanel-spaces\">" + new Array(analyzerData["SpaceData"].length).fill("<div></div>").join("") + "</div>" +
            "</div>"
        );

        maxCenterXFactor = analyzerData["SpaceData"][0]["SpaceLayoutData"][spaceLayoutIndex]["CenterXFactor"];
        maxCenterYFactor = analyzerData["SpaceData"][0]["SpaceLayoutData"][spaceLayoutIndex]["CenterYFactor"];

        for (let i = 1; i < analyzerData["SpaceData"].length; ++i)
        {
            let currentCenterXFactor = analyzerData["SpaceData"][i]["SpaceLayoutData"][spaceLayoutIndex]["CenterXFactor"];

            if (currentCenterXFactor > maxCenterXFactor)
                maxCenterXFactor = currentCenterXFactor;

            let currentCenterYFactor = analyzerData["SpaceData"][i]["SpaceLayoutData"][spaceLayoutIndex]["CenterYFactor"];

            if (currentCenterYFactor > maxCenterYFactor)
                maxCenterYFactor = currentCenterYFactor;
        }

        updateMapSizing();

        for (let i = 0; i < analyzerData["SpaceData"].length; ++i)
        {
            updateMapSpace(i);

            $("#board-subpanel-spaces > div:nth-of-type(" + (i + 1) + ")").on("mouseenter mousemove", function ()
            {
                let currentSpaceInformationContainer = $(this).find(".space-information");

                spacePopupDialog(currentSpaceInformationContainer);
            }).on("mouseleave", function ()
            {
                $(this).find(".space-information").hide();
            });
        }
    }

    let boardSubpanelOffsetX = 0;
    let boardSubpanelOffsetY = 0;

    let spacePopupDialogScrollDebounce;

    function updateMapSizing()
    {
        let boardSubpanelContainer = $("#board-subpanel > div:first-of-type");

        boardSubpanelOffsetX = Math.max((boardSubpanelContainer.width() - SPACE_SQUARE_SIZE * (maxCenterXFactor + 0.5)) / 2, 0);
        boardSubpanelOffsetY = Math.max((boardSubpanelContainer.height() - SPACE_SQUARE_SIZE * (maxCenterYFactor + 0.5)) / 2, 0);

        for (let i = 0; i < analyzerData["SpaceData"].length; ++i)
            $("#board-subpanel-spaces > div:nth-of-type(" + (i + 1) + ")").css
            (
                {
                    left: boardSubpanelOffsetX + SPACE_SQUARE_SIZE * (analyzerData["SpaceData"][i]["SpaceLayoutData"][spaceLayoutIndex]["CenterXFactor"] - 0.5) + "px",
                    top: boardSubpanelOffsetY + SPACE_SQUARE_SIZE * (analyzerData["SpaceData"][i]["SpaceLayoutData"][spaceLayoutIndex]["CenterYFactor"] - 0.5) + "px",
                    width: SPACE_SQUARE_SIZE + "px",
                    height: SPACE_SQUARE_SIZE + "px"
                }
            );

        boardSubpanelContainer.css({ overflow: "" });

        if (SPACE_SQUARE_SIZE * (maxCenterXFactor + 0.5) > boardSubpanelContainer.width() || SPACE_SQUARE_SIZE * (maxCenterYFactor + 0.5) > boardSubpanelContainer.height())
        {
            boardSubpanelContainer.css({ overflow: "scroll" });

            boardSubpanelContainer.on("scroll", function ()
            {
                clearTimeout(spacePopupDialogScrollDebounce);

                $(this).find(".space-information").hide();

                spacePopupDialogScrollDebounce = setTimeout(function ()
                {
                    let currentSpaceInformationContainer = null;

                    $("#board-subpanel-spaces > div").each(function ()
                    {
                        if ($(this).is(":hover"))
                        {
                            currentSpaceInformationContainer = $(this).find(".space-information");

                            return;
                        }
                    });

                    if (currentSpaceInformationContainer !== null)
                        spacePopupDialog(currentSpaceInformationContainer);
                }, 100);
            });
        }
    }

    function updateMapSpace(spaceIndex)
    {
        let spaceContainer = $("#board-subpanel-spaces > div:nth-of-type(" + (spaceIndex + 1) + ")");

        spaceContainer.empty().append("<div></div>");

        let spaceSquareContainer = spaceContainer.children().last();

        let spaceIconValue = analyzerData["SpaceTypeData"][analyzerData["SpaceData"][spaceIndex]["SpaceTypeIndex"]]["Icon"];

        if (spaceIconValue == null)
        {
            if (analyzerData["SpaceTypeData"][analyzerData["SpaceData"][spaceIndex]["SpaceTypeIndex"]]["Name"] == "suit")
                spaceIconValue = SUIT_DATA[SUIT_ORDER.indexOf(analyzerData["SpaceData"][spaceIndex]["AdditionalPropertiesData"]["SuitData"]["Name"])]["Icon"];
        }

        spaceSquareContainer.append
        (
            "<div class=\"space-icon\">" +
                "<span class=\"fas fa-" + spaceIconValue + "\"></span>" +
            "</div>"
        );

        let spaceSquareIconContainer = spaceSquareContainer.find(".fa-" + spaceIconValue);

        let spaceInformationPlaceholders =
        {
            BankData:
            {
                StockInformation: ""
            },
            ShopData:
            {
                ShopName: "",
                ShopValue: "",
                ShopPrice: "",
                MaxCapital: ""
            },
            SuitData:
            {
                SuitIcon: ""
            }
        };

        if (analyzerData["GameData"]["RuleData"]["Name"] === "Standard")
            spaceInformationPlaceholders["BankData"]["StockInformation"] = ", and you can buy stocks as you pass through";

        if (analyzerData["SpaceTypeData"][analyzerData["SpaceData"][spaceIndex]["SpaceTypeIndex"]]["Name"] == "shop")
        {
            spaceContainer.css({ borderColor: "#" + (analyzerData["SpaceData"][spaceIndex]["DistrictIndex"] !== null ? analyzerData["DistrictData"][analyzerData["SpaceData"][spaceIndex]["DistrictIndex"]]["Color"] : "F2D4A9") });

            if (analyzerData["SpaceData"][spaceIndex]["AdditionalPropertiesData"]["ShopData"]["OwnerCharacterIndex"] !== null)
            {
                spaceSquareIconContainer.css({ color: "#" + analyzerData["CharacterData"]["PlayerData"][analyzerData["SpaceData"][spaceIndex]["AdditionalPropertiesData"]["ShopData"]["OwnerCharacterIndex"]]["ColorData"]["GameColor"] });

                spaceSquareContainer.append
                (
                    "<div class=\"shop-owned-price\">" +
                        "<div>" + analyzerData["SpaceData"][spaceIndex]["AdditionalPropertiesData"]["ShopData"]["Price"] + "</div>" +
                    "</div>"
                );

                spaceInformationPlaceholders["ShopData"]["MaxCapital"] =
                    "<div>" +
                        "<div>Max. capital</div>" +

                        "<div>" + (2 * analyzerData["ShopData"][analyzerData["SpaceData"][spaceIndex]["ShopIndex"]]["Value"]) + "</div>" +
                    "</div>";
            }
            else
            {
                spaceSquareContainer.append
                (
                    "<div class=\"shop-unowned-value\">" +
                        "<div>" + analyzerData["ShopData"][analyzerData["SpaceData"][spaceIndex]["ShopIndex"]]["Value"] + "</div>" +
                    "</div>"
                );
            }

            spaceInformationPlaceholders["ShopData"]["ShopName"] = "<div>" + analyzerData["ShopData"][analyzerData["SpaceData"][spaceIndex]["ShopIndex"]]["Name"] + "</div>";

            spaceInformationPlaceholders["ShopData"]["ShopValue"] =
                "<div>" +
                    "<div>Shop value</div>" +

                    "<div>" + analyzerData["ShopData"][analyzerData["SpaceData"][spaceIndex]["ShopIndex"]]["Value"] + "</div>" +
                "</div>";

            spaceInformationPlaceholders["ShopData"]["ShopPrice"] =
                "<div>" +
                    "<div>Shop prices</div>" +

                    "<div>" + analyzerData["SpaceData"][spaceIndex]["AdditionalPropertiesData"]["ShopData"]["Price"] + "</div>" +
                "</div>";
        }
        else if (analyzerData["SpaceTypeData"][analyzerData["SpaceData"][spaceIndex]["SpaceTypeIndex"]]["Name"] == "suit")
        {
            spaceSquareIconContainer.css({ color: "#" + SUIT_DATA[SUIT_ORDER.indexOf(analyzerData["SpaceData"][spaceIndex]["AdditionalPropertiesData"]["SuitData"]["Name"])]["Color"] });

            spaceInformationPlaceholders["SuitData"]["SuitIcon"] = "<span class=\"fas fa-" + SUIT_DATA[SUIT_ORDER.indexOf(analyzerData["SpaceData"][spaceIndex]["AdditionalPropertiesData"]["SuitData"]["Name"])]["Icon"] + "\" style=\"color: #" + SUIT_DATA[SUIT_ORDER.indexOf(analyzerData["SpaceData"][spaceIndex]["AdditionalPropertiesData"]["SuitData"]["Name"])]["Color"] + ";\"></span>";
        }

        spaceSquareContainer.append("<div class=\"character-markers\"></div>");

        let spaceCharacterMarkerIndices = $.map(analyzerData["CharacterData"]["PlayerData"], function (value, index)
        {
            if (analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1][index]["TurnBeforeRollCurrentData"]["SpaceIndexCurrent"] === spaceIndex)
                return index;
        });

        if (spaceCharacterMarkerIndices.length > 0)
        {
            let spaceCharacterMarkersContainer = spaceSquareContainer.find(".character-markers");

            for (let i = 0; i < spaceCharacterMarkerIndices.length; ++i)
            {
                spaceCharacterMarkersContainer.append("<div></div>");

                let currentSpaceCharacterMarkerContainer = spaceCharacterMarkersContainer.children().last();

                currentSpaceCharacterMarkerContainer.css
                (
                    $.extend
                    (
                        {},
                        updateCirclePosition
                        (
                            currentSpaceCharacterMarkerContainer,
                            {
                                x: 0.25 + (i % 2) * 0.5,
                                y: 0.25 + Math.floor(i / 2) * 0.5
                            }
                        ),
                        {
                            backgroundColor: "#" + analyzerData["CharacterData"]["PlayerData"][spaceCharacterMarkerIndices[i]]["ColorData"]["GameColor"]
                        }
                    )
                );
            }
        }

        spaceContainer.append
        (
            "<div class=\"popup-dialog space-information" + (analyzerData["SpaceTypeData"][analyzerData["SpaceData"][spaceIndex]["SpaceTypeIndex"]]["Name"] == "shop" ? " space-shop" : "") + "\">" +
                "<div></div>" +
            "</div>"
        );

        let spaceInformationContainer = spaceContainer.children().last().children().first();

        spaceInformationContainer.append
        (
            "<div>" +
                "<div class=\"die-rolls\"></div>" +

                "<div>" + (analyzerData["SpaceTypeData"][analyzerData["SpaceData"][spaceIndex]["SpaceTypeIndex"]]["Title"] !== null ? analyzerData["SpaceTypeData"][analyzerData["SpaceData"][spaceIndex]["SpaceTypeIndex"]]["Title"] : "") + "</div>" +
            "</div>"
        );

        spaceInformationContainer.append("<div>" + analyzerData["SpaceTypeData"][analyzerData["SpaceData"][spaceIndex]["SpaceTypeIndex"]]["Description"].replace("{stock-information}", spaceInformationPlaceholders["BankData"]["StockInformation"]).replace("{shop-name}", spaceInformationPlaceholders["ShopData"]["ShopName"]).replace("{shop-value}", spaceInformationPlaceholders["ShopData"]["ShopValue"]).replace("{shop-price}", spaceInformationPlaceholders["ShopData"]["ShopPrice"]).replace("{max-capital}", spaceInformationPlaceholders["ShopData"]["MaxCapital"]).replace("{suit-icon}", spaceInformationPlaceholders["SuitData"]["SuitIcon"]) + "</div>");
    }

    let animateCharacterMarkerFlag = false;

    function animateCurrentPlayerCharacterMarker()
    {
        if (!animateCharacterMarkerFlag)
            return;

        $("#board-subpanel-spaces > div:nth-of-type(" + (analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1][playerTurnCharacterIndex]["TurnBeforeRollCurrentData"]["SpaceIndexCurrent"] + 1) + ") > div:first-of-type > .character-markers > div:first-of-type").animate({ opacity: 0.25 }, 1500, function ()
        {
            $(this).css({ opacity: 1 });

            animateCurrentPlayerCharacterMarker();
        });
    }

    function movePlayerAroundMap(characterTreeGraph, spacesRemaining)
    {
        let playerTurnCharacterData = analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1][playerTurnCharacterIndex];

        if (spacesRemaining === 0)
        {
            let spaceContainer = $("#board-subpanel-spaces > div:nth-of-type(" + (characterTreeGraph["Node"]["SpaceIndexCurrent"] + 1) + ")");

            let spaceIndexCurrentCopy = playerTurnCharacterData["TurnBeforeRollCurrentData"]["SpaceIndexCurrent"];

            playerTurnCharacterData["TurnBeforeRollCurrentData"]["SpaceIndexCurrent"] = characterTreeGraph["Node"]["SpaceIndexCurrent"];

            updateMapSpace(spaceIndexCurrentCopy);
            updateMapSpace(characterTreeGraph["Node"]["SpaceIndexCurrent"]);

            addLogEntry("Landed on " + spaceContainer.find(".space-icon").html() + ".");

            saveAnalyzerData([ "GameData" ]);

            animateCharacterMarkerFlag = false;

            $("#board-subpanel-spaces-remaining").remove();

            if (playerTurnCharacterIndex === analyzerData["CharacterData"]["PlayerData"].length - 1)
            {
                let nextTurnData = [];

                for (let i = 0; i < analyzerData["CharacterData"]["PlayerData"].length; ++i)
                {
                    let currentPlayerTurnCharacterData = analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1][i];

                    nextTurnData.push
                    (
                        {
                            TurnBeforeRollStartData: currentPlayerTurnCharacterData["TurnBeforeRollCurrentData"],
                            TurnBeforeRollCurrentData: currentPlayerTurnCharacterData["TurnBeforeRollCurrentData"],
                            TurnAfterRollData: null,
                            Logs: [],
                            CameoCharacterIndices: currentPlayerTurnCharacterData["CameoCharacterIndices"],
                            TurnCameoCharacterData: currentPlayerTurnCharacterData["TurnCameoCharacterData"]
                        }
                    );
                }

                analyzerData["GameData"]["TurnData"].push(nextTurnData);

                displayNewTurn(analyzerData["GameData"]["TurnData"].length - 1);
            }

            playerTurnCharacterIndex = (playerTurnCharacterIndex + 1) % analyzerData["CharacterData"]["PlayerData"].length;

            $("#turns > div:last-of-type").append("<div></div>");

            startNextPlayerTurn();

            return;
        }

        let spaceTreeGraphPlayerEvents = function (spaceTreeGraph)
        {
            playerTurnCharacterData["TurnBeforeRollCurrentData"]["SpaceIndexFrom"] = characterTreeGraph["Node"]["SpaceIndexCurrent"];

            let spaceTypeName = analyzerData["SpaceTypeData"][analyzerData["SpaceData"][spaceTreeGraph["Node"]["SpaceIndexCurrent"]]["SpaceTypeIndex"]]["Name"];

            if (spaceTypeName === "suit")
            {
                let spaceContainer = $("#board-subpanel-spaces > div:nth-of-type(" + (spaceTreeGraph["Node"]["SpaceIndexCurrent"] + 1) + ")");

                let spaceSuitAdditionalProperties = analyzerData["SpaceData"][spaceTreeGraph["Node"]["SpaceIndexCurrent"]]["AdditionalPropertiesData"]["SuitData"];

                let suitName = spaceSuitAdditionalProperties["Name"];

                if (playerTurnCharacterData["TurnBeforeRollCurrentData"]["CollectedSuits"].indexOf(suitName) === -1)
                {
                    playerTurnCharacterData["TurnBeforeRollCurrentData"]["CollectedSuits"].push(suitName);

                    addLogEntry("Picked up " + spaceContainer.find(".space-icon").html() + ".");

                    updatePlayerStandings(playerTurnCharacterIndex);
                }

                if (spaceSuitAdditionalProperties["Rotate"])
                {
                    analyzerData["SpaceData"][spaceTreeGraph["Node"]["SpaceIndexCurrent"]]["AdditionalPropertiesData"]["SuitData"]["Name"] = SUIT_ORDER[(SUIT_ORDER.indexOf(suitName) + 1) % SUIT_ORDER.length];

                    updateMapSpace(spaceTreeGraph["Node"]["SpaceIndexCurrent"]);
                }
            }

            movePlayerAroundMap(spaceTreeGraph, spacesRemaining - 1);
        };

        let playerSpaceTreeGraphs = traverseTreeGraphMovePlayerAroundMapSpaces(characterTreeGraph);

        if (playerSpaceTreeGraphs.length > 1)
        {
            if (characterTreeGraph["Node"]["SpaceIndexCurrent"] !== playerTurnCharacterData["TurnBeforeRollCurrentData"]["SpaceIndexCurrent"])
            {
                let spaceIndexCurrentCopy = playerTurnCharacterData["TurnBeforeRollCurrentData"]["SpaceIndexCurrent"];

                playerTurnCharacterData["TurnBeforeRollCurrentData"]["SpaceIndexCurrent"] = characterTreeGraph["Node"]["SpaceIndexCurrent"];

                updateMapSpace(spaceIndexCurrentCopy);
                updateMapSpace(characterTreeGraph["Node"]["SpaceIndexCurrent"]);

                let boardSubpanelSpacesRemainingDieRollsContainer = $("#board-subpanel-spaces-remaining > .die-rolls");

                boardSubpanelSpacesRemainingDieRollsContainer.empty();

                renderDie(boardSubpanelSpacesRemainingDieRollsContainer, spacesRemaining);

                $("#board-subpanel-spaces > div > .space-information > div:first-of-type > div:first-of-type > .die-rolls").empty();

                traverseTreeGraphDieRollOptionsSpaceInformation(characterTreeGraph, playerTurnCharacterData["TurnAfterRollData"][playerTurnCharacterData["TurnAfterRollData"].length - 1]["DieRollValue"] - spacesRemaining);
            }

            $("#board-subpanel > div:first-of-type").append("<div id=\"board-subpanel-direction-choices\"></div>");

            let sourceSpaceLayoutData = analyzerData["SpaceData"][characterTreeGraph["Node"]["SpaceIndexCurrent"]]["SpaceLayoutData"][spaceLayoutIndex];

            for (let currentPlayerSpaceTreeGraph of playerSpaceTreeGraphs)
            {
                $("#board-subpanel-direction-choices").append
                (
                    "<div>" +
                        "<span class=\"fas fa-right\"></span>" +
                    "</div>"
                );

                let currentArrowContainer = $("#board-subpanel-direction-choices > div:last-of-type");

                let destinationSpaceLayoutData = analyzerData["SpaceData"][currentPlayerSpaceTreeGraph["Node"]["SpaceIndexCurrent"]]["SpaceLayoutData"][spaceLayoutIndex];

                currentArrowContainer.css
                (
                    {
                        left: boardSubpanelOffsetX + SPACE_SQUARE_SIZE * (sourceSpaceLayoutData["CenterXFactor"] - 1) + "px",
                        top: boardSubpanelOffsetY + SPACE_SQUARE_SIZE * (sourceSpaceLayoutData["CenterYFactor"] - 1) + "px",
                        width: 2 * SPACE_SQUARE_SIZE + "px",
                        height: 2 * SPACE_SQUARE_SIZE + "px",
                        transform: "rotateZ(" + Math.atan2(destinationSpaceLayoutData["CenterYFactor"] - sourceSpaceLayoutData["CenterYFactor"], destinationSpaceLayoutData["CenterXFactor"] - sourceSpaceLayoutData["CenterXFactor"]) * 180 / Math.PI + "deg)"
                    }
                );

                setTimeout(function ()
                {
                    let currentArrowIcon = currentArrowContainer.find("path");

                    currentArrowIcon.css({ fill: "#" + analyzerData["CharacterData"]["PlayerData"][playerTurnCharacterIndex]["ColorData"]["GameColor"] });

                    let currentArrowIconCopy = currentArrowIcon.clone();

                    currentArrowIconCopy.prependTo(currentArrowIcon.parent());

                    currentArrowIconCopy.css
                    (
                        {
                            fill: "transparent",
                            stroke: "#" + analyzerData["CharacterData"]["PlayerData"][playerTurnCharacterIndex]["ColorData"]["GameColor"],
                            strokeWidth: "10px",
                            filter: "brightness(0.25)"
                        }
                    );

                    currentArrowIcon.on("click", function ()
                    {
                        $("#board-subpanel-direction-choices").remove();

                        spaceTreeGraphPlayerEvents(currentPlayerSpaceTreeGraph);
                    })
                }, 100);
            }

            return;
        }

        spaceTreeGraphPlayerEvents(playerSpaceTreeGraphs[0]);
    }

    function addLogEntry(entry)
    {
        analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1][playerTurnCharacterIndex]["Logs"].push(entry);

        $("#turns > div:last-of-type > div:last-of-type > .logs").append("<div>" + entry + "</div>");
    }

    function displayNewTurn(turnIndex)
    {
        $("#turns").append
        (
            "<div>" +
                "<div>" +
                    "<div>" +
                        "<h2>Turn " + (turnIndex + 1) + "</h2>" +
                    "</div>" +

                    "<div>" +
                        "<button type=\"button\" class=\"btn btn-danger\" name=\"reset\">Reset To This Turn</button>" +
                    "</div>" +
                "</div>" +
            "</div>"
        );

        $("#turns > div:last-of-type button[name=\"reset\"]").on("click", function ()
        {
            $("body").prepend
            (
                "<div class=\"modal fade\" tabindex=\"-1\" role=\"dialog\">" +
                    "<div class=\"modal-dialog modal-dialog-centered\">" +
                        "<div class=\"modal-content\">" +
                            "<div class=\"modal-body\">" +
                                "<div class=\"alert alert-danger\">" +
                                    "<div>" +
                                        "<strong>You are about to reset to the beginning of turn " + (turnIndex + 1) + ". Any progress made during and after this turn will be lost and cannot be recovered.</strong>" +
                                    "</div>" +
                                "</div>" +

                                "<div class=\"modal-confirmation\">" +
                                    "<div>Do you wish to continue?</div>" +

                                    "<div>" +
                                        "<button type=\"button\" class=\"btn btn-danger\" name=\"modal-confirmation-reset-turn-danger-yes\" data-bs-dismiss=\"modal\">Yes</button>" +
                                        "<button type=\"button\" class=\"btn btn-secondary\" name=\"modal-confirmation-reset-turn-danger-no\" data-bs-dismiss=\"modal\">No</button>" +
                                    "</div>" +
                                "</div>" +
                            "</div>" +
                        "</div>" +
                    "</div>" +
                "</div>"
            );

            let modalDialog = new bootstrap.Modal
            (
                $("body > .modal"),
                {
                    backdrop: "static",
                    keyboard: false
                }
            );

            modalDialog.show();

            $("body > .modal").on("hidden.bs.modal", function ()
            {
                $(this).remove();
            });

            $("body > .modal button[name=\"modal-confirmation-reset-turn-danger-yes\"]").on("click", function ()
            {
                for (let i = 0; i < analyzerData["CharacterData"]["PlayerData"].length; ++i)
                    analyzerData["GameData"]["TurnData"][turnIndex][i]["TurnBeforeRollCurrentData"] = analyzerData["GameData"]["TurnData"][turnIndex][i]["TurnBeforeRollStartData"];

                analyzerData["GameData"]["TurnData"].length = turnIndex + 1;

                initializeStandings();

                initializeMap();

                animateCharacterMarkerFlag = false;

                $("#turns").remove();

                initializeTurns();
            });
        });
    }

    function displayPlayerTurnOptions()
    {
        let playerTurnContainer = $("#turns > div:last-of-type > div:last-of-type");

        playerTurnContainer.find(".logs").nextAll().remove();

        playerTurnContainer.append
        (
            createConfirmationActions
            (
                "center-items",
                [
                    "<button type=\"button\" class=\"btn btn-lg btn-primary\" name=\"roll\">Roll</button>",
                    "<button type=\"button\" class=\"btn btn-lg btn-primary\" name=\"auction\">Auction</button>",
                    "<button type=\"button\" class=\"btn btn-lg btn-primary\" name=\"sell-shop\">Sell Shop</button>",
                    "<button type=\"button\" class=\"btn btn-lg btn-primary\" name=\"buy-shop\">Buy Shop</button>",
                    "<button type=\"button\" class=\"btn btn-lg btn-primary\" name=\"exchange-shops\">Exchange Shops</button>",
                    "<button type=\"button\" class=\"btn btn-lg btn-primary\" name=\"renovate-vacant-plot\">Renovate Vacant Plot</button>",
                    "<button type=\"button\" class=\"btn btn-lg btn-primary\" name=\"sell-stocks\">Sell Stocks</button>"
                ]
            )
        );

        let playerTurnConfirmationActionsContainer = playerTurnContainer.find(".confirmation-actions");

        let playerTurnCharacterBeforeRollData = analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1][playerTurnCharacterIndex]["TurnBeforeRollCurrentData"];

        playerTurnConfirmationActionsContainer.find("button[name=\"roll\"]").on("click", function ()
        {
            playerTurnConfirmationActionsContainer.remove();

            playerTurnContainer.append
            (
                "<div class=\"die-rolls die-roll-selection\"></div>" +

                createConfirmationActions
                (
                    "center-items",
                    [
                        "<button type=\"button\" class=\"btn btn-lg btn-primary disabled\" name=\"confirm\" disabled=\"disabled\">Confirm</button>",
                        "<button type=\"button\" class=\"btn btn-lg btn-secondary\" name=\"cancel\">Cancel</button>"
                    ]
                )
            );

            let playerTurnEligibleDieRolls = Array.from(Array(analyzerData["GameData"]["BoardData"]["MaxDieRoll"]).keys(), n => n + 1);

            if (playerTurnCharacterBeforeRollData["DieRollRestrictions"] !== null)
                playerTurnEligibleDieRolls = playerTurnCharacterBeforeRollData["DieRollRestrictions"];

            let currentDieRollsContainer = playerTurnContainer.find(".die-rolls");

            let playerTurnDieRollValue = null;

            let playerTurnOptionsConfirmButtonContainer = playerTurnContainer.find("button[name=\"confirm\"]");

            for (let i = 0; i < playerTurnEligibleDieRolls.length; ++i)
            {
                let currentPlayerTurnEligibleDieRollValue = playerTurnEligibleDieRolls[i];

                renderDie(currentDieRollsContainer, currentPlayerTurnEligibleDieRollValue);

                currentDieRollsContainer.children().eq(i).on("click", function ()
                {
                    if (currentPlayerTurnEligibleDieRollValue !== playerTurnDieRollValue)
                    {
                        playerTurnDieRollValue = currentPlayerTurnEligibleDieRollValue;

                        currentDieRollsContainer.find(".active").removeClass("active");

                        $(this).addClass("active");

                        playerTurnOptionsConfirmButtonContainer.prop("disabled", false);
                        playerTurnOptionsConfirmButtonContainer.removeClass("disabled");
                    }
                });
            }

            playerTurnOptionsConfirmButtonContainer.on("click", function ()
            {
                currentDieRollsContainer.prev().nextAll().remove();

                analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1][playerTurnCharacterIndex]["TurnAfterRollData"].push
                (
                    {
                        DieRollValue: playerTurnDieRollValue
                    }
                );

                $("#board-subpanel > div:first-of-type").append
                (
                    "<div id=\"board-subpanel-spaces-remaining\">" +
                        "<div class=\"die-rolls\"></div>" +
                    "</div>"
                );

                renderDie($("#board-subpanel-spaces-remaining > .die-rolls"), playerTurnDieRollValue);

                addLogEntry("Rolled " + playerTurnDieRollValue + ".");

                movePlayerAroundMap(playerTurnCharacterTreeGraph, playerTurnDieRollValue);
            });

            playerTurnContainer.find("button[name=\"cancel\"]").on("click", displayPlayerTurnOptions);

            $("#settings-panel").scrollTop($("#settings-panel").prop("scrollHeight"));
        });

        let playerOwnShopsFlag = playerTurnCharacterBeforeRollData["OwnedShopIndices"].length > 0;

        let playerTurnOptionAuctionButton = playerTurnConfirmationActionsContainer.find("button[name=\"auction\"]");

        let playerTurnOptionAuctionEnableFlag = playerOwnShopsFlag;

        playerTurnOptionAuctionButton.prop("disabled", !playerTurnOptionAuctionEnableFlag);
        playerTurnOptionAuctionButton.toggleClass("disabled", !playerTurnOptionAuctionEnableFlag);

        playerTurnOptionAuctionButton.on("click", function ()
        {
            playerTurnConfirmationActionsContainer.remove();
        });

        let playerTurnOptionSellShopButton = playerTurnConfirmationActionsContainer.find("button[name=\"sell-shop\"]");

        let playerTurnOptionSellShopEnableFlag = playerOwnShopsFlag;

        playerTurnOptionSellShopButton.prop("disabled", !playerTurnOptionSellShopEnableFlag);
        playerTurnOptionSellShopButton.toggleClass("disabled", !playerTurnOptionSellShopEnableFlag);

        playerTurnOptionSellShopButton.on("click", function ()
        {
            playerTurnConfirmationActionsContainer.remove();
        });

        let opponentsCharacterIndices = Array.from(Array(analyzerData["CharacterData"]["PlayerData"].length).keys());

        opponentsCharacterIndices.splice(playerTurnCharacterIndex, 1);

        let opponentsOwnShopsFlag = false;

        for (let currentOpponentCharacterIndex of opponentsCharacterIndices)
        {
            if (analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1][currentOpponentCharacterIndex]["TurnBeforeRollCurrentData"]["OwnedShopIndices"].length > 0)
            {
                opponentsOwnShopsFlag = true;

                break;
            }
        }

        let playerTurnOptionBuyShopButton = playerTurnConfirmationActionsContainer.find("button[name=\"buy-shop\"]");

        let playerTurnOptionBuyShopEnableFlag = opponentsOwnShopsFlag && playerTurnCharacterBeforeRollData["ReadyCash"] + playerTurnCharacterBeforeRollData["TotalStockValue"] > 0;

        playerTurnOptionBuyShopButton.prop("disabled", !playerTurnOptionBuyShopEnableFlag);
        playerTurnOptionBuyShopButton.toggleClass("disabled", !playerTurnOptionBuyShopEnableFlag);

        playerTurnOptionBuyShopButton.on("click", function ()
        {
            playerTurnConfirmationActionsContainer.remove();
        });

        let playerTurnOptionExchangeShopsButton = playerTurnConfirmationActionsContainer.find("button[name=\"exchange-shops\"]");

        let playerTurnOptionExchangeShopsEnableFlag = playerTurnOptionSellShopEnableFlag && playerTurnOptionBuyShopEnableFlag;

        playerTurnOptionExchangeShopsButton.prop("disabled", !playerTurnOptionExchangeShopsEnableFlag);
        playerTurnOptionExchangeShopsButton.toggleClass("disabled", !playerTurnOptionExchangeShopsEnableFlag);

        playerTurnOptionExchangeShopsButton.on("click", function ()
        {
            playerTurnConfirmationActionsContainer.remove();
        });

        let playerTurnOptionRenovateVacantPlotButton = playerTurnConfirmationActionsContainer.find("button[name=\"renovate-vacant-plot\"]");

        let playerTurnOptionRenovateVacantPlotEnableFlag = false;

        playerTurnOptionRenovateVacantPlotButton.prop("disabled", !playerTurnOptionRenovateVacantPlotEnableFlag);
        playerTurnOptionRenovateVacantPlotButton.toggleClass("disabled", !playerTurnOptionRenovateVacantPlotEnableFlag);

        playerTurnOptionRenovateVacantPlotButton.on("click", function ()
        {
            playerTurnConfirmationActionsContainer.remove();
        });

        let playerTurnOptionSellStocksButton = playerTurnConfirmationActionsContainer.find("button[name=\"sell-stocks\"]");

        playerTurnOptionSellStocksButton.parent().toggle(analyzerData["GameData"]["RuleData"]["Name"] === "Standard");

        if (analyzerData["GameData"]["RuleData"]["Name"] === "Standard")
        {
            let playerTurnOptionSellStocksEnableFlag = playerTurnCharacterBeforeRollData["TotalStockValue"] > 0;

            playerTurnOptionSellStocksButton.prop("disabled", !playerTurnOptionSellStocksEnableFlag);
            playerTurnOptionSellStocksButton.toggleClass("disabled", !playerTurnOptionSellStocksEnableFlag);

            playerTurnOptionSellStocksButton.on("click", function ()
            {
                playerTurnConfirmationActionsContainer.remove();
            });
        }

        $("#settings-panel").scrollTop($("#settings-panel").prop("scrollHeight"));
    }

    function initializeTurns()
    {
        $("#settings-content").append("<div id=\"turns\"></div>");

        for (let i = 0; i < analyzerData["GameData"]["TurnData"].length; ++i)
        {
            displayNewTurn(i);

            for (let j = 0; j < analyzerData["CharacterData"]["PlayerData"].length; ++j)
            {
                $("#turns > div:last-of-type").append("<div></div>");

                if (analyzerData["GameData"]["TurnData"].length - 1 === i && playerTurnCharacterIndex === j)
                    break;

                $("#turns > div:last-of-type > div:last-of-type").append
                (
                    "<div class=\"player-information\" style=\"background-color: #" + analyzerData["CharacterData"]["PlayerData"][j]["ColorData"]["GameColor"] + ";\">" + renderPlayerContainer(j) + "</div>" +

                    "<div class=\"logs\">" +

                        $.map(analyzerData["GameData"]["TurnData"][i][j]["Logs"], function (value)
                        {
                            return "<div>" + value + "</div>";
                        }).join("") +

                    "</div>"
                );
            }
        }

        startNextPlayerTurn();
    }

    function startNextPlayerTurn()
    {
        initializeTreeGraphPlayerTurnCharacter();

        $("#board-subpanel-spaces > div > .space-information > div:first-of-type > div:first-of-type > .die-rolls").empty();

        traverseTreeGraphDieRollOptionsSpaceInformation(playerTurnCharacterTreeGraph, 0);

        analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1][playerTurnCharacterIndex]["TurnAfterRollData"] = [];

        animateCharacterMarkerFlag = true;

        animateCurrentPlayerCharacterMarker();

        $("#turns > div:last-of-type > div:last-of-type").append
        (
            "<div class=\"player-information\" style=\"background-color: #" + analyzerData["CharacterData"]["PlayerData"][playerTurnCharacterIndex]["ColorData"]["GameColor"] + ";\">" + renderPlayerContainer(playerTurnCharacterIndex) + "</div>" +

            "<div class=\"logs\"></div>"
        );

        displayPlayerTurnOptions();
    }

    $("#standings-subpanel").hide();

    settingsContainer.append(loadingDisplay());

    ajaxCall
    (
        "POST",
        "Startup",
        {
            id: URL_QUERY_PARAMETERS.get("id")
        }
    ).done(function (response)
    {
        settingsContainer.empty().append("<div id=\"settings-content\"></div>");

        alertNotificationMessageDisplay(response["AlertData"]);

        if (!response["Error"])
        {
            let JSONResponse = JSON.parse(response["HTMLResponse"]);

            analyzerData = JSONResponse["Data"];

            $("#settings-content").append(JSONResponse["Response"]);

            if (analyzerData["GameData"] === null)
            {
                loadGameData();

                return;
            }

            if (analyzerData["CharacterData"] === null)
            {
                initializeDetermineCharacterSettings();

                return;
            }

            initializeGameSetup();

            $("#settings-panel").scrollTop($("#settings-panel").prop("scrollHeight"));
        }
    });
});
