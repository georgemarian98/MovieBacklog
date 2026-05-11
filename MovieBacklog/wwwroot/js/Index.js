function updateThumbnail(img, recordId) {
    $.ajax({
        url: "/Home/UpdateRecord",
        method: "POST",
        data: {
            id: recordId
        },
        success: function (result) {
            img.src = result;
        }
    });
}

function deleteMediaRecord(id) {
    $.ajax({
        url: "/Home/DeleteMediaRecord",
        method: "DELETE",
        data: {
            id: id
        },
        success: function (result) {
            location.reload();
        }
    });
}

function addMediaRecord(mediaRecord, btn) {
    // btn is the clicked button element (optional)
    var target = btn || document.getElementById(mediaRecord.title + mediaRecord.year);

    $.ajax({
        url: "/Home/AddMediaRecord",
        method: "POST",
        data: mediaRecord,
        success: function (result) {
            console.log("Add new media record");
            if (target) {
                target.disabled = true;
                target.classList.remove('btn-success');
                target.classList.add('btn-secondary');
                target.innerText = 'Added';
            }
        }
    });
}

// New helper: read data-* attributes safely then call addMediaRecord
function addMediaRecordFromButton(btn) {
    if (!btn || !btn.dataset) return;
    var media = {
        title: btn.dataset.title,
        year: parseInt(btn.dataset.year, 10) || 0,
        imdbUrl: btn.dataset.imdb,
        thumbnailUrl: btn.dataset.thumb,
        isMovie: (btn.dataset.ismovie === 'true')
    };
    addMediaRecord(media, btn);
}

function searchMediaRecord() {
    var mediaRecordTitle = document.getElementById("mediaRecordTitle").value;

    $.ajax({
        url: "/Home/Search",
        method: "POST",
        data: {
            title: mediaRecordTitle
        }
    });
}