window.checkMapExists = function () {
    return document.getElementById('map') !== null;
};

window.loadMapbox = function (token, latitude, longitude, trailName) {
    if (typeof mapboxgl !== 'undefined') {
        window.initMapNow(token, latitude, longitude, trailName);
        return;
    }

    var script = document.createElement('script');
    script.src = 'https://api.mapbox.com/mapbox-gl-js/v2.15.0/mapbox-gl.js';
    script.onload = function () {
        window.initMapNow(token, latitude, longitude, trailName);
    };
    document.head.appendChild(script);
};

window.initMapNow = function (token, latitude, longitude, trailName) {
    mapboxgl.accessToken = token;
    const map = new mapboxgl.Map({
        container: 'map',
        style: 'mapbox://styles/mapbox/outdoors-v12',
        center: [longitude, latitude],
        zoom: 13
    });

    new mapboxgl.Marker()
        .setLngLat([longitude, latitude])
        .setPopup(new mapboxgl.Popup().setHTML(`<h3>${trailName}</h3>`))
        .addTo(map);

    map.addControl(new mapboxgl.NavigationControl());
};

window.showAndInitMap = function (token, latitude, longitude, trailName) {
    var mapDiv = document.getElementById('map');
    if (mapDiv) {
        mapDiv.style.display = 'block';
    }
    window.loadMapbox(token, latitude, longitude, trailName);
};