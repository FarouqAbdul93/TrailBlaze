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

window.readFileAsBase64 = function (inputId) {
    return new Promise((resolve, reject) => {
        const input = document.getElementById(inputId);
        if (!input || !input.files || !input.files[0]) {
            reject('No file selected');
            return;
        }
        const file = input.files[0];
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result.split(',')[1]);
        reader.onerror = reject;
        reader.readAsDataURL(file);
    });
};

window.showTrailRoute = function (token, routeData, latitude, longitude, trailName) {
    if (typeof mapboxgl === 'undefined') {
        setTimeout(function () {
            window.showTrailRoute(token, routeData, latitude, longitude, trailName);
        }, 500);
        return;
    }

    var mapDiv = document.getElementById('map');
    if (mapDiv) {
        mapDiv.style.display = 'block';
    }

    mapboxgl.accessToken = token;
    const map = new mapboxgl.Map({
        container: 'map',
        style: 'mapbox://styles/mapbox/outdoors-v12',
        center: [longitude, latitude],
        zoom: 11
    });

    new mapboxgl.Marker()
        .setLngLat([longitude, latitude])
        .setPopup(new mapboxgl.Popup().setHTML(`<h3>${trailName}</h3>`))
        .addTo(map);

    map.addControl(new mapboxgl.NavigationControl());

    if (routeData && routeData.length > 10) {
        try {
            const geojson = JSON.parse(routeData);
            map.on('load', function () {
                map.addSource('trail-route', {
                    type: 'geojson',
                    data: {
                        type: 'Feature',
                        properties: {},
                        geometry: geojson
                    }
                });

                map.addLayer({
                    id: 'trail-route',
                    type: 'line',
                    source: 'trail-route',
                    layout: {
                        'line-join': 'round',
                        'line-cap': 'round'
                    },
                    paint: {
                        'line-color': '#e85d04',
                        'line-width': 4,
                        'line-opacity': 0.8
                    }
                });
            });
        } catch (e) {
            console.log('Could not parse route data:', e);
        }
    }
};

