'use strict';

const ipLocationUrl = 'ip/location';
const cityLocationsUrl = 'city/locations';

(function() {
    function init() {
        var router = new Router([
            new Route('cityLocations', 'cityLocations.html', true),
            new Route('ipLocation', 'ipLocation.html')
        ]);
    }

    init();
}());

function GetIpLocation() {
    var ipAddressTextBox = document.getElementById('ipAddressInput');

    const ipAddress = ipAddressTextBox.value.trim();

    var requestUrl = ipLocationUrl + "?ip=" + ipAddress;

    fetch(requestUrl)
        .then(response => response.json())
        .then(data => displayResult(data));
}

function GetCityLocations() {
    var cityNameTextBox = document.getElementById('cityNameInput');

    const cityName = cityNameTextBox.value.trim();

    var requestUrl = cityLocationsUrl + "?city=" + cityName;

    fetch(requestUrl)
        .then(response => response.json())
        .then(data => displayResult(data));
}

function displayResult(resultData) {
    var tBody = document.getElementById('results');
    tBody.innerText = JSON.stringify(resultData);
}