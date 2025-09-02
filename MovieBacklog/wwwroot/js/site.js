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