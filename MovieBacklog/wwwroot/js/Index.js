function deleteMediaRecord(id) {
    $.ajax({
        url: "/Home/DeleteMediaRecord",
        method: "DELETE",
        data: {
            id: id
        },
        success: function (result) {
            location.reload()
        }
    })
}

function addMediaRecord(mediaRecord) {
    const buttonId = mediaRecord.title + mediaRecord.year
    var target = document.getElementById(buttonId)

    $.ajax({
        url: "/Home/AddMediaRecord",
        method: "POST",
        data: mediaRecord,
        success: function (result) {
            console.log("Add new media record")
            target.disabled = true
            target.style.background = '#54f542';
        }
    })
}

function searchMediaRecord() {
    var mediaRecordTitle = document.getElementById("mediaRecordTitle").value;

    $.ajax({
        url: "/Home/Search",
        method: "POST",
        data: {
            title: mediaRecordTitle
        }
    })
}