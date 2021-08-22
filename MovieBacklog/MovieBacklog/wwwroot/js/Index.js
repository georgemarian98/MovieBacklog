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

function addMovie(title, year, imdbUrl, thumbnailUrl) {
    var target = document.getElementById(title)

    $.ajax({
        url: "/Home/AddMovie",
        method: "POST",
        data: {
            title: title,
            year: year,
            imdbUrl: imdbUrl,
            thumbnailUrl: thumbnailUrl,
        },
        success: function (result) {
            console.log("Add")
            target.disabled = true
        }
    })
}

function searchMovie() {
    var movieTitle = document.getElementById("movieTitle").value;

    $.ajax({
        url: "/Home/Search",
        method: "POST",
        data: {
            title: movieTitle
        }
    })
}