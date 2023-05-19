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
            x: 1/2,
            y: 1/2
        }
    ],
    [
        {
            x: 1/6,
            y: 1/6
        },
        {
            x: 5/6,
            y: 5/6
        }
    ],
    [
        {
            x: 1/6,
            y: 1/6
        },
        {
            x: 1/2,
            y: 1/2
        },
        {
            x: 5/6,
            y: 5/6
        }
    ],
    [
        {
            x: 1/6,
            y: 1/6
        },
        {
            x: 5/6,
            y: 1/6
        },
        {
            x: 1/6,
            y: 5/6
        },
        {
            x: 5/6,
            y: 5/6
        }
    ],
    [
        {
            x: 1/6,
            y: 1/6
        },
        {
            x: 5/6,
            y: 1/6
        },
        {
            x: 1/2,
            y: 1/2
        },
        {
            x: 1/6,
            y: 5/6
        },
        {
            x: 5/6,
            y: 5/6
        }
    ],
    [
        {
            x: 1/6,
            y: 1/6
        },
        {
            x: 5/6,
            y: 1/6
        },
        {
            x: 1/6,
            y: 1/2
        },
        {
            x: 5/6,
            y: 1/2
        },
        {
            x: 1/6,
            y: 5/6
        },
        {
            x: 5/6,
            y: 5/6
        }
    ],
    [
        {
            x: 1/6,
            y: 1/6
        },
        {
            x: 5/6,
            y: 1/6
        },
        {
            x: 1/6,
            y: 1/2
        },
        {
            x: 1/2,
            y: 1/2
        },
        {
            x: 5/6,
            y: 1/2
        },
        {
            x: 1/6,
            y: 5/6
        },
        {
            x: 5/6,
            y: 5/6
        }
    ],
    [
        {
            x: 1/6,
            y: 1/6
        },
        {
            x: 5/6,
            y: 1/6
        },
        {
            x: 1/6,
            y: 1/2
        },
        {
            x: 1/2,
            y: 1/3
        },
        {
            x: 1/2,
            y: 2/3
        },
        {
            x: 1/6,
            y: 1/2
        },
        {
            x: 1/6,
            y: 5/6
        },
        {
            x: 5/6,
            y: 5/6
        }
    ]
];
const ALERT_STATUS_MESSAGES =
{
    DATABASE_SAVE_LOAD:
    {
        Type: "alert-primary",
        Title: "Saving to database..."
    },
    DATABASE_SAVE_SUCCESS:
    {
        Type: "alert-success",
        Title: "Saved to database..."
    },
    DATABASE_SAVE_ERROR:
    {
        Type: "alert-warning",
        Title: "Cannot save to database..."
    },
    RESET_TURN_SUCCESS:
    {
        Type: "alert-success",
        Title: "Successfully resetted to turn {turn-number}..."
    },
    RESET_TURN_ERROR:
    {
        Type: "alert-danger",
        Title: "Cannot reset to turn {turn-number}..."
    }
};
const ANIMATE_ACTIVE_STATES =
{
    CharacterMarker:
    {
        Player: [],
        CameoCharacter: []
    },
    Suits: [],
    Shops: []
};

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

    function alertStatusMessageDisplay(alert)
    {
        if (alert !== null)
        {
            settingsContainer.find(".alert.status").remove();

            settingsContainer.append
            (
                "<div class=\"alert " + alert["Type"] + " status\">" +
                    "<div>" +
                        "<strong>" + alert["Title"] + "</strong>" +
                    "</div>" +
                "</div>"
            );

            let alertStatusContainer = settingsContainer.find(".alert.status");

            alertStatusContainer.animate({ opacity: 0 }, 3000, function ()
            {
                alertStatusContainer.remove();
            });
        }
    }

    function createConfirmationActions(buttonProperties)
    {
        return  "<div class=\"confirmation-actions center-items\">" +

                    $.map(buttonProperties, function (value)
                    {
                        return  "<div>" +
                                    "<button type=\"button\" class=\"" + value["Class"] + "\" name=\"" + value["Name"] + "\">" + value["Value"] + "</button>" +
                                "</div>";
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

    function loadGameSettings()
    {
        let gameSelectionRulesSelect = $("#game-selection > div:first-of-type select");

        gameSelectionRulesSelect.on("change", function ()
        {
            analyzerData["GameSettingsData"]["RuleData"]["ID"] = Number($(this).val());
        });

        let gameSelectionBoardsSelect = $("#game-selection > div:nth-of-type(2) select");

        gameSelectionBoardsSelect.on("change", function ()
        {
            analyzerData["GameSettingsData"]["BoardData"]["ID"] = Number($(this).val());
        });

        let gameSelectionColorSelectionContainer = $("#game-selection > div:nth-of-type(3) > .color-selection");

        gameSelectionColorSelectionContainer.find("input[type=\"hidden\"]").each(function ()
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
                if (!gameSelectionColorSelectionContainer.hasClass("disabled") && analyzerData["GameSettingsData"]["ColorData"]["ID"] !== colorSelectionInputData["ColorID"])
                {
                    analyzerData["GameSettingsData"]["ColorData"]["ID"] = colorSelectionInputData["ColorID"];

                    currentColorContainer.parent().find(".selected").removeClass("selected");

                    $(this).addClass("selected");
                }
            });
        });

        let gameSelectionConfirmButton = $("#game-selection > div:last-of-type button[name=\"confirm\"]");

        let gameSelectionFieldChanges = function (state)
        {
            gameSelectionConfirmButton.toggleClass("disabled", !state);
            gameSelectionConfirmButton.prop("disabled", !state);

            gameSelectionRulesSelect.toggleClass("disabled", !state);
            gameSelectionRulesSelect.prop("disabled", !state);

            gameSelectionBoardsSelect.toggleClass("disabled", !state);
            gameSelectionBoardsSelect.prop("disabled", !state);

            gameSelectionColorSelectionContainer.toggleClass("disabled", !state);
        };

        gameSelectionConfirmButton.on("click", function ()
        {
            gameSelectionFieldChanges(false);

            alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_LOAD"]);

            $("#settings-content").append("<div>" + loadingDisplay() + "</div>");

            ajaxCall
            (
                "POST",
                "SaveGameSettings",
                analyzerData
            ).done(function (response)
            {
                if (response["Error"])
                {
                    alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_ERROR"]);

                    gameSelectionFieldChanges(true);

                    return;
                }

                alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_SUCCESS"]);

                gameSelectionConfirmButton.remove();

                let JSONResponse = JSON.parse(response["HTMLResponse"]);

                analyzerData["GameSettingsData"] = JSONResponse["Data"]["GameSettingsData"];
                analyzerData["CharacterData"] = JSONResponse["Data"]["CharacterData"];

                $("#settings-content").append(JSONResponse["Response"]);

                initializeTurnOrderDeterminationSettings();
            }).fail(function ()
            {
                alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_ERROR"]);

                gameSelectionFieldChanges(true);
            }).always(function ()
            {
                $("#settings-content").find(".loading").parent().remove();
            });
        });
    }

    function initializeTurnOrderDeterminationSettings()
    {
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

            if (!$("#player-turn-determination").hasClass("disabled") && (playerTurnDeterminationValues[playerTurnDeterminationCharacterIndex][playerTurnDeterminationColumnIndex] === null || playerTurnDeterminationValues[playerTurnDeterminationCharacterIndex][playerTurnDeterminationColumnIndex] !== playerTurnDeterminationColumnValue))
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

        let playerTurnDeterminationFieldChanges = function (state)
        {
            playerTurnDeterminationConfirmButton.toggleClass("disabled", !state);
            playerTurnDeterminationConfirmButton.prop("disabled", !state);

            $("#player-turn-determination").toggleClass("disabled", !state);
        };

        playerTurnDeterminationConfirmButton.on("click", function ()
        {
            for (let i = 0; i < playerTurnDeterminationSumValues.length; ++i)
                analyzerData["CharacterData"]["PlayerData"][i]["TurnOrderValue"] = playerTurnDeterminationSumValues[i];

            playerTurnDeterminationFieldChanges(false);

            alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_LOAD"]);

            $("#settings-content").append("<div>" + loadingDisplay() + "</div>");

            ajaxCall
            (
                "POST",
                "SaveTurnOrderDeterminationSettings",
                analyzerData
            ).done(function (response)
            {
                if (response["Error"])
                {
                    alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_ERROR"]);

                    playerTurnDeterminationFieldChanges(true);

                    return;
                }

                alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_SUCCESS"]);

                playerTurnDeterminationConfirmButton.remove();

                analyzerData["CharacterData"]["PlayerData"].sort((a, b) => -1 * (a["TurnOrderValue"] - b["TurnOrderValue"]));

                let JSONResponse = JSON.parse(response["HTMLResponse"]);

                $.extend
                (
                    true,
                    analyzerData,
                    {
                        GameSettingsData:
                        {
                            TurnData: JSONResponse["Data"]["GameSettingsData"]["TurnData"]
                        },
                        CharacterData:
                        {
                            PlayerData: $.map(JSONResponse["Data"]["CharacterData"]["PlayerData"], function (value)
                            {
                                return {
                                    ColorData: value["ColorData"]
                                };
                            })
                        },
                        SpaceData: JSONResponse["Data"]["SpaceData"],
                        SpaceTypeData: JSONResponse["Data"]["SpaceTypeData"],
                        ShopData: JSONResponse["Data"]["ShopData"],
                        DistrictData: JSONResponse["Data"]["DistrictData"]
                    }
                );

                initializeGameSetup();
            }).fail(function ()
            {
                alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_ERROR"]);

                playerTurnDeterminationFieldChanges(true);
            }).always(function ()
            {
                $("#settings-content").find(".loading").parent().remove();
            });
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

    function savePreRollData(characterIndices)
    {
        let turnData = analyzerData["GameSettingsData"]["TurnData"][analyzerData["GameSettingsData"]["TurnData"].length - 1];

        alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_LOAD"]);

        ajaxCall
        (
            "POST",
            "SavePreRollTurnData",
            {
                PreRollsRecords: $.map(characterIndices, function (value)
                {
                    let turnCharacterData = turnData[value]["TurnPlayerData"];

                    let turnCharacterRollData = turnCharacterData[turnCharacterData.length - 1];

                    return {
                        TurnIteratorID: turnCharacterRollData["TurnIteratorID"],
                        SpaceIDCurrent: analyzerData["SpaceData"][turnCharacterRollData["SpaceIndexCurrent"]]["ID"],
                        SpaceIDFrom: analyzerData["SpaceData"][turnCharacterRollData["SpaceIndexFrom"]]["ID"],
                        LayoutIndex: layoutIndex,
                        Level: turnCharacterRollData["Level"],
                        Placing: turnCharacterRollData["Placing"],
                        ReadyCash: turnCharacterRollData["ReadyCash"],
                        TotalShopValue: turnCharacterRollData["TotalShopValue"],
                        TotalStockValue: turnCharacterRollData["TotalStockValue"],
                        NetWorth: turnCharacterRollData["NetWorth"],
                        OwnedShopIndices: JSON.stringify(turnCharacterRollData["OwnedShopIndices"]),
                        TotalSuitCards: turnCharacterRollData["TotalSuitCards"],
                        CollectedSuits: JSON.stringify(turnCharacterRollData["CollectedSuits"]),
                        ArcadeIndex: turnCharacterRollData["ArcadeIndex"],
                        DieRollRestrictions: turnCharacterRollData["DieRollRestrictions"] !== null ? JSON.stringify(turnCharacterRollData["DieRollRestrictions"]) : null
                    };
                })
            }
        ).done(function (response)
        {
            if (response["Error"])
            {
                alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_ERROR"]);

                return;
            }

            alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_SUCCESS"]);
        }).fail(function ()
        {
            alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_ERROR"]);
        });
    }

    let playerTurnCharacterIndex = 0;

    let playerTurnCharacterRollData = null;

    let layoutIndex = 0;

    let mouseCoordinates =
    {
        x: 0,
        y: 0
    };

    function animateChangeState(element, states, index, active)
    {
        let stateIndex = states.indexOf(index);

        if (active)
        {
            if (stateIndex === -1)
            {
                states.push(index);

                animateFlashing(element, states, index);
            }

            return;
        }

        if (stateIndex > -1)
            states.splice(stateIndex, 1);
    }

    function animateFlashing(element, states, index)
    {
        $(element).animate
        (
            {
                opacity: 0.25
            },
            {
                duration: 1500,
                step: function ()
                {
                    if (states.indexOf(index) === -1)
                    {
                        $(element).css({ opacity: 1 });

                        $(this).stop();

                        return;
                    }
                },
                complete: function ()
                {
                    $(element).css({ opacity: 1 });

                    animateFlashing(element, states, index);
                }
            }
        );
    }

    function initializeGameSetup()
    {
        for (let i = 0; i < analyzerData["CharacterData"]["PlayerData"].length; ++i)
        {
            let currentPlayerTurnCharacterData = analyzerData["GameSettingsData"]["TurnData"][analyzerData["GameSettingsData"]["TurnData"].length - 1][i]["TurnPlayerData"]

            if (currentPlayerTurnCharacterData[currentPlayerTurnCharacterData.length - 1]["DieRollValue"] === null)
            {
                playerTurnCharacterIndex = i;

                break;
            }
        }

        let playerTurnCharacterData = analyzerData["GameSettingsData"]["TurnData"][analyzerData["GameSettingsData"]["TurnData"].length - 1][playerTurnCharacterIndex]["TurnPlayerData"];

        playerTurnCharacterRollData = playerTurnCharacterData[playerTurnCharacterData.length - 1];

        initializeStandings();

        layoutIndex = playerTurnCharacterRollData["LayoutIndex"];

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
                "<div class=\"player-information\" style=\"background-color: #" + analyzerData["CharacterData"]["PlayerData"][i]["ColorData"]["CharacterColor"] + ";\">" +
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

            standingsContainer.children().last().children().eq(2).children().eq(2).toggle(analyzerData["GameSettingsData"]["RuleData"]["Name"] === "Standard");

            updatePlayerStandings(i);
        }

        $("#standings-subpanel").show();
    }

    function updatePlayerStandings(playerIndex)
    {
        let playerContainer = $("#standings-subpanel > div:last-of-type > div:nth-of-type(" + (playerIndex + 1) + ")");

        playerContainer.children().first().empty().append
        (
            "<span>" + playerTurnCharacterRollData["Placing"] + "</span>" +

            "<span>" +
                "<sup>" + ordinalNumberSuffix(playerTurnCharacterRollData["Placing"]) + "</sup>" +
            "</span>"
        );

        let playerStatsContainer = playerContainer.children().eq(2);

        playerStatsContainer.children().first().children().last().text(playerTurnCharacterRollData["ReadyCash"]);
        playerStatsContainer.children().eq(1).children().last().text(playerTurnCharacterRollData["TotalShopValue"]);

        if (analyzerData["GameSettingsData"]["RuleData"]["Name"] === "Standard")
            playerStatsContainer.children().eq(2).children().last().text(playerTurnCharacterRollData["TotalStockValue"]);

        playerStatsContainer.children().last().children().last().text(playerTurnCharacterRollData["NetWorth"]);

        playerContainer.children().last().empty().append
        (
            "<div class=\"suit-card\">" +
                "<div>" +
                    "<div></div>" +

                    "<div>" + playerTurnCharacterRollData["TotalSuitCards"] + "</div>" +
                "</div>" +
            "</div>"
        );

        let playerSuitCardContainer = playerContainer.children().last().find(".suit-card");

        playerSuitCardContainer.children().first().toggle(playerTurnCharacterRollData["TotalSuitCards"] > 0);

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
                    "<span class=\"fas fa-" + currentSuitData["Icon"] + "\" style=\"color: #" + (playerTurnCharacterRollData["CollectedSuits"].indexOf(currentSuitData["Icon"]) > -1 ? currentSuitData["Color"] : "333") + ";\"></span>" +
                "</div>"
            );
        }

        animateChangeState($("#standings-subpanel > div:last-of-type > div:nth-of-type(" + (playerIndex + 1) + ") > div:last-of-type > div"), ANIMATE_ACTIVE_STATES["Suits"], playerIndex, playerTurnCharacterRollData["CollectedSuits"].length + playerTurnCharacterRollData["TotalSuitCards"] >= SUIT_DATA.length);
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
        playerTurnCharacterTreeGraph = createTreeGraphNode
        (
            {
                SpaceIndexCurrent: playerTurnCharacterRollData["SpaceIndexCurrent"],
                SpaceIndexFrom: playerTurnCharacterRollData["SpaceIndexFrom"],
                DieRollValue: 0
            }
        );

        let maxPlayerTurnEligibleDieRollValue = analyzerData["GameSettingsData"]["BoardData"]["MaxDieRoll"];

        if (playerTurnCharacterRollData["DieRollRestrictions"] !== null)
            maxPlayerTurnEligibleDieRollValue = playerTurnCharacterRollData["DieRollRestrictions"];

        let playerTurnCharacterTreeGraphPreviousParentNodes = [ playerTurnCharacterTreeGraph ];

        for (let currentPlayerTurnEligibleDieRollValue = 1; currentPlayerTurnEligibleDieRollValue <= maxPlayerTurnEligibleDieRollValue; ++currentPlayerTurnEligibleDieRollValue)
        {
            let playerTurnCharacterTreeGraphCurrentParentNodes = [];

            for (let currentPlayerTurnCharacterTreeGraphPreviousParentNode of playerTurnCharacterTreeGraphPreviousParentNodes)
            {
                let currentSpaceConstraintData = $.grep(analyzerData["SpaceData"][currentPlayerTurnCharacterTreeGraphPreviousParentNode["Node"]["SpaceIndexCurrent"]]["SpaceConstraintData"], function (value)
                {
                    return layoutIndex === value["LayoutIndex"] && (currentPlayerTurnCharacterTreeGraphPreviousParentNode["Node"]["SpaceIndexFrom"] === null || currentPlayerTurnCharacterTreeGraphPreviousParentNode["Node"]["SpaceIndexFrom"] === value["SpaceIndexFrom"]);
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

        maxCenterXFactor = analyzerData["SpaceData"][0]["SpaceLayoutData"][layoutIndex]["CenterXFactor"];
        maxCenterYFactor = analyzerData["SpaceData"][0]["SpaceLayoutData"][layoutIndex]["CenterYFactor"];

        for (let i = 1; i < analyzerData["SpaceData"].length; ++i)
        {
            let currentCenterXFactor = analyzerData["SpaceData"][i]["SpaceLayoutData"][layoutIndex]["CenterXFactor"];

            if (currentCenterXFactor > maxCenterXFactor)
                maxCenterXFactor = currentCenterXFactor;

            let currentCenterYFactor = analyzerData["SpaceData"][i]["SpaceLayoutData"][layoutIndex]["CenterYFactor"];

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
                    left: boardSubpanelOffsetX + SPACE_SQUARE_SIZE * (analyzerData["SpaceData"][i]["SpaceLayoutData"][layoutIndex]["CenterXFactor"] - 0.5) + "px",
                    top: boardSubpanelOffsetY + SPACE_SQUARE_SIZE * (analyzerData["SpaceData"][i]["SpaceLayoutData"][layoutIndex]["CenterYFactor"] - 0.5) + "px",
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

        let spaceData = analyzerData["SpaceData"][spaceIndex];

        let spaceTypeData = analyzerData["SpaceTypeData"][spaceData["SpaceTypeIndex"]];

        let spaceIconValue = spaceTypeData["Icon"];

        if (spaceIconValue === null)
        {
            if (spaceTypeData["Name"] === "suit")
                spaceIconValue = SUIT_DATA[SUIT_ORDER.indexOf(spaceData["AdditionalPropertiesData"]["SuitData"]["Name"])]["Icon"];
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

        if (analyzerData["GameSettingsData"]["RuleData"]["Name"] === "Standard")
            spaceInformationPlaceholders["BankData"]["StockInformation"] = ", and you can buy stocks as you pass through";

        if (spaceTypeData["Name"] === "shop")
        {
            spaceContainer.css({ borderColor: "#" + (spaceData["DistrictIndex"] !== null ? analyzerData["DistrictData"][spaceData["DistrictIndex"]]["Color"] : "F2D4A9") });

            let shopData = analyzerData["ShopData"][spaceData["ShopIndex"]];

            if (spaceData["AdditionalPropertiesData"]["ShopData"]["OwnerCharacterIndex"] !== null)
            {
                spaceSquareIconContainer.css({ color: "#" + analyzerData["CharacterData"]["PlayerData"][spaceData["AdditionalPropertiesData"]["ShopData"]["OwnerCharacterIndex"]]["ColorData"]["CharacterColor"] });

                spaceSquareContainer.append
                (
                    "<div class=\"shop-owned-price\">" +
                        "<div>" + spaceData["AdditionalPropertiesData"]["ShopData"]["Price"] + "</div>" +
                    "</div>"
                );

                spaceInformationPlaceholders["ShopData"]["MaxCapital"] =
                    "<div>" +
                        "<div>Max. capital</div>" +

                        "<div>" + (2 * shopData["Value"]) + "</div>" +
                    "</div>";
            }
            else
            {
                spaceSquareContainer.append
                (
                    "<div class=\"shop-unowned-value\">" +
                        "<div>" + shopData["Value"] + "</div>" +
                    "</div>"
                );
            }

            spaceInformationPlaceholders["ShopData"]["ShopName"] = "<div>" + shopData["Name"] + "</div>";

            spaceInformationPlaceholders["ShopData"]["ShopValue"] =
                "<div>" +
                    "<div>Shop value</div>" +

                    "<div>" + shopData["Value"] + "</div>" +
                "</div>";

            spaceInformationPlaceholders["ShopData"]["ShopPrice"] =
                "<div>" +
                    "<div>Shop prices</div>" +

                    "<div>" + spaceData["AdditionalPropertiesData"]["ShopData"]["Price"] + "</div>" +
                "</div>";
        }
        else if (spaceTypeData["Name"] === "suit")
        {
            spaceSquareIconContainer.css({ color: "#" + SUIT_DATA[SUIT_ORDER.indexOf(spaceData["AdditionalPropertiesData"]["SuitData"]["Name"])]["Color"] });

            spaceInformationPlaceholders["SuitData"]["SuitIcon"] = "<span class=\"fas fa-" + SUIT_DATA[SUIT_ORDER.indexOf(spaceData["AdditionalPropertiesData"]["SuitData"]["Name"])]["Icon"] + "\" style=\"color: #" + SUIT_DATA[SUIT_ORDER.indexOf(spaceData["AdditionalPropertiesData"]["SuitData"]["Name"])]["Color"] + ";\"></span>";
        }

        spaceSquareContainer.append("<div class=\"character-markers\"></div>");

        let spaceCharacterMarkerIndices = $.map(analyzerData["CharacterData"]["PlayerData"], function (value, index)
        {
            let currentPlayerTurnCharacterData = analyzerData["GameSettingsData"]["TurnData"][analyzerData["GameSettingsData"]["TurnData"].length - 1][index]["TurnPlayerData"];

            if (currentPlayerTurnCharacterData[currentPlayerTurnCharacterData.length - 1]["SpaceIndexCurrent"] === spaceIndex)
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
                            backgroundColor: "#" + analyzerData["CharacterData"]["PlayerData"][spaceCharacterMarkerIndices[i]]["ColorData"]["CharacterColor"]
                        }
                    )
                );
            }
        }

        spaceContainer.append
        (
            "<div class=\"popup-dialog space-information" + (spaceTypeData["Name"] === "shop" ? " space-shop" : "") + "\">" +
                "<div></div>" +
            "</div>"
        );

        let spaceInformationContainer = spaceContainer.children().last().children().first();

        spaceInformationContainer.append
        (
            "<div>" +
                "<div class=\"die-rolls\"></div>" +

                "<div>" + (spaceTypeData["Title"] !== null ? spaceTypeData["Title"] : "") + "</div>" +
            "</div>"
        );

        spaceInformationContainer.append("<div>" + spaceTypeData["Description"].replace("{stock-information}", spaceInformationPlaceholders["BankData"]["StockInformation"]).replace("{shop-name}", spaceInformationPlaceholders["ShopData"]["ShopName"]).replace("{shop-value}", spaceInformationPlaceholders["ShopData"]["ShopValue"]).replace("{shop-price}", spaceInformationPlaceholders["ShopData"]["ShopPrice"]).replace("{max-capital}", spaceInformationPlaceholders["ShopData"]["MaxCapital"]).replace("{suit-icon}", spaceInformationPlaceholders["SuitData"]["SuitIcon"]) + "</div>");
    }

    function movePlayerAroundMap(characterTreeGraph, spacesRemaining)
    {
        if (spacesRemaining === 0)
        {
            let spaceContainer = $("#board-subpanel-spaces > div:nth-of-type(" + (characterTreeGraph["Node"]["SpaceIndexCurrent"] + 1) + ")");

            let spaceIndexCurrentCopy = playerTurnCharacterRollData["SpaceIndexCurrent"];

            playerTurnCharacterRollData["SpaceIndexCurrent"] = characterTreeGraph["Node"]["SpaceIndexCurrent"];

            updateMapSpace(spaceIndexCurrentCopy);
            updateMapSpace(characterTreeGraph["Node"]["SpaceIndexCurrent"]);

            let spaceData = analyzerData["SpaceData"][characterTreeGraph["Node"]["SpaceIndexCurrent"]];

            let spaceTypeData = analyzerData["SpaceTypeData"][spaceData["SpaceTypeIndex"]];

            let shopData = analyzerData["ShopData"][spaceData["ShopIndex"]];

            let logEntryMessage = "Landed on " + spaceContainer.find(".space-icon").html();

            if (spaceTypeData["Name"] === "shop")
            {
                let logEntryMessageProperties =
                [
                    shopData["Name"],
                    spaceData["AdditionalPropertiesData"]["ShopData"]["OwnerCharacterIndex"] !== null ? "Owned by " + analyzerData["CharacterData"]["PlayerData"][spaceData["AdditionalPropertiesData"]["ShopData"]["OwnerCharacterIndex"]]["Name"] : "Unowned"
                ];

                logEntryMessage += " (" + logEntryMessageProperties.join(", ") + ")";
            }

            addLogEntry(logEntryMessage);

            if (spaceTypeData["Name"] === "bank" && playerTurnCharacterRollData["NetWorth"] >= analyzerData["GameSettingsData"]["RuleData"]["NetWorthThreshold"])
            {
                endGame();

                return;
            }

            let endSpaceEvents = function ()
            {
                animateChangeState($("#board-subpanel-spaces > div:nth-of-type(" + (playerTurnCharacterRollData["SpaceIndexCurrent"] + 1) + ") > div:first-of-type > .character-markers > div:first-of-type"), ANIMATE_ACTIVE_STATES["CharacterMarker"]["Player"], playerTurnCharacterIndex, false);

                $("#board-subpanel-spaces-remaining").remove();

                savePreRollData([ playerTurnCharacterIndex ]);

                alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_LOAD"]);

                ajaxCall
                (
                    "POST",
                    "SavePostRollTurnData",
                    {
                        PostRollsRecord:
                        {
                            TurnIteratorID: playerTurnCharacterRollData["TurnIteratorID"],
                            SpaceIDLandedOn: analyzerData["SpaceData"][playerTurnCharacterRollData["SpaceIndexCurrent"]]["ID"],
                            DieRollValue: playerTurnCharacterRollData["DieRollValue"],
                            Logs: JSON.stringify(playerTurnCharacterRollData["Logs"])
                        }
                    }
                ).done(function (response)
                {
                    if (response["Error"])
                    {
                        alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_ERROR"]);

                        return;
                    }

                    alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_SUCCESS"]);

                    playerTurnCharacterIndex = (playerTurnCharacterIndex + 1) % analyzerData["CharacterData"]["PlayerData"].length;

                    let playerTurnCharacterData = analyzerData["GameSettingsData"]["TurnData"][analyzerData["GameSettingsData"]["TurnData"].length - 1][playerTurnCharacterIndex]["TurnPlayerData"];

                    playerTurnCharacterRollData = playerTurnCharacterData[playerTurnCharacterData.length - 1];

                    if (playerTurnCharacterIndex === 0)
                    {
                        alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_LOAD"]);

                        ajaxCall
                        (
                            "POST",
                            "NewTurn",
                            {
                                TurnIteratorsRecord:
                                {
                                    AnalyzerInstanceID: analyzerData["AnalyzerInstanceID"],
                                    TurnNumber: analyzerData["GameSettingsData"]["TurnData"].length + 1
                                }
                            }
                        ).done(function (response)
                        {
                            if (response["Error"])
                            {
                                alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_ERROR"]);

                                return;
                            }

                            let JSONResponse = JSON.parse(response["HTMLResponse"]);

                            analyzerData["GameSettingsData"]["TurnData"].push(JSONResponse["Data"]);

                            alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_SUCCESS"]);

                            displayNewTurn(analyzerData["GameSettingsData"]["TurnData"].length - 1);

                            $("#turns > div:last-of-type").append("<div></div>");

                            startNextPlayerTurn();
                        }).fail(function ()
                        {
                            alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_ERROR"]);
                        });

                        return;
                    }

                    $("#turns > div:last-of-type").append("<div></div>");

                    startNextPlayerTurn();
                }).fail(function ()
                {
                    alertStatusMessageDisplay(ALERT_STATUS_MESSAGES["DATABASE_SAVE_ERROR"]);
                });
            };

            endSpaceEvents();

            return;
        }

        let spaceTreeGraphPlayerEvents = function (spaceTreeGraph)
        {
            playerTurnCharacterRollData["SpaceIndexFrom"] = characterTreeGraph["Node"]["SpaceIndexCurrent"];

            let spaceTypeName = analyzerData["SpaceTypeData"][analyzerData["SpaceData"][spaceTreeGraph["Node"]["SpaceIndexCurrent"]]["SpaceTypeIndex"]]["Name"];

            if (spaceTypeName === "bank")
            {
                if (playerTurnCharacterRollData["NetWorth"] >= analyzerData["GameSettingsData"]["RuleData"]["NetWorthThreshold"])
                {
                    endGame();

                    return;
                }

                if (playerTurnCharacterRollData["CollectedSuits"].length + playerTurnCharacterRollData["TotalSuitCards"] >= SUIT_DATA.length)
                {
                    executePlayerPromotion();

                    if (playerTurnCharacterRollData["NetWorth"] >= analyzerData["GameSettingsData"]["RuleData"]["NetWorthThreshold"])
                    {
                        endGame();

                        return;
                    }
                }

                if (analyzerData["GameSettingsData"]["RuleData"]["Name"] === "Standard")
                {
                }
            }
            else if (spaceTypeName === "suit")
            {
                let spaceContainer = $("#board-subpanel-spaces > div:nth-of-type(" + (spaceTreeGraph["Node"]["SpaceIndexCurrent"] + 1) + ")");

                let spaceSuitAdditionalProperties = analyzerData["SpaceData"][spaceTreeGraph["Node"]["SpaceIndexCurrent"]]["AdditionalPropertiesData"]["SuitData"];

                let suitName = spaceSuitAdditionalProperties["Name"];

                if (playerTurnCharacterRollData["CollectedSuits"].indexOf(suitName) === -1)
                {
                    playerTurnCharacterRollData["CollectedSuits"].push(suitName);

                    addLogEntry("Picked up " + spaceContainer.find(".space-icon").html());

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
            if (characterTreeGraph["Node"]["SpaceIndexCurrent"] !== playerTurnCharacterRollData["SpaceIndexCurrent"])
            {
                let spaceIndexCurrentCopy = playerTurnCharacterRollData["SpaceIndexCurrent"];

                playerTurnCharacterRollData["SpaceIndexCurrent"] = characterTreeGraph["Node"]["SpaceIndexCurrent"];

                updateMapSpace(spaceIndexCurrentCopy);
                updateMapSpace(characterTreeGraph["Node"]["SpaceIndexCurrent"]);

                let boardSubpanelSpacesRemainingDieRollsContainer = $("#board-subpanel-spaces-remaining > .die-rolls");

                boardSubpanelSpacesRemainingDieRollsContainer.empty();

                renderDie(boardSubpanelSpacesRemainingDieRollsContainer, spacesRemaining);

                $("#board-subpanel-spaces > div > .space-information > div:first-of-type > div:first-of-type > .die-rolls").empty();

                traverseTreeGraphDieRollOptionsSpaceInformation(characterTreeGraph, playerTurnCharacterRollData["DieRollValue"] - spacesRemaining);
            }

            $("#board-subpanel > div:first-of-type").append("<div id=\"board-subpanel-direction-choices\"></div>");

            let sourceSpaceLayoutData = analyzerData["SpaceData"][characterTreeGraph["Node"]["SpaceIndexCurrent"]]["SpaceLayoutData"][layoutIndex];

            for (let currentPlayerSpaceTreeGraph of playerSpaceTreeGraphs)
            {
                $("#board-subpanel-direction-choices").append
                (
                    "<div>" +
                        "<span class=\"fas fa-right\"></span>" +
                    "</div>"
                );

                let currentArrowContainer = $("#board-subpanel-direction-choices > div:last-of-type");

                let destinationSpaceLayoutData = analyzerData["SpaceData"][currentPlayerSpaceTreeGraph["Node"]["SpaceIndexCurrent"]]["SpaceLayoutData"][layoutIndex];

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

                    currentArrowIcon.css({ fill: "#" + analyzerData["CharacterData"]["PlayerData"][playerTurnCharacterIndex]["ColorData"]["CharacterColor"] });

                    let currentArrowIconCopy = currentArrowIcon.clone();

                    currentArrowIconCopy.prependTo(currentArrowIcon.parent());

                    currentArrowIconCopy.css
                    (
                        {
                            fill: "transparent",
                            stroke: "#" + analyzerData["CharacterData"]["PlayerData"][playerTurnCharacterIndex]["ColorData"]["CharacterColor"],
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
        playerTurnCharacterRollData["Logs"].push(entry);

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
                $("#board-subpanel").empty().append("<div>No board loaded.</div>");

                $("#standings-subpanel").hide();

                $("#turns").remove();

                animateChangeState($("#board-subpanel-spaces > div:nth-of-type(" + (playerTurnCharacterRollData["SpaceIndexCurrent"] + 1) + ") > div:first-of-type > .character-markers > div:first-of-type"), ANIMATE_ACTIVE_STATES["CharacterMarker"]["Player"], playerTurnCharacterIndex, false);

                let alertStatusMessageResetTurn = function (type)
                {
                    let alertStatusMessage = $.extend(true, {}, ALERT_STATUS_MESSAGES[type]);

                    alertStatusMessage["Title"] = alertStatusMessage["Title"].replace("{turn-number}", turnIndex + 1);

                    alertStatusMessageDisplay(alertStatusMessage);
                };

                $("#settings-content").append("<div>" + loadingDisplay() + "</div>");

                ajaxCall
                (
                    "POST",
                    "ResetTurn",
                    {
                        TurnIteratorsRecord:
                        {
                            AnalyzerInstanceID: analyzerData["AnalyzerInstanceID"],
                            TurnNumber: turnIndex + 1
                        }
                    }
                ).done(function (response)
                {
                    if (response["Error"])
                    {
                        alertStatusMessageResetTurn("RESET_TURN_ERROR");

                        return;
                    }

                    let JSONResponse = JSON.parse(response["HTMLResponse"]);

                    analyzerData["GameSettingsData"]["TurnData"] = JSONResponse["Data"];

                    alertStatusMessageResetTurn("RESET_TURN_SUCCESS");

                    playerTurnCharacterIndex = 0;

                    let playerTurnCharacterData = analyzerData["GameSettingsData"]["TurnData"][analyzerData["GameSettingsData"]["TurnData"].length - 1][playerTurnCharacterIndex]["TurnPlayerData"];

                    playerTurnCharacterRollData = playerTurnCharacterData[playerTurnCharacterData.length - 1];

                    layoutIndex = playerTurnCharacterRollData["LayoutIndex"];
                }).fail(function ()
                {
                    alertStatusMessageResetTurn("RESET_TURN_ERROR");
                }).always(function ()
                {
                    $("#settings-content").find(".loading").parent().remove();

                    initializeStandings();

                    initializeMap();

                    initializeTurns();

                    animateChangeState($("#board-subpanel-spaces > div:nth-of-type(" + (playerTurnCharacterRollData["SpaceIndexCurrent"] + 1) + ") > div:first-of-type > .character-markers > div:first-of-type"), ANIMATE_ACTIVE_STATES["CharacterMarker"]["Player"], playerTurnCharacterIndex, true);
                });
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
                [
                    {
                        Name: "roll",
                        Value: "Roll",
                        Class: "btn btn-lg btn-primary"
                    },
                    {
                        Name: "auction",
                        Value: "Auction",
                        Class: "btn btn-lg btn-primary"
                    },
                    {
                        Name: "sell-shop",
                        Value: "Sell Shop",
                        Class: "btn btn-lg btn-primary"
                    },
                    {
                        Name: "buy-shop",
                        Value: "Buy Shop",
                        Class: "btn btn-lg btn-primary"
                    },
                    {
                        Name: "exchange-shops",
                        Value: "Exchange Shops",
                        Class: "btn btn-lg btn-primary"
                    },
                    {
                        Name: "renovate-vacant-plot",
                        Value: "Renovate Vacant Plot",
                        Class: "btn btn-lg btn-primary"
                    },
                    {
                        Name: "sell-stocks",
                        Value: "Sell Stocks",
                        Class: "btn btn-lg btn-primary"
                    }
                ]
            )
        );

        let playerTurnConfirmationActionsContainer = playerTurnContainer.find(".confirmation-actions");

        playerTurnConfirmationActionsContainer.find("button[name=\"roll\"]").on("click", function ()
        {
            playerTurnConfirmationActionsContainer.remove();

            playerTurnContainer.append
            (
                "<div class=\"die-rolls die-roll-selection\"></div>" +

                createConfirmationActions
                (
                    [
                        {
                            Name: "confirm",
                            Value: "Confirm",
                            Class: "btn btn-lg btn-primary"
                        },
                        {
                            Name: "cancel",
                            Value: "Cancel",
                            Class: "btn btn-lg btn-secondary"
                        }
                    ]
                )
            );

            let playerTurnOptionsConfirmButtonContainer = playerTurnContainer.find("button[name=\"confirm\"]");

            playerTurnOptionsConfirmButtonContainer.prop("disabled", true);
            playerTurnOptionsConfirmButtonContainer.addClass("disabled");

            let playerTurnEligibleDieRolls = Array.from(Array(analyzerData["GameSettingsData"]["BoardData"]["MaxDieRoll"]).keys(), n => n + 1);

            if (playerTurnCharacterRollData["DieRollRestrictions"] !== null)
                playerTurnEligibleDieRolls = playerTurnCharacterRollData["DieRollRestrictions"];

            let currentDieRollsContainer = playerTurnContainer.find(".die-rolls");

            let playerTurnDieRollValue = null;

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

                playerTurnCharacterRollData["DieRollValue"] = playerTurnDieRollValue;

                $("#board-subpanel > div:first-of-type").append
                (
                    "<div id=\"board-subpanel-spaces-remaining\">" +
                        "<div class=\"die-rolls\"></div>" +
                    "</div>"
                );

                renderDie($("#board-subpanel-spaces-remaining > .die-rolls"), playerTurnDieRollValue);

                addLogEntry("Rolled " + playerTurnDieRollValue);

                movePlayerAroundMap(playerTurnCharacterTreeGraph, playerTurnDieRollValue);
            });

            playerTurnContainer.find("button[name=\"cancel\"]").on("click", displayPlayerTurnOptions);

            $("#settings-panel").scrollTop($("#settings-panel").prop("scrollHeight"));
        });

        let playerOwnShopsFlag = playerTurnCharacterRollData["OwnedShopIndices"].length > 0;

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
            let currentOpponentTurnCharacterData = analyzerData["GameSettingsData"]["TurnData"][analyzerData["GameSettingsData"]["TurnData"].length - 1][currentOpponentCharacterIndex]["TurnPlayerData"];

            let currentOpponentTurnCharacterRollData = currentOpponentTurnCharacterData[currentOpponentTurnCharacterData.length - 1];

            if (currentOpponentTurnCharacterRollData["OwnedShopIndices"].length > 0)
            {
                opponentsOwnShopsFlag = true;

                break;
            }
        }

        let playerTurnOptionBuyShopButton = playerTurnConfirmationActionsContainer.find("button[name=\"buy-shop\"]");

        let playerTurnOptionBuyShopEnableFlag = opponentsOwnShopsFlag && playerTurnCharacterRollData["ReadyCash"] + playerTurnCharacterRollData["TotalStockValue"] > 0;

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

        playerTurnOptionSellStocksButton.parent().toggle(analyzerData["GameSettingsData"]["RuleData"]["Name"] === "Standard");

        if (analyzerData["GameSettingsData"]["RuleData"]["Name"] === "Standard")
        {
            let playerTurnOptionSellStocksEnableFlag = playerTurnCharacterRollData["TotalStockValue"] > 0;

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
        $("#turns").remove();

        $("#settings-content").append("<div id=\"turns\"></div>");

        for (let i = 0; i < analyzerData["GameSettingsData"]["TurnData"].length; ++i)
        {
            displayNewTurn(i);

            for (let j = 0; j < analyzerData["CharacterData"]["PlayerData"].length; ++j)
            {
                $("#turns > div:last-of-type").append("<div></div>");

                if (analyzerData["GameSettingsData"]["TurnData"].length - 1 === i && playerTurnCharacterIndex === j)
                    break;

                let currentPlayerTurnLogsData =
                {
                    TurnPlayerData: [],
                    TurnCameoCharactersData: []
                };

                for (let currentTurnPlayerData of analyzerData["GameSettingsData"]["TurnData"][i][j]["TurnPlayerData"])
                    currentPlayerTurnLogsData["TurnPlayerData"].push
                    (
                        "<div>" +

                            $.map(currentTurnPlayerData["Logs"], function (value)
                            {
                                return "<div>" + value + "</div>";
                            }).join("") +

                        "</div>"
                    );

                for (let currentTurnCameoCharactersData of analyzerData["GameSettingsData"]["TurnData"][i][j]["TurnCameoCharactersData"])
                    currentPlayerTurnLogsData["TurnCameoCharactersData"].push
                    (
                        "<div>" +

                            $.map(currentTurnCameoCharactersData["Logs"], function (value)
                            {
                                return "<div>" + value + "</div>";
                            }).join("") +

                        "</div>"
                    );

                $("#turns > div:last-of-type > div:last-of-type").append
                (
                    "<div class=\"player-information\" style=\"background-color: #" + analyzerData["CharacterData"]["PlayerData"][j]["ColorData"]["CharacterColor"] + ";\">" + renderPlayerContainer(j) + "</div>" +

                    "<div class=\"logs\">" +
                        currentPlayerTurnLogsData["TurnPlayerData"].join("") +

                        currentPlayerTurnLogsData["TurnCameoCharactersData"].join("") +
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

        playerTurnCharacterRollData["Logs"] = [];

        animateChangeState($("#board-subpanel-spaces > div:nth-of-type(" + (playerTurnCharacterRollData["SpaceIndexCurrent"] + 1) + ") > div:first-of-type > .character-markers > div:first-of-type"), ANIMATE_ACTIVE_STATES["CharacterMarker"]["Player"], playerTurnCharacterIndex, true);

        $("#turns > div:last-of-type > div:last-of-type").append
        (
            "<div class=\"player-information\" style=\"background-color: #" + analyzerData["CharacterData"]["PlayerData"][playerTurnCharacterIndex]["ColorData"]["CharacterColor"] + ";\">" + renderPlayerContainer(playerTurnCharacterIndex) + "</div>" +

            "<div class=\"logs\"></div>"
        );

        displayPlayerTurnOptions();
    }

    function updatePlayerStats(data)
    {
        let turnData = analyzerData["GameSettingsData"]["TurnData"][analyzerData["GameSettingsData"]["TurnData"].length - 1];

        let turnDataCopy = $.extend(true, [], turnData);

        for (let currentData of data)
        {
            let currentTurnCharacterData = turnData[currentData["CharacterIndex"]]["TurnPlayerData"];

            let currentTurnCharacterRollData = currentTurnCharacterData[currentTurnCharacterData.length - 1];

            currentTurnCharacterRollData["ReadyCash"] += currentData["ReadyCash"];
            currentTurnCharacterRollData["NetWorth"] += currentData["ReadyCash"];
        }

        let playerNetWorthOrder = $.map(turnData, function (value, index)
        {
            let turnCharacterData = value["TurnPlayerData"];

            let turnCharacterRollData = turnCharacterData[turnCharacterData.length - 1];

            return {
                CharacterIndex: index,
                NetWorth: turnCharacterRollData["NetWorth"]
            };
        }).sort((a, b) => -1 * (a["NetWorth"] - b["NetWorth"]));

        let previousNetWorthValue = null;
        let previousPlacingValue = null;

        let turnDataChangesCharacterIndices = [];

        for (let i = 0; i < playerNetWorthOrder.length; ++i)
        {
            if (previousNetWorthValue === null || previousNetWorthValue !== playerNetWorthOrder[i]["NetWorth"])
            {
                previousNetWorthValue = playerNetWorthOrder[i]["NetWorth"];
                previousPlacingValue = i + 1;
            }

            let currentTurnCharacterData = turnData[playerNetWorthOrder[i]["CharacterIndex"]]["TurnPlayerData"];

            let currentTurnCharacterRollData = currentTurnCharacterData[currentTurnCharacterData.length - 1];

            currentTurnCharacterRollData["Placing"] = previousPlacingValue;

            updatePlayerStandings(playerNetWorthOrder[i]["CharacterIndex"]);

            if (JSON.stringify(turnData[playerNetWorthOrder[i]["CharacterIndex"]]["TurnPlayerData"]) !== JSON.stringify(turnDataCopy[playerNetWorthOrder[i]["CharacterIndex"]]["TurnPlayerData"]))
                turnDataChangesCharacterIndices.push(playerNetWorthOrder[i]["CharacterIndex"]);
        }

        savePreRollData(turnDataChangesCharacterIndices);
    }

    function executePlayerPromotion()
    {
        let baseSalaryValue = analyzerData["GameSettingsData"]["BoardData"]["SalaryStart"];
        let promotionBonusValue = analyzerData["GameSettingsData"]["BoardData"]["SalaryIncrease"] * playerTurnCharacterRollData["Level"];
        let shopBonusValue = Math.floor(playerTurnCharacterRollData["TotalShopValue"] / 10);

        let totalPromotionValue = baseSalaryValue + promotionBonusValue + shopBonusValue;

        ++playerTurnCharacterRollData["Level"];

        playerTurnCharacterRollData["CollectedSuits"] = [];

        playerTurnCharacterRollData["TotalSuitCards"] -= Math.min(SUIT_DATA.length - playerTurnCharacterRollData["CollectedSuits"].length, playerTurnCharacterRollData["TotalSuitCards"]);

        updatePlayerStats
        (
            [
                {
                    CharacterIndex: playerTurnCharacterIndex,
                    ReadyCash: totalPromotionValue
                }
            ]
        );

        let logEntryMessageProperties =
        [
            baseSalaryValue + " base salary",
            promotionBonusValue + " promotion bonus",
            shopBonusValue + " shop bonus"
        ];

        addLogEntry("Received a promotion of " + totalPromotionValue + " ready cash (" + logEntryMessageProperties.join(", ") + ")");
    }

    function endGame()
    {
        ANIMATE_ACTIVE_STATES["CharacterMarker"]["Player"] = [];
        ANIMATE_ACTIVE_STATES["CharacterMarker"]["CameoCharacter"] = [];
        ANIMATE_ACTIVE_STATES["Suits"] = [];
        ANIMATE_ACTIVE_STATES["Shops"] = [];
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

            if (analyzerData["GameSettingsData"]["RuleData"]["Name"] === null)
            {
                loadGameSettings();

                return;
            }

            if (analyzerData["CharacterData"]["PlayerData"][0]["TurnOrderValue"] === null)
            {
                initializeTurnOrderDeterminationSettings();

                return;
            }

            initializeGameSetup();

            $("#settings-panel").scrollTop($("#settings-panel").prop("scrollHeight"));
        }
    });
});
