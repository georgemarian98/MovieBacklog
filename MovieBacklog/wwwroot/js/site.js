// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const usersList = document.getElementById("usersList")

$.ajax({
    url: "/Home/CurrentUser",
    method: "GET",
    success: function (result) {
        usersList.value = result;
        console.log("Change user to ", result)
    }
})

usersList.onchange = function () {
    var value = usersList.value;
    changeUser(value);
}

function changeUser(user) {
    $.ajax({
        url: "/Home/Users",
        method: "POST",
        data: { newUser: user },
        success: function (result) {
            console.log("Change user to ", user)
        }
    }).done(function () {
        location.reload();
    });
}

function importLibrary() {
    $.ajax({
        url: "/Home/ImportBacklog",
        method: "POST",
        data: {
            jsonString: "YOOOOOOOOOOOOOOOO MERGEEEE?"
        },
        success: function (result) {
            console.log("A MERS!!!");
        }
    });
}

function exportLibrary() {
    $.ajax({
        url: "/Home/ExportBacklog",
        method: "POST",
        success: function (result) {
            const content = typeof result === "string"
                ? result
                : JSON.stringify(result, null, 2);

            const blob = new Blob([content], { type: "application/json" });
            const downloadUrl = URL.createObjectURL(blob);
            const link = document.createElement("a");

            link.href = downloadUrl;
            link.download = "media-backlog.json";
            document.body.appendChild(link);
            link.click();
            link.remove();

            URL.revokeObjectURL(downloadUrl);
        },
        error: function (error) {
            console.error("Failed to export library.", error);
        }
    });
}