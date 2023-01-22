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

function addMovie(media) {
    const buttonId = media.title + media.year
    var target = document.getElementById(buttonId)

    $.ajax({
        url: "/Home/AddMovie",
        method: "POST",
        data: media,
        success: function (result) {
            console.log("Add")
            target.disabled = true
            target.style.background = '#54f542';
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