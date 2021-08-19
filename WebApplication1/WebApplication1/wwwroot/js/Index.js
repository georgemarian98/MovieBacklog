function deleteMovie(id) {
    $.ajax({
        url: "/Home/DeleteMovie",
        method: "DELETE",
        data: {
            id: id
        },
        success: function (result) {
            location.reload()
        }
    })
}

function searchMovie() {

    var movieTitle = document.getElementById("movieTitle").value;

    $.ajax({
        url: "/Home/AddMovie",
        method: "POST",
        data: {
            title: movieTitle
        },
        success: function (result) {
            location.reload()
        }
    })
}