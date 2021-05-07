$(document).ready(function ()
{
    const applicationURI = $("input[name=\"__ApplicationURI\"]").val();

    $("input[name=\"__ApplicationURI\"]").remove();

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

    let settingsContainer = $("#settings-panel > div:last-of-type");

    settingsContainer.append(loadingDisplay());
});
