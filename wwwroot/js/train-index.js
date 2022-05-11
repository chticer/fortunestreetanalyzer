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
const SUIT_ORDER = ["spade", "heart", "diamond", "club"];
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

                    $("#player-turn-determination").addClass("disabled");

                    $("#player-turn-determination button").off("click");

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

    let spaceLayoutIndex = 0;

    let mouseCoordinates =
    {
        x: 0,
        y: 0
    };

    let spacePopupDialogScrollDebounce;

    function renderPlayerContainer(playerIndex)
    {
        return  "<div>" +
                    "<div class=\"character-portrait-icon small\">" +

                    (
                        analyzerData["CharacterData"][playerIndex]["PortraitURL"] !== null
                        ?
                        "<img src=\"" + analyzerData["CharacterData"][playerIndex]["PortraitURL"] + "\" alt=\"Character Portrait for " + analyzerData["CharacterData"][playerIndex]["Name"] + "\" />"
                        :
                        ""
                    ) +

                    "</div>" +

                    "<div>" + (analyzerData["CharacterData"][playerIndex]["Name"] !== null ? analyzerData["CharacterData"][playerIndex]["Name"] : "You") + "</div>" +
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
		                CharacterData: $.map($.extend(true, [], analyzerData["CharacterData"]), function (value, index)
		                {
			                return $.extend
			                (
				                true,
				                value,
				                {
                                    SpaceIndex: JSONResponse["Data"]["CharacterData"][index]["SpaceIndex"],
                                    Level: JSONResponse["Data"]["CharacterData"][index]["Level"],
                                    Placing: JSONResponse["Data"]["CharacterData"][index]["Placing"],
                                    ReadyCash: JSONResponse["Data"]["CharacterData"][index]["ReadyCash"],
                                    TotalShopValue: JSONResponse["Data"]["CharacterData"][index]["TotalShopValue"],
                                    TotalStockValue: JSONResponse["Data"]["CharacterData"][index]["TotalStockValue"],
                                    NetWorth: JSONResponse["Data"]["CharacterData"][index]["NetWorth"],
                                    OwnedShopIndices: JSONResponse["Data"]["CharacterData"][index]["OwnedShopIndices"],
                                    OwnedSuits: JSONResponse["Data"]["CharacterData"][index]["OwnedSuits"]
				                }
			                );
		                }),
		                DistrictData: JSONResponse["Data"]["DistrictData"],
		                ShopData: JSONResponse["Data"]["ShopData"],
		                SpaceData: JSONResponse["Data"]["SpaceData"],
		                SpaceTypeData: JSONResponse["Data"]["SpaceTypeData"]
	                }
                );

                saveAnalyzerData([ "GameData", "CharacterData", "DistrictData", "ShopData", "SpaceData", "SpaceTypeData" ]);

                initializeGameSetup();
            }
        });
    }

    function initializeGameSetup()
    {
        initializeStandings();

        $("#standings-subpanel").show();

        initializeMap();

        $(document).on("mousemove", function (e)
        {
            mouseCoordinates["x"] = e.clientX;
            mouseCoordinates["y"] = e.clientY;
        });

        $(window).on("resize", initializeMap);

        initializeTurns();
    }

    function updateCirclePosition(element, centerPercentage)
    {
        return {
            left: ((centerPercentage["x"] * $(element).parent().width() - $(element).outerWidth(true) / 2) / $(element).parent().width() * 100).toFixed(4) + "%",
            top: ((centerPercentage["y"] * $(element).parent().height() - $(element).outerHeight(true) / 2) / $(element).parent().height() * 100).toFixed(4) + "%"
        };
    }

    function spacePopupDialog(element)
    {
        let currentSpaceInformationContainer = $(element);

        currentSpaceInformationContainer.show();

        let boardSubpanelContainer = $("#board-subpanel > div:first-of-type");

        let boardSubpanelOffset = boardSubpanelContainer.offset();

        let currentSpaceOffset = currentSpaceInformationContainer.parent().offset();

        let currentSpacePopupDialogOffsetX = $(document).scrollLeft() + mouseCoordinates["x"] + 10;

        if (currentSpacePopupDialogOffsetX + currentSpaceInformationContainer.outerWidth() > boardSubpanelContainer.width())
            currentSpacePopupDialogOffsetX = $(document).scrollLeft() + mouseCoordinates["x"] - 10 - currentSpaceInformationContainer.outerWidth();

        let currentSpacePopupDialogOffsetY = $(document).scrollTop() + mouseCoordinates["y"] - currentSpaceInformationContainer.outerHeight() / 2;

        if (currentSpacePopupDialogOffsetY < boardSubpanelOffset.top)
            currentSpacePopupDialogOffsetY = $(document).scrollTop() + mouseCoordinates["y"];
        else if (currentSpacePopupDialogOffsetY + currentSpaceInformationContainer.outerHeight() > boardSubpanelContainer.height())
            currentSpacePopupDialogOffsetY = $(document).scrollTop() + mouseCoordinates["y"] - currentSpaceInformationContainer.outerHeight();

        currentSpaceInformationContainer.css
        (
            {
                left: (currentSpacePopupDialogOffsetX - currentSpaceOffset.left - parseInt(currentSpaceInformationContainer.parent().css("border-left-width"), 10)) + "px",
                top: (currentSpacePopupDialogOffsetY - currentSpaceOffset.top - parseInt(currentSpaceInformationContainer.parent().css("border-top-width"), 10)) + "px"
            }
        );
    }

    function initializeMap()
    {
        $("#board-subpanel").empty().append("<div></div>");

        let boardSubpanelContainer = $("#board-subpanel > div:first-of-type");

        let maxCenterXFactor = analyzerData["SpaceData"][0]["SpaceLayoutData"][spaceLayoutIndex]["CenterXFactor"];
        let maxCenterYFactor = analyzerData["SpaceData"][0]["SpaceLayoutData"][spaceLayoutIndex]["CenterYFactor"];

        for (let i = 1; i < analyzerData["SpaceData"].length; ++i)
        {
            let currentCenterXFactor = analyzerData["SpaceData"][i]["SpaceLayoutData"][spaceLayoutIndex]["CenterXFactor"];

            if (currentCenterXFactor > maxCenterXFactor)
                maxCenterXFactor = currentCenterXFactor;

            let currentCenterYFactor = analyzerData["SpaceData"][i]["SpaceLayoutData"][spaceLayoutIndex]["CenterYFactor"];

            if (currentCenterYFactor > maxCenterYFactor)
                maxCenterYFactor = currentCenterYFactor;
        }

        let boardSubpanelOffsetX = Math.max((boardSubpanelContainer.width() - SPACE_SQUARE_SIZE * (maxCenterXFactor + 0.5)) / 2, 0);
        let boardSubpanelOffsetY = Math.max((boardSubpanelContainer.height() - SPACE_SQUARE_SIZE * (maxCenterYFactor + 0.5)) / 2, 0);

        for (let i = 0; i < analyzerData["SpaceData"].length; ++i)
        {
            boardSubpanelContainer.append("<div></div>");

            let currentSpaceContainer = boardSubpanelContainer.children().last();

            currentSpaceContainer.css
            (
                {
                    left: boardSubpanelOffsetX + SPACE_SQUARE_SIZE * (analyzerData["SpaceData"][i]["SpaceLayoutData"][spaceLayoutIndex]["CenterXFactor"] - 0.5) + "px",
                    top: boardSubpanelOffsetY + SPACE_SQUARE_SIZE * (analyzerData["SpaceData"][i]["SpaceLayoutData"][spaceLayoutIndex]["CenterYFactor"] - 0.5) + "px",
                    width: SPACE_SQUARE_SIZE + "px",
                    height: SPACE_SQUARE_SIZE + "px"
                }
            );

            updateMapSpace(i);

            currentSpaceContainer.on("mouseenter mousemove", function ()
            {
                let currentSpaceInformationContainer = $(this).find(".space-information");

                spacePopupDialog(currentSpaceInformationContainer);
            }).on("mouseleave", function ()
            {
                $(this).find(".space-information").hide();
            });
        }

        for (let i = 0; i < analyzerData["CharacterData"].length; ++i)
        {
            let currentSpaceInformationContainer = $("#board-subpanel > div:first-of-type > div:nth-of-type(" + (analyzerData["CharacterData"][i]["SpaceIndex"] + 1) + ") > div:first-of-type");

            let currentSpaceCharacterMarkersContainer = currentSpaceInformationContainer.find(".character-markers");

            currentSpaceCharacterMarkersContainer.append("<div></div>");

            let currentCharacterMarkerContainer = currentSpaceCharacterMarkersContainer.children().last();

            currentCharacterMarkerContainer.css
            (
                $.extend
                (
                    {},
                    updateCirclePosition
                    (
                        currentCharacterMarkerContainer,
                        {
                            x: 0.25 + (i % 2) * 0.5,
                            y: 0.25 + Math.floor(i / 2) * 0.5
                        }
                    ),
                    {
                        backgroundColor: "#" + analyzerData["CharacterData"][i]["ColorData"]["GameColor"]
                    }
                )
            );
        }

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

                    boardSubpanelContainer.children().each(function ()
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
        let spaceContainer = $("#board-subpanel > div:first-of-type > div:nth-of-type(" + (spaceIndex + 1) + ")");

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
                spaceSquareIconContainer.css({ color: "#" + analyzerData["CharacterData"][analyzerData["SpaceData"][spaceIndex]["AdditionalPropertiesData"]["ShopData"]["OwnerCharacterIndex"]]["ColorData"]["GameColor"] });

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

        spaceContainer.append
        (
            "<div class=\"popup-dialog space-information" + (analyzerData["SpaceTypeData"][analyzerData["SpaceData"][spaceIndex]["SpaceTypeIndex"]]["Name"] == "shop" ? " space-shop" : "") + "\">" +
                "<div></div>" +
            "</div>"
        );

        let spaceInformationContainer = spaceContainer.children().last().children().first();

        spaceInformationContainer.append("<div>" + (analyzerData["SpaceTypeData"][analyzerData["SpaceData"][spaceIndex]["SpaceTypeIndex"]]["Title"] !== null ? analyzerData["SpaceTypeData"][analyzerData["SpaceData"][spaceIndex]["SpaceTypeIndex"]]["Title"] : "&nbsp;") + "</div>");

        spaceInformationContainer.append("<div>" + analyzerData["SpaceTypeData"][analyzerData["SpaceData"][spaceIndex]["SpaceTypeIndex"]]["Description"].replace("{stock-information}", spaceInformationPlaceholders["BankData"]["StockInformation"]).replace("{shop-name}", spaceInformationPlaceholders["ShopData"]["ShopName"]).replace("{shop-value}", spaceInformationPlaceholders["ShopData"]["ShopValue"]).replace("{shop-price}", spaceInformationPlaceholders["ShopData"]["ShopPrice"]).replace("{max-capital}", spaceInformationPlaceholders["ShopData"]["MaxCapital"]).replace("{suit-icon}", spaceInformationPlaceholders["SuitData"]["SuitIcon"]) + "</div>");
    }

    function initializeStandings()
    {
        let standingsContainer = $("#standings-subpanel > div:last-of-type");

        standingsContainer.empty();

        for (let i = 0; i < analyzerData["CharacterData"].length; ++i)
        {
            standingsContainer.append
            (
                "<div class=\"player-information\" style=\"background-color: #" + analyzerData["CharacterData"][i]["ColorData"]["GameColor"] + ";\">" +
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

            updateStandingsPlayer(i);
        }
    }

    function updateStandingsPlayer(playerIndex)
    {
        let playerContainer = $("#standings-subpanel > div:last-of-type > div:nth-of-type(" + (playerIndex + 1) + ")");

        playerContainer.children().first().empty().append
        (
            "<span>" + analyzerData["CharacterData"][playerIndex]["Placing"] + "</span>" +

            "<span>" +
                "<sup>" + ordinalNumberSuffix(analyzerData["CharacterData"][playerIndex]["Placing"]) + "</sup>" +
            "</span>"
        );

        let playerStatsContainer = playerContainer.children().eq(2);

        playerStatsContainer.children().first().children().last().text(analyzerData["CharacterData"][playerIndex]["ReadyCash"]);
        playerStatsContainer.children().eq(1).children().last().text(analyzerData["CharacterData"][playerIndex]["TotalShopValue"]);

        if (analyzerData["GameData"]["RuleData"]["Name"] === "Standard")
            playerStatsContainer.children().eq(2).children().last().text(analyzerData["CharacterData"][playerIndex]["TotalStockValue"]);

        playerStatsContainer.children().last().children().last().text(analyzerData["CharacterData"][playerIndex]["NetWorth"]);

        playerContainer.children().last().empty().append
        (
            "<div class=\"suit-card\">" +
                "<div>" +
                    "<div></div>" +

                    "<div>" + analyzerData["CharacterData"][playerIndex]["OwnedSuits"]["TotalSuitCards"] + "</div>" +
                "</div>" +
            "</div>"
        );

        let playerSuitCardContainer = playerContainer.children().last().find(".suit-card");

        playerSuitCardContainer.children().first().toggle(analyzerData["CharacterData"][playerIndex]["OwnedSuits"]["TotalSuitCards"] > 0);

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
                    "<span class=\"fas fa-" + currentSuitData["Icon"] + "\" style=\"color: #" + (analyzerData["CharacterData"][playerIndex]["OwnedSuits"]["SuitNames"].indexOf(currentSuitData["Icon"]) > -1 ? currentSuitData["Color"] : "333") + ";\"></span>" +
                "</div>"
            );
        }
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

                "<div></div>" +
            "</div>"
        );

        $("#turns > div:last-of-type button[name=\"reset\"]").on("click", function ()
        {

        });
    }

    function displayPlayerTurnOptions()
    {
        let playerTurnOptionsContainer = $("#turns > div:last-of-type > div:last-of-type > div:last-of-type");

        playerTurnOptionsContainer.empty().append
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

        playerTurnOptionsContainer.find("button[name=\"roll\"]").on("click", function ()
        {
            playerTurnOptionsContainer.empty().append
            (
                "<div class=\"roll-die-options\"></div>" +

                createConfirmationActions
                (
                    "center-items",
                    [
                        "<button type=\"button\" class=\"btn btn-lg btn-primary disabled\" name=\"confirm\" disabled=\"disabled\">Confirm</button>",
                        "<button type=\"button\" class=\"btn btn-lg btn-secondary\" name=\"cancel\">Cancel</button>"
                    ]
                )
            );

            let playerTurnRollDieOptionsContainer = playerTurnOptionsContainer.find(".roll-die-options");

            for (let i = 0; i < analyzerData["GameData"]["BoardData"]["MaxDieRoll"]; ++i)
            {
                playerTurnRollDieOptionsContainer.append("<div></div>");

                let currentPlayerTurnDieDotLocationContainer = playerTurnRollDieOptionsContainer.children().last();

                for (let currentDieDotLocationDataPosition of DIE_DOT_LOCATIONS_DATA[i])
                {
                    currentPlayerTurnDieDotLocationContainer.append("<div></div>");

                    let currentPlayerTurnDieDotLocationPositionContainer = currentPlayerTurnDieDotLocationContainer.children().last();

                    currentPlayerTurnDieDotLocationPositionContainer.css(updateCirclePosition(currentPlayerTurnDieDotLocationPositionContainer, currentDieDotLocationDataPosition));
                }
            }

            playerTurnOptionsContainer.find("button[name=\"confirm\"]").on("click", function ()
            {
            });

            playerTurnOptionsContainer.find("button[name=\"cancel\"]").on("click", displayPlayerTurnOptions);

            $("#settings-panel").scrollTop($("#settings-panel").prop("scrollHeight"));
        });

        let playerOwnShopsFlag = analyzerData["CharacterData"][analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1].length]["OwnedShopIndices"].length > 0;

        let playerTurnOptionAuctionButton = playerTurnOptionsContainer.find("button[name=\"auction\"]");

        let playerTurnOptionAuctionEnableFlag = playerOwnShopsFlag;

        playerTurnOptionAuctionButton.prop("disabled", !playerTurnOptionAuctionEnableFlag);
        playerTurnOptionAuctionButton.toggleClass("disabled", !playerTurnOptionAuctionEnableFlag);

        playerTurnOptionAuctionButton.on("click", function ()
        {

        });

        let playerTurnOptionSellShopButton = playerTurnOptionsContainer.find("button[name=\"sell-shop\"]");

        let playerTurnOptionSellShopEnableFlag = playerOwnShopsFlag;

        playerTurnOptionSellShopButton.prop("disabled", !playerTurnOptionSellShopEnableFlag);
        playerTurnOptionSellShopButton.toggleClass("disabled", !playerTurnOptionSellShopEnableFlag);

        playerTurnOptionSellShopButton.on("click", function ()
        {

        });

        let opponentsOwnShopsFlag = false;

        for (let i = 0; i < analyzerData["CharacterData"].length; ++i)
        {
            if (i !== analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1].length - 1 && analyzerData["CharacterData"][i]["OwnedShopIndices"].length > 0)
            {
                opponentsOwnShopsFlag = true;

                break;
            }
        }

        let playerTurnOptionBuyShopButton = playerTurnOptionsContainer.find("button[name=\"buy-shop\"]");

        let playerTurnOptionBuyShopEnableFlag = opponentsOwnShopsFlag && analyzerData["CharacterData"][analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1].length]["ReadyCash"] + analyzerData["CharacterData"][analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1].length]["TotalStockValue"] > 0;

        playerTurnOptionBuyShopButton.prop("disabled", !playerTurnOptionBuyShopEnableFlag);
        playerTurnOptionBuyShopButton.toggleClass("disabled", !playerTurnOptionBuyShopEnableFlag);

        playerTurnOptionBuyShopButton.on("click", function ()
        {

        });

        let playerTurnOptionExchangeShopsButton = playerTurnOptionsContainer.find("button[name=\"exchange-shops\"]");

        let playerTurnOptionExchangeShopsEnableFlag = playerTurnOptionSellShopEnableFlag && playerTurnOptionBuyShopEnableFlag;

        playerTurnOptionExchangeShopsButton.prop("disabled", !playerTurnOptionExchangeShopsEnableFlag);
        playerTurnOptionExchangeShopsButton.toggleClass("disabled", !playerTurnOptionExchangeShopsEnableFlag);

        playerTurnOptionExchangeShopsButton.on("click", function ()
        {

        });

        let playerTurnOptionRenovateVacantPlotButton = playerTurnOptionsContainer.find("button[name=\"renovate-vacant-plot\"]");

        let playerTurnOptionRenovateVacantPlotEnableFlag = false;

        playerTurnOptionRenovateVacantPlotButton.prop("disabled", !playerTurnOptionRenovateVacantPlotEnableFlag);
        playerTurnOptionRenovateVacantPlotButton.toggleClass("disabled", !playerTurnOptionRenovateVacantPlotEnableFlag);

        playerTurnOptionRenovateVacantPlotButton.on("click", function ()
        {

        });

        let playerTurnOptionSellStocksButton = playerTurnOptionsContainer.find("button[name=\"sell-stocks\"]");

        playerTurnOptionSellStocksButton.parent().toggle(analyzerData["GameData"]["RuleData"]["Name"] === "Standard");

        if (analyzerData["GameData"]["RuleData"]["Name"] === "Standard")
        {
            let playerTurnOptionSellStocksEnableFlag = analyzerData["CharacterData"][analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1].length]["TotalStockValue"] > 0;

            playerTurnOptionSellStocksButton.prop("disabled", !playerTurnOptionSellStocksEnableFlag);
            playerTurnOptionSellStocksButton.toggleClass("disabled", !playerTurnOptionSellStocksEnableFlag);

            playerTurnOptionSellShopButton.on("click", function ()
            {

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

            for (let j = 0; j < analyzerData["GameData"]["TurnData"][i].length; ++j)
                $("#turns > div:last-of-type > div:last-of-type").append
                (
                    "<div>" +
                        renderPlayerContainer(j) +

                        
                    "</div>"
                );
        }

        startNextPlayerTurn();
    }

    function startNextPlayerTurn()
    {
        if (analyzerData["GameData"]["TurnData"].length === 0 || analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1].length === analyzerData["CharacterData"].length)
        {
            analyzerData["GameData"]["TurnData"].push([]);

            displayNewTurn(analyzerData["GameData"]["TurnData"].length - 1);
        }

        analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1].push([]);

        $("#turns > div:last-of-type > div:last-of-type").append
        (
            "<div class=\"player-information\" style=\"background-color: #" + analyzerData["CharacterData"][analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1].length - 1]["ColorData"]["GameColor"] + ";\">" + renderPlayerContainer(analyzerData["GameData"]["TurnData"][analyzerData["GameData"]["TurnData"].length - 1].length - 1) + "</div>" +

            "<div></div>"
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
