// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function imdbQuery() {
    var axios = require("axios").default;

    var options = {
        method: 'GET',
        url: 'https://imdb8.p.rapidapi.com/title/find',
        params: { q: 'american history x' },
        headers: {
            'x-rapidapi-key': 'd5490fab4bmsh0bc23a740cc46a9p1969dejsnee359aef5d88',
            'x-rapidapi-host': 'imdb8.p.rapidapi.com'
        }
    };

    axios.request(options).then(function (response) {
        var result = response.data.results[0]
        var image = result.image

        console.log(image);

        show_image(image, image.width, image.height, result.title)
    }).catch(function (error) {
        console.error(error);
    });
}

function show_image(src, width, height, alt) {
    var img = document.createElement("img");
    img.src = src;
    img.width = width;
    img.height = height;
    img.alt = alt;

    // This next line will just add it to the <body> tag
    document.body.appendChild(img);
}