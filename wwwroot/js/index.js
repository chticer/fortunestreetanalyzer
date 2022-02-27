$(document).ready(function ()
{
    const applicationURI = $("input[name=\"__ApplicationURI\"]").val();

    $("input[name=\"__ApplicationURI\"]").remove();

    let trainSectionContainer = $("#train-section > div:last-of-type");
    let predictSectionContainer = $("#predict-section > div:last-of-type");

    let previousAnalyzerInstanceData = {};
    let previousAnalyzerInstanceCurrentPage = {};

    function ajaxCall(type, action, data)
    {
        return $.ajax
        (
            {
                type: type,
                url: applicationURI + "/Index?handler=" + action,
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8",
                error: function (jqXHR)
                {
                    serverErrorDisplay(jqXHR);
                }
            }
        );
    }

    function serverErrorDisplay(jqXHR)
    {
        alertDisplay
        (
            {
                Type: "error",
                Title: "Server Issues!",
                Descriptions:
                [
                    "An error response was received from a server request.",
                    "Code: " + jqXHR["status"],
                    "Description: " + jqXHR["statusText"],
                    "Timestamp: " + moment.utc().format("YYYY-MM-DD HH:mm:ss"),
                    "<div>Additional Information:</div>" +

                    "<div>" + navigator.userAgent + "</div>"
                ]
            }
        );
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
                $("#alert-container").removeClass().addClass(alert["Type"]);

                $("#alert-container").empty().append
                (
                    "<div class=\"alert alert-dismissible\">" +
                        "<button type=\"button\" class=\"btn-close\" data-bs-dismiss=\"alert\"></button>" +

                        "<div>" +
                            "<h2>" + alert["Title"] + "</h2>" +
                        "</div>" +

                        "<div>" + descriptions.join("") + "</div>" +
                    "</div>"
                );

                $("html, body").animate({ scrollTop: $("#alert-container").offset().top }, "fast");
            }
        }
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

    function toggleAnalyzerInstanceFields(state)
    {
        for (let currentAnalyzerInstanceType of [ "train", "predict" ])
        {
            let previousAnalyzerInstancesTypeContainer = $("#" + currentAnalyzerInstanceType + "-previous-analyzer-instances");

            let previousAnalyzerInstancesTypeNavigationItems = previousAnalyzerInstancesTypeContainer.children().first().children();

            previousAnalyzerInstancesTypeNavigationItems.find(".fa-arrow-left").toggleClass("disabled", !state);
            previousAnalyzerInstancesTypeNavigationItems.find(".fa-arrow-right").toggleClass("disabled", !state);

            let previousAnalyzerInstancesLoadTypeButtonContainer = previousAnalyzerInstancesTypeContainer.find("button[name=\"" + currentAnalyzerInstanceType + "-previous-analyzer-instances-load\"]");

            previousAnalyzerInstancesLoadTypeButtonContainer.prop("disabled", !state);
            previousAnalyzerInstancesLoadTypeButtonContainer.toggleClass("disabled", !state);

            let createAnalyzerInstanceTypeContainer = $("#" + currentAnalyzerInstanceType + "-create-analyzer-instance");

            let createAnalyzerInstanceNameInputContainer = createAnalyzerInstanceTypeContainer.find("input[name=\"" + currentAnalyzerInstanceType + "-create-analyzer-instance-name\"]");

            createAnalyzerInstanceNameInputContainer.prop("disabled", !state);
            createAnalyzerInstanceNameInputContainer.toggleClass("disabled", !state);

            let createAnalyzerInstanceCreateTypeButtonContainer = createAnalyzerInstanceTypeContainer.find("button[name=\"" + currentAnalyzerInstanceType + "-create-analyzer-instance-create\"]");

            createAnalyzerInstanceCreateTypeButtonContainer.prop("disabled", !state);
            createAnalyzerInstanceCreateTypeButtonContainer.toggleClass("disabled", !state);
        }
    }

    function updatePreviousAnalyzerInstance(type)
    {
        let previousAnalyzerInstancesTypeContainer = $("#" + type + "-previous-analyzer-instances");

        let previousAnalyzerInstancesTypeNavigationItems = previousAnalyzerInstancesTypeContainer.children().first().children();

        previousAnalyzerInstancesTypeNavigationItems.first().toggle(previousAnalyzerInstanceCurrentPage[type] !== 1);

        previousAnalyzerInstancesTypeNavigationItems.eq(1).text(previousAnalyzerInstanceCurrentPage[type] + " of " + previousAnalyzerInstanceData[type].length.toLocaleString());

        previousAnalyzerInstancesTypeNavigationItems.last().toggle(previousAnalyzerInstanceCurrentPage[type] !== previousAnalyzerInstanceData[type].length);

        previousAnalyzerInstancesTypeContainer.children().last().empty().append
        (
            "<div>" +

                (
                    previousAnalyzerInstanceData[type][previousAnalyzerInstanceCurrentPage[type] - 1]["AnalyzerInstanceName"] !== null && previousAnalyzerInstanceData[type][previousAnalyzerInstanceCurrentPage[type] - 1]["AnalyzerInstanceName"].trim() !== ""
                    ?
                    "<div class=\"previous-analyzer-instance-name\">" +
                        "<strong>" + previousAnalyzerInstanceData[type][previousAnalyzerInstanceCurrentPage[type] - 1]["AnalyzerInstanceName"].trim() + "</strong>" +
                    "</div>"
                    :
                    ""
                ) +

                (
                    previousAnalyzerInstanceData[type][previousAnalyzerInstanceCurrentPage[type] - 1]["GameData"] !== null
                    ?
                    "<div>Rule: <strong>" + previousAnalyzerInstanceData[type][previousAnalyzerInstanceCurrentPage[type] - 1]["GameData"]["RuleData"]["Name"] + "</strong></div>" +
                    "<div>Board: <strong>" + previousAnalyzerInstanceData[type][previousAnalyzerInstanceCurrentPage[type] - 1]["GameData"]["BoardData"]["Name"] + "</strong></div>" +
                    (
                        previousAnalyzerInstanceData[type][previousAnalyzerInstanceCurrentPage[type] - 1]["CharacterData"] !== null
                        ?
                            "<div>Standing Threshold: <strong>" + previousAnalyzerInstanceData[type][previousAnalyzerInstanceCurrentPage[type] - 1]["GameData"]["RuleData"]["StandingThreshold"] + "<sup>" + ordinalNumberSuffix(previousAnalyzerInstanceData[type][previousAnalyzerInstanceCurrentPage[type] - 1]["GameData"]["RuleData"]["StandingThreshold"]) + "</sup> Place</strong></div>" +
                            "<div>Net Worth Threshold: <strong>" + previousAnalyzerInstanceData[type][previousAnalyzerInstanceCurrentPage[type] - 1]["GameData"]["RuleData"]["NetWorthThreshold"].toLocaleString() + "</strong></div>" +
                            "<div>Turn: <strong>" + (previousAnalyzerInstanceData[type][previousAnalyzerInstanceCurrentPage[type] - 1]["GameData"]["TurnData"].length + 1).toLocaleString() + "</strong></div>"
                        :
                        ""
                    )
                    :
                    "<div>" +
                        "<strong>No rule and board selection has been made for this analyzer instance.</strong>" +
                    "</div>"
                ) +

            "</div>" +

            "<div>This analyzer instance was started on " + moment.utc(previousAnalyzerInstanceData[type][previousAnalyzerInstanceCurrentPage[type] - 1]["AnalyzerInstanceStarted"]).local().format("MMMM D, YYYY h:mm:ss A") + ".</div>" +

            "<div>" +
                "<button type=\"button\" class=\"btn btn-primary btn-lg\" name=\"train-previous-analyzer-instances-load\">Load</button>" +
            "</div>"
        );

        previousAnalyzerInstancesTypeContainer.find("button[name=\"" + type + "-previous-analyzer-instances-load\"]").on("click", function ()
        {
            toggleAnalyzerInstanceFields(false);

            $("#" + type + "-previous-analyzer-instances").append
            (
                "<div class=\"alert alert-success\">" +
                    "<div>" +
                        "<div>" +
                            "<strong>Redirecting...</strong>" +
                        "</div>" +

                        "<div><a href=\"" + applicationURI + "/" + type + "?id=" + previousAnalyzerInstanceData[type][previousAnalyzerInstanceCurrentPage[type] - 1]["AnalyzerInstanceID"] + "\">Click here</a> if you have not been redirected.</div>" +
                    "</div>" +
                "</div>"
            );

            window.location.href = applicationURI + "/" + type + "?id=" + previousAnalyzerInstanceData[type][previousAnalyzerInstanceCurrentPage[type] - 1]["AnalyzerInstanceID"];
        });
    }

    function validateAnalyzerInstanceName(type)
    {
        let createAnalyzerInstanceNameInputValue = $("#" + type + "-create-analyzer-instance").find("input[name=\"" + type + "-create-analyzer-instance-name\"]").val().trim();

        let createAnalyzerInstanceWarningMessage = "";

        if (createAnalyzerInstanceNameInputValue === "")
            createAnalyzerInstanceWarningMessage = "It is not recommended to leave the name blank.";
        else
        {
            let previousAnalyzerInstanceDataIndex = previousAnalyzerInstanceData[type].findIndex((currentPreviousAnalyzerInstanceData) =>
            {
                return currentPreviousAnalyzerInstanceData["Name"] === createAnalyzerInstanceNameInputValue;
            });

            if (previousAnalyzerInstanceDataIndex > -1)
                createAnalyzerInstanceWarningMessage = "It is not recommended to use the same name as a previous analyzer instance.";
        }

        if (createAnalyzerInstanceWarningMessage !== "")
        {
            $("body").prepend
            (
                "<div class=\"modal fade\" tabindex=\"-1\" role=\"dialog\">" +
                    "<div class=\"modal-dialog modal-dialog-centered\">" +
                        "<div class=\"modal-content\">" +
                            "<div class=\"modal-body\">" +
                                "<div class=\"alert alert-warning\">" +
                                    "<div>" +
                                        "<strong>" + createAnalyzerInstanceWarningMessage + "</strong>" +
                                    "</div>" +
                                "</div>" +

                                "<div class=\"modal-confirmation\">" +
                                    "<div>Do you wish to continue?</div>" +

                                    "<div>" +
                                        "<button type=\"button\" class=\"btn btn-warning\" name=\"modal-confirmation-create-analyzer-instance-warning-yes\" data-bs-dismiss=\"modal\">Yes</button>" +
                                        "<button type=\"button\" class=\"btn btn-secondary\" name=\"modal-confirmation-create-analyzer-instance-warning-no\" data-bs-dismiss=\"modal\">No</button>" +
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

            $("body > .modal button[name=\"modal-confirmation-create-analyzer-instance-warning-yes\"]").on("click", function ()
            {
                processCreateAnalyzerInstance(type);
            });

            $("body > .modal button[name=\"modal-confirmation-create-analyzer-instance-warning-no\"]").on("click", function ()
            {
                toggleAnalyzerInstanceFields(true);
            });
        }

        return createAnalyzerInstanceWarningMessage === "";
    }

    function processCreateAnalyzerInstance(type)
    {
        let createAnalyzerInstanceTypeContainer = $("#" + type + "-create-analyzer-instance");

        createAnalyzerInstanceTypeContainer.parent().append("<div>" + loadingDisplay() + "</div>");

        ajaxCall
        (
            "POST",
            "CreateAnalyzerInstance",
            {
                type: type,
                name: createAnalyzerInstanceTypeContainer.find("input[name=\"" + type + "-create-analyzer-instance-name\"]").val().trim()
            }
        ).done(function (response)
        {
            createAnalyzerInstanceTypeContainer.parent().find(".loading").parent().remove();

            if (response["AlertData"] !== null)
            {
                let alertDataDescriptions = $.map(response["AlertData"]["Descriptions"], function (value)
                {
                    if (value !== null)
                        return "<div>" + value.replace("{redirect-placeholder}", "<a href=\"" + applicationURI + "/train?id=" + JSONResponse["Data"]["AnalyzerInstanceID"] + "\">Click here</a>") + "</div>";
                });

                createAnalyzerInstanceTypeContainer.parent().append
                (
                    "<div class=\"alert " + response["AlertData"]["Type"] + "\">" +
                        "<div>" + alertDataDescriptions.join("") + "</div>" +
                    "</div>"
                );
            }

            if (!response["Error"])
            {
                let JSONResponse = JSON.parse(response["HTMLResponse"]);

                window.location.href = applicationURI + "/train?id=" + JSONResponse["Data"]["AnalyzerInstanceID"];
            }
        });
    }

    trainSectionContainer.append("<div>" + loadingDisplay() + "</div>");

    ajaxCall
    (
        "GET",
        "Startup",
        {}
    ).done(function (response)
    {
        trainSectionContainer.find(".loading").parent().remove();

        alertDisplay(response["AlertData"]);

        if (!response["Error"])
        {
            let JSONResponse = JSON.parse(response["HTMLResponse"]);

            previousAnalyzerInstanceData["train"] = JSONResponse["Data"]["Train"];

            trainSectionContainer.append(JSONResponse["Response"]["Train"]);

            previousAnalyzerInstanceCurrentPage["train"] = 1;

            if (previousAnalyzerInstanceData["train"].length > 1)
            {
                let previousAnalyzerInstanceTrainContainer = $("#train-previous-analyzer-instances");

                let previousAnalyzerInstanceTrainPreviousIcon = previousAnalyzerInstanceTrainContainer.find(".fa-arrow-left");

                let previousAnalyzerInstanceTrainNextIcon = previousAnalyzerInstanceTrainContainer.find(".fa-arrow-right");

                previousAnalyzerInstanceTrainPreviousIcon.on("click", function ()
                {
                    if (!$(this).hasClass("disabled") && previousAnalyzerInstanceCurrentPage["train"] !== 1)
                    {
                        --previousAnalyzerInstanceCurrentPage["train"];

                        updatePreviousAnalyzerInstance("train");
                    }
                });

                previousAnalyzerInstanceTrainNextIcon.on("click", function ()
                {
                    if (!$(this).hasClass("disabled") && previousAnalyzerInstanceCurrentPage["train"] !== previousAnalyzerInstanceData["train"].length)
                    {
                        ++previousAnalyzerInstanceCurrentPage["train"];

                        updatePreviousAnalyzerInstance("train");
                    }
                });
            }

            if (previousAnalyzerInstanceData["train"].length > 0)
                updatePreviousAnalyzerInstance("train");

            trainSectionContainer.find("button[name=\"train-create-analyzer-instance-create\"]").on("click", function ()
            {
                toggleAnalyzerInstanceFields(false);

                if (validateAnalyzerInstanceName("train"))
                    processCreateAnalyzerInstance("train");
            });
        }
    });
});
